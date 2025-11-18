using System.Diagnostics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities;
using BepuUtilities.Memory;
using Leatha.WarOfTheElements.World.Terrain;
using System.Numerics;

namespace Leatha.WarOfTheElements.World.Physics
{
    public sealed class PhysicsWorld : IDisposable
    {
        public PhysicsWorld(Vector3 gravity)
        {
            _bufferPool = new BufferPool();

            var narrowPhaseCallbacks = new SimpleNarrowPhaseCallbacks();
            var poseIntegratorCallbacks = new SimplePoseIntegratorCallbacks(gravity);

            // Bepu 2.4.0 signature:
            // Simulation.Create(BufferPool, TNarrowPhaseCallbacks, TPoseIntegratorCallbacks, SolveDescription)
            _simulation = Simulation.Create(
                _bufferPool,
                narrowPhaseCallbacks,
                poseIntegratorCallbacks,
                new SolveDescription(8, 1));
        }

        private readonly BufferPool _bufferPool;
        private readonly Simulation _simulation;

        // PlayerId -> BodyHandle
        private readonly Dictionary<Guid, BodyHandle> _playerBodies = new();

        // Optional heightfield for terrain
        private TerrainHeightfield? _terrain;

        // Fallback flat ground Y (if no terrain)
        private float _groundY = 0f;

        public Simulation Simulation => _simulation;

        private readonly object _simLock = new();

        public void Dispose()
        {
            _simulation.Dispose();
            _bufferPool.Clear();
        }

        /// <summary>
        /// Adds a big flat box as the ground.
        /// This keeps things simple while you wire networking and player movement.
        /// </summary>
        public void AddFlatGround(float sizeX, float sizeZ, float groundY = 0f)
        {
            lock (_simLock)
            {
                _groundY = groundY;

                var groundBox = new Box(sizeX, 1f, sizeZ); // 1 unit thick
                var groundShapeIndex = _simulation.Shapes.Add(groundBox);

                // Center the box so its top is at groundY.
                var position = new Vector3(0f, groundY - 0.5f, 0f);
                var staticDescription = new StaticDescription(position, groundShapeIndex);

                _simulation.Statics.Add(staticDescription);
            }
        }

        /// <summary>
        /// Builds a static mesh from a TerrainHeightfield and adds it to the simulation.
        /// </summary>
        public void AddTerrain(TerrainHeightfield terrain)
        {
            lock (_simLock)
            {
                _terrain = terrain;

                var heights = terrain.Heights;
                var width = heights.GetLength(0);
                var height = heights.GetLength(1);

                var vertexCount = width * height;
                var vertices = new Vector3[vertexCount];

                for (var z = 0; z < height; z++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        var idx = z * width + x;
                        var worldX = terrain.Origin.X + x * terrain.CellSize;
                        var worldZ = terrain.Origin.Z + z * terrain.CellSize;
                        var worldY = terrain.Origin.Y + heights[x, z];

                        vertices[idx] = new Vector3(worldX, worldY, worldZ);
                    }
                }

                // Two triangles per cell
                var quadCount = (width - 1) * (height - 1);
                var indices = new int[quadCount * 6];
                var index = 0;

                for (var z = 0; z < height - 1; z++)
                {
                    for (var x = 0; x < width - 1; x++)
                    {
                        var i0 = z * width + x;
                        var i1 = z * width + (x + 1);
                        var i2 = (z + 1) * width + x;
                        var i3 = (z + 1) * width + (x + 1);

                        // Triangle 1
                        indices[index++] = i0;
                        indices[index++] = i2;
                        indices[index++] = i1;

                        // Triangle 2
                        indices[index++] = i1;
                        indices[index++] = i2;
                        indices[index++] = i3;
                    }
                }

                var triangleCount = indices.Length / 3;
                _bufferPool.Take(triangleCount, out Buffer<Triangle> triangleBuffer);

                for (var triIndex = 0; triIndex < triangleCount; triIndex++)
                {
                    var i0 = indices[triIndex * 3 + 0];
                    var i1 = indices[triIndex * 3 + 1];
                    var i2 = indices[triIndex * 3 + 2];

                    ref var tri = ref triangleBuffer[triIndex];
                    tri.A = vertices[i0];
                    tri.B = vertices[i1];
                    tri.C = vertices[i2];
                }

                var mesh = new Mesh(triangleBuffer, new Vector3(1f, 1f, 1f), _bufferPool);
                var meshIndex = _simulation.Shapes.Add(mesh);

                // Vertices are already in world space, so origin is zero transform.
                var staticDescription = new StaticDescription(Vector3.Zero, meshIndex);
                _simulation.Statics.Add(staticDescription);

                // Fallback for very simple grounded check when sampling fails.
                _groundY = terrain.Origin.Y;
            }
        }

        // ---------- Player bodies ----------

        public BodyHandle AddPlayer(Guid playerId, Vector3 startPosition, float radius = 0.5f, float halfHeight = 0.9f)
        {
            lock (_simLock)
            {
                if (_playerBodies.TryGetValue(playerId, out var player))
                    return player;

                // Capsule character (like a FPS controller).
                // Capsule(radius, length). length ~= cylinder height between the caps.
                var capsule = new Capsule(radius, halfHeight * 2f);
                //var inertia = capsule.ComputeInertia(1f); // mass = 1

                var inertia = new BodyInertia
                {
                    InverseMass = 1f,
                    InverseInertiaTensor = new Symmetric3x3() // all zeros
                };

                var pose = new RigidPose(startPosition);
                var shapeIndex = _simulation.Shapes.Add(capsule);

                var collidable = new CollidableDescription(shapeIndex, 0.1f);
                var activity = new BodyActivityDescription(sleepThreshold: 0.01f);

                var bodyDesc = BodyDescription.CreateDynamic(pose, inertia, collidable, activity);
                var handle = _simulation.Bodies.Add(bodyDesc);

                _playerBodies[playerId] = handle;

                return handle;
            }
        }

        public void RemovePlayer(Guid playerId)
        {
            lock (_simLock)
            {
                if (!_playerBodies.TryGetValue(playerId, out var handle))
                    return;

                // 1) Grab the shape index *before* removing the body
                var body = _simulation.Bodies[handle];
                var shapeIndex = body.Collidable.Shape;

                // 2) Remove body from the simulation
                _simulation.Bodies.Remove(handle);

                // 3) Now it’s safe to remove the shape (since no body uses it anymore)
                _simulation.Shapes.Remove(shapeIndex);

                // 4) Remove from dictionary
                _playerBodies.Remove(playerId);
            }
        }

        public bool TryGetPlayer(Guid playerId, out BodyHandle handle)
        {
            lock (_simLock)
            {
                return _playerBodies.TryGetValue(playerId, out handle);
            }
        }

        /// <summary>
        /// Sets the horizontal velocity, and optionally vertical component for flying/jump.
        /// Does not stomp vertical velocity in grounded mode, so gravity works.
        /// </summary>
        public Vector3 SetPlayerVelocity(
            Guid playerId,
            Vector3 desiredVelocity,
            bool isFlying,
            bool jump,
            float jumpImpulse,
            bool isOnGround)
        {
            lock (_simLock)
            {
                if (!_playerBodies.TryGetValue(playerId, out var handle))
                    return Vector3.Zero;

                var body = _simulation.Bodies.GetBodyReference(handle);

                var v = body.Velocity.Linear;

                // Always control horizontal movement
                v.X = desiredVelocity.X;
                v.Z = desiredVelocity.Z;

                if (isFlying)
                {
                    // Full control in Y when flying
                    v.Y = desiredVelocity.Y;
                }
                else
                {
                    // Only apply jump impulse when grounded;
                    // let gravity (from pose integrator callbacks) handle the rest.
                    if (jump && isOnGround)
                    {
                        v.Y = jumpImpulse;
                    }
                }

                body.Velocity.Linear = v;

                return body.Velocity.Linear;
            }
        }

        public Vector3 GetPlayerVelocity(Guid playerId)
        {
            lock (_simLock)
            {
                if (!_playerBodies.TryGetValue(playerId, out var handle))
                    return Vector3.Zero;

                var body = _simulation.Bodies[handle];
                return body.Velocity.Linear;
            }
        }

        public Vector3 MovePlayerKinematic(
            Guid playerId,
            Vector3 desiredVelocity, // X/Z from ComputeDesiredVelocity
            float dt,
            bool jump,
            bool isOnGround,
            float jumpImpulse)
        {
            lock (_simLock)
            {
                if (!_playerBodies.TryGetValue(playerId, out var handle))
                    return Vector3.Zero;

                var body = _simulation.Bodies.GetBodyReference(handle);

                // --- 1) KINEMATIC HORIZONTAL (X/Z) ---
                var pos = body.Pose.Position;

                // Only move horizontally here, Y is handled by physics
                pos.X += desiredVelocity.X * dt;
                pos.Z += desiredVelocity.Z * dt;

                body.Pose.Position = pos;

                // --- 2) DYNAMIC VERTICAL (Y) WITH JUMP ---
                var v = body.Velocity.Linear;

                // Jump: only when grounded and not already going up
                if (jump && isOnGround && v.Y <= 0f)
                {
                    var oldY = v.Y;
                    v.Y = jumpImpulse; // e.g. 7.0f
                    Debug.WriteLine($"[JumpTest] jump={jump}, ground={isOnGround}, vYBefore={oldY:0.000}, vYAfter={v.Y:0.000}");
                }

                // Don't touch v.Y otherwise: gravity from SimplePoseIntegratorCallbacks
                // will pull the player back down and contacts will zero it when hitting ground.
                body.Velocity.Linear = v;

                // Return the effective velocity (horizontal + vertical)
                return new Vector3(desiredVelocity.X, v.Y, desiredVelocity.Z);
            }
        }

        public bool TryGetPlayerTransform(
            BodyHandle handle,
            out Vector3 position,
            out Quaternion orientation,
            out bool grounded)
        {
            lock (_simLock)
            {
                if (!_simulation.Bodies.BodyExists(handle))
                {
                    Debug.WriteLine("BODY DOES NOT EXIST!!!!!!!");

                    position = Vector3.Zero;
                    orientation = Quaternion.Identity;
                    grounded = true;
                    return false;
                }

                var body = _simulation.Bodies[handle];
                position = body.Pose.Position;
                orientation = body.Pose.Orientation;

                const float groundedEpsilon = 0.05f;

                // Compute the bottom of the capsule using the actual shape.
                float bottomY = position.Y;

                var shapeIndex = body.Collidable.Shape;
                if (shapeIndex.Exists)
                {
                    // We know we used Capsule when adding the player.
                    // GetShape<T>(index) returns a ref to the stored shape.
                    ref var capsule = ref _simulation.Shapes.GetShape<Capsule>(shapeIndex.Index);

                    // Capsule in Bepu is aligned along Y:
                    // total half height from center to bottom = radius + halfLength.
                    var totalHalfHeight = capsule.Radius + capsule.HalfLength;
                    bottomY = position.Y - totalHalfHeight;
                }

                float surfaceY;
                if (_terrain is { } terrain)
                {
                    surfaceY = SampleTerrainHeight(position.X, position.Z, terrain);
                }
                else
                {
                    surfaceY = _groundY;
                }

                grounded = bottomY <= surfaceY + groundedEpsilon;

                return true;
            }
        }

        private static float SampleTerrainHeight(float worldX, float worldZ, TerrainHeightfield terrain)
        {
            var cellSize = terrain.CellSize;

            var localX = (worldX - terrain.Origin.X) / cellSize;
            var localZ = (worldZ - terrain.Origin.Z) / cellSize;

            var heights = terrain.Heights;
            var width = heights.GetLength(0);
            var height = heights.GetLength(1);

            // Clamp to valid range
            var x0 = (int)MathF.Floor(localX);
            var z0 = (int)MathF.Floor(localZ);

            x0 = Math.Clamp(x0, 0, width - 1);
            z0 = Math.Clamp(z0, 0, height - 1);

            var x1 = Math.Clamp(x0 + 1, 0, width - 1);
            var z1 = Math.Clamp(z0 + 1, 0, height - 1);

            var tx = Math.Clamp(localX - x0, 0f, 1f);
            var tz = Math.Clamp(localZ - z0, 0f, 1f);

            var h00 = heights[x0, z0];
            var h10 = heights[x1, z0];
            var h01 = heights[x0, z1];
            var h11 = heights[x1, z1];

            // Bilinear interpolation
            var h0 = h00 + (h10 - h00) * tx;
            var h1 = h01 + (h11 - h01) * tx;
            var h = h0 + (h1 - h0) * tz;

            return terrain.Origin.Y + h;
        }

        public void Step(float dt)
        {
            lock (_simLock)
            {
                _simulation.Timestep(dt);
            }
        }
    }
}

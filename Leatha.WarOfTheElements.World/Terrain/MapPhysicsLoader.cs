using Leatha.WarOfTheElements.World.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Leatha.WarOfTheElements.Common.Environment.Collisions;

namespace Leatha.WarOfTheElements.World.Terrain
{
    public sealed class MapPhysicsLoader
    {
        private readonly PhysicsWorld _world;
        private readonly JsonSerializerOptions _jsonOptions;

        public MapPhysicsLoader(PhysicsWorld world)
        {
            _world = world;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        /// <summary>
        /// basePath example:
        ///   "Data/maps/initial_player_spawn_map"
        /// -> will load basePath + "_archetypes.json" and "_environment.json".
        /// </summary>
        public void LoadMap(string basePath)
        {
            var archetypesPath = basePath + "_archetypes.json";
            var environmentPath = basePath + "_environment.json";

            if (!File.Exists(archetypesPath))
                throw new FileNotFoundException("Archetypes file not found", archetypesPath);

            if (!File.Exists(environmentPath))
                throw new FileNotFoundException("Environment file not found", environmentPath);

            var archJson = File.ReadAllText(archetypesPath);
            var envJson = File.ReadAllText(environmentPath);

            var archetypeWrapper = JsonSerializer.Deserialize<ColliderArchetypeExport>(archJson, _jsonOptions)
                                   ?? throw new InvalidOperationException("Failed to deserialize archetypes");

            var envWrapper = JsonSerializer.Deserialize<EnvironmentExport>(envJson, _jsonOptions)
                              ?? throw new InvalidOperationException("Failed to deserialize environment instances");

            var archetypeByName = archetypeWrapper.Archetypes
                .ToDictionary(a => a.ArchetypeName, StringComparer.Ordinal);

            foreach (var inst in envWrapper.Instances)
            {
                if (!inst.IsStatic)
                    continue;

                if (!archetypeByName.TryGetValue(inst.ArchetypeName, out var arch))
                {
                    Console.WriteLine($"[MapPhysicsLoader] Missing archetype '{inst.ArchetypeName}' for '{inst.Name}'.");
                    continue;
                }

                CreateCollider(inst, arch);
            }
        }

        private void CreateCollider(EnvironmentInstanceObject inst, ColliderArchetypeObject archetype)
        {
            var position = inst.Position;
            var rotation = FromEulerDegrees(inst.RotationDegrees);

            switch (inst.ShapeType)
            {
                case ColliderArchetypeType.Box:
                {
                    var half = new Vector3(
                        inst.ColliderSize.X * 0.5f,
                        inst.ColliderSize.Y * 0.5f,
                        inst.ColliderSize.Z * 0.5f);

                    //_world.AddStaticBox(position, rotation, half); // #TODO
                    break;
                }
                case ColliderArchetypeType.Cylinder:
                {
                    var radius = inst.ColliderSize.X;
                    var height = inst.ColliderSize.Y;
                    //_world.AddStaticCylinder(position, rotation, radius, height); // #TODO
                        break;
                }
                case ColliderArchetypeType.ConvexHull:
                {
                    // If we have real hull points, build a real convex hull.
                    if (inst.ConvexHullPoints != null && inst.ConvexHullPoints.Count >= 4)
                    {
                        var pts = inst.ConvexHullPoints
                            .ToArray();

                        //_world.AddStaticConvexHull(position, rotation, pts); // #TODO
                        }
                        else
                    {
                        // Fallback: oriented box from AABB size
                        var half = new Vector3(
                            inst.ColliderSize.X * 0.5f,
                            inst.ColliderSize.Y * 0.5f,
                            inst.ColliderSize.Z * 0.5f);

                        //_world.AddStaticBox(position, rotation, half); // #TODO
                        }

                    break;
                }
                default:
                    Console.WriteLine($"[MapPhysicsLoader] Unsupported ShapeType '{inst.ShapeType}' on '{inst.Name}'.");
                    break;
            }
        }

        private static Vector3 ToVector3(Vector3 v) => new Vector3(v.X, v.Y, v.Z);

        private static Quaternion FromEulerDegrees(Vector3 deg)
        {
            var radX = MathF.PI / 180f * deg.X;
            var radY = MathF.PI / 180f * deg.Y;
            var radZ = MathF.PI / 180f * deg.Z;

            var qx = Quaternion.CreateFromAxisAngle(Vector3.UnitX, radX);
            var qy = Quaternion.CreateFromAxisAngle(Vector3.UnitY, radY);
            var qz = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, radZ);

            // Same order we used before
            return qy * qx * qz;
        }
    }
}

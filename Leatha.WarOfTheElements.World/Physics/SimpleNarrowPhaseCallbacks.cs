using System.Diagnostics;
using System.Runtime.CompilerServices;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;

namespace Leatha.WarOfTheElements.World.Physics
{
    /// <summary>
    /// Narrow phase callbacks:
    ///  - block static vs static
    ///  - set simple material properties
    ///  - queue projectile hits when a projectile body collides with something
    /// </summary>
    public struct SimpleNarrowPhaseCallbacks : INarrowPhaseCallbacks
    {
        public SimpleNarrowPhaseCallbacks(PhysicsWorld physicsWorld)
        {
            _physicsWorld = physicsWorld;
            _simulation = null!;
        }

        private readonly PhysicsWorld _physicsWorld;
        private Simulation _simulation;

        public void Initialize(Simulation simulation)
        {
            _simulation = simulation;
        }

        public void Dispose()
        {
        }

        // ----------------------------------------------------------
        // 1) Broad-phase contact filter
        // ----------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowContactGeneration(
            int workerIndex,
            CollidableReference a,
            CollidableReference b,
            ref float speculativeMargin)
        {
            // Ignore static-vs-static; everything else is allowed.
            if (a.Mobility == CollidableMobility.Static &&
                b.Mobility == CollidableMobility.Static)
            {
                return false;
            }

            return true;
        }

        // ----------------------------------------------------------
        // 2) Child-pair filter (for compounds) — allow everything
        // ----------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowContactGeneration(
            int workerIndex,
            CollidablePair pair,
            int childIndexA,
            int childIndexB)
        {
            return true;
        }

        // ----------------------------------------------------------
        // 3) Child manifold config (compound children)
        // ----------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ConfigureContactManifold(
            int workerIndex,
            CollidablePair pair,
            int childIndexA,
            int childIndexB,
            ref ConvexContactManifold manifold)
        {
            // We don't need to modify child manifolds; let them pass.
            return true;
        }

        // ----------------------------------------------------------
        // 4) Top-level manifold config + projectile hit detection
        // ----------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ConfigureContactManifold<TManifold>(
            int workerIndex,
            CollidablePair pair,
            ref TManifold manifold,
            out PairMaterialProperties pairMaterial)
            where TManifold : unmanaged, IContactManifold<TManifold>
        {
            pairMaterial = CreateDefaultMaterial();

            // --- Projectile hit detection here ---
            //
            // Check if either collidable is a projectile body; if so,
            // queue a hit against the OTHER collidable in the PhysicsWorld.
            //
            // PhysicsWorld should implement:
            //   bool TryGetProjectileId(BodyHandle handle, out Guid projectileId)
            //   void QueueProjectileHit(Guid projectileId, CollidableReference other)

            if (pair.A.Mobility == CollidableMobility.Dynamic)
            {
                var handleA = pair.A.BodyHandle;
                if (_physicsWorld.TryGetProjectileId(handleA, out var projectileId))
                {
                    _physicsWorld.QueueProjectileHit(projectileId, pair.B);
                }
            }

            if (pair.B.Mobility == CollidableMobility.Dynamic)
            {
                var handleB = pair.B.BodyHandle;
                if (_physicsWorld.TryGetProjectileId(handleB, out var projectileId))
                {
                    _physicsWorld.QueueProjectileHit(projectileId, pair.A);
                }
            }

            // Still create constraints for this manifold
            return true;
        }

        // ----------------------------------------------------------
        // 5) Default material
        // ----------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static PairMaterialProperties CreateDefaultMaterial()
        {
            return new PairMaterialProperties
            {
                FrictionCoefficient = 1.0f,
                MaximumRecoveryVelocity = 2.0f,
                SpringSettings = new SpringSettings(
                    frequency: 30f,
                    dampingRatio: 1f)
            };
        }
    }
}

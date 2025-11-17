using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;

namespace Leatha.WarOfTheElements.World.Physics
{
    /// <summary>
    /// Minimal narrow phase callbacks:
    ///  - allow all dynamic vs anything contacts
    ///  - block static vs static
    ///  - set simple material properties
    /// </summary>
    public struct SimpleNarrowPhaseCallbacks : INarrowPhaseCallbacks
    {
        private Simulation _simulation;

        public void Initialize(Simulation simulation)
        {
            _simulation = simulation;
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Global filter for potential pairs from the broad phase.
        /// </summary>
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

            // You could tweak speculativeMargin here if you wanted.
            return true;
        }

        /// <summary>
        /// Filter for compound children; we don't use compounds yet, so just allow.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowContactGeneration(
            int workerIndex,
            CollidablePair pair,
            int childIndexA,
            int childIndexB)
        {
            return true;
        }

        /// <summary>
        /// Configure convex child manifold for compounds.
        /// We don't need anything fancy, just keep the contacts.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ConfigureContactManifold(
            int workerIndex,
            CollidablePair pair,
            int childIndexA,
            int childIndexB,
            ref ConvexContactManifold manifold)
        {
            // No material set here; top-level generic overload will handle it.
            return true;
        }

        /// <summary>
        /// Configure a top-level manifold (convex or nonconvex) for a pair.
        /// This overload does NOT expose speculativeMargin.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ConfigureContactManifold<TManifold>(
            int workerIndex,
            CollidablePair pair,
            ref TManifold manifold,
            [UnscopedRef] out PairMaterialProperties pairMaterial)
            where TManifold : unmanaged, IContactManifold<TManifold>
        {
            pairMaterial = CreateDefaultMaterial();
            return true;
        }

        /// <summary>
        /// Configure a top-level manifold with speculativeMargin available.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ConfigureContactManifold<TManifold>(
            int workerIndex,
            CollidablePair pair,
            ref TManifold manifold,
            ref float speculativeMargin,
            out PairMaterialProperties pairMaterial)
            where TManifold : struct, IContactManifold<TManifold>
        {
            // You can choose to tweak speculativeMargin here; for now we just keep it.
            pairMaterial = CreateDefaultMaterial();
            return true;
        }

        /// <summary>
        /// Configure per-child manifolds for compound shapes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ConfigureContactManifold<TManifold>(
            int workerIndex,
            CollidablePair pair,
            int childIndexA,
            int childIndexB,
            ref TManifold manifold,
            [UnscopedRef] out PairMaterialProperties pairMaterial)
            where TManifold : unmanaged, IContactManifold<TManifold>
        {
            pairMaterial = CreateDefaultMaterial();
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static PairMaterialProperties CreateDefaultMaterial()
        {
            return new PairMaterialProperties
            {
                // Simple “sticky” material so characters don't slide everywhere.
                FrictionCoefficient = 1.0f,
                MaximumRecoveryVelocity = 2.0f,
                SpringSettings = new SpringSettings(
                    frequency: 30f,
                    dampingRatio: 1f)
            };
        }
    }
}

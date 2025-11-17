using BepuPhysics;
using BepuUtilities;
using System.Numerics;

namespace Leatha.WarOfTheElements.World.Physics
{
    public readonly struct SimplePoseIntegratorCallbacks : IPoseIntegratorCallbacks
    {
        public Vector3 Gravity { get; }

        public AngularIntegrationMode AngularIntegrationMode
            => AngularIntegrationMode.Nonconserving;

        public bool AllowSubstepsForUnconstrainedBodies
            => false;

        public bool IntegrateVelocityForKinematics
            => false;

        public SimplePoseIntegratorCallbacks(Vector3 gravity)
        {
            Gravity = gravity;
        }

        public void Initialize(Simulation simulation)
        {
        }

        public void PrepareForIntegration(float dt)
        {
        }

        public void IntegrateVelocity(
            Vector<int> bodyIndices,
            Vector3Wide position,
            QuaternionWide orientation,
            BodyInertiaWide localInertia,
            Vector<int> integrationMask,
            int workerIndex,
            Vector<float> dt,
            ref BodyVelocityWide velocity)
        {
            velocity.Linear.Y += Gravity.Y * dt;
        }
    }
}

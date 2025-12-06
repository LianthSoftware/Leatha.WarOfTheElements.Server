using BepuPhysics;
using BepuUtilities;
using System.Numerics;

namespace Leatha.WarOfTheElements.World.Physics
{
    public readonly struct SimplePoseIntegratorCallbacks : IPoseIntegratorCallbacks
    {
        public SimplePoseIntegratorCallbacks(Vector3 gravity, PhysicsWorld physicsWorld)
        {
            _physicsWorld = physicsWorld;
            Gravity = gravity;
        }

        public Vector3 Gravity { get; }

        private readonly PhysicsWorld _physicsWorld;

        public AngularIntegrationMode AngularIntegrationMode
            => AngularIntegrationMode.Nonconserving;

        public bool AllowSubstepsForUnconstrainedBodies
            => false;

        public bool IntegrateVelocityForKinematics
            => false;

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
            // gravity per lane: Gravity.Y * gravityScale(lane)

            // Gravity.Y as a vector
            var gravityYWide = new Vector<float>(Gravity.Y);

            // gravityScale is stored in InverseInertiaTensor.XX per body
            var gravityScaleWide = localInertia.InverseInertiaTensor.XX; // Vector<float>

            // effective g per lane
            var effectiveGY = gravityYWide * gravityScaleWide;

            // Mask out kinematic / non-integrated bodies
            // integrationMask is 0/!0 per lane; convert to 0 or 1
            var maskWide = Vector.Equals(integrationMask, Vector<int>.Zero);
            // maskWide == true means "skip", so invert:
            var integrateMask = Vector.ConditionalSelect(maskWide, Vector<float>.Zero, Vector<float>.One);

            // apply mask
            effectiveGY *= integrateMask;

            // finally integrate
            velocity.Linear.Y += effectiveGY * dt;



            //velocity.Linear.Y += Gravity.Y * dt;

            //for (var lane = 0; lane < Vector<float>.Count; lane++)
            //{
            //    if (integrationMask[lane] == 0)
            //        continue; // kinematic or otherwise non-integrated

            //    var bodyIndex = bodyIndices[lane];

            //    // Get the handle for this body index
            //    var handle = _physicsWorld.Simulation.Bodies[bodyIndex].Handle;

            //    var scale = _physicsWorld.GetGravityScale(handle); // 1.0f or 0.0f for projectiles

            //    var g = Gravity.Y * scale;
            //    velocity.Linear.Y[lane] += g * dt[lane];
            //}

            //for (int lane = 0; lane < Vector<float>.Count; lane++)
            //{
            //    if (integrationMask[lane] == 0)
            //        continue;

            //    // Read gravity scale from inertia tensor
            //    // (assuming you stored it in inertia.InverseInertiaTensor.XX)
            //    var gravityScale = localInertia.InverseInertiaTensor.XX[lane];

            //    var g = Gravity.Y * gravityScale;

            //    velocity.Linear.Y[lane] += g * dt[lane];
            //}
        }
    }
}

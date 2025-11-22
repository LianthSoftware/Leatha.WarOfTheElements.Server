using BepuPhysics;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.Scripts.NonPlayers;
using Newtonsoft.Json;
using System.Numerics;
using Leatha.WarOfTheElements.Server.Objects.Characters.Movement;

namespace Leatha.WarOfTheElements.Server.Objects.Characters
{
    public sealed class NonPlayerState : ICharacterState
    {
        public Guid NonPlayerId { get; set; }

        public int TemplateId { get; set; }

        public WorldObjectId WorldObjectId { get; }

        public string CharacterName { get; set; } = null!;

        public int CharacterLevel { get; set; }

        public int MapId { get; set; }

        public Guid? InstanceId { get; set; }

        public CharacterResourceObject Resources { get; set; } = new();

        public List<AuraObject> Auras { get; set; } = [];

        [JsonIgnore]
        public BodyHandle Body { get; private set; }

        [JsonIgnore]
        public NonPlayerScriptBase? Script { get; private set; }

        [JsonIgnore]
        public MotionMaster MotionMaster { get; private set; }

        public Vector3 Position { get; set; }

        public Quaternion Orientation { get; set; }

        public Vector3 Velocity { get; set; }

        public float Yaw { get; set; }

        public float Pitch { get; set; }

        public bool IsOnGround { get; set; }

        public bool IsFlying { get; set; }

        public bool IsSprinting { get; set; }

        public const float WalkSpeed = 2f;
        public const float SprintMultiplier = 1.8f;
        public const float FlySpeed = 8f;
        public const float JumpImpulse = 7f;



        public Vector3 SpawnPosition { get; set; }

        public Quaternion SpawnOrientation { get; set; }

        public NonPlayerState()
        {
            MotionMaster = new MotionMaster(this);
        }

        public NonPlayerState(Guid nonPlayerId, Vector3 position)
        {
            NonPlayerId = nonPlayerId;
            Position = position;

            WorldObjectId = new WorldObjectId(nonPlayerId, WorldObjectType.NonPlayer);
            MotionMaster = new MotionMaster(this);
        }

        public NonPlayerState(Guid nonPlayerId, Vector3 position, Quaternion orientation)
        {
            NonPlayerId = nonPlayerId;
            Position = position;
            Orientation = orientation;

            WorldObjectId = new WorldObjectId(nonPlayerId, WorldObjectType.NonPlayer);
            MotionMaster = new MotionMaster(this);
        }

        public void SetPhysicsBody(BodyHandle bodyHandle, Vector3 spawnPosition, Quaternion spawnOrientation)
        {
            Body = bodyHandle;

            SpawnPosition = spawnPosition;
            SpawnOrientation = spawnOrientation;
        }

        public void SetScript(NonPlayerScriptBase script)
        {
            Script = script;
        }


        public Vector3 ComputeDesiredVelocity(double dt)
        {
            // 1) Copy view & state from client
            //Yaw = input.Yaw;          // radians
            //Pitch = input.Pitch;      // radians

            //IsFlying = input.IsFlying;
            //IsSprinting = input.IsSprinting;

            //var forwardOverride = MotionMaster.GetVelocity();

            // 2) Local input (camera space), normalize so diagonal isn't faster
            //var localMove = new Vector2(0.0f, forwardOverride); // X = strafe, Y = forward
            var localMove = MotionMaster.Velocity;
            if (localMove.LengthSquared() > 1e-4f)
                localMove = Vector3.Normalize(localMove);

            // 3) Build world-space basis from yaw (Godot-style: forward = -Z when yaw = 0)
            var sin = MathF.Sin(Yaw);
            var cos = MathF.Cos(Yaw);

            // camera forward: yaw = 0 -> (0, 0, -1)
            var forward = new Vector3(-sin, 0f, -cos);
            // camera right:   yaw = 0 -> (1, 0,  0)
            var right = new Vector3(cos, 0f, -sin);

            // 4) Combine into horizontal movement direction
            var moveHorizontal = forward * localMove.Y + right * localMove.X;
            if (moveHorizontal.LengthSquared() > 1e-4f)
                moveHorizontal = Vector3.Normalize(moveHorizontal);

            // 5) Speed
            var speed = WalkSpeed;
            if (IsSprinting)
                speed *= SprintMultiplier;

            var desired = moveHorizontal * speed; // X/Z filled

            // 6) Vertical (Y)
            //if (IsFlying)
            //{
            //    // Up is -1..1 while flying
            //    desired.Y = input.Up * FlySpeed;
            //}
            //else
            {
                // Gravity + jump are handled elsewhere; keep Y controlled by physics
                desired.Y = 0f;
            }

            //if (desired != Vector3.Zero)
            //    Debug.WriteLine($"Desired Velocity = {desired}");

            return desired;
        }

        public void Update(double delta)
        {
            // Update character's movement.
            MotionMaster.Update(delta);

            // Update script.
            Script?.OnUpdate(delta);
        }
    }
}

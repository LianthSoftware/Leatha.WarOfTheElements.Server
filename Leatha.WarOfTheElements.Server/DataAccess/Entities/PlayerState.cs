using System.Diagnostics;
using BepuPhysics;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using Leatha.WarOfTheElements.Server.Objects.Characters;

namespace Leatha.WarOfTheElements.Server.DataAccess.Entities
{
    public sealed class PlayerState : MongoEntity, ICharacterState
    {
        public Guid AccountId { get; set; }

        public Guid PlayerId { get; init; }

        public string CharacterName { get; set; } = null!;

        public int CharacterLevel { get; set; }

        [BsonIgnore]
        public WorldObjectId WorldObjectId { get; private set; }

        public int MapId { get; set; }

        public Guid? InstanceId { get; set; }

        public CharacterResourceObject Resources { get; set; } = new();

        public List<AuraObject> Auras { get; set; } = [];

        [BsonIgnore]
        [JsonIgnore]
        public BodyHandle Body { get; private set; }

        public Vector3 Position { get; set; }

        public Quaternion Orientation { get; set; }

        [BsonIgnore]
        public Vector3 Velocity { get; set; }

        public float Yaw { get; set; }

        public float Pitch { get; set; }

        public bool IsOnGround { get; set; }

        public bool IsFlying { get; set; }

        public bool IsSprinting { get; set; }

        [BsonIgnore]
        public int LastProcessedInputSeq { get; set; }

        public const float WalkSpeed = 5f;
        public const float SprintMultiplier = 1.8f;
        public const float FlySpeed = 8f;
        public const float JumpImpulse = 7f;

        public PlayerState()
        {
        }

        public PlayerState(Guid accountId, Guid playerId, Vector3 position)
        {
            AccountId = accountId;
            PlayerId = playerId;
            Position = position;

            WorldObjectId = new WorldObjectId(playerId, WorldObjectType.Player);
        }

        public PlayerState(Guid accountId, Guid playerId, Vector3 position, Quaternion orientation)
        {
            AccountId = accountId;
            PlayerId = playerId;
            Position = position;
            Orientation = orientation;

            WorldObjectId = new WorldObjectId(playerId, WorldObjectType.Player);
        }

        public void SetPhysicsBody(BodyHandle bodyHandle)
        {
            Body = bodyHandle;
        }

        public void SetObjectId(Guid playerId)
        {
            WorldObjectId = new WorldObjectId(playerId, WorldObjectType.Player);
        }

    /// <summary>
        /// Computes desired velocity from input. PhysicsWorld applies it to the body.
        /// </summary>
        public Vector3 ComputeDesiredVelocity(PlayerInputObject input, double dt)
        {
            // 1) Copy view & state from client
            Yaw = input.Yaw;          // radians
            Pitch = input.Pitch;      // radians

            IsFlying = input.IsFlying;
            IsSprinting = input.IsSprinting;

            // 2) Local input (camera space), normalize so diagonal isn't faster
            var localMove = new Vector2(input.Right, input.Forward); // X = strafe, Y = forward
            if (localMove.LengthSquared() > 1e-4f)
                localMove = Vector2.Normalize(localMove);

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
            if (IsFlying)
            {
                // Up is -1..1 while flying
                desired.Y = input.Up * FlySpeed;
            }
            else
            {
                // Gravity + jump are handled elsewhere; keep Y controlled by physics
                desired.Y = 0f;
            }

            //if (desired != Vector3.Zero)
            //    Debug.WriteLine($"Desired Velocity = {desired}");

            return desired;
        }

        // Old version kept for reference:
        //public Vector3 ComputeDesiredVelocity(PlayerInputObject input, double dt)
        //{
        //    // View direction from client
        //    Yaw = input.Yaw;
        //    Pitch = input.Pitch;
        //
        //    IsFlying = input.IsFlying;
        //    IsSprinting = input.IsSprinting;
        //
        //    // Yaw is in radians.
        //    var forward = new Vector3(MathF.Sin(Yaw), 0, MathF.Cos(Yaw));
        //    var right = new Vector3(MathF.Cos(Yaw), 0, -MathF.Sin(Yaw));
        //
        //    // Forward/Right: -1..1 axes
        //    var moveHorizontal = forward * input.Forward + right * input.Right;
        //    if (moveHorizontal.LengthSquared() > 1e-4f)
        //        moveHorizontal = Vector3.Normalize(moveHorizontal);
        //
        //    var speed = WalkSpeed;
        //    if (IsSprinting)
        //        speed *= SprintMultiplier;
        //
        //    var desired = Vector3.Zero;
        //    desired.X = moveHorizontal.X * speed;
        //    desired.Z = moveHorizontal.Z * speed;
        //
        //    if (IsFlying)
        //    {
        //        // Up is used for vertical flying (-1..1)
        //        desired.Y = input.Up * FlySpeed;
        //    }
        //    else
        //    {
        //        // Vertical (Y) handled in PhysicsWorld (gravity + jump).
        //        desired.Y = 0f;
        //    }
        //
        //    if (desired != Vector3.Zero)
        //        Debug.WriteLine($"Desired Velocity = { desired }");
        //
        //    return desired;
        //}
    }
}

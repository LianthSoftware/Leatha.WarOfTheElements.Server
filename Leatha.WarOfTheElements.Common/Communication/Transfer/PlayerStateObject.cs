namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class PlayerStateObject : ICharacterStateObject
    {
        public WorldObjectId WorldObjectId { get; set; }

        public string CharacterName { get; set; } = null!;

        public int CharacterLevel { get; set; }

        public int MapId { get; set; }

        public Guid? InstanceId { get; set; }

        public CharacterResourceObject Resources { get; set; } = new();

        public List<AuraObject> Auras { get; set; } = [];

        // Position
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        // Orientation quaternion
        public float Qx { get; set; }

        public float Qy { get; set; }

        public float Qz { get; set; }

        public float Qw { get; set; }

        // View angles (optional but nice to have directly)
        public float Yaw { get; set; }

        public float Pitch { get; set; }

        // Velocity (for interpolation/extrapolation on client)
        public float Vx { get; set; }

        public float Vy { get; set; }

        public float Vz { get; set; }

        // State flags
        public bool IsOnGround { get; set; }

        public bool IsFlying { get; set; }

        public bool IsSprinting { get; set; }

        // For client-side prediction (optional to use now)
        public int LastProcessedInputSeq { get; set; }
    }
}

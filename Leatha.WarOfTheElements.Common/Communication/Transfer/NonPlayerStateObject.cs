using System.Numerics;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class NonPlayerStateObject : ICharacterStateObject
    {
        public WorldObjectId WorldObjectId { get; set; }

        public string CharacterName { get; set; } = null!;

        public int CharacterLevel { get; set; }

        public int TemplateId { get; set; }

        public int MapId { get; set; }

        public Guid? InstanceId { get; set; }

        public Vector3 Position { get; set; }

        public CharacterResourceObject Resources { get; set; } = new();

        public List<AuraObject> Auras { get; set; } = [];

        public Quaternion Orientation { get; set; }

        public Vector3 Velocity { get; set; }

        public float Yaw { get; set; }

        public float Pitch { get; set; }

        // State flags
        public bool IsOnGround { get; set; }

        public bool IsFlying { get; set; }

        public bool IsSprinting { get; set; }
    }
}

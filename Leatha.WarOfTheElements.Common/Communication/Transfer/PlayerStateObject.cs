using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class PlayerStateObject : ICharacterStateObject
    {
        public WorldObjectId WorldObjectId { get; set; }

        public string CharacterName { get; set; } = null!;

        public int CharacterLevel { get; set; }

        public int MapId { get; set; }

        public Guid? InstanceId { get; set; }

        [Required]
        [JsonPropertyName("position")]
        public Vector3 Position { get; set; }

        public CharacterResourceObject Resources { get; set; } = new();

        public List<AuraObject> Auras { get; set; } = [];

        public Quaternion Orientation { get; set; }

        public Vector3 Velocity { get; set; }

        // View angles (optional but nice to have directly)
        public float Yaw { get; set; }

        public float Pitch { get; set; }

        // State flags
        public bool IsOnGround { get; set; }

        public bool IsFlying { get; set; }

        public bool IsSprinting { get; set; }

        // For client-side prediction (optional to use now)
        public int LastProcessedInputSeq { get; set; }
    }
}

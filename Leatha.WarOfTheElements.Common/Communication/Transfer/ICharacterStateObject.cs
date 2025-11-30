using System.Numerics;
using System.Text.Json.Serialization;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public interface ICharacterStateObject : IWorldObjectStateObject
    {
        public string CharacterName { get; set; }

        public int CharacterLevel { get; set; }

        public CharacterResourceObject Resources { get; set; }

        public List<AuraObject> Auras { get; set; }

        public Vector3 Velocity { get; set; }

        // View angles (optional but nice to have directly)
        public float Yaw { get; set; }

        public float Pitch { get; set; }


        // State flags
        public bool IsOnGround { get; set; }

        public bool IsFlying { get; set; }

        public bool IsSprinting { get; set; }
    }
}

using BepuPhysics;
using Leatha.WarOfTheElements.Server.Scripts.NonPlayers;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Numerics;
using Leatha.WarOfTheElements.Common.Communication.Transfer;

namespace Leatha.WarOfTheElements.Server.Objects.Characters
{
    public interface ICharacterState
    {
        public WorldObjectId WorldObjectId { get; }

        public string CharacterName { get; set; }

        public int CharacterLevel { get; set; }

        public int MapId { get; set; }

        public Guid? InstanceId { get; set; }

        public CharacterResourceObject Resources { get; set; }

        public List<AuraObject> Auras { get; set; }

        [BsonIgnore]
        [JsonIgnore]
        public BodyHandle Body { get; }

        public Vector3 Position { get; set; }

        public Quaternion Orientation { get; set; }

        [BsonIgnore]
        public Vector3 Velocity { get; set; }

        public float Yaw { get; set; }

        public float Pitch { get; set; }

        public bool IsOnGround { get; set; }

        public bool IsFlying { get; set; }

        public bool IsSprinting { get; set; }
    }
}

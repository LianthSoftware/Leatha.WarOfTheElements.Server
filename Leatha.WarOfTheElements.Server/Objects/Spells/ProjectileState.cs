using BepuPhysics;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using System.Numerics;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Leatha.WarOfTheElements.Server.Objects.Spells
{
    public sealed class ProjectileState
    {
        public Guid ProjectileId { get; }

        public Guid SpellGuid { get; set; }

        public int MapId { get; set; }

        public Guid? InstanceId { get; set; }

        public WorldObjectId CasterId { get; set; }

        public int SpellId { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Velocity { get; set; }

        public Quaternion Orientation { get; set; }

        [BsonIgnore]
        [JsonIgnore]
        public BodyHandle Body { get; private set; }

        public float RemainingLifetimeMs { get; set; }

        public bool Launched { get; set; }

        public ProjectileState(Guid id, Vector3 position, Quaternion orientation)
        {
            ProjectileId = id;
            Position = position;
            Orientation = orientation;
        }

        public void SetPhysicsBody(BodyHandle body)
        {
            Body = body;
        }
    }
}

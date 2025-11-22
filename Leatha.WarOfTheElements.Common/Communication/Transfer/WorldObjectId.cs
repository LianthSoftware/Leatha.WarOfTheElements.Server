using System.Text.Json.Serialization;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public readonly struct WorldObjectId : IEquatable<WorldObjectId>, IComparable<WorldObjectId>
    {
        public Guid ObjectId { get; }

        public WorldObjectType ObjectType { get; }

        public static WorldObjectId Empty
            => new WorldObjectId(Guid.Empty, WorldObjectType.None);

        [JsonConstructor]
        public WorldObjectId(Guid objectId, WorldObjectType objectType)
        {
            ObjectId = objectId;
            ObjectType = objectType;
        }

        public bool Equals(WorldObjectId other)
        {
            return ObjectId == other.ObjectId && ObjectType == other.ObjectType;
        }

        public override bool Equals(object? obj)
        {
            return obj is WorldObjectId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ObjectId, ObjectType);
        }

        public int CompareTo(WorldObjectId other)
        {
            var typeComparison = ObjectType.CompareTo(other.ObjectType);
            if (typeComparison != 0)
                return typeComparison;

            return ObjectId.CompareTo(other.ObjectId);
        }

        public static bool operator ==(WorldObjectId left, WorldObjectId right) => left.Equals(right);
        public static bool operator !=(WorldObjectId left, WorldObjectId right) => !left.Equals(right);

        public override string ToString() => $"{ObjectType}:{ObjectId}";
    }

}

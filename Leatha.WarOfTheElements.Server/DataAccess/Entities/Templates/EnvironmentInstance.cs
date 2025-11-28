using Leatha.WarOfTheElements.Common.Environment.Collisions;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates
{
    public sealed class EnvironmentInstance : MongoEntity
    {
        public string ArchetypeName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int MapId { get; set; }

        public Vector3 Position { get; set; } = Vector3.Zero;

        public Vector3 RotationDegrees { get; set; } = Vector3.Zero;

        public Vector3 ColliderSize { get; set; } = Vector3.Zero;

        public ColliderArchetypeType ShapeType { get; set; }

        public bool IsStatic { get; set; }

        // NEW – optional convex hull points for ConvexHull shapes
        public List<Vector3>? ConvexHullPoints { get; set; }
    }
}

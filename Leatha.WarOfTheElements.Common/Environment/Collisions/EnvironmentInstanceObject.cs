using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Environment.Collisions
{
    public sealed class EnvironmentInstanceObject
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

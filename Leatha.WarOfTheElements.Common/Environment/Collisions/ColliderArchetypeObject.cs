using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Environment.Collisions
{
    public sealed class ColliderArchetypeObject
    {
        public string ArchetypeName { get; set; } = null!;

        public string Name { get; set; } = null!;          // "capsule_small", "crate_box_01", ...

        public ColliderArchetypeType ShapeType { get; set; }      // "Capsule", "Box", "ConvexHull", "Compound"

        public Vector3 Size { get; set; }          // box extents / capsule radius+height / etc.

        // For more complex shapes:
        public List<Vector3>? HullVertices { get; set; }

        //public List<CompoundChildDto> Children { get; set; } // for compound

        public bool IsStaticDefault { get; set; }  // e.g. rocks vs crates
    }
}

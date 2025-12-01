using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class ProjectileStateObject
    {
        public Guid ProjectileId { get; set; }

        public Guid SpellGuid { get; set; }

        public int MapId { get; set; }

        public Guid? InstanceId { get; set; }

        public WorldObjectId CasterId { get; set; }

        public int SpellId { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Velocity { get; set; }

        public Quaternion Orientation { get; set; }

        public float RemainingLifetimeMs { get; set; }

        public bool Launched { get; set; }
    }
}

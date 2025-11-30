using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public interface IWorldObjectStateObject
    {
        WorldObjectId WorldObjectId { get; set; }

        public int MapId { get; set; }

        public Guid? InstanceId { get; set; }

        public Vector3 Position { get; set; }

        public Quaternion Orientation { get; set; }
    }
}

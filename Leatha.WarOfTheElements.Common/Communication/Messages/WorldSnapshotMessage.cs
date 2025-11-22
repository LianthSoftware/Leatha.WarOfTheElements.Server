using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leatha.WarOfTheElements.Common.Communication.Transfer;

namespace Leatha.WarOfTheElements.Common.Communication.Messages
{
    public sealed class WorldSnapshotMessage
    {
        public Guid MessageGuid { get; set; } = Guid.NewGuid();

        public long Tick { get; set; }

        public double ServerTime { get; set; }

        public int MapId { get; set; }

        public Guid? InstanceId { get; set; }

        public List<PlayerStateObject> Players { get; set; } = [];

        public List<NonPlayerStateObject> NonPlayers { get; set; } = [];
    }
}

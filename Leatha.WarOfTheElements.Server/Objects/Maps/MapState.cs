using System.Collections.Concurrent;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.Objects.Characters;

namespace Leatha.WarOfTheElements.Server.Objects.Maps
{
    public class MapState
    {
        public Guid MapGuid { get; set; }

        public int MapId { get; set; }

        public string MapName { get; set; } = null!;

        public int MaxPlayers { get; set; }

        public Guid? InstanceId { get; set; }

        //public ConcurrentBag<PlayerState> Players { get; set; } = [];

        //public ConcurrentBag<NonPlayerState> NonPlayers { get; set; } = [];






        //public Guid? InstanceId { get; set; }
    }
}

using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;

namespace Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates
{
    public sealed class MapTemplate : MongoEntity
    {
        public int MapId { get; set; }

        public string MapName { get; set; } = null!;

        public string MapDescription { get; set; } = null!;

        public string MapPath { get; set; } = null!;

        public int MapSizeX { get; set; }

        public int MapSizeY { get; set; }

        public int MaxPlayers { get; set; }

        public MapFlags MapFlags { get; set; }
    }
}

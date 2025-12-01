using Leatha.WarOfTheElements.Common.Communication.Transfer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Messages
{
    public sealed class WorldSnapshotMessage
    {
        [Required]
        [JsonPropertyName("messageGuid")]
        public Guid MessageGuid { get; set; } = Guid.NewGuid();

        [Required]
        [JsonPropertyName("tick")]
        public long Tick { get; set; }

        [Required]
        [JsonPropertyName("serverTime")]
        public double ServerTime { get; set; }

        [Required]
        [JsonPropertyName("mapId")]
        public int MapId { get; set; }

        [Required]
        [JsonPropertyName("instanceId")]
        public Guid? InstanceId { get; set; }

        [Required]
        [JsonPropertyName("players")]
        public List<PlayerStateObject> Players { get; set; } = [];

        [Required]
        [JsonPropertyName("nonPlayers")]
        public List<NonPlayerStateObject> NonPlayers { get; set; } = [];

        [Required]
        [JsonPropertyName("gameObjects")]
        public List<GameObjectStateObject> GameObjects { get; set; } = [];

        [Required]
        [JsonPropertyName("spells")]
        public List<SpellObject> Spells { get; set; } = [];

        //public List<NonPlayerStateObject> Effects { get; set; } = []; // #TODO
    }
}

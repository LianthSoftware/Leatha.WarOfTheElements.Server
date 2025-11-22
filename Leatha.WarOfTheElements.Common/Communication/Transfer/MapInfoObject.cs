using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class MapInfoObject
    {
        [Required]
        [JsonPropertyName("mapId")]
        public int MapId { get; set; }

        [Required]
        [JsonPropertyName("mapName")]
        public string MapName { get; set; } = null!;

        [Required]
        [JsonPropertyName("mapDescription")]
        public string MapDescription { get; set; } = null!;

        [Required]
        [JsonPropertyName("mapPath")]
        public string MapPath { get; set; } = null!;

        [Required]
        [JsonPropertyName("mapSizeX")]
        public int MapSizeX { get; set; }

        [Required]
        [JsonPropertyName("mapSizeY")]
        public int MapSizeY { get; set; }

        [Required]
        [JsonPropertyName("mapFlags")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MapFlags MapFlags { get; set; }
    }
}

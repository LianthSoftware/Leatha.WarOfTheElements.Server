using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class GameObjectInfoObject
    {
        [Required]
        [JsonPropertyName("gameObjectId")]
        public int GameObjectId { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [Required]
        [JsonPropertyName("sceneName")]
        public string SceneName { get; set; } = null!;
    }
}

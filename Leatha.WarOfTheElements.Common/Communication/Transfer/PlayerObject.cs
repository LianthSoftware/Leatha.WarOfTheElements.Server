using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class PlayerObject
    {
        [Required]
        [JsonPropertyName("playerId")]
        public Guid PlayerId { get; set; }

        [Required]
        [JsonPropertyName("playerName")]
        public string PlayerName { get; set; } = null!;

        [Required]
        [JsonPropertyName("created")]
        public DateTime Created { get; set; }
    }
}

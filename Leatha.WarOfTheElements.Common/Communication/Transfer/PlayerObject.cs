using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class PlayerObject
    {
        [Required]
        [JsonPropertyName("accountId")]
        public Guid AccountId { get; set; }

        [Required]
        [JsonPropertyName("playerId")]
        public Guid PlayerId { get; set; }

        [Required]
        [JsonPropertyName("playerName")]
        public string PlayerName { get; set; } = null!;

        [Required]
        [JsonPropertyName("level")]
        public int Level { get; set; }

        [Required]
        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        [Required]
        [JsonPropertyName("primaryElementType")]
        public ElementTypes PrimaryElementType { get; set; }

        [Required]
        [JsonPropertyName("secondaryElementType")]
        public ElementTypes SecondaryElementType { get; set; }

        [Required]
        [JsonPropertyName("tertiaryElementType")]
        public ElementTypes TertiaryElementType { get; set; }
    }
}

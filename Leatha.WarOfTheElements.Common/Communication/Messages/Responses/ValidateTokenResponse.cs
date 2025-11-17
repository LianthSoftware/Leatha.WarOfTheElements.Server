using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Leatha.WarOfTheElements.Common.Communication.Messages.Responses
{
    public sealed class ValidateTokenResponse
    {
        [Required]
        [JsonPropertyName("playerId")]
        public Guid PlayerId { get; set; }

        [Required]
        [JsonPropertyName("isTokenValid")]
        public bool IsTokenValid { get; set; }
    }
}

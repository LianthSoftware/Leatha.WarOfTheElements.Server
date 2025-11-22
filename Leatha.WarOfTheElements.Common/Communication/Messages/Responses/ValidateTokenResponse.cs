using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Leatha.WarOfTheElements.Common.Communication.Messages.Responses
{
    public sealed class ValidateTokenResponse
    {
        [Required]
        [JsonPropertyName("accountId")]
        public Guid AccountId { get; set; }

        [Required]
        [JsonPropertyName("isTokenValid")]
        public bool IsTokenValid { get; set; }
    }
}

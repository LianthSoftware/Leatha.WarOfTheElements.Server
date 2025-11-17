using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Leatha.WarOfTheElements.Common.Communication.Messages.Responses
{
    public sealed class RefreshTokenResponse
    {
        [Required]
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; } = null!;

        [Required]
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = null!;
    }
}

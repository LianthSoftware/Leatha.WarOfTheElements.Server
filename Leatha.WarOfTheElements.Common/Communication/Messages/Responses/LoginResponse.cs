using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Leatha.WarOfTheElements.Common.Communication.Messages.Responses
{
    public sealed class LoginResponse
    {
        [Required]
        [JsonPropertyName("playerId")]
        public Guid PlayerId { get; set; }

        [Required]
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [Required]
        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        //[Required]
        //public PlayerStatus PlayerStatus { get; set; }

        [Required]
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; } = null!;

        [JsonPropertyName("refreshToken")]
        public string? RefreshToken { get; set; }
    }
}

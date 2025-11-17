using System.Text.Json.Serialization;

namespace Leatha.WarOfTheElements.Common.Communication.Messages.Responses
{
    public sealed class SignupResponse
    {
        [JsonPropertyName("loginResponse")]
        public LoginResponse LoginResponse { get; set; } = null!;

        //public PlayerObject Player { get; set; } = null!;
    }
}

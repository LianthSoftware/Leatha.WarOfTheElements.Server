using System.ComponentModel.DataAnnotations;

namespace Leatha.WarOfTheElements.Common.Communication.Messages.Requests
{
    public sealed class ValidateTokenRequest
    {
        [Required]
        public string AccessToken { get; set; } = null!;
    }
}

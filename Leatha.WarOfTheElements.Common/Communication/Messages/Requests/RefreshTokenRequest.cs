using System.ComponentModel.DataAnnotations;

namespace Leatha.WarOfTheElements.Common.Communication.Messages.Requests
{
    public sealed class RefreshTokenRequest
    {
        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}

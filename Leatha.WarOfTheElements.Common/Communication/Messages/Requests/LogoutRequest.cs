using System.ComponentModel.DataAnnotations;

namespace Leatha.WarOfTheElements.Common.Communication.Messages.Requests
{
    public sealed class LogoutRequest
    {
        [Required]
        public Guid AccountId { get; set; }

        public string? RefreshToken { get; set; } = null!;
    }
}

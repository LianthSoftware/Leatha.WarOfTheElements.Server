using System.ComponentModel.DataAnnotations;

namespace Leatha.WarOfTheElements.Common.Communication.Messages.Requests
{
    public sealed class LogoutRequest
    {
        [Required]
        public Guid PlayerId { get; set; }

        public string? RefreshToken { get; set; } = null!;
    }
}

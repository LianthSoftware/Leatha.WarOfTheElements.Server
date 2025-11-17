using System.ComponentModel.DataAnnotations;

namespace Leatha.WarOfTheElements.Common.Communication.Messages.Requests
{
    public sealed class LoginRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public bool RememberMe { get; set; }
    }
}

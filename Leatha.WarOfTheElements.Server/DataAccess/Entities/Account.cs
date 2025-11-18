using System.ComponentModel.DataAnnotations;

namespace Leatha.WarOfTheElements.Server.DataAccess.Entities
{
    public sealed class Account : MongoEntity
    {
        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        public DateTime Created { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Leatha.WarOfTheElements.Server.DataAccess.Entities
{
    public sealed class Player : MongoEntity
    {
        [Required]
        public Guid PlayerId { get; set; }

        [Required]
        public string PlayerName { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        public DateTime Created { get; set; }
    }
}

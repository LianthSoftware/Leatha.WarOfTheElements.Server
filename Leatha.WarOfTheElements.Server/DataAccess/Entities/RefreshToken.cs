using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Leatha.WarOfTheElements.Server.DataAccess.Entities
{
    public sealed class RefreshToken : MongoEntity
    {
        [Required]
        public Guid AccountId { get; set; }

        public Guid? PlayerId { get; set; }

        [Required]
        public string Token { get; set; } = null!;

        [Required]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime Expires { get; set; } = DateTime.UtcNow.AddDays(30); // Token is valid for 30 days

        public DateTime? Revoked { get; set; }


        //public string CreatedByIp { get; set; }

        //public string ReplacedByToken { get; set; }

        [NotMapped]
        public bool IsActive
            => Revoked == null && !IsExpired;

        [NotMapped]
        public bool IsExpired
            => DateTime.UtcNow >= Expires;
    }
}

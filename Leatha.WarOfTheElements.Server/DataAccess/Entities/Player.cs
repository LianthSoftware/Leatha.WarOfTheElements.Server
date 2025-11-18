using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Leatha.WarOfTheElements.Server.DataAccess.Entities
{
    public sealed class Player : MongoEntity
    {
        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public Guid PlayerId { get; set; }

        [Required]
        public string PlayerName { get; set; } = null!;

        [Required]
        public int Level { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public ElementTypes PrimaryElementType { get; set; }

        [Required]
        public ElementTypes SecondaryElementType { get; set; }

        [Required]
        public ElementTypes TertiaryElementType { get; set; }
    }
}

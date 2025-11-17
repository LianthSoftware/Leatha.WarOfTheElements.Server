using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Leatha.WarOfTheElements.Server.DataAccess.Entities
{
    public abstract class MongoEntity
    {
        [BsonId]
        [Required]
        public ObjectId Id { get; set; }
    }
}

using System.Numerics;

namespace Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates
{
    public sealed class GameObjectSpawnTemplate : MongoEntity
    {
        public int GameObjectId { get; set; }

        public int MapId { get; set; }

        public Vector3 SpawnPosition { get; set; }

        public Quaternion Orientation { get; set; }
    }
}

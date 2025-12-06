using System.Numerics;

namespace Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates
{
    public sealed class NonPlayerSpawnTemplate : MongoEntity
    {
        public int NonPlayerId { get; set; }

        public int MapId { get; set; }

        //public Guid? InstanceId { get; set; }

        public Vector3 SpawnPosition { get; set; }

        public Quaternion Orientation { get; set; }

        public string NodeName { get; set; } = null!;
    }
}

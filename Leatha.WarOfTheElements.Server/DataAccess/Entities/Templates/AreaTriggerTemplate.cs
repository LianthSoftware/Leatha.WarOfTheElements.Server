using System.Numerics;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;

namespace Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates
{
    public sealed class AreaTriggerTemplate : MongoEntity
    {
        public int AreaTriggerId { get; set; }

        public string AreaTriggerName { get; set; } = null!;

        public int MapId { get; set; }

        public float Radius { get; set; }

        public Vector3 SpawnPosition { get; set; }

        public Quaternion Orientation { get; set; }

        public AreaTriggerFlags Flags { get; set; }
    }
}

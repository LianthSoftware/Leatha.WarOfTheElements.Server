using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;

namespace Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates
{
    public sealed class GameObjectTemplate : MongoEntity
    {
        public int GameObjectId { get; set; }

        public string Name { get; set; } = null!;

        public int ColliderArchetypeId { get; set; }

        public string? ScriptName { get; set; }
    }
}

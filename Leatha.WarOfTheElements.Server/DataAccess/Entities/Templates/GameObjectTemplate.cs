using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;

namespace Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates
{
    public sealed class GameObjectTemplate : MongoEntity
    {
        public int GameObjectId { get; set; }

        public string Name { get; set; } = null!;

        public int ColliderArchetypeId { get; set; }

        public string SceneName { get; set; } = null!;

        public string? ScriptName { get; set; }

        //public Dictionary<string, object> AdditionalProperties { get; set; } = [];
    }
}

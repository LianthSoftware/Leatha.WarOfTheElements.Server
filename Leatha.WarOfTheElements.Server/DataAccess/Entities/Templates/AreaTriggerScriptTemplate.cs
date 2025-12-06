using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;

namespace Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates
{
    public sealed class AreaTriggerScriptTemplate : MongoEntity
    {
        public int AreaTriggerId { get; set; }

        public int EventIndex { get; set; }

        public AreaTriggerActionType ActionType { get; set; }

        public string? TargetNodeName { get; set; }

        public WorldObjectType TargetType { get; set; }

        public float Delay { get; set; }
    }

    public enum AreaTriggerActionType
    {
        None                            = 0,
        ActivateGameObject              = 1,
        NonPlayerStartEvent             = 2,
    }
}

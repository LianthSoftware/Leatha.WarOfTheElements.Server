using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;

namespace Leatha.WarOfTheElements.Server.DataAccess.Entities
{
    public sealed class EventScript : MongoEntity
    {
        public int WorldObjectId { get; set; }

        public WorldObjectType WorldObjectType { get; set; }

        public int Index { get; set; }

        public EventType EventType { get; set; }

        public EventActionType ActionType { get; set; }
    }

    public enum EventType
    {
        None                            = 0,
        Update                          = 1,
        PlayerMovedInRadius             = 2,
        PlayerMovedOutRadius            = 3,
        WaypointReached                 = 4,
    }

    public enum EventActionType
    {
        None                            = 0,
        MoveWaypoints                   = 1,
        Talk                            = 2,
    }
}

using BepuPhysics;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using Leatha.WarOfTheElements.Server.Objects.Characters.Movement;
using Leatha.WarOfTheElements.Server.Scripts.NonPlayers;
using System.Numerics;
using Leatha.WarOfTheElements.Server.Scripts.AreaTriggers;
using Newtonsoft.Json;

namespace Leatha.WarOfTheElements.Server.Objects.AreaTriggers
{
    public sealed class AreaTriggerState
    {
        public WorldObjectId WorldObjectId { get; }

        public int TemplateId { get; set; }

        public string AreaTriggerName { get; set; } = null!;

        public int MapId { get; set; }

        public Guid? InstanceId { get; set; }

        [JsonIgnore]
        public BodyHandle Body { get; private set; }

        [JsonIgnore]
        public AreaTriggerScript? Script { get; private set; }

        public Vector3 Position { get; set; }

        public Quaternion Orientation { get; set; }

        public AreaTriggerState()
        {
        }

        public AreaTriggerState(Guid areaTriggerId, Vector3 position)
        {
            Position = position;

            WorldObjectId = new WorldObjectId(areaTriggerId, WorldObjectType.AreaTrigger);
        }

        public AreaTriggerState(Guid areaTriggerId, Vector3 position, Quaternion orientation)
        {
            Position = position;
            Orientation = orientation;

            WorldObjectId = new WorldObjectId(areaTriggerId, WorldObjectType.AreaTrigger);
        }

        public void SetPhysicsBody(BodyHandle bodyHandle, Vector3 spawnPosition, Quaternion spawnOrientation)
        {
            Body = bodyHandle;

            Position = spawnPosition;
            Orientation = spawnOrientation;
        }

        public void SetScript(AreaTriggerScript script)
        {
            Script = script;
        }
    }
}

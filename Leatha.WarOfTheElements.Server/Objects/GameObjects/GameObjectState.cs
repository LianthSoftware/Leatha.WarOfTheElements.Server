using BepuPhysics;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using Leatha.WarOfTheElements.Server.Objects.Characters.Movement;
using Leatha.WarOfTheElements.Server.Scripts.NonPlayers;
using System.Numerics;
using Newtonsoft.Json;

namespace Leatha.WarOfTheElements.Server.Objects.GameObjects
{
    public sealed class GameObjectState
    {
        public Guid GameObjectId { get; set; }

        public int TemplateId { get; set; }

        public WorldObjectId WorldObjectId { get; }

        public string GameObjectName { get; set; } = null!;

        public string NodeName { get; set; } = null!;

        public int MapId { get; set; }

        public Guid? InstanceId { get; set; }

        public List<AuraObject> Auras { get; set; } = []; // #TODO: Can GameObject have auras?

        [JsonIgnore]
        public StaticHandle StaticBody { get; private set; }

        //[JsonIgnore]
        //public NonPlayerScriptBase? Script { get; private set; }

        public Vector3 Position { get; set; }

        public Quaternion Orientation { get; set; }

        public Vector3 SpawnPosition { get; set; }

        public Quaternion SpawnOrientation { get; set; }

        public GameObjectState()
        {
        }

        public GameObjectState(Guid gameObjectId, Vector3 position)
        {
            GameObjectId = gameObjectId;
            Position = position;

            WorldObjectId = new WorldObjectId(gameObjectId, WorldObjectType.GameObject);
        }

        public GameObjectState(Guid gameObjectId, Vector3 position, Quaternion orientation)
        {
            GameObjectId = gameObjectId;
            Position = position;
            Orientation = orientation;

            WorldObjectId = new WorldObjectId(gameObjectId, WorldObjectType.GameObject);
        }

        public void SetPhysicsBody(StaticHandle staticBody, Vector3 spawnPosition, Quaternion spawnOrientation)
        {
            StaticBody = staticBody;

            SpawnPosition = spawnPosition;
            SpawnOrientation = spawnOrientation;
        }

        //public void SetScript(NonPlayerScriptBase script)
        //{
        //    Script = script;
        //}
    }
}

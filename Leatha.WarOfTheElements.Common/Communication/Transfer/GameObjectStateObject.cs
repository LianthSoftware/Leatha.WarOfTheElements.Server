using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class GameObjectStateObject : IWorldObjectStateObject
    {
        public WorldObjectId WorldObjectId { get; set; }

        public int TemplateId { get; set; }

        public string GameObjectName { get; set; } = null!;

        public int MapId { get; set; }

        public Guid? InstanceId { get; set; }

        public List<AuraObject> Auras { get; set; } = []; // #TODO: Can GameObject have auras?

        public GameObjectStateType StateType { get; set; }

        //[JsonIgnore]
        //public NonPlayerScriptBase? Script { get; private set; }

        public Vector3 Position { get; set; }

        public Quaternion Orientation { get; set; }

        public Vector3 SpawnPosition { get; set; }

        public Quaternion SpawnOrientation { get; set; }
    }


    public enum GameObjectStateType
    {
        NotReady                        = 0,
        Ready                           = 1,
        Activated                       = 2,
    }
}

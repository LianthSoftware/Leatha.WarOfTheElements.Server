using System.Diagnostics;
using Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates;
using Leatha.WarOfTheElements.Server.Objects.Attributes;
using Leatha.WarOfTheElements.Server.Objects.Characters;
using Leatha.WarOfTheElements.Server.Objects.Characters.Movement;

namespace Leatha.WarOfTheElements.Server.Scripts.NonPlayers
{
    [ScriptName("magus_cedrik", ScriptType.NonPlayer)]
    public sealed class MagusCedrikNonPlayerScript : NonPlayerScriptBase
    {
        public MagusCedrikNonPlayerScript(NonPlayerState state, NonPlayerTemplate template) : base(state, template)
        {
        }

        public override async void OnSpawn()
        {
            base.OnSpawn();

            Debug.WriteLine($"Magus Cedrink - OnSpawn.");

            //await Task.Delay(10000);

            State.MotionMaster.MoveWaypoints(new List<WaypointData>
            {
                new WaypointData { PositionX = 5.0f, PositionY = 1.4f, PositionZ = 0.0f, DelayMin = 0, DelayMax = 0 },
                new WaypointData { PositionX = 0.0f, PositionY = 1.4f, PositionZ = 5.0f, DelayMin = 0, DelayMax = 0 },
                new WaypointData { PositionX = -5.0f, PositionY = 1.4f, PositionZ = -5.0f, DelayMin = 0, DelayMax = 0 },
                new WaypointData { PositionX = 0.0f, PositionY = 1.4f, PositionZ = -5.0f, DelayMin = 0, DelayMax = 0 },
            },
            true);

            Debug.WriteLine($"Magus Cedrink - Started Waypoints.");
        }

        public override void OnWaypointReached(int waypointIndex)
        {
            base.OnWaypointReached(waypointIndex);

            Debug.WriteLine($"Magus Cedrink - reached waypoing ({ waypointIndex }).");
        }
    }
}

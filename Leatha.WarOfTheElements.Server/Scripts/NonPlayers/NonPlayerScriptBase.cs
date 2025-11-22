using System.Diagnostics;
using System.Numerics;
using Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates;
using Leatha.WarOfTheElements.Server.Objects.Characters;

namespace Leatha.WarOfTheElements.Server.Scripts.NonPlayers
{
    public abstract class NonPlayerScriptBase
    {
        //protected NonPlayerScriptBase()
        //{
        //}

        protected NonPlayerScriptBase(NonPlayerState state, NonPlayerTemplate template)// : this()
        {
            State = state;
            Template = template;
        }

        public NonPlayerState State { get; init; }

        public NonPlayerTemplate Template { get; init; }

        // #TODO: CharacterState like PlayerState




        public void SetSpeed(float speedWalk)
        {
            SetSpeed(speedWalk, Template.SpeedRun);
        }

        public void SetSpeed(float speedWalk, float speedRun)
        {
            // #TODO: Add it to CharacterState.
        }

        public void Talk(string message, int soundId = 0) // #TODO: Sound Id.
        {
        }

        public void MoveTo(Vector3 position)
        {
        }

        public void MoveWaypoints(List<Vector3> waypoints, bool repeat)
        {

        }




        // *** Virtual Hook Methods ***

        public virtual void OnInitialize()
        {
        }

        public virtual void OnReset()
        {
        }

        public virtual void OnSpawn()
        {
            Debug.WriteLine("NPC added to the world - " + Template.Name);
        }

        public virtual void OnDespawn()
        {
        }

        public virtual void OnUpdate(double delta)
        {
        }

        public virtual void OnWaypointReached(int waypointIndex)
        {
        }
    }
}

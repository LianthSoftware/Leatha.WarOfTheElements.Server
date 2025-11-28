using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates;
using Leatha.WarOfTheElements.Server.Objects.Characters;
using Leatha.WarOfTheElements.Server.Objects.Game;
using System.Diagnostics;
using System.Numerics;

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

        public float DistanceRadius { get; set; } = 10.0f; // #TODO

        public List<ICharacterState> CharactersInDistance { get; } = [];

        private IGameWorld _gameWorld = null!;

        // #TODO: CharacterState like PlayerState



        // #NOTE: Infrastructure ONLY.
        internal void SetGameWorld(IGameWorld gameWorld)
        {
            _gameWorld = gameWorld;
        }




        public void Update(double delta)
        {
            OnUpdate(delta);
        }

        public void SetSpeed(float speedWalk)
        {
            SetSpeed(speedWalk, Template.SpeedRun);
        }

        public void SetSpeed(float speedWalk, float speedRun)
        {
            // #TODO: Add it to CharacterState.
        }

        public void Talk(string message, ChatMessageType messageType, float duration, int soundId = 0) // #TODO: Sound Id.
        {
            _ = _gameWorld.TalkAsync(message, messageType, duration, State);
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



        public virtual void OnPlayerMovedToRadius(PlayerState playerState)
        {
            Debug.WriteLine("PLAYER MOVED TO RADIUS");
        }
    }
}

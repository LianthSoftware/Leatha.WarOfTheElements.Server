using System.Collections.Concurrent;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates;
using Leatha.WarOfTheElements.Server.Objects.AreaTriggers;
using Leatha.WarOfTheElements.Server.Objects.Characters;
using Leatha.WarOfTheElements.Server.Objects.Game;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;

namespace Leatha.WarOfTheElements.Server.Scripts.AreaTriggers
{
    public class AreaTriggerScript
    {
        public AreaTriggerScript(AreaTriggerState state, AreaTriggerTemplate template)// : this()
        {
            State = state;
            Template = template;
        }

        public AreaTriggerState State { get; init; }

        public AreaTriggerTemplate Template { get; init; }

        public List<ICharacterState> CharactersInDistance { get; } = [];

        //public event EventHandler<AreaTriggerTriggeredEventArgs>? Triggered;

        private readonly HashSet<Guid> _alreadyProcessedPlayers = [];
        private IGameWorld _gameWorld = null!;

        private ConcurrentBag<AreaTriggerScriptTemplate> _areaTriggerScripts = [];
        private readonly List<AreaTriggerEventInfo> _delayedScripts = [];



        // #NOTE: Infrastructure ONLY.
        internal void SetGameWorld(IGameWorld gameWorld)
        {
            _gameWorld = gameWorld;
        }

        internal void SetAreaTriggerEvents(List<AreaTriggerScriptTemplate> scripts)
        {
            _areaTriggerScripts = new ConcurrentBag<AreaTriggerScriptTemplate>(scripts);
        }

        public virtual void OnPlayerMovedToRadius(PlayerState playerState)
        {
            if (Template.Flags.HasFlag(AreaTriggerFlags.OneShot) &&
                !_alreadyProcessedPlayers.Add(playerState.WorldObjectId.ObjectId))
            {
                // Add to the list. If already present and flag is set to one-shot, return.
                return;
            }

            Debug.WriteLine("[AreaTrigger]: PLAYER MOVED TO RADIUS");

            if (!_areaTriggerScripts.Any())
                return;

            var scriptTemplates = _areaTriggerScripts.OrderBy(i => i.EventIndex);
            foreach (var template in scriptTemplates)
            {
                var areaTriggerEventInfo = new AreaTriggerEventInfo
                {
                    AreaTriggerId = template.AreaTriggerId,
                    EventIndex = template.EventIndex,
                    Template = template,
                    RemainingDelay = template.Delay,
                };

                if (template.Delay <= 0.0f)
                {
                    _ = ProcessAreaTriggerEventAsync(areaTriggerEventInfo);
                    continue;
                }

                _delayedScripts.Add(areaTriggerEventInfo);
            }

            //Triggered?.Invoke(this, new AreaTriggerTriggeredEventArgs { AreaTriggerState = State });
        }

        public void Update(double fixedDelta)
        {
            foreach (var delayedScript in _delayedScripts.ToList())
            {
                if (delayedScript.RemainingDelay <= 0.0f)
                {
                    _ = ProcessAreaTriggerEventAsync(delayedScript);

                    _delayedScripts.Remove(delayedScript);
                }
                else
                    delayedScript.RemainingDelay -= (float) fixedDelta;
            }
        }

        private async Task ProcessAreaTriggerEventAsync(AreaTriggerEventInfo eventInfo)
        {
            switch (eventInfo.Template.ActionType)
            {
                case AreaTriggerActionType.ActivateGameObject:
                    await HandleGameObjectActivationAsync(eventInfo);
                    break;
            }

            //_gameWorld.SetGameObjectStateAsync();
        }

        private async Task HandleGameObjectActivationAsync(AreaTriggerEventInfo eventInfo)
        {
            if (eventInfo.Template.TargetType != WorldObjectType.GameObject)
                return;

            // Select target.
            var target = _gameWorld
                .GameObjects
                .Select(i => i.Value)
                .SingleOrDefault(i => i.NodeName == eventInfo.Template.TargetNodeName);
            if (target == null)
            {
                Debug.WriteLine($"[AreaTriggerScript][HandleGameObjectActivationAsync]: Target \"{ eventInfo.Template.TargetNodeName }\" was not found.");
                return;
            }

            await _gameWorld.SetGameObjectStateAsync(target.WorldObjectId, GameObjectStateType.Activated);
        }
    }

    //public sealed class AreaTriggerTriggeredEventArgs : EventArgs
    //{
    //    public AreaTriggerState AreaTriggerState { get; set; } = null!;
    //}

    public sealed class AreaTriggerEventInfo
    {
        public int AreaTriggerId { get; set; }

        public int EventIndex { get; set; }

        public AreaTriggerScriptTemplate Template { get; set; } = null!;

        public float RemainingDelay { get; set; }
    }
}

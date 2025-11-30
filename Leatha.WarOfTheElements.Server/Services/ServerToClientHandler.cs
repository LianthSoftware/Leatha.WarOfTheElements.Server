using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Common.Communication.Services;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Server.Objects.Characters;
using Leatha.WarOfTheElements.Server.Objects.Game;
using Leatha.WarOfTheElements.Server.Objects.GameObjects;
using System.Threading;

namespace Leatha.WarOfTheElements.Server.Services
{
    public sealed class ServerToClientHandler : IServerToClientHandler
    {
        public ServerToClientHandler(IGameHubService gameHubService)
        {
            _gameHubService = gameHubService;
        }

        private readonly IGameHubService _gameHubService;

        public Task SendSnapshot(WorldSnapshotMessage message, CancellationToken cancellationToken)
        {
            var groupName = MapGroupName.For(message.MapId, message.InstanceId);
            return _gameHubService.SendMessageToGroup(
                groupName,
                nameof(IServerToClientHandler.SendSnapshot),
                message,
                cancellationToken);
        }

        public Task SendSpellStart(SpellObject spellObject, ICharacterStateObject caster)
        {
            var groupName = MapGroupName.For(caster.MapId, caster.InstanceId);
            return _gameHubService.SendMessageToGroup(
                groupName,
                nameof(IServerToClientHandler.SendSpellStart),
                spellObject);
        }

        public Task SendSpellFinished(SpellObject spellObject, ICharacterStateObject caster)
        {
            var groupName = MapGroupName.For(caster.MapId, caster.InstanceId);
            return _gameHubService.SendMessageToGroup(
                groupName,
                nameof(IServerToClientHandler.SendSpellFinished),
                spellObject);
        }

        public Task SendAuraApply(AuraObject auraObject, ICharacterStateObject target)
        {
            var groupName = MapGroupName.For(target.MapId, target.InstanceId);
            return _gameHubService.SendMessageToGroup(
                groupName,
                nameof(IServerToClientHandler.SendAuraApply),
                auraObject);
        }

        public Task SendAuraRemove(AuraObject auraObject, ICharacterStateObject target)
        {
            var groupName = MapGroupName.For(target.MapId, target.InstanceId);
            return _gameHubService.SendMessageToGroup(
                groupName,
                nameof(IServerToClientHandler.SendAuraRemove),
                auraObject);
        }

        public Task Talk(ChatMessageObject chatMessage, List<Guid> accountIds)
        {
            return Parallel.ForEachAsync(accountIds, async  (accountId, cancellationToken) =>
            {
                await _gameHubService.SendMessageToClient(
                    accountId,
                    nameof(IServerToClientHandler.Talk),
                    chatMessage,
                    cancellationToken);
            });
        }

        public Task SetGameObjectState(SetGameStateMessage message, int mapId, Guid? instanceId)
        {
            var groupName = MapGroupName.For(mapId, instanceId);
            return _gameHubService.SendMessageToGroup(
                groupName,
                nameof(IServerToClientHandler.SetGameObjectState),
                message);
        }

        public Task PlayerEnteredMap(PlayerStateObject playerState)
        {
            var groupName = MapGroupName.For(playerState.MapId, playerState.InstanceId);
            return _gameHubService.SendMessageToGroup(
                groupName,
                nameof(IServerToClientHandler.PlayerEnteredMap),
                playerState);
        }
    }
}

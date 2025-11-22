using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Common.Communication.Services;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Server.Objects.Game;
using System.Threading;
using Leatha.WarOfTheElements.Server.Objects.Characters;

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
    }
}

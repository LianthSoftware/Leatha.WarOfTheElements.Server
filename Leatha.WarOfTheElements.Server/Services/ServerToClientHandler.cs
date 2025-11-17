using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Common.Communication.Services;
using Leatha.WarOfTheElements.Server.Objects.Game;

namespace Leatha.WarOfTheElements.Server.Services
{
    public sealed class ServerToClientHandler : IServerToClientHandler
    {
        public ServerToClientHandler(IGameHubService gameHubService)
        {
            _gameHubService = gameHubService;
        }

        private readonly IGameHubService _gameHubService;

        //public Task SendSnapshot(WorldSnapshotMessage message)
        //{
        //    var groupName = MapGroupName.For(message.MapId, message.InstanceId);
        //    return _gameHubService.SendMessageToGroup(
        //        groupName,
        //        nameof(IServerToClientHandler.SendSnapshot),
        //        message);
        //}

        public Task SendSnapshot(WorldSnapshotMessage message, CancellationToken cancellationToken)
        {
            var groupName = MapGroupName.For(message.MapId, message.InstanceId);
            return _gameHubService.SendMessageToGroup(
                groupName,
                nameof(IServerToClientHandler.SendSnapshot),
                message,
                cancellationToken);
        }
    }
}

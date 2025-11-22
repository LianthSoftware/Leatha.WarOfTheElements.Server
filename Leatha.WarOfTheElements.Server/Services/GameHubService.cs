using Leatha.WarOfTheElements.Server.Objects.Game;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Leatha.WarOfTheElements.Server.Services
{
    public interface IGameHubService
    {
        Task SendMessageToClient<TMessage>(Guid playerId, string actionName, TMessage message);

        Task SendMessageToClient<TMessage>(Guid playerId, string actionName, TMessage message, CancellationToken cancellationToken);

        Task SendMessageToGroup<TMessage>(string groupName, string actionName, TMessage message);

        Task SendMessageToGroup<TMessage>(string groupName, string actionName, TMessage message, CancellationToken cancellationToken);

        bool IsPlayerOnline(Guid playerId);

        Task AddToMapGroup(Guid playerId, int mapId, Guid? instanceId);

        Task RemoveFromMapGroup(Guid playerId, int mapId, Guid? instanceId);
    }

    public sealed class GameHubService : IGameHubService
    {
        public GameHubService(IHubContext<GameHub> hubContext, IPresenceTracker presenceTracker)
        {
            _hubContext = hubContext;
            _presenceTracker = presenceTracker;
        }

        private readonly IHubContext<GameHub> _hubContext;
        private readonly IPresenceTracker _presenceTracker;

        public async Task SendMessageToClient<TMessage>(Guid playerId, string actionName, TMessage message)
        {
            var user = _hubContext
                .Clients
                .User(playerId.ToString());

            await user.SendAsync(actionName, message);
        }

        public async Task SendMessageToClient<TMessage>(Guid playerId, string actionName, TMessage message, CancellationToken cancellationToken)
        {
            var user = _hubContext
                .Clients
                .User(playerId.ToString());

            await user.SendAsync(actionName, message, cancellationToken);
        }

        public async Task SendMessageToGroup<TMessage>(string groupName, string actionName, TMessage message)
        {
            var group = _hubContext
                .Clients
                .Group(groupName);

            await group.SendAsync(actionName, message);
        }

        public async Task SendMessageToGroup<TMessage>(string groupName, string actionName, TMessage message,
            CancellationToken cancellationToken)
        {
            var group = _hubContext
                .Clients
                .Group(groupName);

            await group.SendAsync(actionName, message, cancellationToken);
        }

        public bool IsPlayerOnline(Guid playerId)
        {
            return false; // #TODO
            //return _presenceTracker.IsOnline(playerId.ToString());
        }

        public async Task AddToMapGroup(Guid accountId, int mapId, Guid? instanceId)
        {
            // #TODO: Can there be more connections?
            var connectionId = _presenceTracker.GetConnections(accountId.ToString()).FirstOrDefault();
            if (connectionId == null)
                return;

            var groupName = MapGroupName.For(mapId, instanceId);
            await _hubContext.Groups.AddToGroupAsync(connectionId, groupName);

            ClientOnMapCount++;
            Debug.WriteLine("AddToMapGroup - Clients Count = " + ClientOnMapCount);
        }

        public async Task RemoveFromMapGroup(Guid accountId, int mapId, Guid? instanceId)
        {
            // #TODO: Can there be more connections?
            var connectionId = _presenceTracker.GetConnections(accountId.ToString()).FirstOrDefault();
            if (connectionId == null)
                return;

            var groupName = MapGroupName.For(mapId, instanceId);
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupName);

            ClientOnMapCount--;
            Debug.WriteLine("RemoveFromMapGroup - Clients Count = " + ClientOnMapCount);
        }

        public static int ClientOnMapCount;
    }
}

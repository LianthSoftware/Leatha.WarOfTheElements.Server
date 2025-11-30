using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Common.Communication.Services;
using Leatha.WarOfTheElements.Common.Communication.Transfer;

namespace Leatha.WarOfTheElements.Server.Services
{
    public interface IGameObjectService
    {
        Task SetGameObjectStateAsync(
            GameObjectStateObject gameObject,
            GameObjectStateType stateType,
            Dictionary<string, object> parameters);
    }

    public sealed class GameObjectService : IGameObjectService
    {
        public GameObjectService(IServerToClientHandler clientHandler)
        {
            _clientHandler = clientHandler;
        }

        private readonly IServerToClientHandler _clientHandler;

        public async Task SetGameObjectStateAsync(
            GameObjectStateObject gameObject,
            GameObjectStateType stateType,
            Dictionary<string, object> parameters)
        {
            gameObject.StateType = stateType;

            var message = new SetGameStateMessage
            {
                GameObjectState = gameObject,
                StateParameters = parameters
            };

            await _clientHandler.SetGameObjectState(
                message,
                gameObject.MapId,
                gameObject.InstanceId);
        }
    }
}

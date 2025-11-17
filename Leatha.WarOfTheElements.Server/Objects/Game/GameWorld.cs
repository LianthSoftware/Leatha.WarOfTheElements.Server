using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.Services;
using Leatha.WarOfTheElements.Server.Utilities;
using Leatha.WarOfTheElements.World.Physics;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Leatha.WarOfTheElements.Server.Objects.Game
{
    public interface IGameWorld
    {
        ConcurrentDictionary<Guid, PlayerState> Players { get; }

        PlayerState? GetPlayerState(Guid playerId);

        PlayerState? RemovePlayerState(Guid playerId);

        Task AddPlayerToWorldAsync(PlayerState? state);

        Task RemovePlayerFromWorldAsync(PlayerState? state);
    }

    public sealed class GameWorld : IGameWorld
    {
        public GameWorld(IGameHubService gameHubService, PhysicsWorld physicsWorld, IPlayerService playerService)
        {
            _gameHubService = gameHubService;
            _physicsWorld = physicsWorld;
            _playerService = playerService;
        }

        public ConcurrentDictionary<Guid, PlayerState> Players { get; } = new();

        private readonly IGameHubService _gameHubService;
        private readonly PhysicsWorld _physicsWorld;
        private readonly IPlayerService _playerService;

        public PlayerState? GetPlayerState(Guid playerId)
        {
            return Players.GetValueOrDefault(playerId);
        }

        public PlayerState? RemovePlayerState(Guid playerId)
        {
            if (Players.Remove(playerId, out var state))
                return state;

            return null;
        }

        public async Task AddPlayerToWorldAsync(PlayerState? playerState)
        {
            if (playerState == null)
                return;

            // Add body to physics
            var playerBody = _physicsWorld.AddPlayer(playerState.PlayerId, playerState.Position);
            playerState.SetPhysicsBody(playerBody);

            // Register in the in-memory game world so GameLoop sees it
            Players[playerState.PlayerId] = playerState;

            // Add it to SignalR group.
            await _gameHubService.AddToMapGroup(playerState.PlayerId, playerState.MapId, playerState.InstanceId);
        }

        public async Task RemovePlayerFromWorldAsync(PlayerState? playerState)
        {
            if (playerState == null)
                return;

            playerState.LastProcessedInputSeq = 0;
            playerState.Velocity = Vector3.Zero;

            await _playerService.SavePlayerStateAsync(playerState);

            await _gameHubService.RemoveFromMapGroup(playerState.PlayerId, playerState.MapId, playerState.InstanceId);

            RemovePlayerState(playerState.PlayerId);
            _physicsWorld.RemovePlayer(playerState.PlayerId);
        }

        //public EnvironmentGeometry Environment { get; } = new();
    }
}

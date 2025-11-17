using System.Diagnostics;
using System.Text.Json;
using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Common.Communication.Services;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using Leatha.WarOfTheElements.Server.Objects.Game;
using Leatha.WarOfTheElements.Server.Utilities;
using Leatha.WarOfTheElements.World.Physics;
using Microsoft.AspNetCore.SignalR;
using OneOf.Types;

namespace Leatha.WarOfTheElements.Server.Services
{
    public class GameHub : Hub, IClientToServerHandler
    {
        public GameHub(
            IPresenceTracker presenceTracker,
            IPlayerService playerService,
            ISpellService spellService,
            PhysicsWorld physicsWorld,
            IGameWorld gameWorld,
            IInputQueueService inputQueueService)
        {
            _presenceTracker = presenceTracker;
            _playerService = playerService;
            _spellService = spellService;
            _physicsWorld = physicsWorld;
            _gameWorld = gameWorld;
            _inputQueueService = inputQueueService;
        }

        private readonly IPresenceTracker _presenceTracker;
        private readonly IPlayerService _playerService;
        private readonly ISpellService _spellService;
        private readonly PhysicsWorld _physicsWorld;
        private readonly IGameWorld _gameWorld;
        private readonly IInputQueueService _inputQueueService;

        public static int ClientsCount;

        public override async Task OnConnectedAsync()
        {
            var playerIdClaim = GetPlayerId();
            if (!Guid.TryParse(playerIdClaim, out var playerId))
                return;

            await _presenceTracker.TrackConnected(playerId.ToString(), Context.ConnectionId);
            await base.OnConnectedAsync();

            ClientsCount++;
            Debug.WriteLine("OnConnected - Clients Count = " + ClientsCount);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var playerIdClaim = GetPlayerId();
            if (!Guid.TryParse(playerIdClaim, out var playerId))
                return;

            await ExitWorld(playerId); // #TODO: Is this correct?

            await _presenceTracker.TrackDisconnected(playerId.ToString(), Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);

            ClientsCount--;
            Debug.WriteLine("OnDisconnected - Clients Count = " + ClientsCount);
        }


        public async Task<TransferMessage<PlayerStateObject>> EnterWorld(Guid playerId)
        {
            var playerResult = await _playerService.GetPlayerAsync(playerId);
            if (playerResult.IsError || playerResult.Data == null)
            {
                return TransferMessage.CreateErrorMessage<PlayerStateObject>(
                    playerResult.ErrorTitle!,
                    playerResult.ErrorMessage!);
            }

            var playerState = await _playerService.GetOrCreatePlayerStateAsync(playerId);

            // Reset last processed input.
            playerState.LastProcessedInputSeq = 0;
            await _playerService.SavePlayerStateAsync(playerState);

            //// Add body to physics
            //var playerBody = _physicsWorld.AddPlayer(playerId, playerState.Position);
            //playerState.SetPhysicsBody(playerBody);

            //// Register in the in-memory game world so GameLoop sees it
            //_gameWorld.Players[playerId] = playerState;

            //// Add it to SignalR group.
            //var groupName = MapGroupName.For(playerState.MapId, playerState.InstanceId);
            //await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await _gameWorld.AddPlayerToWorldAsync(playerState);

            // Map to transfer object
            var result = playerState.AsTransferObject();

            Debug.WriteLine($"Player \"{ playerResult.Data.PlayerName }\" ({ playerId }) entered the world.");

            return TransferMessage.CreateMessage(result);
        }

        public async Task<TransferMessage> ExitWorld(Guid playerId)
        {
            var playerResult = await _playerService.GetPlayerAsync(playerId);
            if (playerResult.IsError || playerResult.Data == null)
            {
                return TransferMessage.CreateErrorMessage<PlayerStateObject>(
                    playerResult.ErrorTitle!,
                    playerResult.ErrorMessage!);
            }

            var playerState = _gameWorld.GetPlayerState(playerId);

            await _gameWorld.RemovePlayerFromWorldAsync(playerState);
            //if (state != null)
            //{
            //    await _playerService.SavePlayerStateAsync(state);

            //    var groupName = MapGroupName.For(state.MapId, state.InstanceId);
            //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            //}

            //_physicsWorld.RemovePlayer(playerId);
            //_gameWorld.RemovePlayerState(playerId);

            Debug.WriteLine($"Player \"{playerResult.Data.PlayerName}\" ({playerId}) removed from the world.");

            return TransferMessage.CreateMessage();
        }

        public async Task<TransferMessage> SendPlayerInput(PlayerInputObject playerInput)
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.ffff}] SendPlayerInput: Sequence = {playerInput.Sequence} | Input DTO: {JsonSerializer.Serialize(playerInput)}");
            _inputQueueService.Enqueue(playerInput);
            return TransferMessage.CreateMessage();
        }

        public async Task<TransferMessage<PlayerObject>> GetPlayer(Guid playerId)
        {
            var result = await _playerService.GetPlayerAsync(playerId);

            return CreateMessage(result, result.Data?.AsTransferObject());

            //var result = await _playerService.GetPlayerAsync(playerId);
            //if (result.IsError || result.Data == null)
            //    return TransferMessage.CreateErrorMessage<PlayerObject>(result.ErrorTitle!, result.ErrorMessage!);

            //return TransferMessage.CreateMessage(result.Data.AsTransferObject());
        }

        public async Task<TransferMessage<List<SpellInfoObject>>> GetPlayerSpellBook(Guid playerId)
        {
            var result = await _playerService.GetPlayerSpellBookAsync(playerId);

            var spells = new List<SpellInfoObject>();
            if (result.Data != null)
            {
                // #TODO: Recalculate spells.

                foreach (var spellId in result.Data.LearntSpells)
                {
                    var spellInfo = await _spellService.GetSpellInfoAsync(spellId);
                    if (spellInfo != null)
                        spells.Add(spellInfo);
                }
            }

            return CreateMessage(result, spells);
        }

        public async Task<TransferMessage<List<SpellInfoObject>>> GetPlayerEnhancements(Guid playerId)
        {
            var result = await _playerService.GetPlayerSpellBookAsync(playerId);

            var spells = new List<SpellInfoObject>();
            if (result.Data != null)
            {
                // #TODO: Recalculate spells.

                foreach (var spellId in result.Data.LearntEnhancements)
                {
                    var spellInfo = await _spellService.GetSpellInfoAsync(spellId);
                    if (spellInfo != null)
                        spells.Add(spellInfo);
                }
            }

            return CreateMessage(result, spells);
        }

        public async Task<TransferMessage<SpellCastResult>> CastSpell(Guid casterId, int spellId)
        {
            var result = await _spellService.CastSpellAsync(casterId, spellId);

            return TransferMessage.CreateMessage(result);
        }









        private static TransferMessage<TMessage> CreateMessage<TMessage, TOriginal>(TransferMessage<TOriginal> result, TMessage? data)
        {
            if (result.IsError || result.Data == null || data == null)
                return TransferMessage.CreateErrorMessage<TMessage>(result.ErrorTitle!, result.ErrorMessage!);

            return TransferMessage.CreateMessage(data);
        }

        private static TransferMessage CreateMessage<TOriginal>(TransferMessage<TOriginal> result)
        {
            if (result.IsError || result.Data == null)
                return TransferMessage.CreateErrorMessage(result.ErrorTitle!, result.ErrorMessage!);

            return TransferMessage.CreateMessage();
        }

        private string? GetPlayerId()
        {
            var playerIdClaim = Context.GetHttpContext()?.User.FindFirst("player_id")?.Value;
            return playerIdClaim;
        }
    }
}

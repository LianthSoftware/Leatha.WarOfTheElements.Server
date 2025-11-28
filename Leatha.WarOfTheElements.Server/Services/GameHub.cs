using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Common.Communication.Services;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.Objects.Game;
using Leatha.WarOfTheElements.Server.Utilities;
using Leatha.WarOfTheElements.World.Physics;
using Microsoft.AspNetCore.SignalR;
using OneOf.Types;
using System.Diagnostics;
using System.Numerics;
using System.Text.Json;

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
            var accountIdClaim = GetAccountId();
            if (!Guid.TryParse(accountIdClaim, out var accountId))
                return;

            await _presenceTracker.TrackConnected(accountId.ToString(), Context.ConnectionId);
            await base.OnConnectedAsync();

            ClientsCount++;
            Debug.WriteLine("OnConnected - Clients Count = " + ClientsCount);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var accountIdClaim = GetAccountId();
            if (!Guid.TryParse(accountIdClaim, out var accountId))
                return;

            await ExitWorld(accountId); // #TODO: Is this correct?

            await _presenceTracker.TrackDisconnected(accountId.ToString(), Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);

            ClientsCount--;
            Debug.WriteLine("OnDisconnected - Clients Count = " + ClientsCount);
        }


        public async Task<TransferMessage<List<PlayerObject>>> GetCharacterList(Guid accountId)
        {
            var characters = await _playerService.GetCharacterListAsync(accountId);
            var result = characters.Select(i => i.AsTransferObject()).ToList();

            return TransferMessage.CreateMessage(result);
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

            var playerState = await _playerService.GetOrCreatePlayerStateAsync(
                playerResult.Data.AccountId,
                playerId);

            // Reset last processed input.
            playerState.LastProcessedInputSeq = 0;
            playerState.Velocity = Vector3.Zero;

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

        public async Task<TransferMessage> ExitWorld(Guid accountId)
        {
            
            //var playerResult = await _playerService.GetPlayerAsync(playerId);
            //if (playerResult.IsError || playerResult.Data == null)
            //{
            //    return TransferMessage.CreateErrorMessage<PlayerStateObject>(
            //        playerResult.ErrorTitle!,
            //        playerResult.ErrorMessage!);
            //}

            var playerState = _gameWorld.GetPlayerStateByAccountId(accountId);
            if (playerState == null)
            {
                return TransferMessage.CreateErrorMessage<PlayerStateObject>(
                    "PlayerState was not found.", 
                    $"Player under account \"{ accountId }\" was not found or is not online.");
            }

            await _gameWorld.RemovePlayerFromWorldAsync(playerState);
            //if (state != null)
            //{
            //    await _playerService.SavePlayerStateAsync(state);

            //    var groupName = MapGroupName.For(state.MapId, state.InstanceId);
            //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            //}

            //_physicsWorld.RemovePlayer(playerId);
            //_gameWorld.RemovePlayerState(playerId);

            Debug.WriteLine($"Player \"{ playerState.CharacterName}\" ({ playerState.PlayerId }) removed from the world.");

            return TransferMessage.CreateMessage();
        }

        public Task<TransferMessage> SendPlayerInput(PlayerInputObject playerInput)
        {
            _inputQueueService.Enqueue(playerInput);
            return Task.FromResult(TransferMessage.CreateMessage());
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
            if (result is { IsError: false, Data: not null })
            {
                // #TODO: Recalculate spells.

                foreach (var spellId in result.Data.LearntEnhancements)
                {
                    var spellInfo = await _spellService.GetSpellInfoAsync(spellId);
                    if (spellInfo != null)
                        spells.Add(spellInfo);
                }
            }
            else
                TransferMessage.CreateErrorMessage<List<SpellInfoObject>>(result.ErrorTitle!, result.ErrorMessage!);

            return CreateMessage(result, spells);
        }

        public async Task<TransferMessage<List<SpellInfoObject>>> GetPlayerSpellBarSpells(Guid playerId)
        {
            var result = await _playerService.GetPlayerSpellBookAsync(playerId);

            var spells = new List<SpellInfoObject>();
            if (result is { IsError: false, Data: not null })
            {
                foreach (var spellId in result.Data.SpellBarSpells)
                {
                    var spellInfo = await _spellService.GetSpellInfoAsync(spellId);
                    if (spellInfo != null)
                        spells.Add(spellInfo);
                }
            }
            else
                TransferMessage.CreateErrorMessage<List<SpellInfoObject>>(result.ErrorTitle!, result.ErrorMessage!);

            return CreateMessage(result, spells);
        }

        public async Task<TransferMessage<SpellCastResult>> CastSpell(Guid casterId, int spellId)
        {
            var result = await _spellService.CastSpellAsync(
                new WorldObjectId(casterId, WorldObjectType.Player),
                spellId);

            return CreateMessage(result, result.Data);
        }

        public async Task<int> Test(int data)
        {
            return data;
        }


        private static TransferMessage<TMessage> CreateMessage<TMessage, TOriginal>(TransferMessage<TOriginal> result, TMessage? data)
        {
            if (result.IsError || result.Data == null || data == null)
                return TransferMessage.CreateErrorMessage(data, result.ErrorTitle!, result.ErrorMessage!);

            return TransferMessage.CreateMessage(data);
        }

        private static TransferMessage CreateMessage<TOriginal>(TransferMessage<TOriginal> result)
        {
            if (result.IsError || result.Data == null)
                return TransferMessage.CreateErrorMessage(result.ErrorTitle!, result.ErrorMessage!);

            return TransferMessage.CreateMessage();
        }

        private string? GetAccountId()
        {
            var accountIdClaim = Context.GetHttpContext()?.User.FindFirst("account_id")?.Value;
            return accountIdClaim;
        }
    }
}

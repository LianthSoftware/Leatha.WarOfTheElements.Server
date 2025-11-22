using System.Numerics;
using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.Objects.Validations;
using Leatha.WarOfTheElements.Server.Utilities;
using MongoDB.Driver;
using OneOf.Types;
using OneOf;

namespace Leatha.WarOfTheElements.Server.Services
{
    public interface IPlayerService
    {
        Task<TransferMessage<Player>> GetPlayerAsync(Guid playerId);

        //Task<TransferMessage<Player>> GetPlayerAsync(Guid playerId, int serverId);

        Task<TransferMessage<Player>> CreatePlayerAsync(Player player);

        Task<TransferMessage<PlayerSpellBook>> GetPlayerSpellBookAsync(Guid playerId);

        Task<PlayerState> GetOrCreatePlayerStateAsync(Guid accountId, Guid playerId);

        Task<PlayerState> SavePlayerStateAsync(PlayerState state);

        Task<List<Player>> GetCharacterListAsync(Guid accountId);
    }

    public sealed class PlayerService : IPlayerService
    {
        public PlayerService(IMongoClient client)
        {
            _mongoAuthDatabase = client.GetDatabase(Constants.MongoAuthDb);
            _mongoGameDatabase = client.GetDatabase(Constants.MongoGameDb);
        }

        private readonly IMongoDatabase _mongoAuthDatabase;
        private readonly IMongoDatabase _mongoGameDatabase;

        public async Task<TransferMessage<Player>> GetPlayerAsync(Guid playerId)
        {
            var filter = Builders<Player>.Filter.Eq(i => i.PlayerId, playerId);

            var player = await _mongoAuthDatabase.GetCollection<Player>(nameof(Player))
                .Find(filter)
                .SingleOrDefaultAsync();

            return TransferMessage.CreateMessage(player);
        }

        //public async Task<TransferMessage<Player>> GetPlayerAsync(Guid playerId, int serverId)
        //{
        //    // #TODO
        //    //var validator = new ServerPlayerInfoValidator();
        //    //var validationResult = await validator.ValidateAsync(info);
        //    //if (!validationResult.IsValid)
        //    //{
        //    //    // Bad Request.
        //    //    return new ValidationFailed(validationResult.Errors);
        //    //}

        //    var player = await GetPlayerAsync(playerId);
        //    if (player == null)
        //        return new NotFound();

        //    return player;
        //}

        public async Task<TransferMessage<Player>> CreatePlayerAsync(Player player)
        {
            // #TODO
            //var validator = new PlayerValidator();
            //var validationResult = await validator.ValidateAsync(player);
            //if (!validationResult.IsValid)
            //{
            //    // Bad Request.
            //    return new ValidationFailed(validationResult.Errors);
            //}

            var collection = _mongoAuthDatabase.GetCollection<Player>(nameof(Player));

            var filter = Builders<Player>.Filter.Eq(i => i.PlayerName, player.PlayerName); // #TODO: ToLower()?
            var existingPlayer = await collection
                .Find(filter)
                .SingleOrDefaultAsync();
            if (existingPlayer != null)
            {
                // Already Exists.
                return TransferMessage.CreateErrorMessage<Player>(
                    "Player already exists",
                    $"Player with name \"{existingPlayer.PlayerName}\" already exists.");
            }

            await collection.InsertOneAsync(player);

            return TransferMessage.CreateMessage(player);
        }

        public async Task<TransferMessage<PlayerSpellBook>> GetPlayerSpellBookAsync(Guid playerId)
        {
            var filter = Builders<PlayerSpellBook>.Filter.Eq(i => i.PlayerId, playerId);

            var playerSpellBook = await _mongoGameDatabase.GetCollection<PlayerSpellBook>(nameof(PlayerSpellBook))
                .Find(filter)
                .SingleOrDefaultAsync();

            return TransferMessage.CreateMessage(playerSpellBook);
        }

        public async Task<PlayerState> GetOrCreatePlayerStateAsync(Guid accountId, Guid playerId)
        {
            var filter = Builders<PlayerState>.Filter.Eq(i => i.PlayerId, playerId);

            var playerState = await _mongoGameDatabase.GetCollection<PlayerState>(nameof(PlayerState))
                .Find(filter)
                .SingleOrDefaultAsync();

            if (playerState == null)
            {
                var defaultPosition = new Vector3(0.0f, 1.0f, 0.0f); // #TODO
                var defaultOrientation = Quaternion.Identity;
                return await CreatePlayerStateAsync(accountId, playerId, defaultPosition, defaultOrientation);
            }

            playerState.SetObjectId(playerId);

            return playerState;
        }

        public async Task<PlayerState> SavePlayerStateAsync(PlayerState state)
        {
            var filter = Builders<PlayerState>.Filter.Eq(i => i.PlayerId, state.PlayerId);

            await _mongoGameDatabase.GetCollection<PlayerState>(nameof(PlayerState))
                .ReplaceOneAsync(filter, state);

            return state;
        }

        public async Task<List<Player>> GetCharacterListAsync(Guid accountId)
        {
            var filter = Builders<Player>.Filter.Eq(i => i.AccountId, accountId);
            var playerCharacters = await _mongoAuthDatabase.GetMongoCollection<Player>()
                .Find(filter)
                .ToListAsync();

            return playerCharacters;
        }

        private async Task<PlayerState> CreatePlayerStateAsync(
            Guid accountId,
            Guid playerId,
            Vector3 position,
            Quaternion orientation)
        {
            var playerState = new PlayerState(accountId, playerId, position, orientation)
            {
                MapId = 1, // MapId = 1 -> Default spawn character map.
            };

            await _mongoGameDatabase.GetCollection<PlayerState>(nameof(PlayerState))
                .InsertOneAsync(playerState);

            return playerState;
        }
    }
}

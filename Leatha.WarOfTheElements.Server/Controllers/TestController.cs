using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.Objects.Game;
using Leatha.WarOfTheElements.Server.Services;
using Leatha.WarOfTheElements.Server.Utilities;
using Leatha.WarOfTheElements.World.Physics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Diagnostics;

namespace Leatha.WarOfTheElements.Server.Controllers
{
    [Route("api/test")]
    [ApiController]
    public sealed class TestController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly PhysicsWorld _physicsWorld;
        private readonly IGameWorld _gameWorld;

        public TestController(IMongoClient client, IPlayerService playerService, PhysicsWorld physicsWorld, IGameWorld gameWorld)
        {
            _playerService = playerService;
            _physicsWorld = physicsWorld;
            _gameWorld = gameWorld;
            _mongoGameDatabase = client.GetDatabase(Constants.MongoGameDb);
        }

        private readonly IMongoDatabase _mongoGameDatabase;

        [HttpPost("spellbook/create/{playerId:guid:required}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateTemplatesAsync(Guid playerId)
        {
            var spellBook = new PlayerSpellBook
            {
                PlayerId = playerId,
                LearntSpells = [ 1, 2, 3, 4, 5, 6, 7, 8 ],
                LearntEnhancements = [ 9, 10, 11, 12 ]
            };

            await _mongoGameDatabase.GetMongoCollection<PlayerSpellBook>()
                .InsertOneAsync(spellBook);
            return Ok();
        }


        [HttpPost("player/enter-world/{playerId:guid:required}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> PlayerEnterWorldAsync(Guid playerId)
        {
            var playerResult = await _playerService.GetPlayerAsync(playerId);
            if (playerResult.IsError || playerResult.Data == null)
            {
                return BadRequest(new {
                    playerResult.ErrorTitle,
                    playerResult.ErrorMessage
                });
            }

            var playerState = await _playerService.GetOrCreatePlayerStateAsync(playerId);

            // Add body to physics
            var playerBody = _physicsWorld.AddPlayer(playerId, playerState.Position);
            playerState.SetPhysicsBody(playerBody);

            // Register in the in-memory game world so GameLoop sees it
            _gameWorld.Players[playerId] = playerState;

            // Map to transfer object
            var result = playerState.AsTransferObject();

            Debug.WriteLine($"Player \"{playerResult.Data.PlayerName}\" ({playerId}) entered the world.");

            return Ok(result);
        }

        [HttpPost("player/exit-world/{playerId:guid:required}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> PlayerExitWorldAsync(Guid playerId)
        {
            var playerResult = await _playerService.GetPlayerAsync(playerId);
            if (playerResult.IsError || playerResult.Data == null)
            {
                return BadRequest(new
                {
                    playerResult.ErrorTitle,
                    playerResult.ErrorMessage
                });
            }

            var state = _gameWorld.GetPlayerState(playerId);
            if (state != null)
                await _playerService.SavePlayerStateAsync(state);

            _physicsWorld.RemovePlayer(playerId);
            _gameWorld.RemovePlayerState(playerId);

            Debug.WriteLine($"Player \"{playerResult.Data.PlayerName}\" ({playerId}) removed from the world.");

            return Ok();
        }
    }
}

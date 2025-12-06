using BepuPhysics;
using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Common.Communication.Services;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using Leatha.WarOfTheElements.Common.Communication.Utilities;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.Objects.Characters;
using Leatha.WarOfTheElements.Server.Objects.Game;
using Leatha.WarOfTheElements.Server.Utilities;
using Leatha.WarOfTheElements.World.Physics;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Numerics;
using System.Text.Json;

namespace Leatha.WarOfTheElements.Server.Services
{
    public sealed class GameLoopBackgroundService : BackgroundService
    {
        public GameLoopBackgroundService(
            IInputQueueService inputQueue,
            PhysicsWorld physicsWorld,
            IGameWorld gameWorld)
        {
            _inputQueue = inputQueue;
            _physicsWorld = physicsWorld;
            _gameWorld = gameWorld;
        }

        private const double TickRate = 60.0;
        private const double FixedDt = 1.0 / TickRate;

        private readonly IInputQueueService _inputQueue;
        private readonly PhysicsWorld _physicsWorld;
        private readonly IGameWorld _gameWorld;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var accumulator = 0.0;
            long tick = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                var frameTime = stopwatch.Elapsed.TotalSeconds;
                stopwatch.Restart();
                accumulator += frameTime;

                while (accumulator >= FixedDt)
                {
                    // Process player inputs.
                    _inputQueue.Drain(inputs
                        => _gameWorld.ProcessPlayerInputs(inputs, FixedDt));

                    // Process NPCs.
                    _gameWorld.ProcessNonPlayers(FixedDt);

                    // Process Area Triggers.
                    _gameWorld.ProcessAreaTriggers(FixedDt);

                    // Process Spells.
                    _gameWorld.ProcessSpells(FixedDt);

                    // Process Projectiles.
                    _gameWorld.ProcessProjectiles(FixedDt);

                    // Process Auras.
                    _gameWorld.ProcessAuras(FixedDt);

                    // Process chakra updates.
                    _gameWorld.ProcessChakra(FixedDt);

                    // Process physics.
                    _physicsWorld.Step((float)FixedDt);

                    //if (_gameWorld.Players.Any())
                    //{
                    //    var body = _gameWorld.Players.SingleOrDefault();
                    //    if (_physicsWorld.TryGetPlayerTransform(body.Value.Body, out var pos, out var o, out var isGrounded))
                    //        Debug.WriteLine($"DEBUG PHYS-BEPU POS {pos} | Ori {o} | IsGrounded = {isGrounded}");
                    //}

                    // Must be called AFTER Physics process.
                    _gameWorld.HandleProjectileHits();

                    // Synchronize characters from physics.
                    _gameWorld.SynchronizeCharactersFromPhysics(FixedDt);

                    tick++;
                    accumulator -= FixedDt;
                }

                // Send snapshots.
                await _gameWorld.SendSnapshotAsync(FixedDt, tick, stoppingToken);

                // Tiny sleep to avoid tight busy-wait
                await Task.Delay(1, stoppingToken);
            }
        }
    }
}

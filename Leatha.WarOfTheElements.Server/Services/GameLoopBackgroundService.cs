using BepuPhysics;
using Leatha.WarOfTheElements.Common.Communication.Services;
using Leatha.WarOfTheElements.Server.Objects.Game;
using Leatha.WarOfTheElements.Server.Utilities;
using Leatha.WarOfTheElements.World.Physics;
using System.Diagnostics;
using System.Numerics;
using System.Text.Json;
using Leatha.WarOfTheElements.Common.Communication.Messages;

namespace Leatha.WarOfTheElements.Server.Services
{
    public sealed class GameLoopBackgroundService : BackgroundService
    {
        public GameLoopBackgroundService(
            IInputQueueService inputQueue,
            PhysicsWorld physicsWorld,
            IGameWorld gameWorld,
            IServerToClientHandler serverClientHandler)
        {
            _inputQueue = inputQueue;
            _physicsWorld = physicsWorld;
            _gameWorld = gameWorld;
            _serverClientHandler = serverClientHandler;
        }

        private const double TickRate = 60.0;
        private const double FixedDt = 1.0 / TickRate;

        private readonly IInputQueueService _inputQueue;
        private readonly PhysicsWorld _physicsWorld;
        private readonly IGameWorld _gameWorld;
        private readonly IServerToClientHandler _serverClientHandler;

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
                    // 1) Process all queued inputs
                    _inputQueue.Drain(inputs =>
                    {
                        foreach (var input in inputs)
                        {
                            var playerState = _gameWorld.GetPlayerState(input.PlayerId);
                            if (playerState is null)
                                continue;

                            var desiredVelocity = playerState.ComputeDesiredVelocity(input, FixedDt);

                            var velocity = _physicsWorld.SetPlayerVelocity(
                                input.PlayerId,
                                desiredVelocity,
                                playerState.IsFlying,
                                input.Jump,
                                PlayerState.JumpImpulse,
                                playerState.IsOnGround);

                            // Let client know which input seq we processed last.
                            playerState.LastProcessedInputSeq = input.Sequence;

                            //Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.ffff}] (1): Sequence = { input.Sequence } | Body Velocity = { velocity } | Input DTO: { JsonSerializer.Serialize(input) }");
                        }
                    });

                    // 2) Step physics world
                    _physicsWorld.Step((float)FixedDt);

                    // 3) Sync PlayerState from physics
                    foreach (var kvp in _gameWorld.Players)
                    {
                        var playerState = kvp.Value;

                        if (!_physicsWorld.TryGetPlayerTransform(
                                playerState.Body,
                                out var position,
                                out var orientation,
                                out var grounded))
                        {
                            // This can happen if the body is removed from the world, but still processed.
                            continue;
                        }

                        playerState.Position = position;
                        playerState.Velocity = _physicsWorld.GetPlayerVelocity(playerState.PlayerId);
                        //playerState.Orientation = orientation;
                        playerState.IsOnGround = grounded;

                        playerState.Orientation =
                            Quaternion.CreateFromAxisAngle(Vector3.UnitY, playerState.Yaw);

                        //Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.ffff}] (2): PlayerState: {JsonSerializer.Serialize(playerState)}");
                    }

                    tick++;
                    accumulator -= FixedDt;
                }

                // 4) Build snapshots grouped by (MapId, InstanceId)
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;

                var groups = _gameWorld.Players
                    .Values
                    .GroupBy(p => (p.MapId, p.InstanceId));

                foreach (var group in groups)
                {
                    var (mapId, instanceId) = group.Key;

                    var snapshot = new WorldSnapshotMessage
                    {
                        Tick = tick,
                        ServerTime = now,
                        MapId = mapId,
                        InstanceId = instanceId,
                        Players = group
                            .Select(p => p.AsTransferObject())
                            .ToList()
                    };

                    // Send correct snapshot.
                    await _serverClientHandler.SendSnapshot(snapshot, stoppingToken);
                }

                // Tiny sleep to avoid tight busy-wait
                await Task.Delay(1, stoppingToken);
            }
        }
    }
}

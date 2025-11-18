using BepuPhysics;
using Leatha.WarOfTheElements.Common.Communication.Services;
using Leatha.WarOfTheElements.Server.Objects.Game;
using Leatha.WarOfTheElements.Server.Utilities;
using Leatha.WarOfTheElements.World.Physics;
using System.Diagnostics;
using System.Numerics;
using System.Text.Json;
using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;

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

                            if (input.Jump)
                                Debug.WriteLine("Input JUMP");

                            // Compute desired *horizontal* velocity from input.
                            // Y is controlled by our kinematic controller (jump + gravity).
                            var desiredVelocity = playerState.ComputeDesiredVelocity(input, FixedDt);

                            // Combine kinematic horizontal + kinematic vertical (jump + gravity).
                            // We treat playerState.Velocity as the authoritative gameplay velocity.
                            var newVelocity = _physicsWorld.MovePlayerKinematic(
                                input.PlayerId,
                                playerState.Velocity,   // current gameplay velocity
                                desiredVelocity,        // desired horizontal (X/Z)
                                (float)FixedDt,
                                input.Jump,
                                playerState.IsOnGround, // grounded from previous tick
                                PlayerState.JumpImpulse);

                            playerState.Velocity = newVelocity;

                            if (newVelocity.Y > 0)
                                Debug.WriteLine("Velocity Y > 0 (jump applied)");

                            Debug.WriteLine(
                                $"[{DateTime.Now:HH:mm:ss.ffff}] Input: Seq={input.Sequence} " +
                                $"Desired=<{desiredVelocity.X:0.000},{desiredVelocity.Y:0.000},{desiredVelocity.Z:0.000}> " +
                                $"Vel=<{newVelocity.X:0.000},{newVelocity.Y:0.000},{newVelocity.Z:0.000}>");

                            // For reconciliation on client
                            playerState.LastProcessedInputSeq = input.Sequence;
                        }
                    });

                    // 2) Step physics world (other dynamics, contacts, etc.)
                    _physicsWorld.Step((float)FixedDt);

                    // 3) Sync PlayerState from physics (position/orientation + grounded flag)
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
                        // NOTE: Velocity is managed by our kinematic controller;
                        // do NOT overwrite it with Bepu's internal velocity here.
                        playerState.IsOnGround = grounded;

                        playerState.Orientation =
                            Quaternion.CreateFromAxisAngle(Vector3.UnitY, playerState.Yaw);

                        Debug.WriteLine(
                            $"[PostStep] Pos={playerState.Position} Vel={playerState.Velocity} Grounded={grounded}");
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

                    await _serverClientHandler.SendSnapshot(snapshot, stoppingToken);
                }

                // Tiny sleep to avoid tight busy-wait
                await Task.Delay(1, stoppingToken);
            }
        }
    }
}

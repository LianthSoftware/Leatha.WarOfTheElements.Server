using BepuPhysics;
using Leatha.WarOfTheElements.Common.Communication.Services;
using Leatha.WarOfTheElements.Server.Objects.Game;
using Leatha.WarOfTheElements.Server.Utilities;
using Leatha.WarOfTheElements.World.Physics;
using System.Diagnostics;
using System.Numerics;
using System.Text.Json;
using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using Leatha.WarOfTheElements.Common.Communication.Utilities;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.Objects.Characters;

namespace Leatha.WarOfTheElements.Server.Services
{
    public sealed class GameLoopBackgroundService : BackgroundService
    {
        public GameLoopBackgroundService(
            IInputQueueService inputQueue,
            PhysicsWorld physicsWorld,
            IGameWorld gameWorld,
            IServerToClientHandler serverClientHandler,
            ITemplateService templateService)
        {
            _inputQueue = inputQueue;
            _physicsWorld = physicsWorld;
            _gameWorld = gameWorld;
            _serverClientHandler = serverClientHandler;
            _templateService = templateService;
        }

        private const double TickRate = 60.0;
        private const double FixedDt = 1.0 / TickRate;

        private readonly IInputQueueService _inputQueue;
        private readonly PhysicsWorld _physicsWorld;
        private readonly IGameWorld _gameWorld;
        private readonly IServerToClientHandler _serverClientHandler;
        private readonly ITemplateService _templateService;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var accumulator = 0.0;
            long tick = 0;

            // Initial load. #TODO: Move this elsewhere.
            var nonPlayerSpawnTemplates = await _templateService.GetNonPlayerSpawnTemplatesAsync();
            var templates = nonPlayerSpawnTemplates
                .GroupBy(i => i.NonPlayerId)
                .ToDictionary(i => i.Key, n => n.ToList());
            foreach (var template in templates)
            {
                var nonPlayerTemplate = await _templateService.GetNonPlayerTemplateAsync(template.Key);
                if (nonPlayerTemplate == null)
                {
                    Debug.WriteLine($"NonPlayer template with Id = \"{ template.Key }\" does not exist.");
                    continue;
                }

                foreach (var nonPlayerSpawnTemplate in template.Value)
                {
                    var state = new NonPlayerState(Guid.NewGuid(), nonPlayerSpawnTemplate.SpawnPosition, nonPlayerSpawnTemplate.Orientation)
                    {
                        CharacterName = nonPlayerTemplate.Name,
                        CharacterLevel = nonPlayerTemplate.Level,
                        TemplateId = template.Key,
                        MapId = nonPlayerSpawnTemplate.MapId,
                        InstanceId = nonPlayerSpawnTemplate.InstanceId,
                        Velocity = Vector3.Zero,
                        Resources = new CharacterResourceObject // #TODO: From some other table or non player template?
                        {
                            Health = 333,
                            MaxHealth = 450,
                            PrimaryChakra = new ChakraResource
                            {
                                Element = ElementTypes.Nature,
                                Chakra = 14,
                                MaxChakra = 40,
                                ChakraPerSecond = 10
                            }
                        }
                    };

                    await _gameWorld.AddNonPlayerToWorldAsync(state, nonPlayerSpawnTemplate);

                    state.Script?.OnSpawn();
                }
            }

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

                    // Process Spells.
                    _gameWorld.ProcessSpells(FixedDt);

                    // Process Auras.
                    _gameWorld.ProcessAuras(FixedDt);

                    // Process chakra updates.
                    _gameWorld.ProcessChakra(FixedDt);

                    // Process physics.
                    _physicsWorld.Step((float)FixedDt);

                    // Synchronize characters from physics.
                    _gameWorld.SynchronizeCharactersFromPhysics(FixedDt);

                    tick++;
                    accumulator -= FixedDt;
                }

                // Send snapshots.
                await _gameWorld.SendSnapshotAsync(FixedDt, tick, stoppingToken);

                // 4) Build snapshots grouped by (MapId, InstanceId)
                //var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;

                //var groups = _gameWorld.Players
                //    .Values
                //    .GroupBy(p => (p.MapId, p.InstanceId));

                //foreach (var group in groups)
                //{
                //    var (mapId, instanceId) = group.Key;

                //    // #TODO: Maybe group this before?
                //    var nonPlayers = _gameWorld
                //        .NonPlayers
                //        .Where(i =>
                //            i.Value.MapId == mapId &&
                //            (!instanceId.HasValue || i.Value.InstanceId == instanceId))
                //        .Select(i => i.Value.AsTransferObject())
                //        .ToList();

                //    var snapshot = new WorldSnapshotMessage
                //    {
                //        Tick = tick,
                //        ServerTime = now,
                //        MapId = mapId,
                //        InstanceId = instanceId,
                //        Players = group
                //            .Select(p => p.AsTransferObject())
                //            .ToList(),
                //        NonPlayers = nonPlayers
                //    };

                //    await _serverClientHandler.SendSnapshot(snapshot, stoppingToken);
                //}

                // Tiny sleep to avoid tight busy-wait
                await Task.Delay(1, stoppingToken);
            }
        }
    }
}

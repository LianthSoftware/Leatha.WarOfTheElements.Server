using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Common.Communication.Services;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using Leatha.WarOfTheElements.Common.Communication.Utilities;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates;
using Leatha.WarOfTheElements.Server.Objects.Characters;
using Leatha.WarOfTheElements.Server.Scripts.Auras;
using Leatha.WarOfTheElements.Server.Scripts.Spells;
using Leatha.WarOfTheElements.Server.Services;
using Leatha.WarOfTheElements.Server.Utilities;
using Leatha.WarOfTheElements.World.Physics;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using System.Text.RegularExpressions;
using Leatha.WarOfTheElements.Common.Environment.Collisions;
using Leatha.WarOfTheElements.Server.Objects.GameObjects;
using MongoDB.Driver;

namespace Leatha.WarOfTheElements.Server.Objects.Game
{
    public interface IGameWorld
    {
        ConcurrentDictionary<Guid, PlayerState> Players { get; }

        ConcurrentDictionary<Guid, NonPlayerState> NonPlayers { get; }

        ConcurrentDictionary<Guid, Spell> Spells { get; }

        ConcurrentDictionary<Guid, Aura> Auras { get; }


        void RegisterSpell(SpellObject spellObject, SpellScriptBase? spellScript);

        void RegisterAura(AuraObject spellObject, AuraScriptBase? spellScript);



        Task TalkAsync(string message, ChatMessageType messageType, float duration, ICharacterState state);



        PlayerState? GetPlayerState(Guid playerId);

        PlayerState? RemovePlayerState(Guid playerId);

        ICharacterState? GetCharacterState(WorldObjectId worldObjectId);

        Task AddPlayerToWorldAsync(PlayerState? state);

        Task RemovePlayerFromWorldAsync(PlayerState? state);

        List<ICharacterState> GetCharacters();
        List<ICharacterState> GetCharacters(Vector3 position, float distance);




        NonPlayerState? GetNonPlayerState(Guid nonPlayerId);

        NonPlayerState? RemoveNonPlayerState(Guid nonPlayerId);

        Task AddNonPlayerToWorldAsync(NonPlayerState? state, NonPlayerSpawnTemplate spawnTemplate);

        void RemovePlayerFromWorld(NonPlayerState? state);


        void ProcessPlayerInputs(List<PlayerInputObject> inputs, double fixedDelta);

        void ProcessNonPlayers(double fixedDelta);

        void ProcessChakra(double fixedDelta);

        void ProcessSpells(double fixedDelta);

        void ProcessAuras(double fixedDelta);

        void SynchronizeCharactersFromPhysics(double fixedDelta);

        Task SendSnapshotAsync(double fixedDt, long tick, CancellationToken stoppingToken);
    }

    public sealed class GameWorld : IGameWorld
    {
        public GameWorld(
            IMongoClient mongoClient,
            IGameHubService gameHubService,
            PhysicsWorld physicsWorld,
            IPlayerService playerService,
            IScriptService scriptService,
            IChatService chatService,
            ITemplateService templateService,
            IServerToClientHandler serverClientHandler)
        {
            _mongoGameDatabase = mongoClient.GetDatabase(Constants.MongoGameDb);
            _gameHubService = gameHubService;
            _physicsWorld = physicsWorld;
            _playerService = playerService;
            _scriptService = scriptService;
            _chatService = chatService;
            _templateService = templateService;
            _serverClientHandler = serverClientHandler;
        }

        public ConcurrentDictionary<Guid, PlayerState> Players { get; } = new();

        public ConcurrentDictionary<Guid, NonPlayerState> NonPlayers { get; } = new();

        public ConcurrentDictionary<Guid, GameObjectState> GameObjects { get; } = new();

        public ConcurrentDictionary<Guid, Spell> Spells { get; } = new();

        public ConcurrentDictionary<Guid, Aura> Auras { get; } = new();

        private readonly IMongoDatabase _mongoGameDatabase;
        private readonly IGameHubService _gameHubService;
        private readonly PhysicsWorld _physicsWorld;
        private readonly IPlayerService _playerService;
        private readonly IScriptService _scriptService;
        private readonly IChatService _chatService;
        private readonly ITemplateService _templateService;
        private readonly IServerToClientHandler _serverClientHandler;

        public void RegisterSpell(SpellObject spellObject, SpellScriptBase? spellScript)
        {
            Spells[spellObject.SpellGuid] = new Spell
            {
                SpellGuid = spellObject.SpellGuid,
                SpellObject = spellObject,
                Script = spellScript
            };
        }

        public void RegisterAura(AuraObject auraObject, AuraScriptBase? spellScript)
        {
            Auras[auraObject.AuraGuid] = new Aura
            {
                AuraGuid = auraObject.AuraGuid,
                AuraObject = auraObject,
                Script = spellScript
            };
        }

        public Task TalkAsync(string message, ChatMessageType messageType, float duration, ICharacterState state)
        {
            var distance = 50.0f;
            var players = GetCharacters(state.Position, distance)
                .Where(i => i.WorldObjectId.IsPlayer())
                .ToList();

            return _chatService.TalkAsync(message, messageType, duration, state, players);
            // #TODO: pass it or whatever.
            //var distance = 50.0f;

            //var players = GetCharacters(distance, state.Position);

            //var chatMessage = new ChatMessageObject
            //{
            //    TalkerId = state.WorldObjectId,
            //    TalkerName = state.CharacterName,
            //    MessageType = messageType,
            //    Message = message,
            //    Duration = duration
            //};

            //await _serverClientHandler.Talk(chatMessage,
            //    players
            //        .OfType<PlayerState>()
            //        .Select(i => i.AccountId)
            //        .ToList());
        }

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

        public ICharacterState? GetCharacterState(WorldObjectId worldObjectId)
        {
            if (worldObjectId.IsPlayer())
                return GetPlayerState(worldObjectId.ObjectId);

            if (worldObjectId.IsNonPlayer())
                return GetNonPlayerState(worldObjectId.ObjectId);

            return null;
        }

        public async Task AddPlayerToWorldAsync(PlayerState? playerState)
        {
            if (playerState == null)
                return;

            if (!_loadedMaps.ContainsKey(playerState.MapId))
            {
                // Load map.
                await LoadMapAsync(playerState.MapId);
            }

            // Add body to physics
            var playerBody = _physicsWorld.AddPlayer(playerState.PlayerId, playerState.Position);
            playerState.SetPhysicsBody(playerBody);

            // Register in the in-memory game world so GameLoop sees it
            Players[playerState.PlayerId] = playerState;

            // Add it to SignalR group.
            await _gameHubService.AddToMapGroup(playerState.AccountId, playerState.MapId, playerState.InstanceId);
        }

        private Dictionary<int, LoadedMapInfo> _loadedMaps = [];

        private async Task LoadMapAsync(int mapId)
        {
            // Load static environment game objects.
            {
                var filter = Builders<EnvironmentInstance>.Filter.Eq(i => i.MapId, mapId);

                var envStaticInstances = await _mongoGameDatabase.GetMongoCollection<EnvironmentInstance>()
                    .Find(filter)
                    .ToListAsync();

                Debug.WriteLine($"Found \"{ envStaticInstances.Count }\" instances.");

                // #TODO: Load it.
                foreach (var instance in envStaticInstances)
                {
                    _physicsWorld.AddEnvironmentObject(instance.AsTransferObject());
                }
            }

            // Non Players.
            {
                var nonPlayerSpawnTemplates = await _templateService.GetNonPlayerSpawnTemplatesAsync();
                var templates = nonPlayerSpawnTemplates
                    .Where(i => i.MapId == mapId)
                    .GroupBy(i => i.NonPlayerId)
                    .ToDictionary(i => i.Key, n => n.ToList());
                foreach (var template in templates)
                {
                    var nonPlayerTemplate = await _templateService.GetNonPlayerTemplateAsync(template.Key);
                    if (nonPlayerTemplate == null)
                    {
                        Debug.WriteLine($"NonPlayer template with Id = \"{template.Key}\" does not exist.");
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
                            },
                        };

                        await AddNonPlayerToWorldAsync(state, nonPlayerSpawnTemplate);

                        state.Script?.OnSpawn();
                    }
                }
            }

            // GameObjects.
            {
                var gameObjectTemplates = await _templateService.GetGameObjectSpawnTemplatesAsync();
                var templates = gameObjectTemplates
                    .Where(i => i.MapId == mapId)
                    .GroupBy(i => i.GameObjectId)
                    .ToDictionary(i => i.Key, n => n.ToList());

                foreach (var template in templates)
                {
                    var gameObjectTemplate = await _templateService.GetGameObjectTemplateAsync(template.Key);
                    if (gameObjectTemplate == null)
                    {
                        Debug.WriteLine($"GameObject template with Id = \"{template.Key}\" does not exist.");
                        continue;
                    }

                    foreach (var gameObjectSpawnTemplate in template.Value)
                    {
                        var state = new GameObjectState(Guid.NewGuid(), gameObjectSpawnTemplate.SpawnPosition, gameObjectSpawnTemplate.Orientation)
                        {
                            GameObjectName = gameObjectTemplate.Name,
                            TemplateId = template.Key,
                            MapId = gameObjectSpawnTemplate.MapId,
                        };

                        await AddGameObjectToWorldAsync(state, gameObjectSpawnTemplate);

                        //state.Script?.OnSpawn();
                    }
                }
            }
        }

        public sealed class LoadedMapInfo
        {
            public int MapId { get; set; }

            public Guid? InstanceId { get; set; }
        }

        public async Task RemovePlayerFromWorldAsync(PlayerState? playerState)
        {
            if (playerState == null)
                return;

            playerState.LastProcessedInputSeq = 0;
            playerState.Velocity = Vector3.Zero;

            await _playerService.SavePlayerStateAsync(playerState);

            await _gameHubService.RemoveFromMapGroup(playerState.AccountId, playerState.MapId, playerState.InstanceId);

            RemovePlayerState(playerState.PlayerId);
            _physicsWorld.RemovePlayer(playerState.PlayerId);
        }

        public NonPlayerState? GetNonPlayerState(Guid nonPlayerId)
        {
            return NonPlayers.GetValueOrDefault(nonPlayerId);
        }

        public NonPlayerState? RemoveNonPlayerState(Guid nonPlayerId)
        {
            if (NonPlayers.Remove(nonPlayerId, out var state))
                return state;

            return null;
        }

        public async Task AddNonPlayerToWorldAsync(NonPlayerState? state, NonPlayerSpawnTemplate spawnTemplate)
        {
            if (state == null)
                return;

            // Add body to physics
            var playerBody = _physicsWorld.AddNonPlayer(state.NonPlayerId, state.Position, state.Orientation);
            state.SetPhysicsBody(playerBody, spawnTemplate.SpawnPosition, spawnTemplate.Orientation);

            var script = await _scriptService.CreateScriptAsync(state);
            if (script != null)
            {
                script.SetGameWorld(this);
                state.SetScript(script);
            }

            // Register in the in-memory game world so GameLoop sees it
            NonPlayers[state.NonPlayerId] = state;
        }

        public async Task AddGameObjectToWorldAsync(GameObjectState? state, GameObjectSpawnTemplate spawnTemplate)
        {
            if (state == null)
                return;

            // Add body to physics
            var staticBody = _physicsWorld.AddStaticObject(state.GameObjectId, state.Position, state.Orientation);
            state.SetPhysicsBody(staticBody, spawnTemplate.SpawnPosition, spawnTemplate.Orientation);

            //var script = await _scriptService.CreateScriptAsync(state);
            //if (script != null)
            //{
            //    script.SetGameWorld(this);
            //    state.SetScript(script);
            //}

            // Register in the in-memory game world so GameLoop sees it
            GameObjects[state.GameObjectId] = state;
        }

        public void RemovePlayerFromWorld(NonPlayerState? state)
        {
            if (state == null)
                return;

            RemoveNonPlayerState(state.NonPlayerId);
            _physicsWorld.RemoveNonPlayer(state.NonPlayerId);
        }

        public void ProcessPlayerInputs(List<PlayerInputObject> inputs, double fixedDelta)
        {
            foreach (var input in inputs)
            {
                var playerState = GetPlayerState(input.PlayerId);
                if (playerState is null)
                    continue;

                //if (input.Jump)
                //    Debug.WriteLine("Input JUMP");

                // Compute desired *horizontal* velocity from input.
                // Y is controlled by our kinematic controller (jump + gravity).
                var desiredVelocity = playerState.ComputeDesiredVelocity(input, fixedDelta);

                // Combine kinematic horizontal + kinematic vertical (jump + gravity).
                // We treat playerState.Velocity as the authoritative gameplay velocity.
                var newVelocity = _physicsWorld.MovePlayerKinematic(
                    input.PlayerId,
                    playerState.Velocity,   // current gameplay velocity
                    desiredVelocity,        // desired horizontal (X/Z)
                    (float)fixedDelta,
                    input.Jump,
                    playerState.IsOnGround, // grounded from previous tick
                    PlayerState.JumpImpulse);

                playerState.Velocity = newVelocity;

                //if (newVelocity.Y > 0)
                //    Debug.WriteLine("Velocity Y > 0 (jump applied)");

                //Debug.WriteLine(
                //    $"[{DateTime.Now:HH:mm:ss.ffff}] Input: Seq={input.Sequence} " +
                //    $"Desired=<{desiredVelocity.X:0.000},{desiredVelocity.Y:0.000},{desiredVelocity.Z:0.000}> " +
                //    $"Vel=<{newVelocity.X:0.000},{newVelocity.Y:0.000},{newVelocity.Z:0.000}>");

                // For reconciliation on client
                playerState.LastProcessedInputSeq = input.Sequence;
            }
        }

        public void ProcessNonPlayers(double fixedDelta)
        {
            foreach (var kvp in NonPlayers)
            {
                var nonPlayerState = kvp.Value;
                //if (nonPlayerState == null)
                //    continue;

                //if (!_gameWorld.Players.Any())
                //    continue; // TODO: temporary

                // 1) AI script decides what to do this tick (MoveTo, etc.)
                nonPlayerState.Script?.Update(fixedDelta);

                // 2) MotionMaster converts AI decisions into local input (Velocity)
                nonPlayerState.MotionMaster.Update(fixedDelta);

                // 3) Convert local input + yaw into world-space desired velocity
                var desiredVelocity = nonPlayerState.ComputeDesiredVelocity(fixedDelta);

                // 4) Apply kinematic movement in physics
                var newVelocity = _physicsWorld.MoveNonPlayerKinematic(
                    kvp.Key,
                    nonPlayerState.Velocity,    // current gameplay velocity
                    desiredVelocity,            // desired horizontal (X/Z)
                    (float)fixedDelta,
                    false,                      // jump (not used yet)
                    nonPlayerState.IsOnGround,  // grounded from previous tick
                    PlayerState.JumpImpulse);   // you can also define a NonPlayer jump if needed

                nonPlayerState.Velocity = newVelocity;

                // #TODO: Maybe do without script and store in NonPlayerState?
                if (nonPlayerState.Script != null)
                {
                    var characters = Players.Values
                        .Where(i =>
                            Vector3.Distance(i.Position, nonPlayerState.Position) <= nonPlayerState.Script.DistanceRadius)
                        .ToList();

                    foreach (var character in characters)
                    {
                        var foundCharacter = nonPlayerState.Script.CharactersInDistance.SingleOrDefault();
                        if (foundCharacter == null)
                            nonPlayerState.Script.OnPlayerMovedToRadius(character);
                    }

                    nonPlayerState.Script.CharactersInDistance.Clear();
                    nonPlayerState.Script.CharactersInDistance.AddRange(characters);
                }


                //if (nonPlayerState.WorldObjectId.IsNonPlayer())
                //    Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.ffff}] New Velocity = {newVelocity} | Desired = {desiredVelocity}");
            }
        }

        public void ProcessChakra(double fixedDelta)
        {
            var characters = GetCharacters();

            foreach (var kvp in characters)
            {
                // Process each chakra.
                ProcessChakra(kvp.Resources.PrimaryChakra, fixedDelta);
                ProcessChakra(kvp.Resources.SecondaryChakra, fixedDelta);
                ProcessChakra(kvp.Resources.TertiaryChakra, fixedDelta);
            }
        }

        private void ProcessChakra(ChakraResource resource, double fixedDelta)
        {
            if (resource.ChakraPerSecond <= 0f)
                return;

            if (resource.Chakra >= resource.MaxChakra)
                return;

            // Add proportional chakra
            var calculatedChakra = resource.ChakraPerSecond * (float)fixedDelta + resource.Chakra;
            //resource.Chakra += calculatedChakra;

            // Clamp to max
            if (calculatedChakra > resource.MaxChakra)
                calculatedChakra = resource.MaxChakra;

            resource.Chakra = calculatedChakra;
        }


        public void ProcessSpells(double fixedDelta)
        {
            foreach (var kvp in Spells)
            {
                var spell = kvp.Value;
                spell.SpellObject.RemainingCastTime -= (float)fixedDelta * 1000;
                if (spell.SpellObject is { RemainingCastTime: <= 0.0f, IsCastFinished: false })
                {
                    spell.SpellObject.IsCastFinished = true;

                    _serverClientHandler.SendSpellFinished(
                        spell.SpellObject,
                        spell.SpellObject.Caster);
                }

                spell.Script?.OnUpdate(fixedDelta);
            }
        }

        public void ProcessAuras(double fixedDelta) // #TODO: Should not be task?
        {
            foreach (var kvp in Auras)
            {
                kvp.Value.Script?.OnUpdate(fixedDelta);

                kvp.Value.AuraObject.RemainingDuration -= (float)fixedDelta * 1000.0f;
                if (kvp.Value.AuraObject.RemainingDuration <= 0.0f)
                {
                    // Send aura remove.
                    _ = RemoveAuraAsync(kvp.Value);
                }
            }
        }

        private async Task RemoveAuraAsync(Aura aura)
        {
            Debug.WriteLine($"[RemoveAura]: Trying to remove aura \"{ aura.AuraGuid }\".");

            // Send aura remove.
            await _serverClientHandler.SendAuraRemove(
                aura.AuraObject,
                aura.AuraObject.Target);

            if (!Auras.TryRemove(aura.AuraGuid, out _))
                Debug.WriteLine($"---> Could not remove aura \"{ aura.AuraGuid }\".");
        }

        public void SynchronizeCharactersFromPhysics(double fixedDelta)
        {
            var characters = GetCharacters();

            // 3) Sync PlayerState from physics (position/orientation + grounded flag)
            //foreach (var kvp in _gameWorld.Players)
            foreach (var kvp in characters)
            {
                //var playerState = kvp.Value;
                var playerState = kvp;

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

                //if (playerState.WorldObjectId.IsNonPlayer())
                //    Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.ffff}] Position = {playerState.Position}");

                //Debug.WriteLine(
                //    $"[PostStep] Pos={playerState.Position} Vel={playerState.Velocity} Grounded={grounded}");
            }
        }

        public async Task SendSnapshotAsync(double fixedDt, long tick, CancellationToken stoppingToken)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;

            var groups = Players
                .Values
                .GroupBy(p => (p.MapId, p.InstanceId));

            foreach (var group in groups)
            {
                var (mapId, instanceId) = group.Key;

                // #TODO: Maybe group this before?
                var nonPlayers = NonPlayers
                    .Where(i =>
                        i.Value.MapId == mapId &&
                        (!instanceId.HasValue || i.Value.InstanceId == instanceId))
                    .Select(i => i.Value.AsTransferObject())
                    .ToList();

                var snapshot = new WorldSnapshotMessage
                {
                    Tick = tick,
                    ServerTime = now,
                    MapId = mapId,
                    InstanceId = instanceId,
                    Players = group
                        .Select(p => p.AsTransferObject())
                        .ToList(),
                    NonPlayers = nonPlayers
                };

                await _serverClientHandler.SendSnapshot(snapshot, stoppingToken);
            }
        }

        public List<ICharacterState> GetCharacters()
        {
            var characters = new List<ICharacterState>();
            characters.AddRange(Players.Values);
            characters.AddRange(NonPlayers.Values);

            return characters;
        }

        public List<ICharacterState> GetCharacters(Vector3 position, float distance)
        {
            var characters = new List<ICharacterState>();
            characters.AddRange(Players.Values);
            characters.AddRange(NonPlayers.Values);

            if (distance > 0.0f)
            {
                characters = characters
                    .Where(i =>
                        Vector3.Distance(i.Position, position) <= distance)
                    .ToList();
            }

            return characters;
        }

        private List<PlayerState> GetPlayers(Vector3 position, float distance)
        {
            var players = new List<PlayerState>();
            players.AddRange(Players.Values);

            if (distance > 0.0f)
            {
                players = players
                    .Where(i =>
                        Vector3.Distance(i.Position, position) <= distance)
                    .ToList();
            }

            return players;
        }

        //public EnvironmentGeometry Environment { get; } = new();
    }

    public sealed class Spell
    {
        public Guid SpellGuid { get; set; }

        public SpellObject SpellObject { get; set; } = null!;

        public SpellScriptBase? Script { get; set; }
    }

    public sealed class Aura
    {
        public Guid AuraGuid { get; set; }

        public AuraObject AuraObject { get; set; } = null!;

        public AuraScriptBase? Script { get; set; }
    }
}

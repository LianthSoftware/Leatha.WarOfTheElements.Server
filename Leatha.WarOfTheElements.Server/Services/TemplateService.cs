using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates;
using Leatha.WarOfTheElements.Server.Demo;
using Leatha.WarOfTheElements.Server.Objects.Game;
using Leatha.WarOfTheElements.Server.Utilities;
using MongoDB.Driver;
using System.Collections.Concurrent;
using System.Numerics;

namespace Leatha.WarOfTheElements.Server.Services
{
    public interface ITemplateService
    {
        Task<List<MapTemplate>> GetMapTemplatesAsync();

        Task<MapTemplate?> GetMapTemplateAsync(int mapId);


        Task<List<SpellTemplate>> GetSpellTemplatesAsync();

        Task<SpellTemplate?> GetSpellTemplateAsync(int spellId);


        Task<List<AuraTemplate>> GetAuraTemplatesAsync();

        Task<AuraTemplate?> GetAuraTemplateAsync(int auraId);


        Task<List<NonPlayerTemplate>> GetNonPlayerTemplatesAsync();

        Task<NonPlayerTemplate?> GetNonPlayerTemplateAsync(int nonPlayerId);


        Task<List<NonPlayerSpawnTemplate>> GetNonPlayerSpawnTemplatesAsync();

        Task<List<NonPlayerSpawnTemplate>> GetNonPlayerSpawnTemplatesAsync(int nonPlayerId);



        Task ReloadTemplatesAsync();




        Task CreateTemplatesAsync();

        Task<List<SpellTemplate>> CreateSpellTemplatesAsync();

        Task<List<AuraTemplate>> CreateAuraTemplatesAsync();

        Task<List<MapTemplate>> CreateMapTemplatesAsync();

        Task<List<NonPlayerTemplate>> CreateNonPlayerTemplatesAsync();

        Task<List<NonPlayerSpawnTemplate>> CreateNonPlayerSpawnTemplatesAsync();
    }

    public sealed class TemplateService : ITemplateService
    {
        public TemplateService(IMongoClient client)
        {
            _mongoGameDatabase = client.GetDatabase(Constants.MongoGameDb);
        }

        private readonly IMongoDatabase _mongoGameDatabase;

        private readonly ConcurrentBag<MapTemplate> _mapTemplates = [];
        private readonly ConcurrentBag<SpellTemplate> _spellTemplates = [];
        private readonly ConcurrentBag<AuraTemplate> _auraTemplates = [];
        private readonly ConcurrentBag<NonPlayerTemplate> _nonPlayerTemplates = [];
        private readonly ConcurrentBag<NonPlayerSpawnTemplate> _nonPlayerSpawnTemplates = [];

        public async Task<List<MapTemplate>> GetMapTemplatesAsync()
        {
            var filter = Builders<MapTemplate>.Filter.Empty;
            var templates = await _mongoGameDatabase.GetMongoCollection<MapTemplate>()
                .Find(filter)
                .ToListAsync();

            _mapTemplates.Clear();
            foreach (var template in templates)
                _mapTemplates.Add(template);

            return templates;
        }

        public async Task<MapTemplate?> GetMapTemplateAsync(int mapId)
        {
            // First, try to get from cache.
            var cacheTemplate = _mapTemplates.SingleOrDefault(i => i.MapId == mapId);
            if (cacheTemplate != null)
                return cacheTemplate;

            var filter = Builders<MapTemplate>.Filter.Eq(i => i.MapId, mapId);
            var template = await _mongoGameDatabase.GetMongoCollection<MapTemplate>()
                .Find(filter)
                .SingleOrDefaultAsync();

            // Add to cache.
            if (template != null)
                _mapTemplates.Add(template);

            return template;
        }

        public async Task<List<SpellTemplate>> GetSpellTemplatesAsync()
        {
            var filter = Builders<SpellTemplate>.Filter.Empty;
            var templates = await _mongoGameDatabase.GetMongoCollection<SpellTemplate>()
                .Find(filter)
                .ToListAsync();

            _spellTemplates.Clear();
            foreach (var template in templates)
                _spellTemplates.Add(template);

            return templates;
        }

        public async Task<SpellTemplate?> GetSpellTemplateAsync(int spellId)
        {
            // First, try to get from cache.
            var cacheTemplate = _spellTemplates.SingleOrDefault(i => i.SpellId == spellId);
            if (cacheTemplate != null)
                return cacheTemplate;

            var filter = Builders<SpellTemplate>.Filter.Eq(i => i.SpellId, spellId);
            var template = await _mongoGameDatabase.GetMongoCollection<SpellTemplate>()
                .Find(filter)
                .SingleOrDefaultAsync();

            // Add to cache.
            if (template != null)
                _spellTemplates.Add(template);

            return template;
        }

        public async Task<List<AuraTemplate>> GetAuraTemplatesAsync()
        {
            var filter = Builders<AuraTemplate>.Filter.Empty;
            var templates = await _mongoGameDatabase.GetMongoCollection<AuraTemplate>()
                .Find(filter)
                .ToListAsync();

            _auraTemplates.Clear();
            foreach (var template in templates)
                _auraTemplates.Add(template);

            return templates;
        }

        public async Task<AuraTemplate?> GetAuraTemplateAsync(int auraId)
        {
            // First, try to get from cache.
            var cacheTemplate = _auraTemplates.SingleOrDefault(i => i.AuraId == auraId);
            if (cacheTemplate != null)
                return cacheTemplate;

            var filter = Builders<AuraTemplate>.Filter.Eq(i => i.AuraId, auraId);
            var template = await _mongoGameDatabase.GetMongoCollection<AuraTemplate>()
                .Find(filter)
                .SingleOrDefaultAsync();

            // Add to cache.
            if (template != null)
                _auraTemplates.Add(template);

            return template;
        }

        public async Task<List<NonPlayerTemplate>> GetNonPlayerTemplatesAsync()
        {
            var filter = Builders<NonPlayerTemplate>.Filter.Empty;
            var templates = await _mongoGameDatabase.GetMongoCollection<NonPlayerTemplate>()
                .Find(filter)
                .ToListAsync();

            _nonPlayerTemplates.Clear();
            foreach (var template in templates)
                _nonPlayerTemplates.Add(template);

            return templates;
        }

        public async Task<NonPlayerTemplate?> GetNonPlayerTemplateAsync(int nonPlayerId)
        {
            // First, try to get from cache.
            var cacheTemplate = _nonPlayerTemplates.SingleOrDefault(i => i.NonPlayerId == nonPlayerId);
            if (cacheTemplate != null)
                return cacheTemplate;

            var filter = Builders<NonPlayerTemplate>.Filter.Eq(i => i.NonPlayerId, nonPlayerId);
            var template = await _mongoGameDatabase.GetMongoCollection<NonPlayerTemplate>()
                .Find(filter)
                .SingleOrDefaultAsync();

            // Add to cache.
            if (template != null)
                _nonPlayerTemplates.Add(template);

            return template;
        }

        public async Task<List<NonPlayerSpawnTemplate>> GetNonPlayerSpawnTemplatesAsync()
        {
            var filter = Builders<NonPlayerSpawnTemplate>.Filter.Empty;
            var templates = await _mongoGameDatabase.GetMongoCollection<NonPlayerSpawnTemplate>()
                .Find(filter)
                .ToListAsync();

            _nonPlayerSpawnTemplates.Clear();
            foreach (var template in templates)
                _nonPlayerSpawnTemplates.Add(template);

            return templates;
        }

        public async Task<List<NonPlayerSpawnTemplate>> GetNonPlayerSpawnTemplatesAsync(int nonPlayerId)
        {
            var filter = Builders<NonPlayerSpawnTemplate>.Filter.Eq(i => i.NonPlayerId, nonPlayerId);
            var templates = await _mongoGameDatabase.GetMongoCollection<NonPlayerSpawnTemplate>()
                .Find(filter)
                .ToListAsync();

            return templates;
        }

        public async Task ReloadTemplatesAsync()
        {
            // Cache is cleared inside each call.
            {
                await GetMapTemplatesAsync();
                await GetSpellTemplatesAsync();
                await GetAuraTemplatesAsync();
                await GetNonPlayerTemplatesAsync();
                await GetNonPlayerSpawnTemplatesAsync();
            }
        }

        public async Task CreateTemplatesAsync()
        {
            await CreateMapTemplatesAsync();
            await CreateSpellTemplatesAsync();
            await CreateAuraTemplatesAsync();
            await CreateNonPlayerTemplatesAsync();
            await CreateNonPlayerSpawnTemplatesAsync();
        }

        public async Task<List<SpellTemplate>> CreateSpellTemplatesAsync()
        {
            var templates = new List<SpellTemplate>
            {
                new SpellTemplate
                {
                    SpellId = 1,
                    SpellName = "Water Bolt",
                    SpellDescription = "Hurls a water bolt to your direction. Deals 50 damage to the target it hits.",
                    SpellRank = new SpellRank
                    {
                        SpellId = 1,
                        Rank = 1,
                        FirstRankSpellId = 1,
                        PreviousRankSpellId = null,
                        NextRankSpellId = null,
                    },
                    CastTime = 1500,
                    Cooldown = 0,
                    Duration = 0,
                    ElementTypes = ElementTypes.Water,
                    SpellIconPath = "res://resources/textures/spells/spell_water_water_bolt.jpg",
                },
                new SpellTemplate
                {
                    SpellId = 2,
                    SpellName = "Rain Dance",
                    SpellDescription = "Starts a rain at the targeted location for 20 seconds. Deals damage to enemies within 10 meters.",
                    SpellRank = new SpellRank
                    {
                        SpellId = 2,
                        Rank = 1,
                        FirstRankSpellId = 2,
                        PreviousRankSpellId = null,
                        NextRankSpellId = null,
                    },
                    CastTime = 3000,
                    Cooldown = 60000,
                    Duration = 20000,
                    ElementTypes = ElementTypes.Water,
                    SpellIconPath = "res://resources/textures/spells/spell_water_water_rain.jpg",
                },
                new SpellTemplate
                {
                    SpellId = 3,
                    SpellName = "Water Wave",
                    SpellDescription = "Hits all targets in the 90° cone, deals 40 to them and knocks them back.",
                    SpellRank = new SpellRank
                    {
                        SpellId = 3,
                        Rank = 1,
                        FirstRankSpellId = 3,
                        PreviousRankSpellId = null,
                        NextRankSpellId = null,
                    },
                    CastTime = 500,
                    Cooldown = 10000,
                    Duration = 0,
                    ElementTypes = ElementTypes.Water,
                    SpellIconPath = "res://resources/textures/spells/spell_water_water_wave.jpg",
                },
                new SpellTemplate
                {
                    SpellId = 4,
                    SpellName = "Water Clone",
                    SpellDescription = "Creates 1 clone of yourself. Give half your maximum mana and health to your copies. Lasts 1 minute.",
                    SpellRank = new SpellRank
                    {
                        SpellId = 4,
                        Rank = 1,
                        FirstRankSpellId = 4,
                        PreviousRankSpellId = null,
                        NextRankSpellId = null,
                    },
                    CastTime = 0,
                    Cooldown = 120000,
                    Duration = 60000,
                    ElementTypes = ElementTypes.Water,
                    SpellIconPath = "res://resources/textures/spells/spell_water_water_clone.jpg",
                },
                new SpellTemplate
                {
                    SpellId = 5,
                    SpellName = "Water Dragon",
                    SpellDescription = "Conjures a water dragon that charges at the enemies, deals 30 damage to them and slows them by 25%. Maximum 5 targets.",
                    SpellRank = new SpellRank
                    {
                        SpellId = 5,
                        Rank = 1,
                        FirstRankSpellId = 5,
                        PreviousRankSpellId = null,
                        NextRankSpellId = null,
                    },
                    CastTime = 2000,
                    Cooldown = 300000,
                    Duration = 0,
                    ElementTypes = ElementTypes.Water,
                    SpellIconPath = "res://resources/textures/spells/spell_water_water_dragon.jpg",
                },
                new SpellTemplate
                {
                    SpellId = 6,
                    SpellName = "Water Prison",
                    SpellDescription = "Encapsulates the target in the water prison, dealing 5% of the target's maximum health every second. Lasts for 10 seconds or until canceled. Must be channeled.",
                    SpellRank = new SpellRank
                    {
                        SpellId = 6,
                        Rank = 1,
                        FirstRankSpellId = 6,
                        PreviousRankSpellId = null,
                        NextRankSpellId = null,
                    },
                    CastTime = 1500,
                    Cooldown = 45000,
                    Duration = 10000,
                    ElementTypes = ElementTypes.Water,
                    SpellFlags = SpellFlags.IsChanneled,
                    SpellIconPath = "res://resources/textures/spells/spell_water_water_prison.jpg",
                },
                new SpellTemplate
                {
                    SpellId = 7,
                    SpellName = "Water Slide",
                    SpellDescription = "Creates a water path in front of a caster increasing movement speed by 40% for all friendly characters in the slide.",
                    SpellRank = new SpellRank
                    {
                        SpellId = 7,
                        Rank = 1,
                        FirstRankSpellId = 7,
                        PreviousRankSpellId = null,
                        NextRankSpellId = null,
                    },
                    CastTime = 1500,
                    Cooldown = 150000,
                    Duration = 150000,
                    ElementTypes = ElementTypes.Water,
                    SpellIconPath = "res://resources/textures/spells/spell_water_water_slide.jpg",
                },
                new SpellTemplate
                {
                    SpellId = 8,
                    SpellName = "Water Jet",
                    SpellDescription = "Continuously emits jet of water in front of the caster, dealing 40 damage every second and slowing the target's movement speed by 70%. Must be channeled. Lasts 8 seconds.",
                    SpellRank = new SpellRank
                    {
                        SpellId = 8,
                        Rank = 1,
                        FirstRankSpellId = 8,
                        PreviousRankSpellId = null,
                        NextRankSpellId = null,
                    },
                    CastTime = 2000,
                    Cooldown = 60000,
                    Duration = 8000,
                    ElementTypes = ElementTypes.Water,
                    SpellFlags = SpellFlags.IsChanneled,
                    SpellIconPath = "res://resources/textures/spells/spell_water_water_jet.jpg",
                },



                new SpellTemplate
                {
                    SpellId = 9,
                    SpellName = "ENHANCEMENT ONE",
                    SpellDescription = "Conjures a water dragon that charges at the enemies, deals 30 damage to them and slows them by 25%. Maximum 5 targets.",
                    SpellRank = new SpellRank
                    {
                        SpellId = 9,
                        Rank = 1,
                        FirstRankSpellId = 9,
                        PreviousRankSpellId = null,
                        NextRankSpellId = null,
                    },
                    CastTime = 2000,
                    Cooldown = 300000,
                    Duration = 0,
                    ElementTypes = ElementTypes.Water,
                    SpellFlags = SpellFlags.IsEnhancement,
                    SpellIconPath = "res://resources/textures/spell_frost_frost.jpg",
                },
                new SpellTemplate
                {
                    SpellId = 10,
                    SpellName = "ENHANCEMENT TWO",
                    SpellDescription = "Encapsulates the target in the water prison, dealing 5% of the target's maximum health every second. Lasts for 10 seconds or until canceled. Must be channeled.",
                    SpellRank = new SpellRank
                    {
                        SpellId = 10,
                        Rank = 1,
                        FirstRankSpellId = 10,
                        PreviousRankSpellId = null,
                        NextRankSpellId = null,
                    },
                    CastTime = 1500,
                    Cooldown = 45000,
                    Duration = 10000,
                    ElementTypes = ElementTypes.Water,
                    SpellFlags = SpellFlags.IsEnhancement,
                    SpellIconPath = "res://resources/textures/spell_frost_frostbolt.jpg",
                },
                new SpellTemplate
                {
                    SpellId = 11,
                    SpellName = "ENHANCEMENT THREE",
                    SpellDescription = "Creates a water path in front of a caster increasing movement speed by 40% for all friendly characters in the slide.",
                    SpellRank = new SpellRank
                    {
                        SpellId = 11,
                        Rank = 1,
                        FirstRankSpellId = 11,
                        PreviousRankSpellId = null,
                        NextRankSpellId = null,
                    },
                    CastTime = 1500,
                    Cooldown = 150000,
                    Duration = 150000,
                    ElementTypes = ElementTypes.Water,
                    SpellFlags = SpellFlags.IsEnhancement,
                    SpellIconPath = "res://resources/textures/spell_frost_frozencore.jpg",
                },
                new SpellTemplate
                {
                    SpellId = 12,
                    SpellName = "ENHANCEMENT FOUR",
                    SpellDescription = "Continuously emits jet of water in front of the caster, dealing 40 damage every second and slowing the target's movement speed by 70%. Must be channeled. Lasts 8 seconds.",
                    SpellRank = new SpellRank
                    {
                        SpellId = 12,
                        Rank = 1,
                        FirstRankSpellId = 12,
                        PreviousRankSpellId = null,
                        NextRankSpellId = null,
                    },
                    CastTime = 2000,
                    Cooldown = 60000,
                    Duration = 8000,
                    ElementTypes = ElementTypes.Water,
                    SpellFlags = SpellFlags.IsEnhancement,
                    SpellIconPath = "res://resources/textures/spell_water_enhancement.jpg",
                },













                new SpellTemplate
                {
                    SpellId = 1000,
                    SpellName = "HEAL",
                    SpellDescription = "Instant Heal TEST",
                    SpellRank = new SpellRank
                    {
                        SpellId = 1000,
                        Rank = 1,
                        FirstRankSpellId = 1000,
                        PreviousRankSpellId = null,
                        NextRankSpellId = null,
                    },
                    CastTime = 0,
                    Cooldown = 3000,
                    Duration = 10000,
                    ElementChakraCosts = new Dictionary<ElementTypes, int>
                    {
                        { ElementTypes.Water, 20 }
                    },
                    ElementTypes = ElementTypes.Water,
                    SpellEffects = new List<SpellEffectObject>
                    {
                        new SpellEffectObject
                        {
                            SpellId = 1000,
                            EffectType = SpellEffectType.Heal,
                            Value1 = 30,
                            Value2 = 40
                        }
                    },
                    SpellTargets = SpellTargets.Caster,
                    SpellIconPath = "res://resources/textures/icon_ground.png",
                    //ScriptName = "camera_damage_visual_test"
                },
                new SpellTemplate
                {
                    SpellId = 1001,
                    SpellName = "Cast HEAL",
                    SpellDescription = "Cast Heal TEST",
                    SpellRank = new SpellRank
                    {
                        SpellId = 1001,
                        Rank = 1,
                        FirstRankSpellId = 1001,
                        PreviousRankSpellId = null,
                        NextRankSpellId = null,
                    },
                    CastTime = 2000,
                    Cooldown = 2500,
                    Duration = 10000,
                    ElementChakraCosts = new Dictionary<ElementTypes, int>
                    {
                        { ElementTypes.Water, 10 }
                    },
                    ElementTypes = ElementTypes.Water,
                    SpellEffects = new List<SpellEffectObject>
                    {
                        new SpellEffectObject
                        {
                            SpellId = 1001,
                            EffectType = SpellEffectType.Heal,
                            Value1 = 30,
                            Value2 = 40
                        }
                    },
                    SpellTargets = SpellTargets.Caster,
                    SpellIconPath = "res://resources/textures/icon_ground.png",
                    //ScriptName = "camera_damage_visual_test"
                },
                new SpellTemplate
                {
                    SpellId = 1002,
                    SpellName = "DAMAGE",
                    SpellDescription = "Instant Damage TEST",
                    SpellRank = new SpellRank
                    {
                        SpellId = 1002,
                        Rank = 1,
                        FirstRankSpellId = 1002,
                        PreviousRankSpellId = null,
                        NextRankSpellId = null,
                    },
                    CastTime = 0,
                    Cooldown = 5000,
                    Duration = 10000,
                    ElementChakraCosts = new Dictionary<ElementTypes, int>
                    {
                        { ElementTypes.Fire, 30 }
                    },
                    ElementTypes = ElementTypes.Fire,
                    SpellEffects = new List<SpellEffectObject>
                    {
                        new SpellEffectObject
                        {
                            SpellId = 1002,
                            EffectType = SpellEffectType.DealDamage,
                            Value1 = 30,
                            Value2 = 40
                        }
                    },
                    SpellTargets = SpellTargets.Caster,
                    SpellIconPath = "res://resources/textures/icon_fire.png",
                    //ScriptName = "camera_damage_visual_test"
                },
                new SpellTemplate
                {
                    SpellId = 1003,
                    SpellName = "DAMAGE Cast",
                    SpellDescription = "Cast Damage TEST",
                    SpellRank = new SpellRank
                    {
                        SpellId = 1003,
                        Rank = 1,
                        FirstRankSpellId = 1003,
                        PreviousRankSpellId = null,
                        NextRankSpellId = null,
                    },
                    CastTime = 1500,
                    Cooldown = 0,
                    Duration = 0,
                    ElementChakraCosts = new Dictionary<ElementTypes, int>
                    {
                        { ElementTypes.Fire, 10 }
                    },
                    ElementTypes = ElementTypes.Fire,
                    SpellEffects = new List<SpellEffectObject>
                    {
                        new SpellEffectObject
                        {
                            SpellId = 1003,
                            EffectType = SpellEffectType.DealDamage,
                            Value1 = 30,
                            Value2 = 40
                        }
                    },
                    SpellTargets = SpellTargets.Caster,
                    SpellIconPath = "res://resources/textures/icon_lightning.png",
                    //ScriptName = "camera_damage_visual_test"
                },
                new SpellTemplate
                {
                    SpellId = 1004,
                    SpellName = "Periodic Heal TEST",
                    SpellDescription = "Periodic Heal TEST",
                    SpellRank = new SpellRank
                    {
                        SpellId = 1004,
                        Rank = 1,
                        FirstRankSpellId = 1004,
                        PreviousRankSpellId = null,
                        NextRankSpellId = null,
                    },
                    //CastTime = 3000,
                    CastTime = 0,
                    Cooldown = 0,
                    Duration = 0,
                    ElementChakraCosts = new Dictionary<ElementTypes, int>
                    {
                        { ElementTypes.Air, 20 }
                    },
                    ElementTypes = ElementTypes.Air,
                    SpellEffects = new List<SpellEffectObject>
                    {
                        new SpellEffectObject
                        {
                            SpellId = 1004,
                            EffectType = SpellEffectType.ApplyAura,
                            Value1 = 10000,
                        }
                    },
                    SpellTargets = SpellTargets.Caster,
                    SpellIconPath = "res://resources/textures/icon_wind.png",
                    //ScriptName = "camera_damage_visual_test"
                },
                new SpellTemplate
                {
                    SpellId = 1005,
                    SpellName = "Periodic Damage TEST",
                    SpellDescription = "Periodic Damage TEST",
                    SpellRank = new SpellRank
                    {
                        SpellId = 1005,
                        Rank = 1,
                        FirstRankSpellId = 1005,
                        PreviousRankSpellId = null,
                        NextRankSpellId = null,
                    },
                    //CastTime = 3000,
                    CastTime = 0,
                    Cooldown = 0,
                    Duration = 0,
                    ElementChakraCosts = new Dictionary<ElementTypes, int>
                    {
                        { ElementTypes.Air, 20 }
                    },
                    ElementTypes = ElementTypes.Air,
                    SpellEffects = new List<SpellEffectObject>
                    {
                        new SpellEffectObject
                        {
                            SpellId = 1004,
                            EffectType = SpellEffectType.ApplyAura,
                            Value1 = 10001,
                        }
                    },
                    SpellTargets = SpellTargets.Caster,
                    SpellIconPath = "res://resources/textures/icon_water.png",
                    //ScriptName = "camera_damage_visual_test"
                },
            };

            await _mongoGameDatabase.GetMongoCollection<SpellTemplate>()
                .DeleteManyAsync(Builders<SpellTemplate>.Filter.Empty);

            await _mongoGameDatabase.GetMongoCollection<SpellTemplate>()
                .InsertManyAsync(templates);

            return templates;
        }

        public async Task<List<AuraTemplate>> CreateAuraTemplatesAsync()
        {
            var templates = new List<AuraTemplate>
            {
                new AuraTemplate
                {
                    AuraId = 10000,
                    SpellId = 1004,
                    AuraName = "Periodic Heal (Aura) TEST",
                    AuraDescription = "Periodic Heal (Aura) TEST",
                    ElementTypes = ElementTypes.Air,
                    AuraEffects = new List<AuraEffectObject>
                    {
                        new AuraEffectObject
                        {
                            AuraId = 10000,
                            EffectType = AuraEffectType.PeriodicHeal,
                            Value1 = 25
                        }
                    },
                    AuraFlags = AuraFlags.IsPositive | AuraFlags.IsPeriodic,
                    AuraIconPath = "res://resources/textures/spells/spell_water_water_jet.jpg",
                    Duration = 15000,
                    TicksCount = 5,
                    ScriptName = null
                },
                new AuraTemplate
                {
                    AuraId = 10001,
                    SpellId = 1005,
                    AuraName = "Periodic Damage (Aura) TEST",
                    AuraDescription = "Periodic Damage (Aura) TEST",
                    ElementTypes = ElementTypes.Air,
                    AuraEffects = new List<AuraEffectObject>
                    {
                        new AuraEffectObject
                        {
                            AuraId = 10001,
                            EffectType = AuraEffectType.PeriodicDamage,
                            Value1 = 25
                        }
                    },
                    AuraFlags = AuraFlags.IsNegative | AuraFlags.IsPeriodic,
                    AuraIconPath = "res://resources/textures/spell_frost_frozencore.jpg",
                    Duration = 15000,
                    TicksCount = 5,
                    ScriptName = null
                },
            };

            await _mongoGameDatabase.GetMongoCollection<AuraTemplate>()
                .DeleteManyAsync(Builders<AuraTemplate>.Filter.Empty);

            await _mongoGameDatabase.GetMongoCollection<AuraTemplate>()
                .InsertManyAsync(templates);

            return templates;
        }

        public async Task<List<MapTemplate>> CreateMapTemplatesAsync()
        {
            var templates = new List<MapTemplate>
            {
                new MapTemplate
                {
                    MapId = 1000,
                    MapName = "Movement Test Map",
                    MapDescription = "Map used to test movement and basic terrain detection.",
                    MapPath = "res://scenes/maps/test_3d_scene.tscn",
                    MapSizeX = 500,
                    MapSizeY = 500,
                    MapFlags = MapFlags.DevelopmentMap,
                },
            };

            await _mongoGameDatabase.GetMongoCollection<MapTemplate>()
                .DeleteManyAsync(Builders<MapTemplate>.Filter.Empty);

            await _mongoGameDatabase.GetMongoCollection<MapTemplate>()
                .InsertManyAsync(templates);

            return templates;
        }

        public async Task<List<NonPlayerTemplate>> CreateNonPlayerTemplatesAsync()
        {
            var templates = new List<NonPlayerTemplate>
            {
                new NonPlayerTemplate
                {
                    NonPlayerId = 1,
                    Name = "Magus Cedrik",
                    Title = "#TODO",
                    Level = 42,
                    SpeedWalk = 5.0f,
                    SpeedRun = 8.0f,
                    MaxHealth = 42000,
                    PrimaryElementType = ElementTypes.Water,
                    SecondaryElementType = ElementTypes.Air,
                    TertiaryElementType = ElementTypes.Nature,
                    MaxPrimaryChakra = 42000,
                    MaxSecondaryChakra = 40000,
                    MaxTertiaryChakra = 35000,
                    ScriptName = "magus_cedrik"
                },
            };

            await _mongoGameDatabase.GetMongoCollection<NonPlayerTemplate>()
                .DeleteManyAsync(Builders<NonPlayerTemplate>.Filter.Empty);

            await _mongoGameDatabase.GetMongoCollection<NonPlayerTemplate>()
                .InsertManyAsync(templates);

            return templates;
        }

        public async Task<List<NonPlayerSpawnTemplate>> CreateNonPlayerSpawnTemplatesAsync()
        {
            var templates = new List<NonPlayerSpawnTemplate>
            {
                new NonPlayerSpawnTemplate
                {
                    NonPlayerId = 1,
                    MapId = 1000,
                    InstanceId = null,
                    SpawnPosition = new Vector3(5.0f, 1.4f, 5.0f),
                    Orientation = Quaternion.Identity,
                },
            };

            await _mongoGameDatabase.GetMongoCollection<NonPlayerSpawnTemplate>()
                .DeleteManyAsync(Builders<NonPlayerSpawnTemplate>.Filter.Empty);

            await _mongoGameDatabase.GetMongoCollection<NonPlayerSpawnTemplate>()
                .InsertManyAsync(templates);

            return templates;
        }
    }
}

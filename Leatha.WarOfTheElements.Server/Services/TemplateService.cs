using Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates;
using Leatha.WarOfTheElements.Server.Demo;
using Leatha.WarOfTheElements.Server.Utilities;
using MongoDB.Driver;
using System.Collections.Concurrent;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;

namespace Leatha.WarOfTheElements.Server.Services
{
    public interface ITemplateService
    {
        Task<List<SpellTemplate>> GetSpellTemplatesAsync();

        Task<SpellTemplate?> GetSpellTemplateAsync(int spellId);



        Task ReloadTemplatesAsync();




        Task CreateTemplatesAsync();

        Task<List<SpellTemplate>> CreateSpellTemplatesAsync();
    }

    public sealed class TemplateService : ITemplateService
    {
        public TemplateService(IMongoClient client)
        {
            _mongoGameDatabase = client.GetDatabase(Constants.MongoGameDb);
        }

        private readonly IMongoDatabase _mongoGameDatabase;

        private readonly ConcurrentBag<SpellTemplate> _spellTemplates = [];

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

        public async Task ReloadTemplatesAsync()
        {
            // Cache is cleared inside each call.
            {
                await GetSpellTemplatesAsync();
            }
        }

        public async Task CreateTemplatesAsync()
        {
            await CreateSpellTemplatesAsync();
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
            };

            await _mongoGameDatabase.GetMongoCollection<SpellTemplate>()
                .DeleteManyAsync(Builders<SpellTemplate>.Filter.Empty);

            await _mongoGameDatabase.GetMongoCollection<SpellTemplate>()
                .InsertManyAsync(templates);

            return templates;
        }
    }
}

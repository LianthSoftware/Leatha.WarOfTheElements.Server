using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates;
using Leatha.WarOfTheElements.Server.Utilities;
using MongoDB.Driver;

namespace Leatha.WarOfTheElements.Server.Services
{
    public interface ISpellService
    {
        Task<SpellInfoObject?> GetSpellInfoAsync(int spellId);

        Task<SpellInfoObject?> GetSpellInfoAsync(int spellId, Guid playerId);

        Task<SpellCastResult> CastSpellAsync(Guid casterId, int spellId);
    }

    public sealed class SpellService : ISpellService
    {
        public SpellService(IMongoClient client, ITemplateService templateService, IPlayerService playerService)
        {
            _mongoGameDatabase = client.GetDatabase(Constants.MongoGameDb);
            _templateService = templateService;
            _playerService = playerService;
        }

        private readonly IMongoDatabase _mongoGameDatabase;
        private readonly ITemplateService _templateService;
        private readonly IPlayerService _playerService;

        public async Task<SpellInfoObject?> GetSpellInfoAsync(int spellId)
        {
            var spellTemplate = await _templateService.GetSpellTemplateAsync(spellId);
            return spellTemplate?.AsTransferObject();
        }

        public Task<SpellInfoObject?> GetSpellInfoAsync(int spellId, Guid playerId)
        {
            throw new NotImplementedException();
        }

        public async Task<SpellCastResult> CastSpellAsync(Guid casterId, int spellId)
        {
            var spellInfo = await GetSpellInfoAsync(spellId);
            if (spellInfo == null)
                return SpellCastResult.InternalError; // #TODO: Error.

            // #TODO: Get Player from memory maybe?
            var caster = (await _playerService.GetPlayerAsync(casterId)).Data?.AsTransferObject();
            if (caster == null)
                return SpellCastResult.InternalError; // #TODO: Error.

            var spell = PrepareSpell(spellInfo, caster);




            return SpellCastResult.Ok;
        }

        private Spell PrepareSpell(SpellInfoObject spellInfo, PlayerObject caster)
        {
            var spell = new Spell(spellInfo, caster);

            SelectTargets(spell);

            return spell;
        }

        private void SelectTargets(Spell spell)
        {
            var targets = new List<PlayerObject>();

            switch (spell.SpellInfo.SpellTargets)
            {
                case SpellTargets.Caster:
                    targets.Add(spell.Caster);
                    break;

                // #TODO: More
            }

            // Set targets.
            spell.Targets.Clear();
            spell.Targets.AddRange(targets);
        }
    }

    public sealed class Spell
    {
        public Spell(SpellInfoObject spellInfo, PlayerObject caster)
        {
            SpellInfo = spellInfo;
            Caster = caster;
        }

        public int SpellId
            => SpellInfo.SpellId;

        public SpellInfoObject SpellInfo { get; }

        public PlayerObject Caster { get; }

        public List<PlayerObject> Targets { get; } = [];
    }
}

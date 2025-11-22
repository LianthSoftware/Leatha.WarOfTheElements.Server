using BCrypt.Net;
using Leatha.WarOfTheElements.Common.Communication.Messages;
using Leatha.WarOfTheElements.Common.Communication.Services;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using Leatha.WarOfTheElements.Common.Communication.Utilities;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates;
using Leatha.WarOfTheElements.Server.Demo;
using Leatha.WarOfTheElements.Server.Objects.Characters;
using Leatha.WarOfTheElements.Server.Objects.Game;
using Leatha.WarOfTheElements.Server.Scripts.Spells;
using Leatha.WarOfTheElements.Server.Utilities;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using System.Diagnostics;

namespace Leatha.WarOfTheElements.Server.Services
{
    public interface ISpellService
    {
        Task<SpellInfoObject?> GetSpellInfoAsync(int spellId);

        Task<AuraInfoObject?> GetAuraInfoAsync(int auraId);

        Task<SpellInfoObject?> GetSpellInfoAsync(int spellId, Guid playerId);

        Task<TransferMessage<SpellCastResult>> CastSpellAsync(WorldObjectId casterId, int spellId);
    }

    public sealed class SpellService : ISpellService
    {
        public SpellService(
            IMongoClient client,
            ITemplateService templateService,
            IPlayerService playerService,
            IScriptService scriptService,
            IGameWorld gameWorld,
            IServerToClientHandler serverHandler)
        {
            _mongoGameDatabase = client.GetDatabase(Constants.MongoGameDb);
            _templateService = templateService;
            _playerService = playerService;
            _scriptService = scriptService;
            _gameWorld = gameWorld;
            _serverHandler = serverHandler;
        }

        private readonly IMongoDatabase _mongoGameDatabase;
        private readonly ITemplateService _templateService;
        private readonly IPlayerService _playerService;
        private readonly IScriptService _scriptService;
        private readonly IGameWorld _gameWorld;
        private readonly IServerToClientHandler _serverHandler;

        public async Task<SpellInfoObject?> GetSpellInfoAsync(int spellId)
        {
            var spellTemplate = await _templateService.GetSpellTemplateAsync(spellId);
            return spellTemplate?.AsTransferObject();
        }

        public async Task<AuraInfoObject?> GetAuraInfoAsync(int auraId)
        {
            var auraTemplate = await _templateService.GetAuraTemplateAsync(auraId);
            return auraTemplate?.AsTransferObject();
        }

        public Task<SpellInfoObject?> GetSpellInfoAsync(int spellId, Guid playerId)
        {
            throw new NotImplementedException();
        }

        public async Task<TransferMessage<SpellCastResult>> CastSpellAsync(WorldObjectId casterId, int spellId)
        {
            var spellInfo = await GetSpellInfoAsync(spellId);
            if (spellInfo == null)
            {
                return TransferMessage.CreateErrorMessage(
                    SpellCastResult.InternalError,
                    "Cast Spell Error",
                    $"Spell with Spell Id = \"{ spellId }\" was not found.");
            }

            // #TODO: Get Player from memory maybe?
            //var caster = (await _playerService.GetPlayerAsync(casterId)).Data?.AsTransferObject();
            var caster = _gameWorld.GetCharacterState(casterId);
            if (caster == null)
            {
                return TransferMessage.CreateErrorMessage(
                    SpellCastResult.InternalError,
                    "Cast Spell Error",
                    $"Caster with Character Id = \"{ casterId.ObjectId }\" was not found.");
            }

            // Prepare the spell.
            var spellObject = PrepareSpell(spellInfo, caster);
            var script = await _scriptService.CreateScriptAsync(spellObject);

            _gameWorld.RegisterSpell(spellObject, script);

            // #TODO: finish it.
            var calculatedValue = CalculateValue(caster, spellObject);

            var hasEnoughChakra = ValidateChakraCost(caster, spellObject);
            if (!hasEnoughChakra)
            {
                return TransferMessage.CreateErrorMessage(
                    SpellCastResult.InternalError,
                    "Cast Spell Error",
                    "Not enough chakra.");
            }

            // Instant cast.
            if (spellInfo.CastTime <= 0)
            {
                spellObject.IsCastFinished = true;
                spellObject.RemainingCastTime = 0.0f;

                await ProcessEffectAsync(spellObject, script);
            }
            else
                spellObject.RemainingCastTime = spellInfo.CastTime;

            // Remove chakra.
            TakeChakra(caster, spellObject);

            // Send spell cast start.
            await _serverHandler.SendSpellStart(spellObject, caster.AsTransferObject());

            return TransferMessage.CreateMessage(SpellCastResult.Ok);
        }

        private float CalculateValue(ICharacterState caster, SpellObject spellObject)
        {
            return 0.0f;
        }

        private float CalculateValue(ICharacterState caster, AuraObject auraObject)
        {
            return 0.0f;
        }

        private bool ValidateChakraCost(ICharacterState caster, SpellObject spellObject)
        {
            // No cost on the spell, validated.
            if (!spellObject.SpellInfo.ElementChakraCosts.Any())
                return true;

            // All costs are zero, validated.
            if (spellObject.SpellInfo.ElementChakraCosts.All(i => i.Value == 0))
                return true;

            var chakraList = new Dictionary<ElementTypes, ChakraResource>
            {
                { caster.Resources.PrimaryChakra.Element, caster.Resources.PrimaryChakra },
                { caster.Resources.SecondaryChakra.Element, caster.Resources.SecondaryChakra },
                { caster.Resources.TertiaryChakra.Element, caster.Resources.TertiaryChakra },
            };

            // Validate per chakra.
            foreach (var cost in spellObject.SpellInfo.ElementChakraCosts)
            {
                // If character does not contain the chakra resource, invalidate cast.
                if (!chakraList.TryGetValue(cost.Key, out var chakra))
                    return false;

                var costValue = cost.Value;

                // #TODO: Apply modifiers to chakra cost.

                // Not enough chakra, invalidate cast.
                if (chakra.Chakra < costValue)
                    return false;
            }

            return true;
        }

        private void TakeChakra(ICharacterState caster, SpellObject spellObject)
        {
            //caster.Resources.PrimaryElementChakra -= spellInfo.PrimaryElementChakraCost;
            //caster.Resources.SecondaryElementChakra -= spellInfo.SecondaryElementChakraCost;
            //caster.Resources.TertiaryElementChakra -= spellInfo.TertiaryElementChakraCost;

            // No cost on the spell, validated.
            if (!spellObject.SpellInfo.ElementChakraCosts.Any())
                return;

            // All costs are zero, validated.
            if (spellObject.SpellInfo.ElementChakraCosts.All(i => i.Value == 0))
                return;

            var chakraList = new Dictionary<ElementTypes, ChakraResource>
            {
                { caster.Resources.PrimaryChakra.Element, caster.Resources.PrimaryChakra },
                { caster.Resources.SecondaryChakra.Element, caster.Resources.SecondaryChakra },
                { caster.Resources.TertiaryChakra.Element, caster.Resources.TertiaryChakra },
            };

            // Validate per chakra.
            foreach (var cost in spellObject.SpellInfo.ElementChakraCosts)
            {
                // If character does not contain the chakra resource, invalidate cast.
                if (!chakraList.TryGetValue(cost.Key, out var chakra))
                    continue;

                chakra.Chakra -= Math.Max(cost.Value, 0);
            }

            Debug.WriteLine($"[TakeChakra]: Primary = {caster.Resources.PrimaryChakra.Chakra} | Secondary = {caster.Resources.SecondaryChakra.Chakra} | Tertiary = {caster.Resources.TertiaryChakra.Chakra}.");
        }

        private async Task ProcessEffectAsync(SpellObject spellObject, SpellScriptBase? script)
        {
            foreach (var spellEffect in spellObject.SpellInfo.SpellEffects)
            {
                //// #TODO: Calculate value, for now, we do random between 20 - 40.
                //var value = CommonExtensions.Random(20, 40);

                var value = spellEffect.Value1;

                switch (spellEffect.EffectType)
                {
                    case SpellEffectType.DealDamage:
                    {
                        foreach (var targetId in spellObject.Targets)
                        {
                            var state = _gameWorld.GetCharacterState(targetId);
                            if (state == null)
                            {
                                Debug.WriteLine($"[DealDamage]: Target with Id = \"{targetId.ObjectId}\" was not found.");
                                continue;
                            }

                            state.Resources.Health -= value;

                            if (state.Resources.Health <= 0)
                            {
                                // #TODO: DIE.

                            }

                            script?.OnTargetHit(state);

                            Debug.WriteLine($"[DealDamage]: Target with Id = \"{targetId.ObjectId}\" took {value} damage.");
                        }

                        break;
                    }
                    case SpellEffectType.ApplyAura:
                    {
                        Debug.WriteLine($"[ApplyAura]: Trying to apply aura \"{ spellEffect.Value1 }\".");

                        // #TODO: Select correct targets, for testing now, caster = target.

                        var targetId = spellObject.CasterId;

                        var auraResult = await AddAuraAsync(spellObject.CasterId, targetId, spellEffect.Value1);
                        if (auraResult.IsError)
                            Debug.WriteLine($"[ApplyAura]: Error = { auraResult.ErrorMessage }");
                        break;
                    }
                    case SpellEffectType.Heal:
                    {
                        foreach (var targetId in spellObject.Targets)
                        {
                            var state = _gameWorld.GetCharacterState(targetId);
                            if (state == null)
                            {
                                Debug.WriteLine($"[Heal]: Target with Id = \"{targetId.ObjectId}\" was not found.");
                                continue;
                            }

                            state.Resources.Health += Math.Min(value, state.Resources.MaxHealth);

                            script?.OnTargetHit(state);

                            Debug.WriteLine($"[DealDamage]: Target with Id = \"{targetId.ObjectId}\" was healed for {value} damage.");
                        }

                        break;
                    }
                }
            }
        }

        private async Task<TransferMessage<AuraObject>> AddAuraAsync(WorldObjectId casterId, WorldObjectId targetId, int auraId)
        {
            var auraInfo = await GetAuraInfoAsync(auraId);
            if (auraInfo == null)
            {
                return TransferMessage.CreateErrorMessage<AuraObject>(
                    "Add Aura Error",
                    $"Aura with Aura Id = \"{ auraId }\" was not found.");
            }

            // #TODO: Get Player from memory maybe?
            //var caster = (await _playerService.GetPlayerAsync(casterId)).Data?.AsTransferObject();
            var caster = _gameWorld.GetCharacterState(casterId);
            if (caster == null)
            {
                return TransferMessage.CreateErrorMessage<AuraObject>(
                    "Add Aura Error",
                    $"Caster with Character Id = \"{casterId.ObjectId}\" was not found.");
            }

            var target = _gameWorld.GetCharacterState(targetId);
            if (target == null)
            {
                return TransferMessage.CreateErrorMessage<AuraObject>(
                    "Add Aura Error",
                    $"Target with Character Id = \"{targetId.ObjectId}\" was not found.");
            }

            var auraObject = PrepareAura(auraInfo, target, caster);
            var script = await _scriptService.CreateScriptAsync(auraObject);

            _gameWorld.RegisterAura(auraObject, script);

            // #TODO: finish it.
            var calculatedValue = CalculateValue(caster, auraObject);



            // Send apply aura.
            await _serverHandler.SendAuraApply(auraObject, caster.AsTransferObject());

            return TransferMessage.CreateMessage(auraObject);
        }

        private SpellObject PrepareSpell(SpellInfoObject spellInfo, ICharacterState caster)
        {
            var spell = new SpellObject
            {
                SpellGuid = Guid.NewGuid(),
                SpellInfo = spellInfo,
                CasterId = caster.WorldObjectId,
                CastTime = spellInfo.CastTime,
                Caster = caster.AsTransferObject()
                //VisualSpellPath = spellInfo
                //Caster = caster.AsTransferObject()
            };

            SelectSpellTargets(spell);

            return spell;
        }

        private AuraObject PrepareAura(AuraInfoObject auraInfo, ICharacterState caster, ICharacterState target)
        {
            var aura = new AuraObject
            {
                AuraGuid = Guid.NewGuid(),
                AuraInfo = auraInfo,
                CasterId = caster.WorldObjectId,
                Duration = auraInfo.Duration,
                RemainingDuration = auraInfo.Duration,
                Caster = caster.AsTransferObject(),
                TargetId = target.WorldObjectId,
                Target = target.AsTransferObject(),
                //VisualSpellPath = spellInfo
                //Caster = caster.AsTransferObject()
            };

            //SelectSpellTargets(aura);

            return aura;
        }

        private void SelectSpellTargets(SpellObject spellObject)
        {
            var targets = new List<WorldObjectId>();

            switch (spellObject.SpellInfo.SpellTargets)
            {
                case SpellTargets.Caster:
                    targets.Add(spellObject.CasterId);
                    break;

                // #TODO: More
            }

            // Set targets.
            spellObject.Targets.Clear();
            spellObject.Targets.AddRange(targets);
        }
    }

    //public sealed class Spell
    //{
    //    public Spell(SpellInfoObject spellInfo, PlayerObject caster)
    //    {
    //        SpellInfo = spellInfo;
    //        Caster = caster;
    //    }

    //    public int SpellId
    //        => SpellInfo.SpellId;

    //    public SpellInfoObject SpellInfo { get; }

    //    public PlayerObject Caster { get; }

    //    public List<PlayerObject> Targets { get; } = [];
    //}
}

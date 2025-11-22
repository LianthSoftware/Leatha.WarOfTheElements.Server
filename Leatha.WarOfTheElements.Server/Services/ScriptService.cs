using BCrypt.Net;
using Leatha.WarOfTheElements.Server.Objects.Attributes;
using Leatha.WarOfTheElements.Server.Objects.Characters;
using Leatha.WarOfTheElements.Server.Scripts.NonPlayers;
using Leatha.WarOfTheElements.Server.Utilities;
using MongoDB.Driver;
using System.Reflection;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Server.Scripts.Auras;
using Leatha.WarOfTheElements.Server.Scripts.Spells;

namespace Leatha.WarOfTheElements.Server.Services
{
    public interface IScriptService
    {
        Task<NonPlayerScriptBase?> CreateScriptAsync(NonPlayerState state);

        Task<SpellScriptBase?> CreateScriptAsync(SpellObject spellObject);

        Task<AuraScriptBase?> CreateScriptAsync(AuraObject auraObject);

        void LoadNonPlayerScripts();

        void LoadSpellScripts();

        void LoadAuraScripts();
    }

    public sealed class ScriptService : IScriptService
    {
        public ScriptService(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        private readonly ITemplateService _templateService;

        public Dictionary<string, Type> NonPlayerScripts { get; private set; } = [];

        public Dictionary<string, Type> SpellScripts { get; private set; } = [];

        public async Task<NonPlayerScriptBase?> CreateScriptAsync(NonPlayerState state)
        {
            var template = await _templateService.GetNonPlayerTemplateAsync(state.TemplateId);
            if (template == null || string.IsNullOrWhiteSpace(template.ScriptName))
                return null;

            if (!NonPlayerScripts.TryGetValue(template.ScriptName, out var type))
                return null;

            var script = Activator.CreateInstance(type, state, template) as NonPlayerScriptBase;
            return script;
        }

        public async Task<SpellScriptBase?> CreateScriptAsync(SpellObject spellObject)
        {
            var template = await _templateService.GetSpellTemplateAsync(spellObject.SpellInfo.SpellId);
            if (template == null || string.IsNullOrWhiteSpace(template.ScriptName))
                return null;

            if (!NonPlayerScripts.TryGetValue(template.ScriptName, out var type))
                return null;

            var script = Activator.CreateInstance(type, spellObject, template) as SpellScriptBase;
            return script;
        }

        public async Task<AuraScriptBase?> CreateScriptAsync(AuraObject auraObject)
        {
            var template = await _templateService.GetAuraTemplateAsync(auraObject.AuraInfo.AuraId);
            if (template == null || string.IsNullOrWhiteSpace(template.ScriptName))
                return null;

            if (!NonPlayerScripts.TryGetValue(template.ScriptName, out var type))
                return null;

            var script = Activator.CreateInstance(type, auraObject, template) as AuraScriptBase;
            return script;
        }

        public void LoadNonPlayerScripts()
        {
            var assembly = typeof(ScriptService).Assembly;
            if (assembly == null)
                throw new InvalidOperationException(
                    $"Could not retrieve assembly for type = \"{typeof(ScriptService).FullName}\".");

            NonPlayerScripts = assembly
                .GetTypes()
                .Select(t => new
                {
                    Type = t,
                    Attr = t.GetCustomAttribute<ScriptNameAttribute>()
                })
                .Where(x => x.Attr is { ScriptType: ScriptType.NonPlayer })
                .ToDictionary(x => x.Attr!.Name, x => x.Type);
        }

        public void LoadSpellScripts()
        {
            var assembly = typeof(ScriptService).Assembly;
            if (assembly == null)
                throw new InvalidOperationException(
                    $"Could not retrieve assembly for type = \"{typeof(ScriptService).FullName}\".");

            SpellScripts = assembly
                .GetTypes()
                .Select(t => new
                {
                    Type = t,
                    Attr = t.GetCustomAttribute<ScriptNameAttribute>()
                })
                .Where(x => x.Attr is { ScriptType: ScriptType.Spell })
                .ToDictionary(x => x.Attr!.Name, x => x.Type);
        }

        public void LoadAuraScripts()
        {
            var assembly = typeof(ScriptService).Assembly;
            if (assembly == null)
                throw new InvalidOperationException(
                    $"Could not retrieve assembly for type = \"{typeof(ScriptService).FullName}\".");

            SpellScripts = assembly
                .GetTypes()
                .Select(t => new
                {
                    Type = t,
                    Attr = t.GetCustomAttribute<ScriptNameAttribute>()
                })
                .Where(x => x.Attr is { ScriptType: ScriptType.Aura })
                .ToDictionary(x => x.Attr!.Name, x => x.Type);
        }
    }
}

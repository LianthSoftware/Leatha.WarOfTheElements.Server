using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using Leatha.WarOfTheElements.Server.Demo;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates
{
    public sealed class SpellTemplate : MongoEntity
    {
        public int SpellId { get; set; }

        public string SpellName { get; set; } = null!;

        public string SpellDescription { get; set; } = null!;

        public ElementTypes ElementTypes { get; set; }

        public int CastTime { get; set; } // Milliseconds.

        public int Cooldown { get; set; } // Milliseconds.

        public int Duration { get; set; } // Milliseconds.

        public int TicksCount { get; set; }


        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<ElementTypes, int> ElementChakraCosts { get; set; } = [];


        public SpellTargets SpellTargets { get; set; }

        public SpellFlags SpellFlags { get; set; }



        public List<SpellEffectObject> SpellEffects { get; set; } = [];

        public SpellRank? SpellRank { get; set; }

        public string SpellIconPath { get; set; } = null!;

        public string? VisualSpellPath { get; set; }

        public string? ScriptName { get; set; }
    }
}

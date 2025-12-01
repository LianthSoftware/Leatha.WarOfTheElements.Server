using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed  class SpellInfoObject
    {
        [Required]
        [JsonPropertyName("spellId")]
        public int SpellId { get; set; }

        [Required]
        [JsonPropertyName("spellName")]
        public string SpellName { get; set; } = null!;

        [Required]
        [JsonPropertyName("spellDescription")]
        public string SpellDescription { get; set; } = null!;

        [Required]
        [JsonPropertyName("elementTypes")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ElementTypes ElementTypes { get; set; }

        [Required]
        [JsonPropertyName("castTime")]
        public int CastTime { get; set; } // Milliseconds.

        [Required]
        [JsonPropertyName("cooldown")]
        public int Cooldown { get; set; } // Milliseconds.

        [Required]
        [JsonPropertyName("duration")]
        public int Duration { get; set; } // Milliseconds.

        public Dictionary<ElementTypes, int> ElementChakraCosts { get; set; } = [];

        [Required]
        [JsonPropertyName("spellTargets")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SpellTargets SpellTargets { get; set; }

        [Required]
        [JsonPropertyName("spellFlags")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SpellFlags SpellFlags { get; set; }


        [Required]
        [JsonPropertyName("spellEffects")]
        public List<SpellEffectObject> SpellEffects { get; set; } = [];

        [Required]
        [JsonPropertyName("spellRank")]
        public SpellRankObject? SpellRank { get; set; }





        [Required]
        [JsonPropertyName("spellIconPath")]
        public string SpellIconPath { get; set; } = null!;

        [Required]
        [JsonPropertyName("visualSpellPath")]
        public string? VisualSpellPath { get; set; }
    }
}

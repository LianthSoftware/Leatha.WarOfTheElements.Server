using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class SpellRankObject
    {
        [Required]
        [JsonPropertyName("spellId")]
        public int SpellId { get; set; }

        [Required]
        [JsonPropertyName("rank")]
        public int Rank { get; set; }

        [Required]
        [JsonPropertyName("firstRankSpellId")]
        public int FirstRankSpellId { get; set; }

        [Required]
        [JsonPropertyName("previousRankSpellId")]
        public int? PreviousRankSpellId { get; set; }

        [Required]
        [JsonPropertyName("nextRankSpellId")]
        public int? NextRankSpellId { get; set; }
    }
}

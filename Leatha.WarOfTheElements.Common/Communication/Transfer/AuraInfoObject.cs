using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class AuraInfoObject
    {
        public int AuraId { get; set; }

        public int SpellId { get; set; }

        public string AuraName { get; set; } = null!;

        public string AuraDescription { get; set; } = null!;

        public ElementTypes ElementTypes { get; set; }

        public int Duration { get; set; } // Milliseconds.

        public int TicksCount { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AuraFlags AuraFlags { get; set; }



        public List<AuraEffectObject> AuraEffects { get; set; } = [];

        public string AuraIconPath { get; set; } = null!;
    }
}

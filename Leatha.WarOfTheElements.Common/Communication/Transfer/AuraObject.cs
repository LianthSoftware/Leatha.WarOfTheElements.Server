using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class AuraObject
    {
        public Guid AuraGuid { get; set; }

        public AuraInfoObject AuraInfo { get; set; } = null!;

        public WorldObjectId CasterId { get; set; }

        [JsonIgnore]
        public ICharacterStateObject Caster { get; set; } = null!;

        public WorldObjectId TargetId { get; set; }

        [JsonIgnore]
        public ICharacterStateObject Target { get; set; } = null!;

        public int Duration { get; set; } = -1; // Milliseconds.

        public float RemainingDuration { get; set; }

        public int NextTick { get; set; }


        public string? VisualSpellPath { get; set; }
    }
}

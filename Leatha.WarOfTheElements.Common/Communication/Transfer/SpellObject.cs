using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class SpellObject
    {
        public Guid SpellGuid { get; set; }

        public SpellInfoObject SpellInfo { get; set; } = null!;

        public WorldObjectId CasterId { get; set; }

        [JsonIgnore]
        public ICharacterStateObject Caster { get; set; } = null!;

        public List<WorldObjectId> Targets { get; set; } = [];

        public int CastTime { get; set; } // Milliseconds.

        public float RemainingCastTime { get; set; } // Milliseconds.

        public bool IsCastFinished { get; set; }


        public ProjectileStateObject? ProjectileState { get; set; }
    }
}

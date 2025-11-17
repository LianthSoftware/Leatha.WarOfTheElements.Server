using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using Leatha.WarOfTheElements.Server.Demo;

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

        public SpellTargets SpellTargets { get; set; }

        public SpellFlags SpellFlags { get; set; }



        public SpellEffectType SpellEffectType { get; set; }

        public SpellRank? SpellRank { get; set; }

        public string SpellIconPath { get; set; } = null!;
    }
}

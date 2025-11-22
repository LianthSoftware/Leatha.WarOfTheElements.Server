using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class SpellEffectObject
    {
        public int SpellId { get; set; }

        public SpellEffectType EffectType { get; set; }

        public int Value1 { get; set; }

        public int Value2 { get; set; }

        public int Value3 { get; set; }
    }
}

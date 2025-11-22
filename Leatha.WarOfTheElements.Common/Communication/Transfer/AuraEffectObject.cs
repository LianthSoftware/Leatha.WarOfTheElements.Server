using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class AuraEffectObject
    {
        public int AuraId { get; set; }

        public AuraEffectType EffectType { get; set; }

        public int Value1 { get; set; }

        public int Value2 { get; set; }

        public int Value3 { get; set; }
    }
}

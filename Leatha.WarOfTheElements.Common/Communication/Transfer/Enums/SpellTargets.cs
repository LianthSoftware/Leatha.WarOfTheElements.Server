using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer.Enums
{
    [Flags]
    public enum SpellTargets
    {
        None                            = 0x0,
        Caster                          = 0x1,
        AcquiredTarget                  = 0x2,
        CasterDirection                 = 0x4,

        // #TODO: More
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer.Enums
{
    public enum SpellCastResult
    {
        Ok                              = 0,
        ErrorNotEnoughChakra            = 1,
        ErrorOnCooldown                 = 2,
        InternalError                   = 3,
    }
}

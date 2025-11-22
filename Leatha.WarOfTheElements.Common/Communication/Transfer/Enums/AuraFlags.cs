using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer.Enums
{
    [Flags]
    public enum AuraFlags
    {
        None                            = 0x0,
        IsPositive                      = 0x1,
        IsNegative                      = 0x2,
        IsStackable                     = 0x4,
        IsPeriodic                      = 0x8,
    }
}

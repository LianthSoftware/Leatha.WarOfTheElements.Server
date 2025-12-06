using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer.Enums
{
    [Flags]
    public enum AreaTriggerFlags
    {
        None                            = 0x0,
        OneShot                         = 0x1,
    }
}

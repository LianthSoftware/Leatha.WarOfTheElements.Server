using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leatha.WarOfTheElements.Common.Communication.Transfer;

namespace Leatha.WarOfTheElements.Common.Communication.Messages
{
    public sealed class SetGameStateMessage
    {
        public GameObjectStateObject GameObjectState { get; set; } = null!;

        public Dictionary<string, object> StateParameters { get; set; } = [];
    }
}

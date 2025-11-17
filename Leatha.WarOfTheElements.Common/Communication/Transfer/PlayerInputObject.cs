using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class PlayerInputObject
    {
        public Guid PlayerId { get; set; }

        public int Sequence { get; set; }

        public float Forward { get; set; }    // -1..1

        public float Right { get; set; }      // -1..1

        public float Up { get; set; }         // for flying (-1..1)

        public bool Jump { get; set; }

        public bool IsSprinting { get; set; }

        public bool IsFlying { get; set; }

        public float Yaw { get; set; }

        public float Pitch { get; set; }

        public float DeltaTime { get; set; }
    }
}

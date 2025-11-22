using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;

namespace Leatha.WarOfTheElements.Common.Communication.Utilities
{
    public static class CommonExtensions
    {
        private static readonly Random _random = new Random();

        public const int MaxSpellBarSlots = 8;

        public static int Random(int min, int max)
        {
            return _random.Next(min, max + 1);
        }

        public static bool IsPlayer(ICharacterStateObject state)
        {
            return IsPlayer(state.WorldObjectId);
        }

        public static bool IsPlayer(this WorldObjectId objectId)
        {
            if (objectId == WorldObjectId.Empty)
                return false;

            return objectId.ObjectType == WorldObjectType.Player;
        }

        public static bool IsNonPlayer(ICharacterStateObject state)
        {
            return IsNonPlayer(state.WorldObjectId);
        }

        public static bool IsNonPlayer(this WorldObjectId objectId)
        {
            if (objectId == WorldObjectId.Empty)
                return false;

            return objectId.ObjectType == WorldObjectType.NonPlayer;
        }
    }
}

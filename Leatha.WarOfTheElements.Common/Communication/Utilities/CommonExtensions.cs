using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Leatha.WarOfTheElements.Common.Communication.Utilities
{
    public static class CommonExtensions
    {
        private static readonly Random _random = new Random();

        public const int MaxSpellBarSlots = 10;

        public static int Random(int min, int max)
        {
            return _random.Next(min, max + 1);
        }

        public static bool IsPlayer(ICharacterStateObject state)
        {
            return IsPlayer(state.WorldObjectId);
        }

        public static bool IsNonPlayer(ICharacterStateObject state)
        {
            return IsNonPlayer(state.WorldObjectId);
        }



        public static bool IsPlayer(this WorldObjectId objectId)
        {
            if (objectId == WorldObjectId.Empty)
                return false;

            return objectId.ObjectType == WorldObjectType.Player;
        }

        public static bool IsNonPlayer(this WorldObjectId objectId)
        {
            if (objectId == WorldObjectId.Empty)
                return false;

            return objectId.ObjectType == WorldObjectType.NonPlayer;
        }

        public static bool IsGameObject(this WorldObjectId objectId)
        {
            if (objectId == WorldObjectId.Empty)
                return false;

            return objectId.ObjectType == WorldObjectType.GameObject;
        }









        public static Quaternion FromEulerDegrees(this Vector3 deg)
        {
            var radX = MathF.PI / 180f * deg.X;
            var radY = MathF.PI / 180f * deg.Y;
            var radZ = MathF.PI / 180f * deg.Z;

            var qx = Quaternion.CreateFromAxisAngle(Vector3.UnitX, radX);
            var qy = Quaternion.CreateFromAxisAngle(Vector3.UnitY, radY);
            var qz = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, radZ);

            // Same order we used before
            return qy * qx * qz;
        }
    }
}

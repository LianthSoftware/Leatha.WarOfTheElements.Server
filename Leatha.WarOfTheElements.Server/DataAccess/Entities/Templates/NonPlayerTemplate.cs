using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;
using System.ComponentModel.DataAnnotations;

namespace Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates
{
    public sealed class NonPlayerTemplate : MongoEntity
    {
        public int NonPlayerId { get; set; }

        public string Name { get; set; } = null!;

        public string? Title { get; set; }

        public int Level { get; set; }


        public float SpeedWalk { get; set; }

        public float SpeedRun { get; set; }


        public int MaxHealth { get; set; }

        public int MaxPrimaryChakra { get; set; }

        public int MaxSecondaryChakra { get; set; }

        public int MaxTertiaryChakra { get; set; }


        public ElementTypes PrimaryElementType { get; set; }

        public ElementTypes SecondaryElementType { get; set; }

        public ElementTypes TertiaryElementType { get; set; }


        public string? ScriptName { get; set; }
    }
}

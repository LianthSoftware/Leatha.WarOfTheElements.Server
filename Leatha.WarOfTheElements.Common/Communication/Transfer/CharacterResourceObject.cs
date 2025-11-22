using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class CharacterResourceObject
    {
        public int Health { get; set; }

        public int MaxHealth { get; set; }

        public ChakraResource PrimaryChakra { get; set; } = new();

        public ChakraResource SecondaryChakra { get; set; } = new();

        public ChakraResource TertiaryChakra { get; set; } = new();

        public float GetHealthPercent()
        {
            if (MaxHealth <= 0)
                return 0;

            return ((float)Health / MaxHealth) * 100.0f;
        }

        public float GetPrimaryChakraPercent()
        {
            if (PrimaryChakra.Chakra <= 0 || PrimaryChakra.MaxChakra <= 0)
                return 0;

            return (PrimaryChakra.Chakra / PrimaryChakra.MaxChakra) * 100.0f;
        }

        public float GetSecondaryChakraPercent()
        {
            if (SecondaryChakra.Chakra <= 0 || SecondaryChakra.MaxChakra <= 0)
                return 0;

            return (SecondaryChakra.Chakra / SecondaryChakra.MaxChakra) * 100.0f;
        }

        public float GetTertiaryChakraPercent()
        {
            if (TertiaryChakra.Chakra <= 0 || TertiaryChakra.MaxChakra <= 0)
                return 0;

            return (TertiaryChakra.Chakra / TertiaryChakra.MaxChakra) * 100.0f;
        }
    }

    public sealed class ChakraResource
    {
        public ElementTypes Element { get; set; }

        public float Chakra { get; set; }

        public float MaxChakra { get; set; }

        public float ChakraPerSecond { get; set; }
    }
}

using System.Runtime.Serialization;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer.Enums
{
    [Flags]
    public enum ElementTypes
    {
        [EnumMember(Value = nameof(Nature))]
        None                            = 0x0, // 0

        [EnumMember(Value = nameof(Air))]
        Air                             = 0x1, // 1

        [EnumMember(Value = nameof(Fire))]
        Fire                            = 0x2, // 2

        [EnumMember(Value = nameof(Lightning))]
        Lightning                       = 0x4, // 4

        [EnumMember(Value = nameof(Nature))]
        Nature                          = 0x8, // 8

        [EnumMember(Value = nameof(Water))]
        Water                           = 0x10, // 16
    }
}
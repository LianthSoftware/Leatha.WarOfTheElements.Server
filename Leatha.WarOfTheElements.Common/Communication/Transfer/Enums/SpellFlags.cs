namespace Leatha.WarOfTheElements.Common.Communication.Transfer.Enums;

[Flags]
public enum SpellFlags
{
    None                                = 0x0,
    IsPassive                           = 0x1,
    IsChanneled                         = 0x2,
    IsEnhancement                       = 0x4
}
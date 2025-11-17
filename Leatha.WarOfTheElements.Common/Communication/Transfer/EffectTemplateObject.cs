using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Leatha.WarOfTheElements.Common.Communication.Transfer
{
    public sealed class EffectTemplateObject
    {
        [Required]
        [JsonPropertyName("effectId")]
        public int EffectId { get; set; }

        [Required]
        [JsonPropertyName("effectName")]
        public string EffectName { get; set; } = null!;

        [Required]
        [JsonPropertyName("effectDescription")]
        public string EffectDescription { get; set; } = null!;

        [Required]
        [JsonPropertyName("effectType")]
        public EffectType EffectType { get; set; }

        [Required]
        [JsonPropertyName("effectScriptName")]
        public string? EffectScriptName { get; set; }
    }

    [Flags]
    public enum EffectType
    {
        None                            = 0x0,
        StartOfCombat                   = 0x1,
        OnAttack                        = 0x2,
        OnAfterAttack                   = 0x4,
        Deathrattle                     = 0x8,
        Battlecry                       = 0x10,
        EndOfTurn                       = 0x20,
        StartOfTurn                     = 0x40,
        OnPlay                          = 0x80,
        OnSummon                        = 0x100,
    }

    public enum EffectActionType
    {
        None                            = 0,
        ChooseNewTarget                 = 1,
        DealDamage                      = 2,
        GainStats                       = 3,
        ApplyAura                       = 4,
    }

    public enum EffectTargetType
    {
        None                            = 0,
        Self                            = 1,
        AllFriendlyExceptSelf           = 2,
        RandomFriendly                  = 3,
        RandomFriendlyExceptSelf        = 4,
        RandomEnemy                     = 5,
        AllEnemies                      = 6,
        RandomCharacter                 = 7,
        RandomCharacterExceptSelf       = 8,
        AllFriendly                     = 9,
    }

    [Flags]
    public enum EffectFags
    {
        None                            = 0x0,
        Passive                         = 0x1,
        IgnoreTaunt                     = 0x2,
    }
}

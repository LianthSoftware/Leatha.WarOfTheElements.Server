namespace Leatha.WarOfTheElements.Server.Objects.Spells
{
    public sealed class SpellRank
    {
        public int SpellId { get; set; }

        public int Rank { get; set; }

        public int FirstRankSpellId { get; set; }

        public int? PreviousRankSpellId { get; set; }

        public int? NextRankSpellId { get; set; }
    }
}

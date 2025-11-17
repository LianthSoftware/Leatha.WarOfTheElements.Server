using Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates;

namespace Leatha.WarOfTheElements.Server.Demo
{
    public static class Spells
    {
        public static List<SpellTemplate> GetSpellTemplates()
        {
            var templates = new List<SpellTemplate>
            {
                new SpellTemplate
                {
                    SpellId = 1,
                    SpellName = "Waterbolt Enhancement (Rank 1)", // #TODO: Better Name.
                    SpellDescription = "Increase the damage of [color=goldenrod]Fireball[/color] by [color=green]5%[/color].",
                    SpellRank = new SpellRank
                    {
                        SpellId = 1,
                        Rank = 1,
                        FirstRankSpellId = 1,
                        PreviousRankSpellId = null,
                        NextRankSpellId = 2,
                    }
                },
                new SpellTemplate
                {
                    SpellId = 2,
                    SpellName = "Waterbolt Enhancement (Rank 2)", // #TODO: Better Name.
                    SpellDescription = "Increase the damage of [color=goldenrod]Fireball[/color] by [color=green]10%[/color].",
                    SpellRank = new SpellRank
                    {
                        SpellId = 2,
                        Rank = 2,
                        FirstRankSpellId = 1,
                        PreviousRankSpellId = 1,
                        NextRankSpellId = 3,
                    }
                },
                new SpellTemplate
                {
                    SpellId = 3,
                    SpellName = "Waterbolt Enhancement (Rank 3)", // #TODO: Better Name.
                    SpellDescription = "Increase the damage of [color=goldenrod]Fireball[/color] by [color=green]15%[/color].",
                    SpellRank = new SpellRank
                    {
                        SpellId = 3,
                        Rank = 3,
                        FirstRankSpellId = 1,
                        PreviousRankSpellId = 2,
                        NextRankSpellId = null,
                    }
                }
            };


            return templates;
        }
    }

    public sealed class SpellRank
    {
        public int SpellId { get; set; }

        public int Rank { get; set; }

        public int FirstRankSpellId { get; set; }

        public int? PreviousRankSpellId { get; set; }

        public int? NextRankSpellId { get; set; }
    }
}

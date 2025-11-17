namespace Leatha.WarOfTheElements.Server.DataAccess.Entities
{
    public sealed class PlayerSpellBook : MongoEntity
    {
        public Guid PlayerId { get; set; }

        public List<int> LearntSpells { get; set; } = [];

        public List<int> LearntEnhancements { get; set; } = [];
    }
}

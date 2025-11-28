using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates;
using Leatha.WarOfTheElements.Server.Utilities;
using MongoDB.Driver;

namespace Leatha.WarOfTheElements.Server.Services
{
    public interface IColliderArchetypesLoader
    {
        Task<List<ColliderArchetype>> LoadArchetypesAsync();
    }

    public sealed class ColliderArchetypesLoader : IColliderArchetypesLoader
    {
        public ColliderArchetypesLoader(IMongoClient mongoClient)
        {
            _mongoGameDatabase = mongoClient.GetDatabase(Constants.MongoGameDb);
        }

        private readonly IMongoDatabase _mongoGameDatabase;

        public Dictionary<string, ColliderArchetype> ColliderArchetypes { get; private set; } = [];

        public async Task<List<ColliderArchetype>> LoadArchetypesAsync()
        {
            var filter = Builders<ColliderArchetype>.Filter.Empty;

            var archetypes = await _mongoGameDatabase.GetMongoCollection<ColliderArchetype>()
                .Find(filter)
                .ToListAsync();

            ColliderArchetypes = archetypes
                .ToDictionary(i => i.Name, n => n);

            return archetypes;
        }
    }
}

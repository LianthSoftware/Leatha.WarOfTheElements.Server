using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.Utilities;
using MongoDB.Driver;

namespace Leatha.WarOfTheElements.Server.Services
{
    public interface IEventService
    {
        Task LoadEvents();
    }
    public sealed class EventService : IEventService
    {
        public EventService(IMongoClient client)
        {
            _mongoGameDatabase = client.GetDatabase(Constants.MongoGameDb);
        }

        private readonly IMongoDatabase _mongoGameDatabase;

        // Key = WorldObjectId | Value = List of events.
        public Dictionary<int, List<EventScript>> EventScripts { get; private set; } = [];

        public async Task LoadEvents()
        {
            var filter = Builders<EventScript>.Filter.Empty;

            var events = await _mongoGameDatabase.GetMongoCollection<EventScript>()
                .Find(filter)
                .ToListAsync();

            EventScripts = events
                .GroupBy(i => i.WorldObjectId)
                .ToDictionary(i => i.Key, n => n.ToList());
        }
    }
}

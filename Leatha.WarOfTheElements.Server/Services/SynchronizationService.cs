using System.Diagnostics;
using Leatha.WarOfTheElements.Common.Communication.Services;
using Microsoft.VisualBasic;
using MongoDB.Driver;

namespace Leatha.WarOfTheElements.Server.Services
{
    public interface ISynchronizationService
    {
        Task ProcessAsync(CancellationToken cancellationToken = default);
    }

    public sealed class SynchronizationService : ISynchronizationService
    {
        public SynchronizationService(
            IMongoClient client,
            IServerToClientHandler serverClientHandler,
            IServiceProvider serviceProvider)
        {
            //_mongoGameDatabase = client.GetDatabase(Constants.MongoGameDb);
            _serverClientHandler = serverClientHandler;
            _serviceProvider = serviceProvider;
        }

        private readonly IMongoDatabase _mongoGameDatabase;
        private readonly IServerToClientHandler _serverClientHandler;
        private readonly IServiceProvider _serviceProvider;

        public async Task ProcessAsync(CancellationToken cancellationToken = default)
        {
            
        }
    }
}

using System.Diagnostics;

namespace Leatha.WarOfTheElements.Server.Services
{
    public sealed class SyncBackgroundService : BackgroundService
    {
        public SyncBackgroundService(
            IServiceProvider serviceProvider,
            ISynchronizationService synchronizationService)
        {
            _serviceProvider = serviceProvider;

            _synchronizationService = synchronizationService;
        }

        private readonly IServiceProvider _serviceProvider;
        private readonly ISynchronizationService _synchronizationService;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //await _synchronizationService.ProcessAsync(stoppingToken);

                    //using var scope = _serviceProvider.CreateScope();
                    //var queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();
                    //await queueService.ProcessQueueAsync(stoppingToken);

                    // Wait for 1 second.
                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    // Something went wrong ?
                    Debug.WriteLine(ex);
                }
            }
        }
    }
}
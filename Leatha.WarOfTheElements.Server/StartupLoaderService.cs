using System.Diagnostics;
using Leatha.WarOfTheElements.Server.Services;
using Leatha.WarOfTheElements.World.Physics;
using Leatha.WarOfTheElements.World.Terrain;
using SixLabors.ImageSharp;
using System.Numerics;

namespace Leatha.WarOfTheElements.Server
{
    public sealed class StartupLoaderService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public StartupLoaderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine("*** Startup Loader Service: Starting ***");

            //using var scope = _serviceProvider.CreateScope();
            //var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

            // Load collision shapes.
            await LoadCollisionArchetypes();

            // Load events.
            await LoadEventsAsync();

            // Load Terrain data.
            LoadTerrainData();

            // Load scripts.
            LoadScripts();

            Debug.WriteLine("*** Startup Loader Service: Finished ***");
        }

        private async Task LoadCollisionArchetypes()
        {
            Debug.WriteLine("*** Startup Loader Service: Loading collision archetypes ***");

            var archetypesLoader = _serviceProvider.GetRequiredService<IColliderArchetypesLoader>();

            // Load events.
            var archetypes = await archetypesLoader.LoadArchetypesAsync();

            Debug.WriteLine($"*** Startup Loader Service: Loaded \"{ archetypes.Count }\" collider archetypes ***");
        }

        private async Task LoadEventsAsync()
        {
            Debug.WriteLine("*** Startup Loader Service: Loading events ***");

            var eventService = _serviceProvider.GetRequiredService<IEventService>();

            // Load events.
            await eventService.LoadEvents();
        }

        private void LoadTerrainData()
        {
            Debug.WriteLine("*** Startup Loader Service: Loading terrain ***");

            var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
            var physicsWorld = _serviceProvider.GetRequiredService<PhysicsWorld>();

            //var gravity = new Vector3(0, -9.81f, 0);
            //var physicsWorld = new PhysicsWorld(gravity);

            // Terrain config (optional, but this is where it hooks in)
            var metaPath = configuration["Terrain:MetaPath"];
            var heightPath = configuration["Terrain:HeightmapPath"];
            var maxHeight = configuration.GetValue("Terrain:MaxHeight", 100f);

            // #TODO: There must be foreach for different maps later.
            if (!string.IsNullOrWhiteSpace(metaPath) &&
                !string.IsNullOrWhiteSpace(heightPath) &&
                File.Exists(metaPath) &&
                File.Exists(heightPath))
            {
                var terrain = TerrainLoader.Load(metaPath, heightPath, maxHeight);
                physicsWorld.AddTerrain(terrain);
            }
            else
                //physicsWorld.AddFlatGround(500.0f, 500.0f, 0.0f); // #TODO
                physicsWorld.AddFlatGround(500.0f, 500.0f, 0.25f); // #TODO
        }

        private void LoadScripts()
        {
            Debug.WriteLine("*** Startup Loader Service: Loading scripts ***");

            var scriptService = _serviceProvider.GetRequiredService<IScriptService>();

            scriptService.LoadNonPlayerScripts();
            scriptService.LoadSpellScripts();
            scriptService.LoadAuraScripts();
            scriptService.LoadAreaTriggerScripts();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}

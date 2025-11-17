
using FluentValidation.AspNetCore;
using Leatha.WarOfTheElements.Common.Communication.Services;
using Leatha.WarOfTheElements.Server.Objects.Game;
using Leatha.WarOfTheElements.Server.Services;
using Leatha.WarOfTheElements.World.Physics;
using Leatha.WarOfTheElements.World.Terrain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;

namespace Leatha.WarOfTheElements.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services, builder.Configuration);

            builder.Services.AddControllers()
                .AddJsonOptions(i => i.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "War Of The Elements Game API", Version = "v1" });

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            },
                            Scheme = "oauth2",
                            Name = JwtBearerDefaults.AuthenticationScheme,
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    var secret = builder.Configuration["JwtSettings:Secret"];
                    if (String.IsNullOrWhiteSpace(secret))
                        throw new ArgumentNullException(nameof(secret), "JwtSettings:Secret is missing.");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            builder.Services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            builder.Services.AddSingleton<IUserIdProvider, AuthUserIdProvider>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<GameHub>("/gamehub");

            app.Run();
        }


        private static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

            //services.AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new StringEnumConverter()));

            // Mongo DB.
            {
                BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

                services.AddSingleton<IMongoClient>(_ =>
                {
                    var connectionString = configuration.GetConnectionString("MongoDB");
                    var client = new MongoClient(connectionString);
                    return client;
                });
            }

            // Services.
            {
                // Scoped.
                services.AddScoped<IAuthService, AuthService>();
                services.AddScoped<ISpellService, SpellService>();

                // Singleton.
                services.AddSingleton<IServerToClientHandler, ServerToClientHandler>();
                services.AddSingleton<IGameHubService, GameHubService>();
                services.AddSingleton<IPresenceTracker, PresenceTracker>();
                services.AddSingleton<ITemplateService, TemplateService>();

                services.AddSingleton<IPlayerService, PlayerService>();

                services.AddSingleton<IInputQueueService, InputQueueService>();
                services.AddSingleton<IGameWorld, GameWorld>();
                services.AddSingleton<PhysicsWorld>(_ =>
                {
                    var gravity = new Vector3(0, -9.81f, 0);
                    var physicsWorld = new PhysicsWorld(gravity);

                    // Terrain config (optional, but this is where it hooks in)
                    var metaPath = configuration["Terrain:MetaPath"];
                    var heightPath = configuration["Terrain:HeightmapPath"];
                    var maxHeight = configuration.GetValue("Terrain:MaxHeight", 100f);

                    if (!string.IsNullOrWhiteSpace(metaPath) &&
                        !string.IsNullOrWhiteSpace(heightPath) &&
                        File.Exists(metaPath) &&
                        File.Exists(heightPath))
                    {
                        var terrain = TerrainLoader.Load(metaPath, heightPath, maxHeight);
                        physicsWorld.AddTerrain(terrain);
                    }
                    else
                        physicsWorld.AddFlatGround(500.0f, 500.0f, 0.0f);

                    return physicsWorld;
                });

                // Hosted Service.
                services.AddHostedService<GameLoopBackgroundService>();
            }
        }
    }
}

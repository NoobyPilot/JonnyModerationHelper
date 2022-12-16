using DatabaseManagement.Abstractions;
using DatabaseManagement.Extensions;
using DatabaseManagement.Objects;
using JonnyModerationHelper.Commands;
using JonnyModerationHelper.Responders;
using JonnyModerationHelper.Services;
using JonnyModerationHelper.Services.Abstractions;
using JonnyModerationHelper.Services.Discord;
using JonnyModerationHelper.Services.Discord.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Commands.Extensions;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Caching.Redis.Extensions;
using Remora.Discord.Caching.Services;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Hosting.Extensions;
using Remora.Rest.Core;

namespace JonnyModerationHelper;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args)
                  .UseConsoleLifetime()
                  .Build();
        var services = host.Services;
        var log = services.GetRequiredService<ILogger<Program>>();
        var configuration = services.GetRequiredService<IConfiguration>();

        Snowflake? debugServer = null;

#if DEBUG
        var debugServerString = configuration.GetValue<string?>("REMORA_DEBUG_SERVER");
        if (debugServerString is not null)
        {
            if (!DiscordSnowflake.TryParse(debugServerString, out debugServer))
            {
                log.LogWarning("Failed to parse debug server from environment");
            }
        }
#endif

        var slashService = services.GetRequiredService<SlashService>();
        var updateSlash = await slashService.UpdateSlashCommandsAsync(debugServer);
        if (!updateSlash.IsSuccess)
        {
            log.LogWarning("Failed to update slash commands: {Reason}", updateSlash.Error.Message);
        }

        var dbManagement = services.GetRequiredService<IDatabaseManager>();
        await dbManagement.PrepareComponents();
        await dbManagement.InitialiseDatabase();

        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                   .AddDiscordService(services =>
                    {
                        var configuration =
                            services
                               .GetRequiredService<
                                    IConfiguration>();
                        return
                            configuration
                               .GetValue<string
                                    ?>("REMORA_BOT_TOKEN") ??
                            throw new
                                InvalidOperationException("No bot token has been provided. Set the REMORA_BOT_TOKEN environment variable to a valid token.");
                    })
                   .ConfigureServices((_, services) =>
                    {
                        services.Configure<CacheSettings>(settings =>
                        {
                            settings.SetDefaultAbsoluteExpiration(TimeSpan.FromSeconds(60));
                            settings.SetDefaultSlidingExpiration(TimeSpan.FromSeconds(20));
                            settings.SetAbsoluteExpiration<IMessage>(TimeSpan.FromSeconds(10));
                        });
                        services.AddDiscordRedisCaching();
                        services.Configure<DiscordGatewayClientOptions>(g => g.Intents |=
                                                                            GatewayIntents.MessageContents);
                        services.AddDiscordCommands(true)
                                .AddCommandTree()
                                .WithCommandGroup<UserCommands>()
                                .WithCommandGroup<DebugCommands>();
                        services.AddDatabaseConnection(serv =>
                        {
                            var config = serv.GetRequiredService<IConfiguration>();
                            return config.GetValue<string>("DATABASE_CONNECTION_STRING") ??
                                         throw new
                                             InvalidOperationException("No database connection string was provided. Set the DATABASE_CONNECTION_STRING environment variable to a valid connection string");
                        });
                        services.AddSingleton<DatabaseManagementParameters>(_ => new DatabaseManagementParameters(new Uri("C:\\Users\\cedri\\RiderProjects\\JonnyModerationHelper\\JonnyModerationHelper\\DatabaseScripts")));
                        
                        services.AddDatabaseManagement();
                        services.AddScoped<IDiscordMemberModerationService, DiscordMemberModerationService>();
                        services.AddScoped<IMemberModerationService, MemberModerationService>();
                        services.AddScoped<IModerationDatabaseService, ModerationDatabaseService>();
                        services.AddScoped<IModeratorLoggingService, ModeratorLoggingService>();
                        services.AddResponder<GuildCreateResponder>();

                    })
                   .ConfigureLogging(c => c.AddConsole()
                                           .AddFilter("System.Net.Http.HttpClient.*.LogicalHandler", LogLevel.Warning)
                                           .AddFilter("System.Net.Http.HttpClient.*.ClientHandler", LogLevel.Warning));
    }
}
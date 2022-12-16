using DatabaseManagement.Abstractions;
using DatabaseManagement.Objects;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseManagement.Extensions;

public static class ServiceProviderExtension
{

    
    public static void AddDatabaseManagement(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IDatabaseStatusService, DatabaseStatusService>();
        serviceCollection.AddSingleton<IDatabaseManager, DatabaseManager>();
        serviceCollection.AddSingleton<IUpdateManager, UpdateManager>();
    }

    public static void AddDatabaseConnection(this IServiceCollection        serviceCollection,
                                             Func<IServiceProvider, string> connectionStringFactory)
    {
        serviceCollection.AddSingleton<IDatabaseConnection, DatabaseConnection>();
        serviceCollection.AddSingleton<DatabaseParameters>(services =>
                                                               new DatabaseParameters(connectionStringFactory(services)));
    }

    public static void AddDatabaseConnection(this IServiceCollection serviceCollection, string connectionString)
    {
        serviceCollection.AddDatabaseConnection(_ => connectionString);
    }

}
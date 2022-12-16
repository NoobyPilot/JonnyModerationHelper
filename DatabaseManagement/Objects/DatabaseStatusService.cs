using DatabaseManagement.Abstractions;
using Microsoft.Extensions.Logging;

namespace DatabaseManagement.Objects;

public class DatabaseStatusService : IDatabaseStatusService
{
    
    private const string SchemaExistsTemplate = @"SELECT COUNT(*)>0
FROM information_schema.schemata
WHERE schema_name = '{0}'";

    private const string SchemaVersionTemplate = @"SELECT DATABASE_VERSION
FROM public.database_log
WHERE SCHEMA_NAME = '{0}'
ORDER BY TIME DESC
LIMIT 1";

    private const string SchemaLastUpdateTemplate = @"SELECT TIME
FROM public.database_log
WHERE schema_name = '{0}'
ORDER BY TIME DESC
LIMIT 1";

    private const string DatabaseManagementInitialisedQuery = @"SELECT COUNT(*)>0
FROM information_schema.tables
WHERE table_name = 'database_log'";
    
    private readonly IDatabaseConnection            _databaseConnection;
    private readonly ILogger<DatabaseStatusService> _logger;

    public DatabaseStatusService(IDatabaseConnection connection, ILogger<DatabaseStatusService> logger)
    {
        _databaseConnection = connection;
        _logger = logger;
    }
    
    public async Task<bool> SchemaExists(string  schemaName)
    {
        _logger.LogInformation($"Checking schema {schemaName} exists");
        var reader = await _databaseConnection.ExecuteQuery(string.Format(SchemaExistsTemplate, schemaName));
        if (!await reader.ReadAsync())
        {
            _logger.LogDebug($"Schema {schemaName} does not exist, there is no row");
            return false;
        }
        _logger.LogDebug($"Schema {schemaName} has a row, the boolean is {reader.GetBoolean(0)}");
        return reader.GetBoolean(0);
    }

    public async Task<int> SchemaVersion(string schemaName, bool schemaChecked = false)
    {
        if (!schemaChecked && !await SchemaExists(schemaName))
        {
            return -1;
        }
        var reader = await _databaseConnection.ExecuteQuery(string.Format(SchemaVersionTemplate, schemaName));
        if (! await reader.ReadAsync())
        {
            return -1;
        }
        return reader.GetInt32(0);
    }

    public async Task<DateTime>     LastSchemaUpdate(string schemaName)
    {
        if (!await SchemaExists(schemaName))
        {
            return DateTime.MinValue;
        }
        var reader = await _databaseConnection.ExecuteQuery(string.Format(SchemaLastUpdateTemplate, schemaName));
        if (! await  reader.ReadAsync())
        {
            return DateTime.MinValue;
        }
        return DateTime.Parse(reader.GetString(0));
    }

    public async Task<bool> WasSystemInitialised()
    {
        var res = await _databaseConnection.ExecuteScalar(DatabaseManagementInitialisedQuery);
        return res is true;
    }
}
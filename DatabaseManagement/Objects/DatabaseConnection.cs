using DatabaseManagement.Abstractions;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DatabaseManagement.Objects;

public class DatabaseConnection : IDatabaseConnection
{
    private readonly NpgsqlDataSource            _dataSource;
    private readonly ILogger<DatabaseConnection> _logger;

    public DatabaseConnection(DatabaseParameters parameters, ILogger<DatabaseConnection> logger)
    {
        _dataSource = NpgsqlDataSource.Create(parameters.ConnectionString);
        _logger = logger;
        _logger.LogInformation("New Database Connection created");
    }
    
    public Task<NpgsqlDataReader> ExecuteQuery(string query)
    {
        _logger.LogDebug($"Preparing SQL {query}");
        var command = _dataSource.CreateCommand(query);
        _logger.LogDebug("Executing reader");
        return command.ExecuteReaderAsync();
    }

    public Task<NpgsqlDataReader> ExecuteQuery(string query, List<NpgsqlParameter> parameters)
    {
        _logger.LogDebug($"Preparing SQL {query} with parameters");
        var command = _dataSource.CreateCommand(query);
        foreach (var parameter in parameters)
        {
            _logger.LogDebug(parameter.Value?.ToString());
            command.Parameters.Add(parameter);
        }
        _logger.LogDebug("Executing reader");
        return command.ExecuteReaderAsync();
    }
    
    public Task<int> ExecuteStatement(string statement)
    {
        _logger.LogDebug($"Preparing SQL {statement}");
        var command = _dataSource.CreateCommand(statement);
        _logger.LogDebug("Executing NonQuery");
        return command.ExecuteNonQueryAsync();
    }

    public Task<object?> ExecuteScalar(string statement)
    {
        _logger.LogDebug($"Preparing SQL {statement}");
        var command = _dataSource.CreateCommand(statement);
        _logger.LogDebug("Executing Scalar");
        return command.ExecuteScalarAsync();
    }
    
}
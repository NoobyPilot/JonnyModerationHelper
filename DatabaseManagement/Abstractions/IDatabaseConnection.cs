using Npgsql;

namespace DatabaseManagement.Abstractions;

public interface IDatabaseConnection
{
    public Task<NpgsqlDataReader> ExecuteQuery(string     query);
    public Task<NpgsqlDataReader> ExecuteQuery(string     query, List<NpgsqlParameter> parameters);
    public Task<int>              ExecuteStatement(string statement);
    public Task<object?>          ExecuteScalar(string    statement);
}
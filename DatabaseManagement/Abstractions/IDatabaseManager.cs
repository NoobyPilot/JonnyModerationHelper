namespace DatabaseManagement.Abstractions;

public interface IDatabaseManager
{
    public Task CreateSchema(string         schemaName);
    public Task UpdateSchema(string         schemaName, int? schemaVersion);
    public Task EnsureSchemaUpToDate(string schemaName);
    public Task InitialiseDatabase();
    public Task PrepareComponents();
}
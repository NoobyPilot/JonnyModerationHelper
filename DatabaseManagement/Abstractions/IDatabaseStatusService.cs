namespace DatabaseManagement.Abstractions;

public interface IDatabaseStatusService
{
    public Task<bool>     SchemaExists(string     schemaName);
    public Task<int>      SchemaVersion(string    schemaName, bool checkedExists = false);
    public Task<DateTime> LastSchemaUpdate(string schemaName);
    public Task<bool>     WasSystemInitialised();
}
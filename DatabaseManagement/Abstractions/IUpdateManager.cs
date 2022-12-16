namespace DatabaseManagement.Abstractions;

public interface IUpdateManager
{

    public Task                            ReadAllScripts(string directoryPath);
    public Task<int>                       GetCurrentGlobalVersion();
    public Task<int>                       GetCurrentSchemaVersion();
    public Task<IEnumerable<IGlobalScript>> GetGlobalScriptsToCurrentVersion(int dbVersion);
    public Task<IEnumerable<ISchemaScript>> GetSchemaScriptsToCurrentVersion(int dbVersion);

}
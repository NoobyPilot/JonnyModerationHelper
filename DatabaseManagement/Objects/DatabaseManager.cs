using DatabaseManagement.Abstractions;
using DatabaseManagement.Objects.Scripts;

namespace DatabaseManagement.Objects;

public class DatabaseManager : IDatabaseManager
{

    private readonly IUpdateManager               _updateManager;
    private readonly IDatabaseStatusService       _databaseStatusService;
    private readonly IDatabaseConnection          _databaseConnection;
    private readonly DatabaseManagementParameters _managementParameters;

    public DatabaseManager(IUpdateManager updateManager, IDatabaseStatusService databaseStatusService, IDatabaseConnection databaseConnection, DatabaseManagementParameters managementParameters)
    {
        _updateManager = updateManager;
        _databaseStatusService = databaseStatusService;
        _databaseConnection = databaseConnection;
        _managementParameters = managementParameters;
    }
    
    public async Task CreateSchema(string         schemaName)
    {
        var scripts = await _updateManager.GetSchemaScriptsToCurrentVersion(0);
        foreach (var schemaScript in scripts)
        {
            await ExecuteScript(schemaScript, schemaName);
        }
    }

    public async Task UpdateSchema(string schemaName, int? schemaVersion = null)
    {
        int curSchemaVersion = schemaVersion ?? await _databaseStatusService.SchemaVersion(schemaName);
        var scripts = await _updateManager.GetSchemaScriptsToCurrentVersion(curSchemaVersion);
        foreach (var schemaScript in scripts)
        {
            await ExecuteScript(schemaScript, schemaName);
        }
    }

    public async Task EnsureSchemaUpToDate(string schemaName)
    {
        var schemaExists = await _databaseStatusService.SchemaExists(schemaName);
        if (!schemaExists)
        {
            await CreateSchema(schemaName);
        }
        else
        {
            var curSchemaVersion = await _databaseStatusService.SchemaVersion(schemaName, true);
            if (await _updateManager.GetCurrentSchemaVersion() > curSchemaVersion)
            {
                await UpdateSchema(schemaName, curSchemaVersion);
            }
        }
    }

    public async Task InitialiseDatabase()
    {
        var globalVersion = await _databaseStatusService.WasSystemInitialised()
            ? await _databaseStatusService.SchemaVersion("public")
            : 0;
        var updateScripts = await _updateManager.GetGlobalScriptsToCurrentVersion(globalVersion);
        foreach (var updateScript in updateScripts)
        {
            await ExecuteScript(updateScript);
        }
    }

    public async Task PrepareComponents()
    {
        string baseScriptDir;
        if (_managementParameters.UpdateScriptLocation.IsFile)
        {
            baseScriptDir = _managementParameters.UpdateScriptLocation.LocalPath;
        }
        else
        {
            baseScriptDir = "";
        }
        await _updateManager.ReadAllScripts(baseScriptDir);
    }

    private async Task ExecuteScript(IGlobalScript script)
    {
        if (script is not DatabaseScript dbScript)
        {
            return;
        }
        foreach (var statement in dbScript.Statements)
        {
            await _databaseConnection.ExecuteStatement(statement);
        }
    }

    private async Task ExecuteScript(ISchemaScript script, string schemaName)
    {
        foreach (var statement in script.PrepareStatementsForSchema(schemaName))
        {
            await _databaseConnection.ExecuteStatement(statement);
        }
    }
    
}
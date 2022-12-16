using System.Text.Json;
using System.Text.Json.Nodes;
using DatabaseManagement.Abstractions;
using DatabaseManagement.Extensions;
using DatabaseManagement.Objects.Scripts;

namespace DatabaseManagement.Objects;

public class UpdateManager : IUpdateManager
{
    private readonly List<GlobalBaselineScript> _globalBaselines = new();
    private readonly List<GlobalUpdateScript>   _globalUpdates = new();
    private readonly List<SchemaBaselineScript> _schemaBaselines = new();
    private readonly List<SchemaUpdateScript>   _schemaUpdates = new();

    private          int  _currentMaxGlobal = -1;
    private          int  _currentMaxSchema = -1;
    private volatile bool _updating;

    public async Task ReadAllScripts(string directoryPath)
    {
        _updating = true;
        _globalBaselines.Clear();
        _globalUpdates.Clear();
        _schemaBaselines.Clear();
        _schemaUpdates.Clear();
        _currentMaxGlobal = -1;
        _currentMaxSchema = -1;
        await ReadAllScriptsInt(directoryPath);
        _currentMaxGlobal = _globalBaselines.Concat<DatabaseScript>(_globalUpdates)
                                            .OrderByDescending(script => script.TargetVersion).First().TargetVersion;
        _currentMaxSchema = _schemaBaselines.Concat<DatabaseScript>(_schemaUpdates)
                                            .OrderByDescending(script => script.TargetVersion).First().TargetVersion;
        _updating = false;
    }

    private async Task ReadAllScriptsInt(string directoryPath)
    {
        foreach (var fileName in Directory.EnumerateFiles(directoryPath))
        {
            // read all files
            var root = JsonNode.Parse(await File.ReadAllTextAsync(fileName));
            if (root is not JsonObject rootObject)
            {
                throw new JsonException();
            }

            var schemaBased = rootObject.GetOrThrow<bool>("schema-based");
            var baseline = rootObject.GetOrThrow<bool>("baseline");
            var toVersion = rootObject.GetOrThrow<int>("to-version");
            var statementArray = rootObject["statements"].GetOrThrow().AsArray();
            if (baseline)
            {
                // baseline script, we can construct
                if (schemaBased)
                {
                    // baseline schema
                    _schemaBaselines.Add(new SchemaBaselineScript(statementArray, toVersion));
                }
                else
                {
                    _globalBaselines.Add(new GlobalBaselineScript(statementArray, toVersion));
                }
            }
            else
            {
                // update script, we need to read the from version still
                var fromVersion = rootObject.GetOrThrow<int>("from-version");
                if (schemaBased)
                {
                    _schemaUpdates.Add(new SchemaUpdateScript(statementArray, fromVersion, toVersion));
                }
                else
                {
                    _globalUpdates.Add(new GlobalUpdateScript(statementArray, fromVersion, toVersion));
                }
            }
        }

        var dirs = Directory.EnumerateDirectories(directoryPath).ToList();
        foreach (var directory in dirs)
        {
            // and recursively read all subdirectories
            await ReadAllScriptsInt(directory);
        }
    }

    public Task<int> GetCurrentGlobalVersion()
    {
        return Task.FromResult(_updating ? -1 : _currentMaxGlobal);
    }

    public Task<int> GetCurrentSchemaVersion()
    {
        return Task.FromResult(_updating ? -1 : _currentMaxSchema);
    }

    public async Task<IEnumerable<IGlobalScript>> GetGlobalScriptsToCurrentVersion(int dbVersion)
    {
        if (_updating)
            throw new
                InvalidOperationException("The system is currently updating all update scripts. Please try again at a later point.");
        if (_currentMaxGlobal <= 0)
            throw new
                InvalidOperationException("Can't get any update scripts before loading any update/baseline scripts");


        if (dbVersion >= _currentMaxGlobal)
        {
            // db is up to date, just give it an empty update list
            return new List<IGlobalScript>();
        }

        if (dbVersion > 0)
        {
            // We just need to find a path from dbVersion to _currentMaxGlobal from only _globalUpdates where the from version is at least the current version
            var filtered = _globalUpdates.Where(update => update.FromVersion >= dbVersion).ToList();

            var scripts = new List<IGlobalScript>();
            var currentVersion = dbVersion;
            while (dbVersion < _currentMaxGlobal)
            {
                var bestUpdate = filtered.Where(update => update.FromVersion == currentVersion)
                                         .OrderByDescending(update => update.TargetVersion).First();
                scripts.Add(bestUpdate);
                currentVersion = bestUpdate.TargetVersion;
                filtered = filtered.Where(update => update.FromVersion >= dbVersion).ToList();
            }

            return scripts;
        }

        // Find the best baseline script, then advance from there
        var bestBaseline = _globalBaselines.OrderByDescending(baseline => baseline.TargetVersion).First();
        var updates = await GetGlobalScriptsToCurrentVersion(bestBaseline.TargetVersion);
        return updates.Prepend(bestBaseline);
    }

    public async Task<IEnumerable<ISchemaScript>> GetSchemaScriptsToCurrentVersion(int dbVersion)
    {
        if (_updating)
            throw new
                InvalidOperationException("The system is currently updating all update scripts. Please try again at a later point.");
        if (_currentMaxSchema <= 0)
            throw new
                InvalidOperationException("Can't get any update scripts before loading any update/baseline scripts");


        if (dbVersion >= _currentMaxSchema)
        {
            return new List<ISchemaScript>();
        }

        if (dbVersion > 0)
        {
            var filtered = _schemaUpdates.Where(update => update.FromVersion >= dbVersion).ToList();

            var scripts = new List<ISchemaScript>();
            var currentVersion = dbVersion;
            while (dbVersion < _currentMaxGlobal)
            {
                var bestUpdate = filtered.Where(update => update.FromVersion == currentVersion)
                                         .OrderByDescending(update => update.TargetVersion).First();
                scripts.Add(bestUpdate);
                currentVersion = bestUpdate.TargetVersion;
                filtered = filtered.Where(update => update.FromVersion >= dbVersion).ToList();
            }

            return scripts;
        }

        var bestBaseline = _schemaBaselines.OrderByDescending(baseline => baseline.TargetVersion).First();
        var updates = await GetSchemaScriptsToCurrentVersion(bestBaseline.TargetVersion);
        return updates.Prepend(bestBaseline);
    }
}
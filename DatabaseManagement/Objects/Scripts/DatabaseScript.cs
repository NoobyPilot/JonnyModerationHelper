using System.Text.Json.Nodes;

namespace DatabaseManagement.Objects.Scripts;

public abstract class DatabaseScript
{
    public int                   TargetVersion { get; protected set; }
    public bool                  Baseline      { get; protected set; }
    public IReadOnlyList<string> Statements    { get => StatementsInt; }

    protected readonly List<string> StatementsInt;

    protected DatabaseScript(JsonArray statementArray, int targetVersion, bool baseline, string schemaOrPlaceholder)
    {
        TargetVersion = targetVersion;
        Baseline = baseline;
        StatementsInt = new List<string>();
        foreach (var node in statementArray)
        {
            if (node != null)
            {
                StatementsInt.Add(node.ToString());
            }
        }
        StatementsInt.Add($"INSERT INTO public.database_log (SCHEMA_NAME, DATABASE_VERSION) VALUES ('{schemaOrPlaceholder}', {TargetVersion})");
    }

    protected DatabaseScript()
    {
        StatementsInt = new List<string>();
    }

}
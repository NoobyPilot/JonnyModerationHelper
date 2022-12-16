using System.Collections.Immutable;
using System.Text.Json.Nodes;
using DatabaseManagement.Abstractions;

namespace DatabaseManagement.Objects.Scripts;

public class SchemaUpdateScript : DatabaseScript, ISchemaScript, IUpdateScript
{
    
    public int FromVersion { get; }
    
    public SchemaUpdateScript(JsonArray statementArray, int fromVersion, int toVersion) : base(statementArray, toVersion, false, "{{SCHEMA_NAME}}")
    {
        FromVersion = fromVersion;
    }
    
    public IReadOnlyList<string> PrepareStatementsForSchema(string schemaName)
    {
        return Statements.Select(statement => statement.Replace("{{SCHEMA_NAME}}", schemaName)).ToImmutableList();
    }
    
}
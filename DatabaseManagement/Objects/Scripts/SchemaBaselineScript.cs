using System.Collections.Immutable;
using System.Text.Json.Nodes;
using DatabaseManagement.Abstractions;

namespace DatabaseManagement.Objects.Scripts;

public class SchemaBaselineScript : DatabaseScript, ISchemaScript
{
    
    public SchemaBaselineScript(JsonArray statementArray, int toVersion) : base(statementArray, toVersion, true, "{{SCHEMA_NAME}}")
    {
        
    }
    
    public IReadOnlyList<string> PrepareStatementsForSchema(string schemaName)
    {
        return Statements.Select(statement => statement.Replace("{{SCHEMA_NAME}}", schemaName)).ToImmutableList();
    }
    
}
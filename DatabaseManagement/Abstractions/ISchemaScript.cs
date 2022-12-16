namespace DatabaseManagement.Abstractions;

public interface ISchemaScript
{
    public IReadOnlyList<string> PrepareStatementsForSchema(string schemaName);
    
}
using System.Text.Json.Nodes;
using DatabaseManagement.Abstractions;

namespace DatabaseManagement.Objects.Scripts;

public class GlobalBaselineScript : DatabaseScript, IGlobalScript
{
    
    public GlobalBaselineScript(JsonArray statementArray, int toVersion) : base(statementArray, toVersion, true, "public")
    {
        
    }
    
}
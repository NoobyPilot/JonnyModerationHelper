using System.Text.Json.Nodes;
using DatabaseManagement.Abstractions;

namespace DatabaseManagement.Objects.Scripts;

public class GlobalUpdateScript : DatabaseScript, IGlobalScript, IUpdateScript
{
    public int FromVersion { get; }
    
    public GlobalUpdateScript(JsonArray statementArray, int fromVersion, int toVersion) : base(statementArray, toVersion, false, "public")
    {
        FromVersion = fromVersion;
    }
    
    
}
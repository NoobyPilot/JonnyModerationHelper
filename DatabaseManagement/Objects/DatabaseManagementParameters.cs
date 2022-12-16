namespace DatabaseManagement.Objects;

public class DatabaseManagementParameters
{
    public readonly Uri UpdateScriptLocation;

    public DatabaseManagementParameters(Uri updateScriptLocation)
    {
        UpdateScriptLocation = updateScriptLocation;
    }
}
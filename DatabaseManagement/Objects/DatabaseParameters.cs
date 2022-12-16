namespace DatabaseManagement.Objects;

public class DatabaseParameters
{
    public readonly string ConnectionString;

    public DatabaseParameters(string connectionString)
    {
        ConnectionString = connectionString;
    }
}
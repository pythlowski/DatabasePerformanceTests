using Microsoft.Data.SqlClient;

namespace DatabasePerformanceTests.Data.Contexts;

public class MssqlDbContext : AbstractDbContext
{
    public MssqlDbContext(string connectionString)
        : base(connectionString)
    {
    }
    
    private SqlConnection GetAdminConnection()
    {
        return new SqlConnection(
            new SqlConnectionStringBuilder(ConnectionString) { InitialCatalog = "master" }.ToString()
        );
    }
    
    public override async Task CreateDatabaseAsync()
    {
        await using var adminConnection = GetAdminConnection();
        await adminConnection.OpenAsync();

        using var cmd = adminConnection.CreateCommand();
        cmd.CommandText = $"CREATE DATABASE [{DatabaseName}]";
        await cmd.ExecuteNonQueryAsync();
        Console.WriteLine($"MSSQL database '{DatabaseName}' created.");
    }

    public override async Task DropDatabaseAsync()
    {
        await using var adminConnection = GetAdminConnection();
        await adminConnection.OpenAsync();

        using var cmd = adminConnection.CreateCommand();
        cmd.CommandText = $"DROP DATABASE [{DatabaseName}]";
        await cmd.ExecuteNonQueryAsync();
        Console.WriteLine($"MSSQL database '{DatabaseName}' dropped.");
    }
}
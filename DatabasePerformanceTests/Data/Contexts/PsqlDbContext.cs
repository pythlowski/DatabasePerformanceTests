using Npgsql;

namespace DatabasePerformanceTests.Data.Contexts;

public class PsqlDbContext : AbstractDbContext
{
    public PsqlDbContext(string connectionString)
        : base(connectionString)
    {
    }
    
    private NpgsqlConnection GetAdminConnection(string databaseName)
    {
        return new NpgsqlConnection(
            new NpgsqlConnectionStringBuilder(ConnectionString) { Database = databaseName }.ToString()
        );
    }
    
    public override async Task CreateDatabaseAsync()
    {
        await using var adminConnection = GetAdminConnection("postgres");
        await adminConnection.OpenAsync();

        using var cmd = adminConnection.CreateCommand();
        cmd.CommandText = $"CREATE DATABASE \"{DatabaseName}\"";
        await cmd.ExecuteNonQueryAsync();
        Console.WriteLine($"PostgreSQL database '{DatabaseName}' created.");

    }

    public override async Task DropDatabaseAsync()
    {
        await using var adminConnection = GetAdminConnection("postgres");
        await adminConnection.OpenAsync();

        using var cmd = adminConnection.CreateCommand();
        cmd.CommandText = $"DROP DATABASE \"{DatabaseName}\"";
        await cmd.ExecuteNonQueryAsync();
        Console.WriteLine($"PostgreSQL database '{DatabaseName}' dropped.");

    }
}
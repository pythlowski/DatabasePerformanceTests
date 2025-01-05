using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Utils.Generators.Models;

namespace DatabasePerformanceTests.Data.Contexts;

public abstract class AbstractDbContext
{
    public string DatabaseName = $"testdb_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}";
    public string ConnectionString { get; }
    protected AbstractDbContext(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public abstract Task CreateDatabaseAsync();
    public abstract Task CreateTablesAsync();
    public abstract Task PopulateDatabaseAsync(GeneratedData data);
    public abstract Task RestoreDatabaseAsync(GeneratedData data);
    public abstract Task DropDatabaseAsync();
}
using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Utils.Database.Models.Enums;
using DatabasePerformanceTests.Utils.Generators.Models;

namespace DatabasePerformanceTests.Data.Contexts;

public abstract class AbstractDbContext
{
    public string DatabaseName = $"testdb_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}";
    public string ConnectionString { get; }
    public abstract DatabaseSystem DatabaseSystem { get; }
    protected AbstractDbContext(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public abstract Task CreateDatabaseAsync();
    public abstract Task CreateTablesAsync();
    public abstract Task PopulateDatabaseAsync(GeneratedData data);
    public abstract Task DropDatabaseAsync();
    
    public abstract Task StartTransactionAsync();
    public abstract Task CommitTransactionAsync();
    public abstract Task RollbackTransactionAsync();
}
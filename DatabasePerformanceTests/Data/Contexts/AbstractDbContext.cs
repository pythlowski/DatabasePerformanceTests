using DatabasePerformanceTests.Utils.Config.Enums;
using DatabasePerformanceTests.Utils.Generators.Models;

namespace DatabasePerformanceTests.Data.Contexts;

public abstract class AbstractDbContext
{
    public string DatabaseName { get; }
    public string ConnectionString { get; }
    public abstract DatabaseSystem DatabaseSystem { get; }
    protected AbstractDbContext(string connectionString, string databaseName)
    {
        ConnectionString = connectionString;
        DatabaseName = databaseName;
    }

    public abstract Task CreateDatabaseAsync();
    public abstract Task CreateTablesAsync();
    public abstract Task PopulateDatabaseAsync(GeneratedData data);
    public abstract Task DropDatabaseAsync();
    
    public abstract Task StartTransactionAsync();
    public abstract Task CommitTransactionAsync();
    public abstract Task RollbackTransactionAsync();

    public abstract Task ClearCacheAsync();
}
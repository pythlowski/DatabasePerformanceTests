namespace DatabasePerformanceTests.Data.Contexts;

public abstract class AbstractDbContext
{
    public string ConnectionString { get; }
    public string DatabaseName => $"testdb_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}";
    protected AbstractDbContext(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public abstract Task CreateDatabaseAsync();
    public abstract Task DropDatabaseAsync();
}
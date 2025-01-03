using DatabasePerformanceTests.Data.Contexts;
using DatabasePerformanceTests.Utils.Models;

namespace DatabasePerformanceTests.Utils;

public class DbContextFactory
{
    public AbstractDbContext CreateDbContext(DatabaseSystem system, string connectionString)
    {
        return system switch
        {
            DatabaseSystem.Mongo => new MongoDbContext(connectionString),
            DatabaseSystem.Postgres => new PsqlDbContext(connectionString),
            DatabaseSystem.MsSql => new MssqlDbContext(connectionString),
            _ => throw new NotSupportedException($"Unsupported database system: {system}")
        };
    }
}
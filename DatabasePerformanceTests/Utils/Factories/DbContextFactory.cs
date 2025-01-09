using DatabasePerformanceTests.Data.Contexts;
using DatabasePerformanceTests.Utils.Config.Enums;

namespace DatabasePerformanceTests.Utils.Factories;

public class DbContextFactory
{
    public AbstractDbContext CreateDbContext(DatabaseSystem system, string connectionString, string databaseName)
    {
        return system switch
        {
            DatabaseSystem.Mongo => new MongoDbContext(connectionString, databaseName),
            DatabaseSystem.Postgres => new PsqlDbContext(connectionString, databaseName),
            DatabaseSystem.MsSql => new MssqlDbContext(connectionString, databaseName),
            _ => throw new NotSupportedException($"Unsupported database system: {system}")
        };
    }
}
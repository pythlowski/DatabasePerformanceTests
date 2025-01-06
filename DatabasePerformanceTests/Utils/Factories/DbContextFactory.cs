using DatabasePerformanceTests.Data.Contexts;
using DatabasePerformanceTests.Utils.Database.Models.Enums;

namespace DatabasePerformanceTests.Utils.Factories;

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
using DatabasePerformanceTests.Data.Contexts;

namespace DatabasePerformanceTests.Utils;

public class DbContextFactory
{
    public AbstractDbContext CreateDbContext(string system, string connectionString)
    {
        return system switch
        {
            "Mongo" => new MongoDbContext(connectionString),
            "PSQL" => new PsqlDbContext(connectionString),
            "MSSQL" => new MssqlDbContext(connectionString),
            _ => throw new NotSupportedException($"Unsupported database system: {system}")
        };
    }
}
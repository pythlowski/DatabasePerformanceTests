using DatabasePerformanceTests.Data.Contexts;
using DatabasePerformanceTests.Data.Operations;
using DatabasePerformanceTests.Data.Operations.Interfaces;

namespace DatabasePerformanceTests.Utils.Factories;

public class OperationsFactory
{
    public static IDbOperations CreateOperations(AbstractDbContext dbContext)
    {
        return dbContext switch
        {
            MongoDbContext => new MongoDbOperations((MongoDbContext)dbContext),
            PsqlDbContext => new PsqlDbOperations((PsqlDbContext)dbContext),
            MssqlDbContext => new MssqlDbOperations((MssqlDbContext)dbContext),
            _ => throw new NotSupportedException($"Unsupported context type: {dbContext.GetType().Name}")
        };
    }
}
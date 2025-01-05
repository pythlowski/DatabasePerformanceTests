using DatabasePerformanceTests.Data.Contexts;

namespace DatabasePerformanceTests.Utils.Database;

public class DatabaseSeeder(AbstractDbContext context)
{
    public async Task PrepareDatabaseAsync()
    {
            await context.CreateDatabaseAsync();
            await context.CreateTablesAsync();
    }
    
    public async Task DropDatabaseAsync()
    {
        await context.DropDatabaseAsync();
    }
}
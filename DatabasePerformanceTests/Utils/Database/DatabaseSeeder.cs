using DatabasePerformanceTests.Data.Contexts;
using DatabasePerformanceTests.Utils.Generators.Models;

namespace DatabasePerformanceTests.Utils.Database;

public class DatabaseSeeder(AbstractDbContext context)
{
    public async Task PrepareDatabaseAsync(GeneratedData generatedData)
    {
            await context.CreateDatabaseAsync();
            await context.CreateTablesAsync();
            await context.PopulateDatabaseAsync(generatedData);
    }
    
    public async Task DropDatabaseAsync()
    {
        await context.DropDatabaseAsync();
    }
}
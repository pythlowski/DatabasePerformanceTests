using DatabasePerformanceTests.Utils;
using DatabasePerformanceTests.Utils.Models;
using Microsoft.Extensions.Configuration;

namespace DatabasePerformanceTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting tests...");

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var databaseConfigs = configuration.GetSection("Connections").Get<DatabaseConfig[]>();

            var factory = new DbContextFactory();

            foreach (var config in databaseConfigs)
            {
                Console.WriteLine(config.ConnectionString);
                var dbContext = factory.CreateDbContext(config.System, config.ConnectionString);
                
                try
                {
                    await dbContext.CreateDatabaseAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating database: {ex.Message}");
                }

                try
                {
                    await dbContext.DropDatabaseAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error dropping database: {ex.Message}");
                }
            }
        }
    }
}
using DatabasePerformanceTests.Utils;
using DatabasePerformanceTests.Utils.Database;
using DatabasePerformanceTests.Utils.Database.Models;
using DatabasePerformanceTests.Utils.Database.Models.Enums;
using DatabasePerformanceTests.Utils.Generators;
using DatabasePerformanceTests.Utils.Generators.Models;
using DatabasePerformanceTests.Utils.Tests.Models;
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

            DataGeneratorConfig dataGeneratorConfig = new()
            {
                StudentsCount = 100_000,
                InstructorsCount = 500,
                CoursesCount = 1000,
                CourseInstancesPerCourse = 20,
                EnrollmentsPerStudent = 100
            };

            var generatedData = new DataGenerator(dataGeneratorConfig).Generate();
            
            var testsResults = new List<TestResult>();
            
            foreach (var config in databaseConfigs)
            {
                var dbContext = factory.CreateDbContext(config.System, config.ConnectionString);
                var databaseSeeder = new DatabaseSeeder(dbContext);
                // await databaseSeeder.PrepareDatabaseAsync();
                // await databaseSeeder.DropDatabaseAsync();
            }
        }
    }
}
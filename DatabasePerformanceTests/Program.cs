using DatabasePerformanceTests.Utils;
using DatabasePerformanceTests.Utils.Config;
using DatabasePerformanceTests.Utils.Database;
using DatabasePerformanceTests.Utils.Factories;
using DatabasePerformanceTests.Utils.Files;
using DatabasePerformanceTests.Utils.Generators;
using DatabasePerformanceTests.Utils.Generators.Models;
using DatabasePerformanceTests.Utils.Tests;
using DatabasePerformanceTests.Utils.Tests.Models;
using Microsoft.Extensions.Configuration;

namespace DatabasePerformanceTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Logger.Log("Starting tests...");

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var testsConfig = configuration.GetSection("Tests").Get<TestsConfig>()
                              ?? throw new InvalidDataException("appsettings.json configuration file requires Tests section");
            var databaseConfigs = configuration.GetSection("Connections").Get<DatabaseConfig[]>() 
                                  ?? throw new InvalidDataException("appsettings.json configuration requires Connections section");

            // DataGeneratorConfig dataGeneratorConfig = new()
            // {
            //     StudentsCount = 1_000_000,
            //     InstructorsCount = 500,
            //     CoursesCount = 10000,
            //     CourseInstancesPerCourse = 20,
            //     EnrollmentsPerStudent = 10
            // };
            
            DataGeneratorConfig dataGeneratorConfig = new()
            {
                StudentsCount = 1000,
                InstructorsCount = 5,
                CoursesCount = 100,
                CourseInstancesPerCourse = 2,
                EnrollmentsPerStudent = 10
            };

            var generatedData = new DataGenerator(dataGeneratorConfig).Generate();
            
            var testsResults = new List<TestResult>();
            
            var factory = new DbContextFactory();
            foreach (var config in databaseConfigs)
            {
                var dbContext = factory.CreateDbContext(config.System, config.ConnectionString);
                var databaseSeeder = new DatabaseSeeder(dbContext);
                var testsRunner = new TestsRunner(dbContext, testsConfig.Iterations);
                await databaseSeeder.PrepareDatabaseAsync(generatedData);
                var results = await testsRunner.RunTestsAsync();
                testsResults.AddRange(results);
                await databaseSeeder.DropDatabaseAsync();
            }
            TestResultsWriter.WriteResultsToFile(
                results:testsResults,
                fileName:$"results_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}",
                outputDirectory:testsConfig.OutputDirectory);
        }
    }
}

using DatabasePerformanceTests.Utils.Config;
using DatabasePerformanceTests.Utils.Factories;
using DatabasePerformanceTests.Utils.Files;
using DatabasePerformanceTests.Utils.Generators;
using DatabasePerformanceTests.Utils.Generators.Models;
using DatabasePerformanceTests.Utils.Tests;
using DatabasePerformanceTests.Utils.Tests.Models;

namespace DatabasePerformanceTests.Utils;

public static class MainMethods
{
    public static async Task CreateTables(string databaseName, DatabaseConfig[] databaseConfigs, DataGeneratorConfig dataGeneratorConfig)
    {
        Logger.Log("Generating data...");
        var generatedData = new DataGenerator(dataGeneratorConfig).Generate();
            
        var factory = new DbContextFactory();
        foreach (var config in databaseConfigs)
        {
            var dbContext = factory.CreateDbContext(config.System, config.ConnectionString, databaseName);
            await dbContext.CreateDatabaseAsync();
            await dbContext.CreateTablesAsync();
            await dbContext.PopulateDatabaseAsync(generatedData);
        }
    }

    public static async Task RunTestsAndCleanup(string databaseName, TestsConfig testsConfig, DatabaseConfig[] databaseConfigs)
    {
        var testsResults = new List<TestResult>();

        var factory = new DbContextFactory();
        foreach (var config in databaseConfigs)
        {
            var dbContext = factory.CreateDbContext(config.System, config.ConnectionString, databaseName);
            var testsRunner = new TestsRunner(dbContext, testsConfig.Iterations);
            var results = await testsRunner.RunTestsAsync();
            testsResults.AddRange(results);
            await dbContext.DropDatabaseAsync();
        }
        TestResultsWriter.WriteResultsToFile(
            results:testsResults,
            fileName:$"results_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}",
            outputDirectory:testsConfig.OutputDirectory);
    }
    
    public static async Task AnalyzeResults(TestsConfig testsConfig, DatabaseConfig[] databaseConfigs)
    {
        
    }
}
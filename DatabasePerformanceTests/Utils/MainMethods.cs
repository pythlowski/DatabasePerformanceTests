using DatabasePerformanceTests.Utils.Config;
using DatabasePerformanceTests.Utils.Factories;
using DatabasePerformanceTests.Utils.Files;
using DatabasePerformanceTests.Utils.Generators;
using DatabasePerformanceTests.Utils.Statistics;
using DatabasePerformanceTests.Utils.Tests;

namespace DatabasePerformanceTests.Utils;

public static class MainMethods
{
    public static async Task CreateDatabases(string databaseName, DatabaseConfig[] databaseConfigs, DataGeneratorConfig dataGeneratorConfig)
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
            await dbContext.CreateIndexesAsync();
        }
    }

    public static async Task RunTests(string databaseName, TestsConfig testsConfig, DatabaseConfig[] databaseConfigs, DataGeneratorConfig dataGeneratorConfig)
    {
        var factory = new DbContextFactory();
        var dbContexts = databaseConfigs.Select(config =>
            factory.CreateDbContext(config.System, config.ConnectionString, databaseName)).ToList();
        var testsRunner = new TestsRunner(dbContexts, dataGeneratorConfig, testsConfig.Iterations);
        var testsResults = await testsRunner.RunTestsAsync();
        
        TestResultsManager.WriteResultsToFile(
            results:testsResults,
            outputDirectory:testsConfig.OutputDirectory);
    }
    
    public static async Task DropDatabases(string databaseName, DatabaseConfig[] databaseConfigs)
    {
        var factory = new DbContextFactory();
        foreach (var config in databaseConfigs)
        {
            var dbContext = factory.CreateDbContext(config.System, config.ConnectionString, databaseName);
            await dbContext.DropDatabaseAsync();
        }
    }
    
    public static async Task AnalyzeResults(TestsConfig testsConfig)
    {
        var results = TestResultsManager.ReadResultsFromFile(testsConfig.OutputDirectory);

        var statisticsTablesGenerator = new StatisticsTablesGenerator();
        statisticsTablesGenerator.GenerateStatisticsTablesInLatex(results);
        var chartsGenerator = new ChartsGenerator();
        chartsGenerator.GenerateCharts(results, testsConfig.OutputDirectory);
    }
}
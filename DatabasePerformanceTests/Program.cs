using DatabasePerformanceTests.Utils;
using DatabasePerformanceTests.Utils.Config;
using Microsoft.Extensions.Configuration;

namespace DatabasePerformanceTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var testsConfig = configuration.GetSection("Tests").Get<TestsConfig>()
                              ?? throw new InvalidDataException("appsettings.json configuration file requires Tests section");
            
            var dataGeneratorConfig = configuration.GetSection("DataGenerator").Get<DataGeneratorConfig>()
                                      ?? throw new InvalidDataException("appsettings.json configuration file requires DataGenerator section");
            
            var databaseConfigs = configuration.GetSection("Connections").Get<DatabaseConfig[]>() 
                                  ?? throw new InvalidDataException("appsettings.json configuration requires Connections section");

            string databaseName = testsConfig.DatabaseName;
            
            string method = args.Length > 0 ? args[0] : "analyze";
            switch (method.ToLower())
            {
                case "create":
                    await MainMethods.CreateDatabases(databaseName, databaseConfigs, dataGeneratorConfig);
                    break;
                case "tests":
                    await MainMethods.RunTests(databaseName, testsConfig, databaseConfigs, dataGeneratorConfig);
                    break;
                case "drop":
                    await MainMethods.DropDatabases(databaseName, databaseConfigs);
                    break;
                case "analyze":
                    await MainMethods.AnalyzeResults(testsConfig);
                    break;
            }
        }
    }
}

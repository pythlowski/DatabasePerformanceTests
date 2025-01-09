using DatabasePerformanceTests.Utils;
using DatabasePerformanceTests.Utils.Config;
using DatabasePerformanceTests.Utils.Generators.Models;
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

            string databaseName = "testdb_latest";
            
            var method = args.Length > 0 ? args[0] : "analyze";
            switch (method.ToLower())
            {
                case "create":
                    await MainMethods.CreateTables(databaseName, databaseConfigs, dataGeneratorConfig);
                    break;
                case "run":
                    await MainMethods.RunTestsAndCleanup(databaseName, testsConfig, databaseConfigs);
                    break;
                case "analyze":
                    await MainMethods.AnalyzeResults(testsConfig, databaseConfigs);
                    break;
            }
        }
    }
}

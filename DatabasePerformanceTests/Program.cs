using DatabasePerformanceTests.Utils;
using DatabasePerformanceTests.Utils.Config;
using DatabasePerformanceTests.Utils.Config.Enums;
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

            // databaseConfigs = databaseConfigs.Where(c => c.System == DatabaseSystem.Postgres).ToArray();
            
            DataGeneratorConfig dataGeneratorConfig = new()
            {
                StudentsCount = 1_000_000,
                InstructorsCount = 500,
                CoursesCount = 1000,
                CourseInstancesPerCourse = 10,
                EnrollmentsPerStudent = 10
            };
            
            // DataGeneratorConfig dataGeneratorConfig = new()
            // {
            //     StudentsCount = 1000,
            //     InstructorsCount = 5,
            //     CoursesCount = 100,
            //     CourseInstancesPerCourse = 2,
            //     EnrollmentsPerStudent = 10
            // };

            string databaseName = "testdb_big";
            
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

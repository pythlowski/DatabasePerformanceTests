﻿using DatabasePerformanceTests.Utils;
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
            //     StudentsCount = 10_000_00,
            //     InstructorsCount = 500,
            //     CoursesCount = 1000,
            //     CourseInstancesPerCourse = 10,
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

            string databaseName = "testdb4";
            
            string method = args.Length > 0 ? args[0] : "tests";
            switch (method.ToLower())
            {
                case "create":
                    await MainMethods.CreateDatabases(databaseName, databaseConfigs, dataGeneratorConfig);
                    break;
                case "tests":
                    await MainMethods.RunTests(databaseName, testsConfig, databaseConfigs);
                    break;
                case "drop":
                    await MainMethods.DropDatabases(databaseName, databaseConfigs);
                    break;
                case "analyze":
                    await MainMethods.AnalyzeResults(testsConfig, databaseConfigs);
                    break;
            }
        }
    }
}

using DatabasePerformanceTests.Data.Contexts;
using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Data.Models.Mongo;
using DatabasePerformanceTests.Utils;
using DatabasePerformanceTests.Utils.Database;
using DatabasePerformanceTests.Utils.Database.Models;
using DatabasePerformanceTests.Utils.Database.Models.Enums;
using DatabasePerformanceTests.Utils.Factories;
using DatabasePerformanceTests.Utils.Generators;
using DatabasePerformanceTests.Utils.Generators.Models;
using DatabasePerformanceTests.Utils.Tests;
using DatabasePerformanceTests.Utils.Tests.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

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

            var databaseConfigs = configuration.GetSection("Connections").Get<DatabaseConfig[]>() 
                                  ?? throw new InvalidDataException("appsettings.json configuration file has invalid data");


            // DataGeneratorConfig dataGeneratorConfig = new()
            // {
            //     StudentsCount = 100_000,
            //     InstructorsCount = 500,
            //     CoursesCount = 10000,
            //     CourseInstancesPerCourse = 20,
            //     EnrollmentsPerStudent = 100
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
                var testsRunner = new TestsRunner(dbContext);
                await databaseSeeder.PrepareDatabaseAsync(generatedData);
                var results = await testsRunner.RunTestsAsync();
                testsResults.AddRange(results);
                await databaseSeeder.DropDatabaseAsync();

                // if (dbContext.DatabaseSystem == DatabaseSystem.Mongo)
                // {
                //     var ctx = (MongoDbContext)dbContext;
                //     var collection = ctx.GetCollection<MongoStudent>("students").WithWriteConcern(WriteConcern.WMajority);
                //     var cnt1 = await collection.CountDocumentsAsync(FilterDefinition<MongoStudent>.Empty);
                //     Logger.Log(cnt1.ToString());
                //     await ctx.StartTransactionAsync();
                //     await collection.DeleteOneAsync(ctx.GetSession(), s => s.Id == "student-1");
                //     await ctx.RollbackTransactionAsync();
                //     var cnt2 = await collection.CountDocumentsAsync(FilterDefinition<MongoStudent>.Empty);
                //     Logger.Log(cnt2.ToString());
                // }
                //
                // if (dbContext.DatabaseSystem == DatabaseSystem.Postgres)
                // {
                //     var ctx = (PsqlDbContext)dbContext;
                //     var cnt1 = await ctx.ExecuteScalarAsync("select count(*) from Enrollments");
                //     Logger.Log(cnt1.ToString());
                //     
                //     await ctx.StartTransactionAsync();
                //     await ctx.ExecuteNonQueryAsync("delete from Enrollments where EnrollmentId = 1", true);
                //     await ctx.RollbackTransactionAsync();
                //
                //     var cnt2 = await ctx.ExecuteScalarAsync("select count(*) from Enrollments");
                //     Logger.Log(cnt2.ToString());
                // }
                //
                // if (dbContext.DatabaseSystem == DatabaseSystem.MsSql)
                // {
                //     var ctx = (MssqlDbContext)dbContext;
                //     var cnt1 = await ctx.ExecuteScalarAsync("select count(*) from Enrollments");
                //     Logger.Log(cnt1.ToString());
                //     
                //     await ctx.StartTransactionAsync();
                //     await ctx.ExecuteNonQueryAsync("delete from Enrollments where EnrollmentId = 1", true);
                //     await ctx.RollbackTransactionAsync();
                //
                //     var cnt2 = await ctx.ExecuteScalarAsync("select count(*) from Enrollments");
                //     Logger.Log(cnt2.ToString());
                // }
            }
        }
    }
}
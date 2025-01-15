using System.Diagnostics;
using DatabasePerformanceTests.Data.Contexts;
using DatabasePerformanceTests.Data.Models;
using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Data.Models.Mongo;
using DatabasePerformanceTests.Data.Operations;
using DatabasePerformanceTests.Data.Operations.Interfaces;
using DatabasePerformanceTests.Utils.Config;
using DatabasePerformanceTests.Utils.Config.Enums;
using DatabasePerformanceTests.Utils.Factories;
using DatabasePerformanceTests.Utils.Generators;
using DatabasePerformanceTests.Utils.Tests.Models;

namespace DatabasePerformanceTests.Utils.Tests;

public class TestsRunner
{
    private readonly int TEST_ITERATIONS;
    
    List<AbstractDbContext> _contexts;
    private List<TestDefinition> _testDefinitions;
    private IDbOperations _operations;
    public TestsRunner(List<AbstractDbContext> contexts, DataGeneratorConfig generatorConfig, int iterations)
    {
        TEST_ITERATIONS = iterations;
        _contexts = contexts;
        var dummyEnrollments = DummyEnrollmentsGenerator.GenerateDomain(generatorConfig, 100_000);
        var dummyMongoEnrollments = DummyEnrollmentsGenerator.GenerateMongo(generatorConfig, 100_000);
        
        _testDefinitions = new()
        {
            new TestDefinition(
                OperationType.BulkInsertEnrollments,
                async (databaseSystem, dataSize, parameters) =>
                {
                    if (dataSize is null) throw new ArgumentNullException(nameof(dataSize));
            
                    List<IEnrollment> enrollments;
                    if (databaseSystem == DatabaseSystem.Mongo)
                    {
                        enrollments = ((List<MongoEnrollment>)parameters["MongoEnrollments"])
                            .Take((int)dataSize).Cast<IEnrollment>().ToList();
                    }
                    else
                    {
                        enrollments = ((List<Enrollment>)parameters["Enrollments"])
                            .Take((int)dataSize).Cast<IEnrollment>().ToList();
                    }
                    await _operations.BulkInsertAsync(enrollments);
                },
                parameters:new Dictionary<string, object>
                {
                    { "Enrollments", dummyEnrollments }, 
                    { "MongoEnrollments", dummyMongoEnrollments }
                },
                dataSizes:new List<int?>{ 100, 1000, 10_000, 100_000 }
            ),
            new TestDefinition(
                OperationType.SelectStudentById,
                async (databaseSystem, dataSize, parameters) =>
                {
                    await _operations.SelectStudentByIdAsync(generatorConfig.StudentsCount / 2);
                }
            ),
            new TestDefinition(
                OperationType.SelectStudentsOrderedById,
                async (databaseSystem, dataSize, parameters) =>
                {
                    if (dataSize is null) throw new ArgumentNullException(nameof(dataSize));
                    await _operations.SelectStudentsOrderedByIdAsync((int)dataSize);
                },
                dataSizes:new List<int?>{ 1000, 10_000, 100_000, 1_000_000 }
            ),
            new TestDefinition(
                    OperationType.SelectCourseInstancesByStudentId,
                    async (databaseSystem, dataSize, parameters) =>
                    {
                        await _operations.SelectCourseInstancesByStudentIdAsync(generatorConfig.StudentsCount / 2);
                    }
                ),
            new TestDefinition(
                OperationType.DeleteEnrollments,
                async (databaseSystem, dataSize, parameters) =>
                {
                    if (dataSize is null) throw new ArgumentNullException(nameof(dataSize));
                    await _operations.DeleteEnrollmentsAsync((int)dataSize);
                },
                dataSizes:new List<int?>{ 1000, 10_000, 100_000 }
            ),
            new TestDefinition(
                OperationType.UpdateEnrollments,
                async (databaseSystem, dataSize, parameters) =>
                {
                    if (databaseSystem == DatabaseSystem.Mongo && dataSize > 100_000) return; // won't work in a transaction

                    if (dataSize is null) throw new ArgumentNullException(nameof(dataSize));
                    await _operations.UpdateEnrollmentDatesAsync((int)dataSize);
                },
                dataSizes:new List<int?>{ 1000, 10_000, 100_000 }
            ),
            new TestDefinition(
                OperationType.SelectEnrollmentsOrderedById,
                async (databaseSystem, dataSize, parameters) =>
                {
                    if (dataSize is null) throw new ArgumentNullException(nameof(dataSize));
                    await _operations.SelectEnrollmentsOrderedByIdAsync((int)dataSize);
                },
                dataSizes:new List<int?>{ 1000, 10_000, 100_000, 1_000_000, 10_000_000 }
            ),
            new TestDefinition(
                OperationType.SelectEnrollmentsFilteredByIsActive,
                async (databaseSystem, dataSize, parameters) =>
                {
                    var isActive = (bool)parameters["IsActive"];
                    await _operations.SelectEnrollmentsFilteredByIsActiveAsync(isActive);
                },
                parameters:new Dictionary<string, object> { { "IsActive", true } }
            ),
            new TestDefinition(
                OperationType.SelectEnrollmentsFilteredByEnrollmentDate,
                async (databaseSystem, dataSize, parameters) =>
                {
                    var dateFrom = (DateTime)parameters["DateFrom"];
                    var dateTo = (DateTime)parameters["DateTo"];
                    await _operations.SelectEnrollmentsFilteredByEnrollmentDateAsync(dateFrom, dateTo);
                },
                parameters:new Dictionary<string, object>
                {
                    { "DateFrom", new DateTime(2020, 1, 1) }, 
                    { "DateTo", new DateTime(2021, 1, 1) }
                }
            ),
            new TestDefinition(
                OperationType.SelectEnrollmentsFilteredByBudget,
                async (databaseSystem, dataSize, parameters) =>
                {
                    var valueFrom = (int)parameters["ValueFrom"];
                    var valueTo = (int)parameters["ValueTo"];
                    await _operations.SelectEnrollmentsFilteredByBudgetAsync(valueFrom, valueTo);
                },
                parameters:new Dictionary<string, object>
                {
                    { "ValueFrom", 25_000_000 }, 
                    { "ValueTo", 75_000_000 }
                }
            ),
            new TestDefinition(
                OperationType.SelectEnrollmentsFilteredByStudentsLastName,
                async (databaseSystem, dataSize, parameters) =>
                {
                    var searchText = (string)parameters["SearchText"];
                    await _operations.SelectEnrollmentsFilteredByStudentsLastNameAsync(searchText);
                },
                parameters:new Dictionary<string, object> { { "SearchText", "a" } }
            ),
            new TestDefinition(
                OperationType.SelectEnrollmentsWithManyFilters,
                async (databaseSystem, dataSize, parameters) =>
                {
                    var isActive = (bool)parameters["IsActive"];
                    var dateFrom = (DateTime)parameters["DateFrom"];
                    var dateTo = (DateTime)parameters["DateTo"];
                    var valueFrom = (int)parameters["ValueFrom"];
                    var valueTo = (int)parameters["ValueTo"];
                    var searchText = (string)parameters["SearchText"];
                    await _operations.SelectEnrollmentsWithManyFiltersAsync(isActive, dateFrom, dateTo, valueFrom, valueTo, searchText);
                },
                parameters:new Dictionary<string, object>
                {
                    { "IsActive", true },
                    { "DateFrom", new DateTime(2020, 1, 1) }, 
                    { "DateTo", new DateTime(2021, 1, 1) },
                    { "ValueFrom", 25_000_000 }, 
                    { "ValueTo", 75_000_000 },
                    { "SearchText", "a" }
                }
            ),
            new TestDefinition(
                OperationType.SelectEnrollmentsWithPagination,
                async (databaseSystem, pageNumber, parameters) =>
                {
                    if (pageNumber is null) throw new ArgumentNullException(nameof(pageNumber));
                    await _operations.SelectEnrollmentsWithPaginationAsync(100, (int)pageNumber);
                },
                dataSizes:new List<int?>{ 1, 5, 10, 20, 50, 100, 250, 500, 1000, 10_000, 100_000 }
            ),
            new TestDefinition(
                OperationType.SelectEnrollmentsWithManySortParameters,
                async (databaseSystem, dataSize, parameters) =>
                {
                    if (dataSize is null) throw new ArgumentNullException(nameof(dataSize));
                    await _operations.SelectEnrollmentsWithManySortParametersAsync((int)dataSize);
                },
                dataSizes:new List<int?>{ 1000, 10_000, 100_000, 1_000_000, 10_000_000 }
            ),
        };
    }
    
    public async Task<List<OperationResults>> RunTestsAsync()
    {
        List<OperationResults> results = new();

        foreach (var (test, defIndex) in _testDefinitions.Select((td, i) => (td, i)))
        {
            foreach (var dataSize in test.DataSizes ?? new List<int?> { null })
            {
                OperationResults operationResults = new(test.OperationType, dataSize);
                
                foreach (var context in _contexts)
                {
                    DatabaseStatistics databaseStatistics = new();
                    _operations = OperationsFactory.CreateOperations(context);
                    
                    int iterationErrors = 0;
                    foreach (var iteration in Enumerable.Range(0, TEST_ITERATIONS))
                    {
                        if (iterationErrors >= 3)
                            break;
                        
                        Logger.Log(
                            $"[{defIndex+1}/{_testDefinitions.Count}] [{iteration + 1}/{TEST_ITERATIONS}] Running test: {test.OperationType} with data size {dataSize} for {context.DatabaseSystem}...");
                        
                        try
                        {
                            await context.ClearCacheAsync();
                            await context.StartTransactionAsync();

                            if (test.PrepareFunction != null) await test.PrepareFunction(); 
                            var stopwatch = Stopwatch.StartNew();
                            await test.TestFunction(context.DatabaseSystem, dataSize, test.Parameters);
                            stopwatch.Stop();

                            var testTime = (int)stopwatch.ElapsedMilliseconds;
                            Logger.Log($"Took {testTime} ms.");
                            databaseStatistics.RegisterTime(testTime);
                        }
                        catch (Exception ex)
                        {
                            iterationErrors++;
                            Console.WriteLine($"Test '{test.OperationType}' failed: {ex.Message}");
                        }
                        finally
                        {
                            await context.RollbackTransactionAsync();
                        }
                    }

                    databaseStatistics.CalculateResultParameters();
                    operationResults.RegisterSystemResults(context.DatabaseSystem, databaseStatistics);
                }
                results.Add(operationResults);
            }
        }

        return results;
    }
}
using System.Diagnostics;
using DatabasePerformanceTests.Data.Contexts;
using DatabasePerformanceTests.Data.Operations;
using DatabasePerformanceTests.Utils.Factories;
using DatabasePerformanceTests.Utils.Tests.Models;

namespace DatabasePerformanceTests.Utils.Tests;

public class TestsRunner
{
    private readonly int TEST_ITERATIONS;
    
    List<AbstractDbContext> _contexts;
    private List<TestDefinition> _testDefinitions;
    private IDbOperations _operations;
    public TestsRunner(List<AbstractDbContext> contexts, int iterations)
    {
        TEST_ITERATIONS = iterations;
        _contexts = contexts;
        
        _testDefinitions = new()
        {
            new TestDefinition(
                OperationType.SelectStudentsOrderedById,
                async (dataSize, parameters) =>
                {
                    if (dataSize is null) throw new ArgumentNullException(nameof(dataSize));
                    await _operations.SelectStudentsOrderedByIdAsync((int)dataSize);
                },
                dataSizes:new List<int?>{ 1000, 10_000, 100_000, 1_000_000 }
            ),
            new TestDefinition(
                OperationType.SelectEnrollmentsOrderedById,
                async (dataSize, parameters) =>
                {
                    if (dataSize is null) throw new ArgumentNullException(nameof(dataSize));
                    await _operations.SelectEnrollmentsOrderedByIdAsync((int)dataSize);
                },
                dataSizes:new List<int?>{ 1000, 10_000, 100_000, 1_000_000, 10_000_000 }
            ),
            new TestDefinition(
                OperationType.SelectEnrollmentsFilteredByIsActive,
                async (dataSize, parameters) =>
                {
                    var isActive = (bool)parameters["IsActive"];
                    await _operations.SelectEnrollmentsFilteredByIsActiveAsync(isActive);
                },
                parameters:new Dictionary<string, object> { { "IsActive", true } }
            ),
            new TestDefinition(
                OperationType.SelectEnrollmentsFilteredByEnrollmentDate,
                async (dataSize, parameters) =>
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
                async (dataSize, parameters) =>
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
                async (dataSize, parameters) =>
                {
                    var searchText = (string)parameters["SearchText"];
                    await _operations.SelectEnrollmentsFilteredByStudentsLastNameAsync(searchText);
                },
                parameters:new Dictionary<string, object> { { "SearchText", "a" } }
            ),
        };
    }
    
    public async Task<List<OperationResults>> RunTestsAsync()
    {
        List<OperationResults> results = new();

        foreach (var test in _testDefinitions)
        {
            foreach (var dataSize in test.DataSizes ?? new List<int?> { null })
            {
                OperationResults operationResults = new(test.OperationType, dataSize);
                
                foreach (var context in _contexts)
                {
                    SystemResults systemResults = new();
                    _operations = OperationsFactory.CreateOperations(context);
                    
                    foreach (var iteration in Enumerable.Range(0, TEST_ITERATIONS))
                    {
                        Logger.Log(
                            $"[{iteration + 1}] Running test: {test.OperationType} with data size {dataSize} for {context.DatabaseSystem}...");
                        
                        try
                        {
                            await context.ClearCacheAsync();
                            await context.StartTransactionAsync();

                            var stopwatch = Stopwatch.StartNew();
                            await test.TestFunction(dataSize, test.Parameters);
                            stopwatch.Stop();

                            systemResults.RegisterTime((int)stopwatch.ElapsedMilliseconds);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Test '{test.OperationType}' failed: {ex.Message}");
                        }
                        finally
                        {
                            await context.RollbackTransactionAsync();
                        }
                    }

                    systemResults.CalculateResultParameters();
                    operationResults.RegisterSystemResults(context.DatabaseSystem, systemResults);
                }
                results.Add(operationResults);
            }
        }

        return results;
    }
}
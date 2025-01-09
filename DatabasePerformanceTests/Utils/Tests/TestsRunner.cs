using System.Diagnostics;
using DatabasePerformanceTests.Data.Contexts;
using DatabasePerformanceTests.Data.Operations;
using DatabasePerformanceTests.Utils.Factories;
using DatabasePerformanceTests.Utils.Tests.Models;

namespace DatabasePerformanceTests.Utils.Tests;

public class TestsRunner
{
    private readonly int TEST_ITERATIONS;
    
    AbstractDbContext _context;
    private List<TestDefinition> _testDefinitions;
    public TestsRunner(AbstractDbContext context, int iterations)
    {
        TEST_ITERATIONS = iterations;
        _context = context;
        var operations = new OperationsFactory().CreateOperationsFactory(context);
        
        _testDefinitions = new()
        {
            new TestDefinition(
                OperationType.SelectStudentsOrderedById,
                async parameters =>
                {
                    int limit = (int)parameters["Limit"];
                    await operations.SelectStudentsOrderedByIdAsync(limit);
                },
                new Dictionary<string, object> { { "Limit", 10 } },
                10
            ),
            new TestDefinition(
                OperationType.SelectStudentsOrderedById,
                async parameters =>
                {
                    int limit = (int)parameters["Limit"];
                    await operations.SelectStudentsOrderedByIdAsync(limit);
                },
                new Dictionary<string, object> { { "Limit", 100 } },
                100
            ),
            // new TestDefinition(
            //     OperationType.SelectEnrollmentsOrderedById,
            //     async parameters =>
            //     {
            //         int limit = (int)parameters["Limit"];
            //         await operations.SelectEnrollmentsOrderedByIdAsync(limit);
            //     },
            //     new Dictionary<string, object> { { "Limit", 100_000 } },
            //     100_000
            // ),
            // new TestDefinition(
            //     OperationType.SelectEnrollmentsOrderedById,
            //     async parameters =>
            //     {
            //         int limit = (int)parameters["Limit"];
            //         await operations.SelectEnrollmentsOrderedByIdAsync(limit);
            //     },
            //     new Dictionary<string, object> { { "Limit", 1000 } },
            //     100
            // ),
        };
    }
    
    public async Task<List<TestResult>> RunTestsAsync()
    {
        List<TestResult> results = new();
        
        foreach (var test in _testDefinitions)
        {
            TestResult result = new(test.OperationType, _context.DatabaseSystem, test.DataSize);
            
            foreach (var iteration in Enumerable.Range(0, TEST_ITERATIONS))
            {
                Logger.Log($"[{iteration+1}] Running test: {test.OperationType} with data size {test.DataSize} for {_context.DatabaseSystem}...");

                IterationResult iterationResult = new(iteration + 1);
                
                try
                {
                    await _context.ClearCacheAsync();
                    await _context.StartTransactionAsync();

                    var stopwatch = Stopwatch.StartNew();
                    await test.TestFunction(test.Parameters);
                    stopwatch.Stop();

                    iterationResult.ExecutionTimeMilliseconds = (int)stopwatch.ElapsedMilliseconds;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Test '{test.OperationType}' failed: {ex.Message}");
                    iterationResult.ErrorMessage = ex.Message;
                }
                finally
                {
                    await _context.RollbackTransactionAsync();
                }
                result.AddIterationResult(iterationResult);
            }

            result.CalculateResultParameters();
            results.Add(result);
        }
        
        return results;
    }
}
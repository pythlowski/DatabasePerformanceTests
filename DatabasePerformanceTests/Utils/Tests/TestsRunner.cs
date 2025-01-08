using System.Diagnostics;
using DatabasePerformanceTests.Data.Contexts;
using DatabasePerformanceTests.Data.Operations;
using DatabasePerformanceTests.Utils.Factories;
using DatabasePerformanceTests.Utils.Tests.Models;

namespace DatabasePerformanceTests.Utils.Tests;

public class TestsRunner
{
    private readonly int TEST_REPETITIONS = 3;
    
    AbstractDbContext _context;
    private List<TestDefinition> _testDefinitions;
    public TestsRunner(AbstractDbContext context)
    {
        _context = context;
        var operations = new OperationsFactory().CreateOperationsFactory(context);
        
        _testDefinitions = new()
        {
            new TestDefinition(
                "select 10 students",
                async parameters =>
                {
                    int limit = (int)parameters["Limit"];
                    await operations.SelectStudentsOrderedByIdAsync(limit);
                },
                new Dictionary<string, object> { { "Limit", 10 } },
                10
            ),
            new TestDefinition(
                "select 100 students",
                async parameters =>
                {
                    int limit = (int)parameters["Limit"];
                    await operations.SelectStudentsOrderedByIdAsync(limit);
                },
                new Dictionary<string, object> { { "Limit", 100 } },
                100
            ),
            new TestDefinition(
                "select 100 enrollments",
                async parameters =>
                {
                    int limit = (int)parameters["Limit"];
                    await operations.SelectEnrollmentsOrderedByIdAsync(limit);
                },
                new Dictionary<string, object> { { "Limit", 100 } },
                100
            ),
            new TestDefinition(
                "select 1000 enrollments",
                async parameters =>
                {
                    int limit = (int)parameters["Limit"];
                    await operations.SelectEnrollmentsOrderedByIdAsync(limit);
                },
                new Dictionary<string, object> { { "Limit", 1000 } },
                100
            ),
        };
    }
    
     
    
    public async Task<List<TestResult>> RunTestsAsync()
    {
        List<TestResult> results = new();
        
        foreach (var test in _testDefinitions)
        {
            Console.WriteLine($"Running test: {test.Name} for {_context.DatabaseSystem}...");

            TestResult result = new(test.Name, _context.DatabaseSystem, test.DataSize);
            
            foreach (var iteration in Enumerable.Range(0, TEST_REPETITIONS))
            {
                TestIterationResult iterationResult = new(iteration + 1);
                
                try
                {
                    await _context.StartTransactionAsync();

                    var stopwatch = Stopwatch.StartNew();
                    await test.TestFunction(test.Parameters);
                    stopwatch.Stop();

                    iterationResult.IsSuccess = true;
                    iterationResult.ExecutionTimeMilliseconds = (int)stopwatch.ElapsedMilliseconds;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Test '{test.Name}' failed: {ex.Message}");
                    iterationResult.IsSuccess = false;
                    iterationResult.ErrorMessage = ex.Message;
                }
                finally
                {
                    await _context.RollbackTransactionAsync();
                }
                result.AddIterationResult(iterationResult);
            }
            results.Add(result);
        }
        
        return results;
    }
}
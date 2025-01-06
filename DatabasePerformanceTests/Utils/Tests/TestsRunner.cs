using System.Diagnostics;
using DatabasePerformanceTests.Data.Contexts;
using DatabasePerformanceTests.Data.Operations;
using DatabasePerformanceTests.Utils.Factories;
using DatabasePerformanceTests.Utils.Tests.Models;

namespace DatabasePerformanceTests.Utils.Tests;

public class TestsRunner
{
    AbstractDbContext _context;
    private List<TestDefinition> _testDefinitions;
    public TestsRunner(AbstractDbContext context)
    {
        _context = context;
        var operations = new OperationsFactory().CreateOperationsFactory(context);
        
        _testDefinitions = new()
        {
            new TestDefinition(
                "Insert 100 Records",
                async (parameters) =>
                {
                    int recordCount = (int)parameters["RecordCount"];
                    await operations.DeleteEnrollmentsAsync(recordCount);
                },
                new Dictionary<string, object> { { "RecordCount", 1 } },
                true
            ),
        };
    }
    
     
    
    public async Task<List<TestResult>> RunTestsAsync()
    {
        var results = new List<TestResult>();
        foreach (var test in _testDefinitions)
        {
            Console.WriteLine($"Running test: {test.Name} for {_context.DatabaseSystem}...");

            try
            {
                await _context.StartTransactionAsync();
                
                var stopwatch = Stopwatch.StartNew();
                await test.TestFunction(test.Parameters);
                stopwatch.Stop();
                
                results.Add(new TestResult
                {
                    TestName = test.Name,
                    System = _context.DatabaseSystem,
                    DataSize = 1, //testDefinition.DataSize,
                    ExecutionTimeMilliseconds = (int)stopwatch.ElapsedMilliseconds
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test '{test.Name}' failed: {ex.Message}");
            }
            finally
            {
                await _context.RollbackTransactionAsync();
            }
        }
        
        return results;
    }
}
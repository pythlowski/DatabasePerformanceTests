using DatabasePerformanceTests.Data.Operations;
using DatabasePerformanceTests.Utils.Config.Enums;

namespace DatabasePerformanceTests.Utils.Tests.Models;

public class TestResult
{
    public TestResult(OperationType operationType, DatabaseSystem system, long dataSize)
    {
        OperationType = operationType;
        System = system;
        DataSize = dataSize;
        IterationResults = new();
    }

    public OperationType OperationType { get; set; }
    public DatabaseSystem System { get; set; }
    public long DataSize { get; set; }
    public List<TestIterationResult> IterationResults { get; set; }
    
    public void AddIterationResult(TestIterationResult result)
    {
        IterationResults.Add(result);
    }
    
    public override string ToString() => $"{OperationType} ({System})";
}

public class TestIterationResult
{
    public TestIterationResult(int iteration)
    {
        Iteration = iteration;
    }

    public int Iteration { get; set; }
    public int ExecutionTimeMilliseconds { get; set; }
    public string ErrorMessage { get; set; }
}
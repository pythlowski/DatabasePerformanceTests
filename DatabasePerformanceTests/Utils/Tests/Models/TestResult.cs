using DatabasePerformanceTests.Utils.Database.Models.Enums;

namespace DatabasePerformanceTests.Utils.Tests.Models;

public class TestResult
{
    public TestResult(string testName, DatabaseSystem system, long dataSize)
    {
        TestName = testName;
        System = system;
        DataSize = dataSize;
        IterationResults = new();
    }

    public string TestName { get; set; }
    public DatabaseSystem System { get; set; }
    public long DataSize { get; set; }
    public List<TestIterationResult> IterationResults { get; set; }
    
    public void AddIterationResult(TestIterationResult result)
    {
        IterationResults.Add(result);
    }
    
    public override string ToString() => $"{TestName} ({System})";
}

public class TestIterationResult
{
    public TestIterationResult(int iteration)
    {
        Iteration = iteration;
    }

    public int Iteration { get; set; }
    public int ExecutionTimeMilliseconds { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
    public object FirstRow { get; set; }
}
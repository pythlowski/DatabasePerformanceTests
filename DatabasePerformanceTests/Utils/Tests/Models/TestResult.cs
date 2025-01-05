using DatabasePerformanceTests.Utils.Database.Models.Enums;

namespace DatabasePerformanceTests.Utils.Tests.Models;

public class TestResult
{
    public string TestName { get; set; }
    public DatabaseSystem System { get; set; }
    public long DataSize { get; set; }
    public int ExecutionTimeMilliseconds { get; set; }
}
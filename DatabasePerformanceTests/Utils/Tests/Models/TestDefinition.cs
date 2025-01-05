namespace DatabasePerformanceTests.Utils.Tests.Models;

public class TestDefinition
{
    public string Name { get; set; }
    public Func<Task> TestFunction { get; set; }
    public bool RequiresDatabaseRestore { get; set; } 
}
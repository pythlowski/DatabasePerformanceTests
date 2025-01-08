namespace DatabasePerformanceTests.Utils.Tests.Models;

public class TestDefinition
{
    public string Name { get; }
    public Func<Dictionary<string, object>, Task> TestFunction { get; }
    public Dictionary<string, object> Parameters { get; }
    public int DataSize { get; }

    public TestDefinition(string name, Func<Dictionary<string, object>, Task> testFunction, 
        Dictionary<string, object> parameters, int dataSize)
    {
        Name = name;
        TestFunction = testFunction;
        Parameters = parameters;
        DataSize = dataSize;
    }

    public override string ToString() => Name;
}
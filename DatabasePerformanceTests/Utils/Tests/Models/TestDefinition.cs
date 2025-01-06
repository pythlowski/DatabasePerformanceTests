namespace DatabasePerformanceTests.Utils.Tests.Models;

public class TestDefinition
{
    public string Name { get; }
    public Func<Dictionary<string, object>, Task> TestFunction { get; }
    public Dictionary<string, object> Parameters { get; }
    public bool RequiresTransaction { get; }

    public TestDefinition(string name, Func<Dictionary<string, object>, Task> testFunction, 
        Dictionary<string, object> parameters, bool requiresTransaction  = false)
    {
        Name = name;
        TestFunction = testFunction;
        Parameters = parameters;
        RequiresTransaction = requiresTransaction;
    }

    public override string ToString() => Name;
}
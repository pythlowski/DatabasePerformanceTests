using Bogus.DataSets;
using DatabasePerformanceTests.Data.Operations;

namespace DatabasePerformanceTests.Utils.Tests.Models;

public class TestDefinition
{
    public TestDefinition(OperationType operationType, Func<int?, Dictionary<string, object>, Task> testFunction, int? dataSize)
    {
        OperationType = operationType;
        TestFunction = testFunction;
        DataSize = dataSize;
    }

    public OperationType OperationType { get; }
    public Func<int?, Dictionary<string, object>, Task> TestFunction { get; }
    public Dictionary<string, object> Parameters { get; }
    public int? DataSize { get; }

    public TestDefinition(OperationType operationType, Func<int?, Dictionary<string, object>, Task> testFunction, 
        Dictionary<string, object> parameters, int? dataSize)
    {
        OperationType = operationType;
        TestFunction = testFunction;
        Parameters = parameters;
        DataSize = dataSize;
    }

    public override string ToString() => $"{OperationType} with {DataSize} data size";
}
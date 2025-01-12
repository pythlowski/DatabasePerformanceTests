using Bogus.DataSets;
using DatabasePerformanceTests.Data.Operations;
using DatabasePerformanceTests.Utils.Config.Enums;

namespace DatabasePerformanceTests.Utils.Tests.Models;

public class TestDefinition
{
    public TestDefinition(OperationType operationType, Func<DatabaseSystem, int?, Dictionary<string, object>, Task> testFunction, 
        Dictionary<string, object> parameters, List<int?> dataSizes)
    {
        OperationType = operationType;
        TestFunction = testFunction;
        Parameters = parameters;
        DataSizes = dataSizes;
    }
    
    public TestDefinition(OperationType operationType, Func<Task> prepareFunction, 
        Func<DatabaseSystem, int?, Dictionary<string, object>, Task> testFunction, 
        Dictionary<string, object> parameters, List<int?> dataSizes)
    {
        OperationType = operationType;
        PrepareFunction = prepareFunction;
        TestFunction = testFunction;
        Parameters = parameters;
        DataSizes = dataSizes;
    }
    
    public TestDefinition(OperationType operationType, Func<DatabaseSystem, int?, Dictionary<string, object>, Task> testFunction, List<int?> dataSizes)
    {
        OperationType = operationType;
        TestFunction = testFunction;
        DataSizes = dataSizes;
    }
    
    public TestDefinition(OperationType operationType, Func<DatabaseSystem, int?, Dictionary<string, object>, Task> testFunction)
    {
        OperationType = operationType;
        TestFunction = testFunction;
        DataSizes = null;
    }
    
    public TestDefinition(OperationType operationType, Func<DatabaseSystem, int?, Dictionary<string, object>, Task> testFunction, Dictionary<string, object> parameters)
    {
        OperationType = operationType;
        TestFunction = testFunction;
        Parameters = parameters;
        DataSizes = null;
    }

    public OperationType OperationType { get; }
    public Func<Task> PrepareFunction { get; }
    public Func<DatabaseSystem, int?, Dictionary<string, object>, Task> TestFunction { get; }
    public Dictionary<string, object> Parameters { get; }
    public List<int?>? DataSizes { get; }

    public override string ToString() => OperationType.ToString();
}
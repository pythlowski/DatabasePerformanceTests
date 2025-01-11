using DatabasePerformanceTests.Data.Operations;
using DatabasePerformanceTests.Utils.Config.Enums;

namespace DatabasePerformanceTests.Utils.Tests.Models;

public class OperationResults
{
    public OperationResults(OperationType operationType, int? dataSize)
    {
        OperationType = operationType;
        DataSize = dataSize;
        Results = new();
    }

    public OperationType OperationType { get; set; }
    public int? DataSize { get; set; }
    public Dictionary<DatabaseSystem, SystemResults> Results { get; set; }
    public override string ToString() => $"{OperationType} for {DataSize ?? -1} data size";
    
    public void RegisterSystemResults(DatabaseSystem system, SystemResults systemResults)
    {
        Results.Add(system, systemResults);
    }
}

public class SystemResults
{
    public SystemResults()
    {
        ExecutionTimesMilliseconds = new();
    }

    public List<int> ExecutionTimesMilliseconds { get; set; }
    public double Average { get; set; }
    public double Median { get; set; }
    public double Minimum { get; set; }
    public double Maximum { get; set; }

    public void RegisterTime(int timeMilliseconds)
    {
        ExecutionTimesMilliseconds.Add(timeMilliseconds);
    }
    public void CalculateResultParameters()
    {
        if (!ExecutionTimesMilliseconds.Any())
            return;
        
        Average = ExecutionTimesMilliseconds.Average();
        Median = GetMedian(ExecutionTimesMilliseconds);
        Minimum = ExecutionTimesMilliseconds.Min();
        Maximum = ExecutionTimesMilliseconds.Max();
    }
    
    private static double GetMedian(List<int> values)
    {
        return values.OrderBy(item => item)     // from sorted sequence
            .Skip((values.Count - 1) / 2)  // we skip leading half items
            .Take(2 - values.Count % 2)    // take one or two middle items 
            .Average();
    }
}
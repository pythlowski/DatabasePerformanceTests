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
    public List<IterationResult> IterationResults { get; set; }
    public ResultParameters ResultParameters { get; set; }

    public void CalculateResultParameters()
    {
        ResultParameters = ResultParameters.CalculateResultParameters(IterationResults);
    }
    
    public void AddIterationResult(IterationResult result)
    {
        IterationResults.Add(result);
    }
    
    public override string ToString() => $"{OperationType} ({System}) - avg: {ResultParameters.Average} ms";
}

public class IterationResult
{
    public IterationResult(int iteration)
    {
        Iteration = iteration;
    }

    public int Iteration { get; set; }
    public int ExecutionTimeMilliseconds { get; set; }
    public string ErrorMessage { get; set; }
}

public class ResultParameters
{
    public double Average { get; set; }
    public double Median { get; set; }
    public double Minimum { get; set; }
    public double Maximum { get; set; }
    public double Percentile90 { get; set; }
    public double Percentile95 { get; set; }
    public static ResultParameters CalculateResultParameters(List<IterationResult> iterationResults)
    {
        var times = iterationResults
            .Where(r => string.IsNullOrEmpty(r.ErrorMessage))
            .Select(r => r.ExecutionTimeMilliseconds)
            .ToList();
        
        return new ResultParameters
        {
            Average = times.Average(),
            Median = times.Count % 2 == 0
                ? (times[times.Count / 2 - 1] + times[times.Count / 2]) / 2
                : times[times.Count / 2],
            Minimum = times.Min(),
            Maximum = times.Max(),
            Percentile90 = times[(int)(times.Count * 0.90) - 1],
            Percentile95 = times[(int)(times.Count * 0.95) - 1],
        };
    }
}
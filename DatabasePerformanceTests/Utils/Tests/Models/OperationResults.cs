using System.Text;
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
    public Dictionary<DatabaseSystem, DatabaseStatistics> Results { get; set; }
    public override string ToString() => $"{OperationType} for {DataSize ?? -1} data size";
    
    public void RegisterSystemResults(DatabaseSystem system, DatabaseStatistics databaseStatistics)
    {
        Results.Add(system, databaseStatistics);
    }
}

public class DatabaseStatistics
{
    public DatabaseStatistics()
    {
        ExecutionTimesMilliseconds = new();
    }
    public DatabaseStatistics(List<int> executionTimesMilliseconds)
    {
        ExecutionTimesMilliseconds = executionTimesMilliseconds;
        CalculateResultParameters();
    }

    public List<int> ExecutionTimesMilliseconds { get; set; }
    public double Average { get; set; }
    public double Median { get; set; }
    public double Minimum { get; set; }
    public double Maximum { get; set; }
    public double StandardDeviation { get; set; }
    public double Q1 { get; set; }
    public double Q3 { get; set; }

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
        StandardDeviation = GetStandardDeviation(ExecutionTimesMilliseconds);
        Minimum = ExecutionTimesMilliseconds.Min();
        Maximum = ExecutionTimesMilliseconds.Max();
        Q1 = GetPercentile(ExecutionTimesMilliseconds, 25);
        Q3 = GetPercentile(ExecutionTimesMilliseconds, 75);
    }
    
    private static double GetMedian(List<int> values)
    {
        return values.OrderBy(item => item)     // from sorted sequence
            .Skip((values.Count - 1) / 2)  // we skip leading half items
            .Take(2 - values.Count % 2)    // take one or two middle items 
            .Average();
    }
    
    private static double GetStandardDeviation(List<int> values)
    {
        double average = values.Average();
        double sumOfSquares = values.Sum(d => Math.Pow(d - average, 2));
        return Math.Sqrt(sumOfSquares / values.Count);
    }
    
    private static double GetPercentile(List<int> values, double percentile)
    {
        var sortedData = values.OrderBy(v => v).ToList();
        
        if (percentile < 0 || percentile > 100)
            throw new ArgumentOutOfRangeException(nameof(percentile), "Percentile must be between 0 and 100.");

        double position = (sortedData.Count - 1) * (percentile / 100.0);
        int lowerIndex = (int)Math.Floor(position);
        int upperIndex = (int)Math.Ceiling(position);

        if (lowerIndex == upperIndex)
            return sortedData[lowerIndex];

        double lowerValue = sortedData[lowerIndex];
        double upperValue = sortedData[upperIndex];
        return lowerValue + (upperValue - lowerValue) * (position - lowerIndex);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"Average - {Average:F2} ms\n");
        sb.Append($"Median - {Median:F2} ms\n");
        sb.Append($"Minimum - {Minimum} ms\n");
        sb.Append($"Maximum - {Maximum} ms\n");
        sb.Append($"Standard deviation - {StandardDeviation:F2} ms\n");
        sb.Append($"Q1 - {Q1:F2} ms\n");
        sb.Append($"Q3 - {Q3:F2} ms");
        return sb.ToString();
    }
}
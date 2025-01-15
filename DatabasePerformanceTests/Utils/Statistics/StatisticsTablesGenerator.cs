using System.Text;
using DatabasePerformanceTests.Utils.Config.Enums;
using DatabasePerformanceTests.Utils.Tests.Models;

namespace DatabasePerformanceTests.Utils.Statistics;

public class StatisticsTablesGenerator
{
    public void PrintStatisticsTables(List<OperationResults> results)
    {
        foreach (var result in results)
        {
            Console.WriteLine($"{result.OperationType} for data size {result.DataSize}:");
            foreach (var (system, statistics) in result.Results)
            {
                Console.WriteLine(system);
                statistics.CalculateResultParameters();
                Console.WriteLine(statistics);
            }
        }
    }
    
    public void GenerateStatisticsTablesInLatex(List<OperationResults> results)
    {
        foreach (var result in results)
        {
            Console.WriteLine($"{result.OperationType} for data size {result.DataSize}:");
            Console.WriteLine(GenerateStatisticsTableInLatex(result));
            Console.WriteLine("\n\n--------------------------------------\n\n");
        }
    }

    private string GenerateStatisticsTableInLatex(OperationResults operationResults)
    {
        var psqlStats = operationResults.Results[DatabaseSystem.Postgres];
        psqlStats.CalculateResultParameters();
        
        var mssqlStats = operationResults.Results[DatabaseSystem.MsSql];
        mssqlStats.CalculateResultParameters();
        
        var mongoStats = operationResults.Results[DatabaseSystem.Mongo];
        mongoStats.CalculateResultParameters();


        List<LatexTableRow> latexRows = new();
        latexRows.Add(new("Średnia", psqlStats.Average, mssqlStats.Average, mongoStats.Average, false));
        latexRows.Add(new("Mediana", psqlStats.Median, mssqlStats.Median, mongoStats.Median, true));
        latexRows.Add(new("Minimum", psqlStats.Minimum, mssqlStats.Minimum, mongoStats.Minimum, true));
        latexRows.Add(new("Maksimum", psqlStats.Maximum, mssqlStats.Maximum, mongoStats.Maximum, true));
        latexRows.Add(new("Odch. std.", psqlStats.StandardDeviation, mssqlStats.StandardDeviation, mongoStats.StandardDeviation, true));
        latexRows.Add(new("1. kwartyl", psqlStats.Q1, mssqlStats.Q1, mongoStats.Q1, true));
        latexRows.Add(new("3. kwartyl", psqlStats.Q3, mssqlStats.Q3, mongoStats.Q3, true));
        
        var sb = new StringBuilder();
        sb.Append("\\begin{table}[H]\n");
        sb.Append("\\centering\n");
        sb.Append("\\begin{tabular}{|>{\\columncolor[gray]{0.9}\\centering\\arraybackslash}p{3cm}|>{\\centering\\arraybackslash}p{2.5cm}|>{\\centering\\arraybackslash}p{2.5cm}|>{\\centering\\arraybackslash}p{2.5cm}|}\n");
        sb.Append("\\rowcolor{gray!40}\n");
        sb.Append("\\hline\n");
        sb.Append("Metryka & PostgreSQL & MSSQL & MongoDB \\\\ \\hline\n");

        foreach (var row in latexRows)
        {
            sb.Append(string.Join(" & ", new[]
            {
                row.Metric,
                row.PostgresRowValue.GetFullLatexCode(),
                row.MsSqlRowValue.GetFullLatexCode(),
                row.MongoRowValue.GetFullLatexCode(),
            }));
            sb.Append(" \\\\ \\hline\n");
        }
        // sb.Append($"Średnia & {psqlStats.Average:F2} ms & {mssqlStats.Average:F2} ms & {mongoStats.Average:F2} ms \\\\ \\hline\n");
        // sb.Append($"Mediana & {psqlStats.Median:F2} ms & {mssqlStats.Median:F2} ms & {mongoStats.Median:F2} ms \\\\ \\hline\n");
        // sb.Append($"Minimum & {psqlStats.Minimum:F2} ms & {mssqlStats.Minimum:F2} ms & {mongoStats.Minimum:F2} ms \\\\ \\hline\n");
        // sb.Append($"Maksimum & {psqlStats.Maximum:F2} ms & {mssqlStats.Maximum:F2} ms & {mongoStats.Maximum:F2} ms \\\\ \\hline\n");
        // sb.Append($"Odch. std. & {psqlStats.StandardDeviation:F2} ms & {mssqlStats.StandardDeviation:F2} ms & {mongoStats.StandardDeviation:F2} ms \\\\ \\hline\n");
        // sb.Append($"1. kwartyl & {psqlStats.Q1:F2} ms & {mssqlStats.Q1:F2} ms & {mongoStats.Q1:F2} ms \\\\ \\hline\n");
        // sb.Append($"3. kwartyl & {psqlStats.Q3:F2} ms & {mssqlStats.Q3:F2} ms & {mongoStats.Q3:F2} ms \\\\ \\hline\n");

        sb.Append("\\end{tabular}\n");
        sb.Append($"\\caption{{Statystyki operacji dla {(operationResults.DataSize < 10000 ? "małego" : "dużego")} zbioru danych (N = {operationResults.DataSize})}}\n");
        sb.Append("\\end{table}\n");

        return sb.ToString();
    }
}

public class LatexTableRow
{
    public LatexTableRow(string metric, double psqlValue, double mssqlValue, double mongoValue, bool isLowerValueBetter)
    {
        Metric = metric;
        
        PostgresRowValue = new(
            value: psqlValue,
            isLowerValueBetter: isLowerValueBetter,
            comparedValues: new[] { mssqlValue, mongoValue }
        );
        
        MsSqlRowValue = new(
            value: mssqlValue,
            isLowerValueBetter: isLowerValueBetter,
            comparedValues: new[] { psqlValue, mongoValue }
        );
        
        MongoRowValue = new(
            value: mongoValue,
            isLowerValueBetter: isLowerValueBetter,
            comparedValues: new[] { psqlValue, mssqlValue }
        );
    }

    public string Metric { get; set; }
    public LatexTableRowValue PostgresRowValue { get; set; }
    public LatexTableRowValue MsSqlRowValue { get; set; }
    public LatexTableRowValue MongoRowValue { get; set; }
}

public class LatexTableRowValue 
{
    private static readonly string BEST_VALUE_COMMAND = "\\cellcolor{green!25}";
    private static readonly string WORST_VALUE_COMMAND = "\\cellcolor{red!25}";

    public LatexTableRowValue(double value, bool isLowerValueBetter, double[] comparedValues)
    {
        Value = value;
        if (
            (isLowerValueBetter && IsLowest(value, comparedValues))
            ||
            (!isLowerValueBetter && IsHighest(value, comparedValues))
        )
        {
            HighlightCommand = BEST_VALUE_COMMAND;
        }
        
        if (
            (isLowerValueBetter && IsHighest(value, comparedValues))
            ||
            (!isLowerValueBetter && IsLowest(value, comparedValues))
        )
        {
            HighlightCommand = WORST_VALUE_COMMAND;
        }
    }

    public double Value { get; set; }
    public string? HighlightCommand { get; set; }

    public string GetFullLatexCode()
    {
        return $"{HighlightCommand} {Value:F2} ms";
    }
    
    private bool IsHighest(double value, double[] comparedValues)
    {
        return comparedValues.All(comparedValue => value >= comparedValue);
    }
    
    private bool IsLowest(double value, double[] comparedValues)
    {
        return comparedValues.All(comparedValue => value <= comparedValue);
    }
}
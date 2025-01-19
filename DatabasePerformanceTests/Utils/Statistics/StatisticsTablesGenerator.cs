using System.Text;
using DatabasePerformanceTests.Data.Operations;
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
        // foreach (var result in results)
        // { 
        //     
        //     Console.WriteLine($"{result.OperationType} for data size {result.DataSize}:");
        //     var latexTable = GenerateStatisticsTableInLatex(result);
        //     Console.WriteLine(latexTable);
        //     Console.WriteLine("\n\n--------------------------------------\n\n");
        // }

        foreach (var databaseSystem in Enum.GetValues(typeof(DatabaseSystem)).Cast<DatabaseSystem>())
        {
            var crudTables = GenerateStatisticsTableInLatex(
                results, 
                new List<OperationType>
                {
                    OperationType.BulkInsertEnrollments,
                    OperationType.UpdateEnrollments,
                    OperationType.DeleteEnrollments,
                    OperationType.SelectEnrollmentsOrderedById,
                },
                new() { "Insert", "Update", "Delete", "Select" },
                databaseSystem,
                useSeconds: false
            );
            
            foreach (var crudTable in crudTables)
            {
                Console.WriteLine(databaseSystem);
                Console.WriteLine(crudTable);
                Console.WriteLine("\n\n--------------------------------------\n\n");
            }
            
            // var sortTables = GenerateStatisticsTableInLatex(
            //     results, 
            //     new List<OperationType>
            //     {
            //         OperationType.SelectEnrollmentsOrderedById,
            //         OperationType.SelectEnrollmentsWithManySortParameters,
            //     },
            //     new() { "Sortowanie po ID", "Wiele parametrów sortowania" },
            //     databaseSystem
            // );
            //
            // var filterTable = GenerateStatisticsTableInLatex(
            //     results, 
            //     new List<OperationType>
            //     {
            //         OperationType.SelectEnrollmentsFilteredByIsActive,
            //         OperationType.SelectEnrollmentsFilteredByEnrollmentDate,
            //         OperationType.SelectEnrollmentsFilteredByBudget,
            //         OperationType.SelectEnrollmentsFilteredByStudentsLastName,
            //         OperationType.SelectEnrollmentsWithManyFilters,
            //     },
            //     new() { "Po wartości logicznej", "Po zakresie dat", "Po wartości liczbowej", "Po zawieraniu tekstu", "Wiele parametrów filtrowania" },
            //     databaseSystem,
            //     useSeconds: true
            // ).First();
            //
            // Console.WriteLine(databaseSystem);
            // Console.WriteLine(filterTable);
            // Console.WriteLine("\n\n--------------------------------------\n\n");
            //
            // foreach (var sortTable in sortTables)
            // {
            //     Console.WriteLine(databaseSystem);
            //     Console.WriteLine(sortTable);
            //     Console.WriteLine("\n\n--------------------------------------\n\n");
            // }
        }
        
    }

    private IEnumerable<string> GenerateStatisticsTableInLatex(List<OperationResults> results,
        List<OperationType> operationTypes, List<string> columnLabels,
        DatabaseSystem databaseSystem, bool useSeconds = false)
    {
        var allDataSizeOperationResults = results
            .Where(r => operationTypes.Contains(r.OperationType))
            // .Select(r => r.Results[databaseSystem])
            .ToList();
        
        var dataSizes = allDataSizeOperationResults
            .Select(r => r.DataSize)
            .Distinct().ToList();

        foreach (var dataSize in dataSizes)
        {
            var statistics = allDataSizeOperationResults
                .Where(r => r.DataSize == dataSize)
                .OrderBy(r => operationTypes.IndexOf(r.OperationType))
                .Select(r => r.Results[databaseSystem])
                .ToList();
            
            yield return GenerateStatisticsTableInLatex(statistics, columnLabels, $"Datasize {dataSize}", useSeconds);
        }
    }

    private string GenerateStatisticsTableInLatex(OperationResults results)
    {
        List<DatabaseStatistics> statistics = new()
        {
            results.Results[DatabaseSystem.Postgres],
            results.Results[DatabaseSystem.MsSql],
            results.Results[DatabaseSystem.Mongo]
        };
        return GenerateStatisticsTableInLatex(statistics, new() { "PostgreSQL", "MSSQL", "MongoDB" });
    }

    private string GenerateStatisticsTableInLatex(List<DatabaseStatistics> statistics, List<string> columnLabels, string caption = null, bool useSeconds = false)
    {
        foreach (var s in statistics)
        {
            s.CalculateResultParameters();
        }
        
        List<LatexTableRow> latexRows = new();
        latexRows.Add(new("Średnia", statistics.Select(s => s.Average).ToArray(), true, useSeconds));
        latexRows.Add(new("Mediana", statistics.Select(s => s.Median).ToArray(), true, useSeconds));
        latexRows.Add(new("Minimum", statistics.Select(s => s.Minimum).ToArray(), true, useSeconds));
        latexRows.Add(new("Maksimum", statistics.Select(s => s.Maximum).ToArray(), true, useSeconds));
        latexRows.Add(new("Odch. std.", statistics.Select(s => s.StandardDeviation).ToArray(), true, useSeconds));
        latexRows.Add(new("1. kwartyl", statistics.Select(s => s.Q1).ToArray(), true, useSeconds));
        latexRows.Add(new("3. kwartyl", statistics.Select(s => s.Q3).ToArray(), true, useSeconds));
        
        return GenerateStatisticsTableInLatex(latexRows, columnLabels, caption);
    }
    
    private string GenerateStatisticsTableInLatex(List<LatexTableRow> latexRows, List<string> columnLabels, string caption = null)
    {
        var columnsCount = latexRows.Max(r => r.RowValues.Count);
        double columnWidthCm = columnsCount switch
        {
            2 => 3,
            3 => 2.5,
            _ => 2
        };
        
        var sb = new StringBuilder();
        sb.Append("\\begin{table}[H]\n");
        sb.Append("\\centering\n");
        sb.Append("\\begin{tabular}{|>{\\columncolor[gray]{0.9}\\centering\\arraybackslash}p{3cm}|");
        sb.Append(string.Join("|", Enumerable.Repeat($">{{\\centering\\arraybackslash}}p{{{columnWidthCm}cm}}", columnsCount)));
        sb.Append("|}\n");
        sb.Append("\\rowcolor{gray!40}\n");
        sb.Append("\\hline\n");
        sb.Append("Metryka & ");
        sb.Append(string.Join(" & ", columnLabels));
        sb.Append("\\\\ \\hline\n");

        foreach (var row in latexRows)
        {
            sb.Append(string.Join(" & ", 
                new[] { row.MetricName }
                    .Concat(row.RowValues.Select(value => value.GetFullLatexCode()))
            ));
            sb.Append(" \\\\ \\hline\n");
        }
        
        sb.Append("\\end{tabular}\n");
        sb.Append($"\\caption{{{caption}}}\n");
        sb.Append("\\end{table}\n");

        return sb.ToString();
    }
}



public class LatexTableRow
{
    public LatexTableRow(string metricName, double[] values, bool isLowerValueBetter, bool useSeconds = false)
    {
        MetricName = metricName;
        RowValues = values.Select(value => new LatexTableRowValue(
            value: value,
            isLowerValueBetter: isLowerValueBetter,
            comparedValues: values.Where(e => e != value).ToArray(),
            useSeconds: useSeconds
        )).ToList();
    }

    public string MetricName { get; set; }
    public List<LatexTableRowValue> RowValues { get; set; }
}

public class LatexTableRowValue 
{
    private static readonly string BEST_VALUE_COMMAND = "\\cellcolor{green!25}";
    private static readonly string WORST_VALUE_COMMAND = "\\cellcolor{red!25}";

    public  LatexTableRowValue(double value, bool isLowerValueBetter, double[] comparedValues, bool useSeconds = false)
    {
        Value = value;
        UseSeconds = useSeconds;
        
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
    public bool UseSeconds { get; set; }

    public string GetFullLatexCode()
    {
        return $"{HighlightCommand} \\numprint{{{(UseSeconds ? Value/1000 : Value):F2}}} {(UseSeconds ? "s" : "ms")}";
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
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

        var sb = new StringBuilder();
        sb.Append("\\begin{table}[H]\n");
        sb.Append("\\centering\n");
        sb.Append("\\begin{tabular}{|>{\\columncolor[gray]{0.9}}l|c|c|c|}\n");
        sb.Append("\\rowcolor{gray!40}\n");
        sb.Append("\\hline\n");
        sb.Append("Metryka & PostgreSQL & MSSQL & MongoDB \\\\ \\hline\n");

        sb.Append($"Średnia & {psqlStats.Average:F2} ms & {mssqlStats.Average:F2} ms & {mongoStats.Average:F2} ms \\\\ \\hline\n");
        sb.Append($"Mediana & {psqlStats.Median:F2} ms & {mssqlStats.Median:F2} ms & {mongoStats.Median:F2} ms \\\\ \\hline\n");
        sb.Append($"Minimum & {psqlStats.Minimum:F2} ms & {mssqlStats.Minimum:F2} ms & {mongoStats.Minimum:F2} ms \\\\ \\hline\n");
        sb.Append($"Maksimum & {psqlStats.Maximum:F2} ms & {mssqlStats.Maximum:F2} ms & {mongoStats.Maximum:F2} ms \\\\ \\hline\n");
        sb.Append($"Odch. std. & {psqlStats.StandardDeviation:F2} ms & {mssqlStats.StandardDeviation:F2} ms & {mongoStats.StandardDeviation:F2} ms \\\\ \\hline\n");
        sb.Append($"1. kwartyl & {psqlStats.Q1:F2} ms & {mssqlStats.Q1:F2} ms & {mongoStats.Q1:F2} ms \\\\ \\hline\n");
        sb.Append($"3. kwartyl & {psqlStats.Q3:F2} ms & {mssqlStats.Q3:F2} ms & {mongoStats.Q3:F2} ms \\\\ \\hline\n");

        sb.Append("\\end{tabular}\n");
        sb.Append($"\\caption{{Statystyki operacji dla {(operationResults.DataSize < 10000 ? "małego" : "dużego")} zbioru danych (N = {operationResults.DataSize})}}\n");
        sb.Append("\\end{table}\n");

        return sb.ToString();
    }
}
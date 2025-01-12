using DatabasePerformanceTests.Data.Operations;
using DatabasePerformanceTests.Utils.Config.Enums;
using DatabasePerformanceTests.Utils.Tests.Models;
using ScottPlot;

namespace DatabasePerformanceTests.Utils.Files;

public class ChartsGenerator
{
    private static readonly string CHARTS_DIRECTORY_NAME = "Plots";
    public void GenerateCharts(List<OperationResults> results, string outputDirectory)
    {
        var selectStudentsOrderedByIdResults = results
            .Where(r => r.OperationType == OperationType.SelectStudentsOrderedById)
            .OrderBy(r => r.DataSize);
        
        var mongoTimes = selectStudentsOrderedByIdResults
            .Select(r => r.Results[DatabaseSystem.Mongo].Average)
            .ToArray();
        
        var psqlTimes = selectStudentsOrderedByIdResults
            .Select(r => r.Results[DatabaseSystem.Postgres].Average)
            .ToArray();
        
        var mssqlTimes = selectStudentsOrderedByIdResults
            .Select(r => r.Results[DatabaseSystem.MsSql].Average)
            .ToArray();

        var dataSizes = selectStudentsOrderedByIdResults.Select(r => r.DataSize).ToArray();
        
        Plot plt = new();
        var mongoScatter = plt.Add.Scatter(dataSizes, mongoTimes, new Color(255, 0, 0));
        mongoScatter.LegendText = "MongoDB";
        
        var psqlScatter = plt.Add.Scatter(dataSizes, psqlTimes, new Color(0, 255, 0));
        psqlScatter.LegendText = "PostgreSQL";
        
        var mssqlScatter = plt.Add.Scatter(dataSizes, mssqlTimes, new Color(0, 0, 255));
        mssqlScatter.LegendText = "MS SQL Server";
        
        plt.Title("Pobieranie listy studentów posortowanych po ID");
        plt.XLabel("Liczba N");
        plt.YLabel("Czas (ms)");
        
        string fileName = $"SelectStudentsOrderedById";
        string fileDirectory = GetFileDirectory(outputDirectory);
        Directory.CreateDirectory(fileDirectory);
        string filePath = Path.Combine(fileDirectory, fileName + ".png");
        plt.SavePng(filePath, 900, 600);
    }
    
    private static string GetFileDirectory(string outputDirectory)
    {
        return Path.Combine(
            FilesManager.GetFileDirectory(outputDirectory, CHARTS_DIRECTORY_NAME), 
            FilesManager.GetFileNameDate()
        );
    }
}
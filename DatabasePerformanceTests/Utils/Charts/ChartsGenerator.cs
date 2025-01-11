using DatabasePerformanceTests.Utils.Tests.Models;
using ScottPlot;

namespace DatabasePerformanceTests.Utils.Charts;

public class ChartsGenerator
{
    private static readonly string FILE_NAME_DATE_FORMAT = "yyyy_MM_dd_HH_mm_ss";

    public void GenerateCharts(List<OperationResults> results, string outputDirectory)
    {
        double[] xs = { 1, 2, 3, 4, 5 };
        double[] ys = { 10, 20, 15, 30, 25 };

        Plot plt = new();
        plt.Add.Scatter(xs, ys);
        plt.Title("Przykładowy wykres");
        plt.XLabel("Liczba N");
        plt.YLabel("Czas (ms)");
        
        string fileName = $"plot{DateTime.Now.ToString(FILE_NAME_DATE_FORMAT)}";
        string fileDirectory = GetFileDirectory(outputDirectory);
        Directory.CreateDirectory(fileDirectory);
        string filePath = Path.Combine(fileDirectory, fileName + ".png");
        plt.SavePng(filePath, 600, 400);
    }
    
    private static string GetFileDirectory(string outputDirectory)
    {
        return Path.Combine(outputDirectory.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)), "Tests", "Plots");
    }
}
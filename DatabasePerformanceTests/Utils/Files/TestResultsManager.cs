using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using DatabasePerformanceTests.Utils.Tests.Models;

namespace DatabasePerformanceTests.Utils.Files;

public class TestResultsManager
{
    private static readonly string RESULTS_DIRECTORY_NAME = "Results";
    public static void WriteResultsToFile(List<OperationResults> results, string outputDirectory)
    {
        string fileName = $"results_{FilesManager.GetFileNameDate()}";
        string fileDirectory = GetFileDirectory(outputDirectory);
        Directory.CreateDirectory(fileDirectory);
        string filePath = Path.Combine(fileDirectory, fileName + ".json");
        
        var json = JsonSerializer.Serialize(results, new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        });
        File.WriteAllText(filePath, json);
        
        Logger.Log($"Results written to {filePath}");
    }
    
    public static List<OperationResults> ReadResultsFromFile(string outputDirectory)
    {
        var fileDirectory = GetFileDirectory(outputDirectory);
        var newestFileName = GetNewestResultsFileName(outputDirectory);
        var filePath = Path.Combine(fileDirectory, newestFileName + ".json");

        if (!File.Exists(filePath))
        {
            Logger.Log($"File {filePath} does not exist");
            return new();
        }
        
        Console.WriteLine($"Reading result from latest file with name: {filePath}");
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<OperationResults>>(json, new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        });
    }

    private static string GetFileDirectory(string outputDirectory)
    {
        return FilesManager.GetFileDirectory(outputDirectory, RESULTS_DIRECTORY_NAME);
    }
    
    private static string GetNewestResultsFileName(string outputDirectory)
    {
        string fileDirectory = GetFileDirectory(outputDirectory);
        string[] filePaths = Directory.GetFiles(fileDirectory, "*.json");
        
        string latestFilePath = filePaths
            .OrderByDescending(filePath => DateTime.ParseExact(
                Path.GetFileNameWithoutExtension(filePath).Substring(8, 19), 
                FilesManager.GetFileNameDateFormat(), 
                CultureInfo.InvariantCulture))
            .FirstOrDefault() ?? "results_latest";

        return Path.GetFileNameWithoutExtension(latestFilePath);
    }
}
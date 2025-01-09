using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using DatabasePerformanceTests.Utils.Tests.Models;

namespace DatabasePerformanceTests.Utils.Files;

public class TestResultsManager
{
    private static readonly string FILE_NAME_DATE_FORMAT = "yyyy_MM_dd_HH_mm_ss";
    public static void WriteResultsToFile(List<TestResult> results, string outputDirectory)
    {
        string fileName = $"results_{DateTime.Now.ToString(FILE_NAME_DATE_FORMAT)}";
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
    
    public static List<TestResult> ReadResultsFromFile(string outputDirectory)
    {
        var fileDirectory = GetFileDirectory(outputDirectory);
        var newestFileName = GetNewestResultsFileName(outputDirectory);
        var filePath = Path.Combine(fileDirectory, newestFileName + ".json");

        if (!File.Exists(filePath))
        {
            Logger.Log($"File {filePath} does not exist");
            return new();
        }
        
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<TestResult>>(json, new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        });
    }

    private static string GetFileDirectory(string outputDirectory)
    {
        return Path.Combine(outputDirectory.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)), "TestResults");
    }
    
    private static string GetNewestResultsFileName(string outputDirectory)
    {
        string fileDirectory = GetFileDirectory(outputDirectory);
        string[] filePaths = Directory.GetFiles(fileDirectory, "*.json");
        
        string latestFilePath = filePaths
            .OrderByDescending(filePath => DateTime.ParseExact(
                Path.GetFileNameWithoutExtension(filePath).Substring(8, 19), 
                FILE_NAME_DATE_FORMAT, 
                CultureInfo.InvariantCulture))
            .FirstOrDefault() ?? "results_latest";

        return Path.GetFileNameWithoutExtension(latestFilePath);
    }
}
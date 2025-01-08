using System.Text.Json;
using System.Text.Json.Serialization;
using DatabasePerformanceTests.Utils.Tests.Models;

namespace DatabasePerformanceTests.Utils.Files;

public class TestResultsWriter
{
    public static void WriteResultsToFile(List<TestResult> results, string fileName, string outputDirectory)
    {
        var fileDirectory = Path.Combine(outputDirectory.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)), "TestResults");
        Directory.CreateDirectory(fileDirectory);
        var filePath = Path.Combine(fileDirectory, fileName + ".json");
        
        var json = JsonSerializer.Serialize(results, new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        });
        File.WriteAllText(filePath, json);
        
        Logger.Log($"Results written to {filePath}");
    }
}
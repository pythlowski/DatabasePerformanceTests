namespace DatabasePerformanceTests.Utils.Files;

public class FilesManager
{
    private static readonly string FILE_NAME_DATE_FORMAT = "yyyy_MM_dd_HH_mm_ss";
    private static readonly string MAIN_DIRECTORY_NAME = "DatabasePerformanceTests";
    
    public static string GetFileNameDateFormat()
    {
        return FILE_NAME_DATE_FORMAT;
    }
    public static string GetFileNameDate()
    {
        return DateTime.Now.ToString(FILE_NAME_DATE_FORMAT);
    }

    public static string GetFileDirectory(string outputDirectory, string subDirectoryName)
    {
        return Path.Combine(
            outputDirectory.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)), 
            MAIN_DIRECTORY_NAME, 
            subDirectoryName
        );
    }
}
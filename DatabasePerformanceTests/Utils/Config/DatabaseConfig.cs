using DatabasePerformanceTests.Utils.Config.Enums;

namespace DatabasePerformanceTests.Utils.Config;

public class DatabaseConfig
{
    public string Name { get; set; }
    public DatabaseSystem System { get; set; }
    public string ConnectionString { get; set; }
}
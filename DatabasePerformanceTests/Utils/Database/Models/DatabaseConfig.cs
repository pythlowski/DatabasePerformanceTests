using DatabasePerformanceTests.Utils.Database.Models.Enums;

namespace DatabasePerformanceTests.Utils.Database.Models;

public class DatabaseConfig
{
    public string Name { get; set; }
    public DatabaseSystem System { get; set; }
    public string ConnectionString { get; set; }
}
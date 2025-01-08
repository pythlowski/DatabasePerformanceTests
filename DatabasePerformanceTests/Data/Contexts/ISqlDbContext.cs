namespace DatabasePerformanceTests.Data.Contexts;

public interface ISqlDbContext
{
    Task ExecuteNonQueryAsync(string query, bool useCurrentTransaction = false);
    Task<object?> ExecuteScalarAsync(string query, bool useCurrentTransaction = false);
    Task<List<Dictionary<string, object>>> ExecuteReaderAsync(string query, bool useCurrentTransaction = false);
}
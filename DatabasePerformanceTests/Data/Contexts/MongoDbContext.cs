using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Utils.Generators.Models;
using MongoDB.Driver;

namespace DatabasePerformanceTests.Data.Contexts;

public class MongoDbContext : AbstractDbContext
{
    private readonly MongoClient _client;

    public MongoDbContext(string connectionString)
        : base(connectionString)
    {
        _client = new MongoClient(connectionString);
    }

    public override async Task CreateDatabaseAsync()
    {
        var database = _client.GetDatabase(DatabaseName);
        await database.CreateCollectionAsync("testCollection");
        Console.WriteLine($"MongoDB database '{DatabaseName}' created.");
    }

    public override async Task CreateTablesAsync()
    {
        var database = _client.GetDatabase(DatabaseName);
        await database.CreateCollectionAsync("testCollection");
    }

    public override Task PopulateDatabaseAsync(GeneratedData data)
    {
        throw new NotImplementedException();
    }

    public override Task RestoreDatabaseAsync(GeneratedData data)
    {
        throw new NotImplementedException();
    }


    public override async Task DropDatabaseAsync()
    {
        var database = _client.GetDatabase(DatabaseName);
        var collections = await database.ListCollectionNamesAsync();

        await _client.DropDatabaseAsync(DatabaseName);
        Console.WriteLine($"MongoDB database '{DatabaseName}' dropped.");
    }
}
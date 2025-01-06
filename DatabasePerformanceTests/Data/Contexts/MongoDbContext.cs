using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Data.Models.Mongo;
using DatabasePerformanceTests.Utils;
using DatabasePerformanceTests.Utils.Database.Models.Enums;
using DatabasePerformanceTests.Utils.Generators.Models;
using MongoDB.Driver;

namespace DatabasePerformanceTests.Data.Contexts;

public class MongoDbContext : AbstractDbContext
{
    private readonly MongoClient _client;
    private IClientSessionHandle _session;
    IMongoDatabase _testDatabase;
    
    public MongoDbContext(string connectionString)
        : base(connectionString)
    {
        _client = new MongoClient(connectionString);
        _testDatabase = _client.GetDatabase(DatabaseName);
    }
    
    public override DatabaseSystem DatabaseSystem => DatabaseSystem.Mongo;

    public override async Task CreateDatabaseAsync()
    {
        await _testDatabase.CreateCollectionAsync("testCollection");
        Logger.Log($"MongoDB database '{DatabaseName}' created.");
    }

    public override async Task CreateTablesAsync()
    {
        await _testDatabase.CreateCollectionAsync("students");
        await _testDatabase.CreateCollectionAsync("instructors");
        await _testDatabase.CreateCollectionAsync("courseInstances");
    }
    
    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _testDatabase.GetCollection<T>(collectionName);
    }

    public override async Task PopulateDatabaseAsync(GeneratedData data)
    {
        Logger.Log("MongoDB Populating MongoDB database...");
        var studentsCollection = _testDatabase.GetCollection<MongoStudent>("students");
        await studentsCollection.InsertManyAsync(data.Students.Select(MongoStudent.FromDomain));
        Logger.Log("MongoDB Finished inserting students.");
        
        var instructorsCollection = _testDatabase.GetCollection<MongoInstructor>("instructors");
        await instructorsCollection.InsertManyAsync(data.Instructors.Select(MongoInstructor.FromDomain));
        Logger.Log("MongoDB Finished inserting instructors.");
        
        var courseInstancesCollection = _testDatabase.GetCollection<MongoCourseInstance>("courseInstances");
        var denormalizedCourseInstances = GetDenormalizedCourseInstances(data).ToList();
        Logger.Log("MongoDB Prepared denormalized course instances.");
        
        // const int batchSize = 10000;
        // for (int i = 0; i < denormalizedCourseInstances.Count; i += batchSize)
        // {
        //     Logger.Log($"batch {i+1}");
        //     var batch = denormalizedCourseInstances.Skip(i).Take(batchSize).ToList();
        //     var bulkOps = new List<WriteModel<MongoCourseInstance>>();
        //     foreach (var mongoCourseInstance in batch)
        //     {
        //         bulkOps.Add(new InsertOneModel<MongoCourseInstance>(mongoCourseInstance));
        //     }
        //     await courseInstancesCollection.BulkWriteAsync(bulkOps);
        // }
        
        Logger.Log("MongoDB Inserting course instances...");
        var bulkOps = new List<WriteModel<MongoCourseInstance>>();
        foreach(var mongoCourseInstance in denormalizedCourseInstances)
        {
            bulkOps.Add(new InsertOneModel<MongoCourseInstance>(mongoCourseInstance));
        }
        await courseInstancesCollection.BulkWriteAsync(bulkOps);

        // await courseInstancesCollection.InsertManyAsync(denormalizedCourseInstances);
        Logger.Log("MongoDB Finished inserting course instances.");
        
        
    }

    private IEnumerable<MongoCourseInstance> GetDenormalizedCourseInstances(GeneratedData data)
    {
        foreach (var courseInstance in data.CourseInstances)
        {
            var course = data.Courses.First(c => c.Id == courseInstance.CourseId);
            var instructor = data.Instructors.First(i => i.Id == courseInstance.InstructorId);
            data.CourseInstanceStudentMap.TryGetValue(courseInstance.Id, out var enrolledStudents);
            
            yield return MongoCourseInstance.FromDomain(courseInstance, course, instructor, enrolledStudents ?? new List<(Student, Enrollment)>());
        }
    }
    
    public override async Task DropDatabaseAsync()
    {
        await _client.DropDatabaseAsync(DatabaseName);
        Logger.Log($"MongoDB database '{DatabaseName}' dropped.");
    }

    public override async Task StartTransactionAsync()
    {
        _session = await _client.StartSessionAsync();
        _session.StartTransaction();
    }

    public override async Task RollbackTransactionAsync()
    {
        if (_session == null) throw new InvalidOperationException("Transaction not started.");
        await _session.AbortTransactionAsync();
        _session.Dispose();
        _session = null;
    }

    public override async Task CommitTransactionAsync()
    {
        if (_session == null) throw new InvalidOperationException("Transaction not started.");
        await _session.CommitTransactionAsync();    
        _session.Dispose();
        _session = null;
    }

    public IClientSessionHandle GetSession()
    {
        return _session;
    }
}
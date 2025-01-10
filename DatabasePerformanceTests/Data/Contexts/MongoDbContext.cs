using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Data.Models.Mongo;
using DatabasePerformanceTests.Utils;
using DatabasePerformanceTests.Utils.Config.Enums;
using DatabasePerformanceTests.Utils.Generators.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DatabasePerformanceTests.Data.Contexts;

public class MongoDbContext : AbstractDbContext
{
    private readonly MongoClient _client;
    private IClientSessionHandle _session;
    IMongoDatabase _testDatabase;
    
    public MongoDbContext(string connectionString, string databaseName)
        : base(connectionString, databaseName)
    {
        _client = new MongoClient(connectionString);
        _testDatabase = _client.GetDatabase(DatabaseName);
    }
    
    public override DatabaseSystem DatabaseSystem => DatabaseSystem.Mongo;

    public override async Task CreateDatabaseAsync()
    {
        await _testDatabase.CreateCollectionAsync("students");
        Logger.Log($"MongoDB database '{DatabaseName}' created.");
    }

    public override async Task CreateTablesAsync()
    {
        await _testDatabase.CreateCollectionAsync("students");
        await _testDatabase.CreateCollectionAsync("instructors");
        await _testDatabase.CreateCollectionAsync("courseInstances");
        await _testDatabase.CreateCollectionAsync("enrollments");
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
        var denormalizedcourseInstances = GetDenormalizedCourseInstances(data).ToList();
        await courseInstancesCollection.InsertManyAsync(denormalizedcourseInstances);
        Logger.Log("MongoDB Finished inserting courseInstances.");

        var enrollmentsCollection = _testDatabase.GetCollection<MongoEnrollment>("enrollments");
        var denormalizedEnrollments = GetDenormalizedEnrollments(data).ToList();

        Logger.Log("MongoDB Inserting course instances...");
        var bulkOps = new List<WriteModel<MongoEnrollment>>();
        foreach(var mongoEnrollment in denormalizedEnrollments)
        {
            bulkOps.Add(new InsertOneModel<MongoEnrollment>(mongoEnrollment));
        }
        await enrollmentsCollection.BulkWriteAsync(bulkOps);

        // await courseInstancesCollection.InsertManyAsync(denormalizedCourseInstances);
        Logger.Log("MongoDB Finished inserting course instances.");
        
        
    }

    public override async Task CreateIndexesAsync()
    {
        // var collection = _testDatabase.GetCollection<MongoCourseInstance>("courseInstances");
        //
        // var budgetIndex = new CreateIndexModel<MongoCourseInstance>(
        //     Builders<MongoCourseInstance>.IndexKeys.Ascending(x => x.Budget)
        // );
        //
        // var enrolledStudentsComplexIndex = new CreateIndexModel<MongoCourseInstance>(
        //     Builders<MongoCourseInstance>.IndexKeys
        //         .Ascending("EnrolledStudents.IsActive")
        //         .Ascending("EnrolledStudents.LastName")
        //         .Ascending("EnrolledStudents.EnrollmentDate")
        // );
        //
        // var enrolledStudentsIsActiveIndex = new CreateIndexModel<MongoCourseInstance>(
        //     Builders<MongoCourseInstance>.IndexKeys
        //         .Ascending("EnrolledStudents.IsActive")
        // );
        //
        // var enrolledStudentsLastNameIndex = new CreateIndexModel<MongoCourseInstance>(
        //     Builders<MongoCourseInstance>.IndexKeys
        //         .Text("EnrolledStudents.LastName")
        // );
        //
        // var enrolledStudentsEnrollmentDateIndex = new CreateIndexModel<MongoCourseInstance>(
        //     Builders<MongoCourseInstance>.IndexKeys
        //         .Ascending("EnrolledStudents.EnrollmentDate")
        // );
        //
        // await collection.Indexes.CreateOneAsync(budgetIndex);
        // await collection.Indexes.CreateOneAsync(enrolledStudentsComplexIndex);
        // await collection.Indexes.CreateOneAsync(enrolledStudentsIsActiveIndex);
        // await collection.Indexes.CreateOneAsync(enrolledStudentsLastNameIndex);
        // await collection.Indexes.CreateOneAsync(enrolledStudentsEnrollmentDateIndex);
    }

    private IEnumerable<MongoEnrollment> GetDenormalizedEnrollments(GeneratedData data)
    {
        foreach (var enrollment in data.Enrollments)
        {
            var student = data.Students.First(s => s.Id == enrollment.StudentId);
            var courseInstance = data.CourseInstances.First(c => c.Id == enrollment.CourseInstanceId);
            var course = data.Courses.First(c => c.Id == courseInstance.CourseId);
            yield return MongoEnrollment.FromDomain(enrollment, student, courseInstance, course);
        }
    }
    
    private IEnumerable<MongoCourseInstance> GetDenormalizedCourseInstances(GeneratedData data)
    {
        foreach (var courseInstance in data.CourseInstances)
        {
            var instructor = data.Instructors.First(i => i.Id == courseInstance.InstructorId);
            var course = data.Courses.First(c => c.Id == courseInstance.CourseId);
            yield return MongoCourseInstance.FromDomain(courseInstance, course, instructor);
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

    public override async Task ClearCacheAsync()
    {
        var command = new BsonDocument { { "planCacheClear", "students" } };
        await _testDatabase.RunCommandAsync<BsonDocument>(command);
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
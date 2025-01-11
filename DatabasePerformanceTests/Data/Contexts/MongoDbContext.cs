using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Data.Models.Mongo;
using DatabasePerformanceTests.Utils;
using DatabasePerformanceTests.Utils.Config.Enums;
using DatabasePerformanceTests.Utils.Generators.Models;
using Microsoft.Extensions.Logging;
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
        var denormalizedStudents = GetDenormalizedStudents(data).ToList();
        await studentsCollection.InsertManyAsync(denormalizedStudents);
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
        Logger.Log("MongoDB Finished preparing denormalized enrollments");
        
        Logger.Log("MongoDB Inserting course instances...");
        var bulkOps = new List<WriteModel<MongoEnrollment>>();
        foreach(var mongoEnrollment in denormalizedEnrollments)
        {
            bulkOps.Add(new InsertOneModel<MongoEnrollment>(mongoEnrollment));
        }
        await enrollmentsCollection.BulkWriteAsync(bulkOps);
        Logger.Log("MongoDB Finished inserting enrollments.");
    }

    private IEnumerable<MongoStudent> GetDenormalizedStudents(GeneratedData data)
    {
        foreach (var student in data.Students)
        {
            var enrolledCourseInstanceIds = data.StudentToEnrolledCourseInstanceIdsMap[student];
            var courseInstances = data.CourseInstances
                .Where(c => enrolledCourseInstanceIds.Contains(c.Id)).ToList();
            yield return MongoStudent.FromDomain(student, courseInstances, data.Courses, data.Instructors);
        }
    }

    private IEnumerable<MongoEnrollment> GetDenormalizedEnrollments(GeneratedData data)
    {
        foreach (var enrollment in data.Enrollments)
        {
            var student = data.EnrollmentIdToStudentMap[enrollment.Id];
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
    
    public override async Task CreateIndexesAsync()
    {
        var collection = _testDatabase.GetCollection<MongoEnrollment>("enrollments");
        
        var enrollmentDateIndex = new CreateIndexModel<MongoEnrollment>(
            Builders<MongoEnrollment>.IndexKeys.Ascending(x => x.EnrollmentDate)
        );
        await collection.Indexes.CreateOneAsync(enrollmentDateIndex);

        var studentComplexIndex = new CreateIndexModel<MongoEnrollment>(
            Builders<MongoEnrollment>.IndexKeys
                .Ascending("Student.IsActive")
                .Ascending("Student.LastName")
        );
        await collection.Indexes.CreateOneAsync(studentComplexIndex);

        var studentIsActiveIndex = new CreateIndexModel<MongoEnrollment>(
            Builders<MongoEnrollment>.IndexKeys
                .Ascending("Student.IsActive")
        );
        await collection.Indexes.CreateOneAsync(studentIsActiveIndex);

        var studentLastNameIndex = new CreateIndexModel<MongoEnrollment>(
            Builders<MongoEnrollment>.IndexKeys
                .Text("Student.LastName")
        );
        await collection.Indexes.CreateOneAsync(studentLastNameIndex);

        var courseBudgetIndex = new CreateIndexModel<MongoEnrollment>(
            Builders<MongoEnrollment>.IndexKeys
                .Ascending("Course.Budget")
        );
        await collection.Indexes.CreateOneAsync(courseBudgetIndex);
        Logger.Log("MongoDB Finished creating indexes");
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
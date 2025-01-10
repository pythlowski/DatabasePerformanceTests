using DatabasePerformanceTests.Data.Contexts;
using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Data.Models.Mongo;
using DatabasePerformanceTests.Data.Models.Results;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DatabasePerformanceTests.Data.Operations;

public class MongoDbOperations(MongoDbContext context) : IDbOperations
{
    private List<BsonDocument> GetBaseEnrollmentsPipeline(BsonDocument? matchFilter = null)
    {
        var pipeline = new List<BsonDocument>
        {
            new("$unwind", "$EnrolledStudents"),
            matchFilter != null ? new BsonDocument("$match", matchFilter) : null,
            new("$project", new BsonDocument
            {
                { "_id", 0 },
                { "EnrollmentId", "$EnrolledStudents.EnrollmentId" },
                { "StudentFirstName", "$EnrolledStudents.FirstName" },
                { "StudentLastName", "$EnrolledStudents.LastName" },
                // { "CourseName", "$Course.Name" },
                // { "IsActive", "$EnrolledStudents.IsActive" },
                // { "EnrollmentDate", "$EnrolledStudents.EnrollmentDate" },
                // { "Budget", "$Budget" }
            })
        };
        pipeline.RemoveAll(stage => stage == null);
        return pipeline;
    }
    
    public Task InsertEnrollmentsAsync(IEnumerable<Enrollment> enrollments)
    {
        throw new NotImplementedException();
    }

    public Task DeleteEnrollmentsAsync(int count)
    {
        var collection = context.GetCollection<CourseInstance>("courseInstances");
        return Task.CompletedTask;
    }

    public Task UpdateEnrollmentDatesAsync(int count)
    {
        throw new NotImplementedException();
    }

    public async Task<List<StudentBase>> SelectStudentsOrderedByIdAsync(int limit)
    {
        var collection = context.GetCollection<MongoStudent>("students");
        var students = await collection.Find(Builders<MongoStudent>.Filter.Empty)
            .Sort(Builders<MongoStudent>.Sort.Ascending(x => x.Id))
            .Limit(limit)
            .ToListAsync();
        return students.Select(StudentBase.FromMongo).ToList();
    }

    public async Task<List<EnrollmentResult>> SelectEnrollmentsOrderedByIdAsync(int limit)
    {
        var collection = context.GetCollection<MongoCourseInstance>("courseInstances");
        var pipeline = GetBaseEnrollmentsPipeline();
        pipeline.Add(new ("$sort", new BsonDocument("EnrollmentId", 1)));

        var results = await collection.Aggregate<EnrollmentResult>(pipeline).ToListAsync();
        return results;
    }

    public async Task<List<EnrollmentResult>> SelectEnrollmentsFilteredByIsActiveAsync(bool isActive)
    {
        var collection = context.GetCollection<MongoCourseInstance>("courseInstances");
        BsonDocument matchFilter = new()
        {
            { "EnrolledStudents.IsActive", isActive }
        };
        var pipeline = GetBaseEnrollmentsPipeline(matchFilter);
        var results = await collection.Aggregate<EnrollmentResult>(pipeline).ToListAsync();
        return results;
    }

    public Task SelectEnrollmentsFilteredByEnrollmentDateAsync(DateTime dateFrom, DateTime dateTo)
    {
        throw new NotImplementedException();
    }

    public Task SelectEnrollmentsFilteredByBudgetAsync(long valueFrom, long valueTo)
    {
        throw new NotImplementedException();
    }

    public Task SelectEnrollmentsFilteredByStudentsLastNameAsync(string lastNameSearchText)
    {
        throw new NotImplementedException();
    }

    public Task SelectEnrollmentsWithManyFiltersAsync(bool isActive, DateTime dateFrom, DateTime dateTo, long valueFrom,
        long valueTo, string lastNameSearchText)
    {
        throw new NotImplementedException();
    }

    public Task SelectEnrollmentsWithPaginationAsync(int pageSize, int pageNumber)
    {
        throw new NotImplementedException();
    }

    public Task SelectEnrollmentsWithManySortParametersAsync(int limit)
    {
        throw new NotImplementedException();
    }
}
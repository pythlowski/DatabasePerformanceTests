using DatabasePerformanceTests.Data.Contexts;
using DatabasePerformanceTests.Data.Models;
using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Data.Models.Mongo;
using DatabasePerformanceTests.Data.Models.Results;
using DatabasePerformanceTests.Data.Operations.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DatabasePerformanceTests.Data.Operations;

public class MongoDbOperations(MongoDbContext context) : IDbOperations
{

    public async Task BulkInsertAsync(List<IEnrollment> data)
    {
        var collection = context.GetCollection<MongoEnrollment>("enrollments");
        
        var bulkOps = new List<WriteModel<MongoEnrollment>>();
        foreach(var element in data)
        {
            bulkOps.Add(new InsertOneModel<MongoEnrollment>((MongoEnrollment)element));
        }
        await collection.BulkWriteAsync(context.GetSession(), bulkOps);
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

    public async Task<List<StudentBaseResult>> SelectStudentsOrderedByIdAsync(int limit)
    {
        var collection = context.GetCollection<MongoStudent>("students");
        var students = await collection.Find(Builders<MongoStudent>.Filter.Empty)
            .Sort(Builders<MongoStudent>.Sort.Ascending(x => x.Id))
            .Limit(limit)
            .ToListAsync();
        return students.Select(StudentBaseResult.FromMongo).ToList();
    }

    public async Task<List<EnrollmentBaseResult>> SelectEnrollmentsOrderedByIdAsync(int limit)
    {
        var collection = context.GetCollection<MongoEnrollment>("enrollments");
        var data = await collection
            .Find(Builders<MongoEnrollment>.Filter.Empty)
            .Sort(Builders<MongoEnrollment>.Sort.Ascending(x => x.Id))
            .Limit(limit)
            .Project(GetEnrollmentProjection())
            .ToListAsync();
        return data;
    }

    public async Task<List<EnrollmentBaseResult>> SelectEnrollmentsFilteredByIsActiveAsync(bool isActive)
    {
        var collection = context.GetCollection<MongoEnrollment>("enrollments");
        var data = await collection
            .Find(
                Builders<MongoEnrollment>.Filter.Eq(x => x.Student.IsActive, true)
            )
            .Project(GetEnrollmentProjection())
            .ToListAsync();
        return data;
    }

    public async Task<List<EnrollmentBaseResult>> SelectEnrollmentsFilteredByEnrollmentDateAsync(DateTime dateFrom, DateTime dateTo)
    {
        var collection = context.GetCollection<MongoEnrollment>("enrollments");
        var data = await collection
            .Find(
                Builders<MongoEnrollment>.Filter.And(
                    Builders<MongoEnrollment>.Filter.Gte(x => x.EnrollmentDate, dateFrom),
                    Builders<MongoEnrollment>.Filter.Lte(x => x.EnrollmentDate, dateTo)
                )
                )
            .Project(GetEnrollmentProjection())
            .ToListAsync();
        return data;
    }

    public async Task<List<EnrollmentBaseResult>> SelectEnrollmentsFilteredByBudgetAsync(int valueFrom, int valueTo)
    {
        var collection = context.GetCollection<MongoEnrollment>("enrollments");
        var data = await collection
            .Find(
                Builders<MongoEnrollment>.Filter.And(
                    Builders<MongoEnrollment>.Filter.Gte(x => x.Course.Budget, valueFrom),
                    Builders<MongoEnrollment>.Filter.Lte(x => x.Course.Budget, valueTo)
                )
            )
            .Project(GetEnrollmentProjection())
            .ToListAsync();
        return data;
    }

    public async Task<List<EnrollmentBaseResult>> SelectEnrollmentsFilteredByStudentsLastNameAsync(string lastNameSearchText)
    {
        var collection = context.GetCollection<MongoEnrollment>("enrollments");
        var data = await collection
            .Find(
                Builders<MongoEnrollment>.Filter.Regex(
                    x => x.Student.LastName, 
                    new BsonRegularExpression(lastNameSearchText, "i")
                )
            )
            .Project(GetEnrollmentProjection())
            .ToListAsync();
        return data;
    }

    public Task SelectEnrollmentsWithManyFiltersAsync(bool isActive, DateTime dateFrom, DateTime dateTo, int valueFrom,
        int valueTo, string lastNameSearchText)
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
    
    private static ProjectionDefinition<MongoEnrollment, EnrollmentBaseResult> GetEnrollmentProjection()
    {
        return Builders<MongoEnrollment>.Projection.Expression(enrollment => new EnrollmentBaseResult
        {
            EnrollmentId = enrollment.Id,
            StudentFirstName = enrollment.Student.FirstName,
            StudentLastName = enrollment.Student.LastName,
        });
    }
}
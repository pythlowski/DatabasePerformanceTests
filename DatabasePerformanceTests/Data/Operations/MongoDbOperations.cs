using DatabasePerformanceTests.Data.Contexts;
using DatabasePerformanceTests.Data.Models;
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

    public async Task DeleteEnrollmentsAsync(int count)
    {
        var collection = context.GetCollection<MongoEnrollment>("enrollments");
        await collection.DeleteManyAsync(Builders<MongoEnrollment>.Filter.Lte(x => x.Id, count));
    }

    public async Task UpdateEnrollmentDatesAsync(int count)
    {
        var collection = context.GetCollection<MongoEnrollment>("enrollments");
        await collection.UpdateManyAsync(
            Builders<MongoEnrollment>.Filter.Lte(x => x.Id, count),
            Builders<MongoEnrollment>.Update.Set(x => x.EnrollmentDate, DateTime.UtcNow)
        );
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

    public async Task<StudentDetailsResult> SelectStudentByIdAsync(int id)
    {
        var collection = context.GetCollection<MongoStudent>("students");
        var mongoStudent = await collection
            .Find(Builders<MongoStudent>.Filter.Eq(x => x.Id, $"student-{id}"))
            .FirstOrDefaultAsync();
        return StudentDetailsResult.FromMongo(mongoStudent);
    }

    public async Task<CourseInstanceBaseResult> SelectCourseInstancesByStudentIdAsync(int studentId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<EnrollmentBaseResult>> SelectEnrollmentsOrderedByIdAsync(int limit)
    {
        var collection = context.GetCollection<MongoEnrollment>("enrollments");
        var data = await collection
            .Find(Builders<MongoEnrollment>.Filter.Empty)
            .Sort(Builders<MongoEnrollment>.Sort.Ascending(x => x.Id))
            .Limit(limit)
            .Project(GetEnrollmentBaseProjection())
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
            .Project(GetEnrollmentBaseProjection())
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
            .Project(GetEnrollmentBaseProjection())
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
            .Project(GetEnrollmentBaseProjection())
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
            .Project(GetEnrollmentBaseProjection())
            .ToListAsync();
        return data;
    }

    public async Task<List<EnrollmentBaseResult>> SelectEnrollmentsWithManyFiltersAsync(bool isActive, DateTime dateFrom, DateTime dateTo, int valueFrom,
        int valueTo, string lastNameSearchText)
    {
        var collection = context.GetCollection<MongoEnrollment>("enrollments");
        var data = await collection
            .Find(
                Builders<MongoEnrollment>.Filter.And(
                    Builders<MongoEnrollment>.Filter.Eq(x => x.Student.IsActive, true),
                    Builders<MongoEnrollment>.Filter.Gte(x => x.EnrollmentDate, dateFrom),
                    Builders<MongoEnrollment>.Filter.Lte(x => x.EnrollmentDate, dateTo),
                    Builders<MongoEnrollment>.Filter.Gte(x => x.Course.Budget, valueFrom),
                    Builders<MongoEnrollment>.Filter.Lte(x => x.Course.Budget, valueTo),
                    Builders<MongoEnrollment>.Filter.Regex(
                        x => x.Student.LastName, 
                        new BsonRegularExpression(lastNameSearchText, "i")
                    )
                )
            )
            .Project(GetEnrollmentBaseProjection())
            .ToListAsync();
        return data;
    }

    public async Task<List<EnrollmentBaseResult>> SelectEnrollmentsWithPaginationAsync(int pageSize, int pageNumber)
    {
        var collection = context.GetCollection<MongoEnrollment>("enrollments");
        var data = await collection
            .Find(Builders<MongoEnrollment>.Filter.Empty)
            .Sort(Builders<MongoEnrollment>.Sort.Ascending(x => x.Id))
            .Skip(pageSize * (pageNumber - 1))
            .Limit(pageSize)
            .Project(GetEnrollmentBaseProjection())
            .ToListAsync();
        return data;
    }

    public async Task<List<EnrollmentBaseResult>> SelectEnrollmentsWithManySortParametersAsync(int limit)
    {
        var collection = context.GetCollection<MongoEnrollment>("enrollments");
        var data = await collection
            .Find(Builders<MongoEnrollment>.Filter.Empty)
            .Sort(Builders<MongoEnrollment>.Sort
                .Ascending(x => x.Student.LastName)
                .Descending(x => x.EnrollmentDate)
                .Descending(x => x.Course.Budget)
                .Descending(x => x.Student.IsActive)
            )
            .Limit(limit)
            .Project(GetEnrollmentBaseProjection())
            .ToListAsync();
        return data;
    }


    private static ProjectionDefinition<MongoEnrollment, EnrollmentBaseResult> GetEnrollmentBaseProjection()
    {
        return Builders<MongoEnrollment>.Projection.Expression(enrollment => new EnrollmentBaseResult
        {
            EnrollmentId = enrollment.Id,
            StudentFirstName = enrollment.Student.FirstName,
            StudentLastName = enrollment.Student.LastName,
        });
    }
}
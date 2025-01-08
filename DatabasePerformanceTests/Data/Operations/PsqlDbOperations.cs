using DatabasePerformanceTests.Data.Contexts;
using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Data.Models.Results;

namespace DatabasePerformanceTests.Data.Operations;

public class PsqlDbOperations(PsqlDbContext context) : IDbOperations
{
    public Task InsertEnrollmentsAsync(IEnumerable<Enrollment> enrollments)
    {
        throw new NotImplementedException();
    }

    public Task DeleteEnrollmentsAsync(int count)
    {
        return Task.CompletedTask;
    }

    public Task UpdateEnrollmentDatesAsync(int count)
    {
        throw new NotImplementedException();
    }

    public async Task<List<StudentBase>> SelectStudentsOrderedByIdAsync(int limit)
    {
        string query = $"SELECT \"StudentId\", \"FirstName\", \"LastName\" FROM \"Students\" ORDER BY \"StudentId\" LIMIT {limit}";
        var data = await context.ExecuteReaderAsync(query:query, useCurrentTransaction:true);
        return data.Select(row => new StudentBase
        {
            Id = (int)row["StudentId"],
            FirstName = (string)row["FirstName"],
            LastName = (string)row["LastName"]
        }).ToList();
    }

    public async Task<List<EnrollmentResult>> SelectEnrollmentsOrderedByIdAsync(int limit)
    {
        string query = @"
            SELECT 
                e.""EnrollmentId"", 
                s.""FirstName"" AS ""StudentFirstName"", 
                s.""LastName"" AS ""StudentLastName"", 
                c.""CourseName"", 
                s.""IsActive"", 
                e.""EnrollmentDate"", 
                ci.""Budget""
            FROM ""Enrollments"" e
            JOIN ""Students"" s ON e.""StudentId"" = s.""StudentId""
            JOIN ""CourseInstances"" ci ON e.""CourseInstanceId"" = ci.""CourseInstanceId""
            JOIN ""Courses"" c ON ci.""CourseId"" = c.""CourseId""
            JOIN ""Instructors"" i ON ci.""InstructorId"" = i.""InstructorId""
            ORDER BY e.""EnrollmentId"";";
        
        var data = await context.ExecuteReaderAsync(query:query, useCurrentTransaction:true);
        return data.Select(row => new EnrollmentResult
        {
            EnrollmentId = (int)row["EnrollmentId"],
            StudentFirstName = (string)row["StudentFirstName"],
            StudentLastName = (string)row["StudentLastName"],
            CourseName = (string)row["CourseName"],
            IsActive = (bool)row["IsActive"],
            EnrollmentDate = (DateTime)row["EnrollmentDate"],
            Budget = (long)row["Budget"]
        }).ToList();
    }

    public Task SelectEnrollmentsFilteredByIsActiveAsync(bool isActive)
    {
        throw new NotImplementedException();
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
using DatabasePerformanceTests.Data.Contexts;
using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Data.Models.Results;

namespace DatabasePerformanceTests.Data.Operations;

public class PsqlDbOperations(PsqlDbContext context) : IDbOperations
{
    private string GetEnrollmentsQuery(List<string>? whereConditions, List<string>? orderByClauses, int? limit = null)
    {
        string limitQuery = limit is not null ? "LIMIT " + limit : "";
        string whereQuery = whereConditions != null && whereConditions.Any()
            ? "WHERE " + string.Join(" AND ", whereConditions)
            : "";
        string orderByQuery = orderByClauses != null && orderByClauses.Any()
            ? "ORDER BY " + string.Join(", ", orderByClauses)
            : "";
        
        return @$"
        SELECT 
	        e.""EnrollmentId"", 
            s.""FirstName"" AS ""StudentFirstName"", 
            s.""LastName"" AS ""StudentLastName""
        FROM ""Enrollments"" e
        JOIN ""Students"" s ON e.""StudentId"" = s.""StudentId""
        JOIN ""CourseInstances"" ci ON e.""CourseInstanceId"" = ci.""CourseInstanceId""
        JOIN ""Courses"" c ON ci.""CourseId"" = c.""CourseId""
        JOIN ""Instructors"" i ON ci.""InstructorId"" = i.""InstructorId""
        {whereQuery}
        {orderByQuery}
        {limitQuery}
        ";  
    }

    private async Task<List<EnrollmentResult>> ExecuteEnrollmentsQueryAsync(string query)
    {
        var data = await context.ExecuteReaderAsync(query:query, useCurrentTransaction:true);
        return data.Select(row => new EnrollmentResult
        {
            EnrollmentId = (int)row["EnrollmentId"],
            StudentFirstName = (string)row["StudentFirstName"],
            StudentLastName = (string)row["StudentLastName"]
        }).ToList();
    }
    
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
        string query = GetEnrollmentsQuery(
            whereConditions:null, 
            orderByClauses:new List<string>{ "e.\"EnrollmentId\"" }, 
            limit:limit);
        return await ExecuteEnrollmentsQueryAsync(query);
    }

    public async Task<List<EnrollmentResult>> SelectEnrollmentsFilteredByIsActiveAsync(bool isActive)
    {
        string isActiveValue = isActive ? "True" : "False";
        string query = GetEnrollmentsQuery(
            whereConditions:new List<string>{ $"s.\"IsActive\" = {isActiveValue}" }, 
            orderByClauses:null, 
            limit:null);
        return await ExecuteEnrollmentsQueryAsync(query);
    }

    public async Task<List<EnrollmentResult>> SelectEnrollmentsFilteredByEnrollmentDateAsync(DateTime dateFrom, DateTime dateTo)
    {
        string query = GetEnrollmentsQuery(
            whereConditions:new List<string>
            {
                $"e.\"EnrollmentDate\" BETWEEN '{dateFrom:yyyy-MM-dd}' AND '{dateTo:yyyy-MM-dd}'",
            }, 
            orderByClauses:null, 
            limit:null);
        return await ExecuteEnrollmentsQueryAsync(query);    
    }

    public async Task<List<EnrollmentResult>> SelectEnrollmentsFilteredByBudgetAsync(int valueFrom, int valueTo)
    {
        string query = GetEnrollmentsQuery(
            whereConditions:new List<string>
            {
                $"ci.\"Budget\" BETWEEN {valueFrom} AND {valueTo}",
            }, 
            orderByClauses:null, 
            limit:null);
        return await ExecuteEnrollmentsQueryAsync(query);
    }

    public async Task<List<EnrollmentResult>> SelectEnrollmentsFilteredByStudentsLastNameAsync(string lastNameSearchText)
    {
        string query = GetEnrollmentsQuery(
            whereConditions:new List<string>
            {
                $"s.\"LastName\" LIKE '%{lastNameSearchText}%'",
            }, 
            orderByClauses:null, 
            limit:null);
        return await ExecuteEnrollmentsQueryAsync(query);    
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
}
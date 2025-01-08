using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Data.Models.Results;

namespace DatabasePerformanceTests.Data.Operations;

public interface IDbOperations
{
    Task InsertEnrollmentsAsync(IEnumerable<Enrollment> enrollments);
    Task DeleteEnrollmentsAsync(int count);
    Task UpdateEnrollmentDatesAsync(int count);
    Task<List<StudentBase>> SelectStudentsOrderedByIdAsync(int limit);
    Task<List<EnrollmentResult>> SelectEnrollmentsOrderedByIdAsync(int limit);
    Task SelectEnrollmentsFilteredByIsActiveAsync(bool isActive);
    Task SelectEnrollmentsFilteredByEnrollmentDateAsync(DateTime dateFrom, DateTime dateTo);
    Task SelectEnrollmentsFilteredByBudgetAsync(long valueFrom, long valueTo);
    Task SelectEnrollmentsFilteredByStudentsLastNameAsync(string lastNameSearchText);
    Task SelectEnrollmentsWithManyFiltersAsync(bool isActive, DateTime dateFrom, DateTime dateTo, long valueFrom, long valueTo, string lastNameSearchText);
    Task SelectEnrollmentsWithPaginationAsync(int pageSize, int pageNumber);
    Task SelectEnrollmentsWithManySortParametersAsync(int limit);
}
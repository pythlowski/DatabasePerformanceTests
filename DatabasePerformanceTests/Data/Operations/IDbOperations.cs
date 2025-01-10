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
    Task<List<EnrollmentResult>> SelectEnrollmentsFilteredByIsActiveAsync(bool isActive);
    // Task<List<EnrollmentResult>> SelectEnrollmentsFilteredByEnrollmentDateAsync(DateTime dateFrom, DateTime dateTo);
    // Task<List<EnrollmentResult>> SelectEnrollmentsFilteredByBudgetAsync(long valueFrom, long valueTo);
    // Task<List<EnrollmentResult>> SelectEnrollmentsFilteredByStudentsLastNameAsync(string lastNameSearchText);
    // Task<List<EnrollmentResult>> SelectEnrollmentsWithManyFiltersAsync(bool isActive, DateTime dateFrom, DateTime dateTo, long valueFrom, long valueTo, string lastNameSearchText);
    // Task<List<EnrollmentResult>> SelectEnrollmentsWithPaginationAsync(int pageSize, int pageNumber);
    // Task<List<EnrollmentResult>> SelectEnrollmentsWithManySortParametersAsync(int limit);
}
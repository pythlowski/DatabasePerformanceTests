using DatabasePerformanceTests.Data.Models;
using DatabasePerformanceTests.Data.Models.Results;

namespace DatabasePerformanceTests.Data.Operations.Interfaces;

public interface IEnrollmentsOperations
{
    Task BulkInsertAsync(List<IEnrollment> data);
    Task DeleteEnrollmentsAsync(int count);
    Task UpdateEnrollmentDatesAsync(int count);
    Task<List<EnrollmentBaseResult>> SelectEnrollmentsOrderedByIdAsync(int limit);
    Task<List<EnrollmentBaseResult>> SelectEnrollmentsFilteredByIsActiveAsync(bool isActive);
    Task<List<EnrollmentBaseResult>> SelectEnrollmentsFilteredByEnrollmentDateAsync(DateTime dateFrom, DateTime dateTo);
    Task<List<EnrollmentBaseResult>> SelectEnrollmentsFilteredByBudgetAsync(int valueFrom, int valueTo);
    Task<List<EnrollmentBaseResult>> SelectEnrollmentsFilteredByStudentsLastNameAsync(string lastNameSearchText);
    Task<List<EnrollmentBaseResult>> SelectEnrollmentsWithManyFiltersAsync(bool isActive, DateTime dateFrom, DateTime dateTo, int valueFrom, int valueTo, string lastNameSearchText);
    Task<List<EnrollmentBaseResult>> SelectEnrollmentsWithPaginationAsync(int pageSize, int pageNumber);
    Task<List<EnrollmentBaseResult>> SelectEnrollmentsWithManySortParametersAsync(int limit);
}
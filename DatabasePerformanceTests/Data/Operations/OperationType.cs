namespace DatabasePerformanceTests.Data.Operations;

public enum OperationType
{
    InsertEnrollments,
    DeleteEnrollments,
    UpdateEnrollments,
    SelectStudentsOrderedById,
    SelectEnrollmentsOrderedById,
    SelectEnrollmentsFilteredByIsActive,
    SelectEnrollmentsFilteredByEnrollmentDate,
    SelectEnrollmentsFilteredByBudget,
    SelectEnrollmentsFilteredByStudentsLastName,
    SelectEnrollmentsWithManyFilters,
    SelectEnrollmentsWithPagination,
    SelectEnrollmentsWithManySortParameters
}
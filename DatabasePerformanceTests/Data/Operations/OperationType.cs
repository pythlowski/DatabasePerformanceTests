namespace DatabasePerformanceTests.Data.Operations;

public enum OperationType
{
    BulkInsertEnrollments,
    InsertEnrollmentsOnEmptyTable,
    DeleteEnrollments,
    DeleteEnrollmentsOnEmptyTable,
    UpdateEnrollments,
    UpdateEnrollmentsOnEmptyTable,
    SelectStudentById,
    SelectEnrollmentsByStudentId,
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
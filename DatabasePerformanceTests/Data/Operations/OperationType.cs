namespace DatabasePerformanceTests.Data.Operations;

public enum OperationType
{
    BulkInsertEnrollments,
    BulkInsertEnrollmentsOnEmptyTable,
    DeleteEnrollments,
    UpdateEnrollments,
    TruncateEnrollments,
    SelectStudentById,
        SelectCourseInstancesByStudentId,
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
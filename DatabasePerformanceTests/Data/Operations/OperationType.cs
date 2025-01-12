namespace DatabasePerformanceTests.Data.Operations;

public enum OperationType
{
    BulkInsertEnrollments,
        InsertEnrollmentsOnEmptyTable,
    DeleteEnrollments,
        DeleteEnrollmentsOnEmptyTable,
    UpdateEnrollments,
        UpdateEnrollmentsOnEmptyTable,
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
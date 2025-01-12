namespace DatabasePerformanceTests.Data.Operations;

public enum OperationType
{
    BulkInsertEnrollments,
    DeleteEnrollments,
    UpdateEnrollments,
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
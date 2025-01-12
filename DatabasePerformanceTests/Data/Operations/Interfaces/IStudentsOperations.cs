using DatabasePerformanceTests.Data.Models.Results;

namespace DatabasePerformanceTests.Data.Operations.Interfaces;

public interface IStudentsOperations
{
    Task<List<StudentBaseResult>> SelectStudentsOrderedByIdAsync(int limit);
    Task<StudentDetailsResult> SelectStudentByIdAsync(int id);
    Task<List<CourseInstanceBaseResult>> SelectCourseInstancesByStudentIdAsync(int studentId);
}
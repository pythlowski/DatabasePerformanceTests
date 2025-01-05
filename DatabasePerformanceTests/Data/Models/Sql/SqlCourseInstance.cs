namespace DatabasePerformanceTests.Data.Models.Sql;

public class SqlCourseInstance
{
    public int CourseInstanceId { get; set; }
    public int CourseId { get; set; }
    public int InstructorId { get; set; }
    public int AcademicYear { get; set; }
    public int Budget { get; set; }
}
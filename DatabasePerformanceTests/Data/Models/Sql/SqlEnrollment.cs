namespace DatabasePerformanceTests.Data.Models.Sql;

public class SqlEnrollment
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseInstanceId { get; set; }
    public float Grade { get; set; }
}
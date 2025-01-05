namespace DatabasePerformanceTests.Data.Models.Domain;

public class Enrollment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseInstanceId { get; set; }
    public float Grade { get; set; }
}
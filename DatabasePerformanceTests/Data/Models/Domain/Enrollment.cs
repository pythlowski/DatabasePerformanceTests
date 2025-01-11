namespace DatabasePerformanceTests.Data.Models.Domain;

public class Enrollment : IEnrollment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseInstanceId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public float Grade { get; set; }
}
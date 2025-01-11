namespace DatabasePerformanceTests.Data.Models.Results;

public class CourseInstanceBaseResult
{
    public int CourseInstanceId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public int InstructorId { get; set; }
    public string InstructorLastName { get; set; }
}
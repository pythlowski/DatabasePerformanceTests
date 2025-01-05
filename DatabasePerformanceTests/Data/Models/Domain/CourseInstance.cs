namespace DatabasePerformanceTests.Data.Models.Domain;

public class CourseInstance
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int InstructorId { get; set; }
    public int AcademicYear { get; set; }
    public long Budget { get; set; }
}
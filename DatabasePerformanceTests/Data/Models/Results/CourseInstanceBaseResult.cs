using DatabasePerformanceTests.Data.Models.Mongo;

namespace DatabasePerformanceTests.Data.Models.Results;

public class CourseInstanceBaseResult
{
    public CourseInstanceBaseResult(int courseInstanceId, int courseId, string courseName, int instructorId, string instructorLastName)
    {
        CourseInstanceId = courseInstanceId;
        CourseId = courseId;
        CourseName = courseName;
        InstructorId = instructorId;
        InstructorLastName = instructorLastName;
    }

    public int CourseInstanceId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public int InstructorId { get; set; }
    public string InstructorLastName { get; set; }

}
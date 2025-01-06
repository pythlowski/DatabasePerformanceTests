using DatabasePerformanceTests.Data.Models.Domain;

namespace DatabasePerformanceTests.Utils.Generators.Models;

public class GeneratedData
{
    public List<Student> Students { get; set; }
    public List<Instructor> Instructors { get; set; }
    public List<Course> Courses { get; set; }
    public List<CourseInstance> CourseInstances { get; set; }
    public List<Enrollment> Enrollments { get; set; }
    public Dictionary<int, List<(Student, Enrollment)>> CourseInstanceStudentMap { get; set; }
}
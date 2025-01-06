using DatabasePerformanceTests.Utils.Generators.Models;

namespace DatabasePerformanceTests.Utils.Generators;

public class DataGenerator(DataGeneratorConfig config)
{
    public GeneratedData Generate()
    {
        var students = new StudentsGenerator().Generate(config.StudentsCount);
        var instructors = new InstructorsGenerator().Generate(config.InstructorsCount);
        var courses = new CoursesGenerator().Generate(config.CoursesCount);
        var courseInstances = new CourseInstancesGenerator().Generate(config.CoursesCount, config.InstructorsCount, config.CourseInstancesPerCourse);
        var (enrollments, courseInstanceStudentMap) = new EnrollmentsGenerator().Generate(students, config.CoursesCount * config.CourseInstancesPerCourse, config.EnrollmentsPerStudent);
        
        return new GeneratedData()
        {
            Students = students,
            Instructors = instructors,
            Courses = courses,
            CourseInstances = courseInstances,
            Enrollments = enrollments,
            CourseInstanceStudentMap = courseInstanceStudentMap
        };
    }
}
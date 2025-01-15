using DatabasePerformanceTests.Data.Models.Mongo;
using DatabasePerformanceTests.Utils.Config;
using DatabasePerformanceTests.Utils.Generators.Models;

namespace DatabasePerformanceTests.Utils.Generators;

public class DataGenerator(DataGeneratorConfig config)
{
    public GeneratedData Generate()
    {
        var students = StudentsGenerator.Generate(config.StudentsCount);
        var instructors = InstructorsGenerator.Generate(config.InstructorsCount);
        var courses = CoursesGenerator.Generate(config.CoursesCount);
        var courseInstances = CourseInstancesGenerator.Generate(config);
        var (enrollments, enrollmentIdToStudentMap, studentToEnrolledCourseInstanceIdsMap) 
            = EnrollmentsGenerator.Generate(students, 1, config.CoursesCount * config.CourseInstancesPerCourse, config.EnrollmentsPerStudent);
        
        return new GeneratedData
        {
            Students = students,
            Instructors = instructors,
            Courses = courses,
            CourseInstances = courseInstances,
            Enrollments = enrollments,
            EnrollmentIdToStudentMap = enrollmentIdToStudentMap,
            StudentToEnrolledCourseInstanceIdsMap = studentToEnrolledCourseInstanceIdsMap
        };
    }
    
    public static IEnumerable<MongoStudent> GetDenormalizedStudents(GeneratedData data)
    {
        foreach (var student in data.Students)
        {
            var enrolledCourseInstanceIds = data.StudentToEnrolledCourseInstanceIdsMap[student];
            var courseInstances = data.CourseInstances
                .Where(c => enrolledCourseInstanceIds.Contains(c.Id)).ToList();
            yield return MongoStudent.FromDomain(student, courseInstances, data.Courses, data.Instructors);
        }
    }

    public static IEnumerable<MongoEnrollment> GetDenormalizedEnrollments(GeneratedData data)
    {
        foreach (var enrollment in data.Enrollments)
        {
            var student = data.EnrollmentIdToStudentMap[enrollment.Id];
            var courseInstance = data.CourseInstances.First(c => c.Id == enrollment.CourseInstanceId);
            var course = data.Courses.First(c => c.Id == courseInstance.CourseId);
            yield return MongoEnrollment.FromDomain(enrollment, student, courseInstance, course);
        }
    }
}
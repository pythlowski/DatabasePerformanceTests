using Bogus;
using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Data.Models.Mongo;
using DatabasePerformanceTests.Utils.Config;
using DatabasePerformanceTests.Utils.Generators.Models;

namespace DatabasePerformanceTests.Utils.Generators;

public class DummyEnrollmentsGenerator
{
    public static List<Enrollment> GenerateDomain(DataGeneratorConfig config, int count)
    {
        var grades = new List<float> { 2, 3, 3.5f, 4, 4.5f, 5 };
        
        var faker = new Faker<Enrollment>()
            .RuleFor(e => e.Id, f => f.IndexFaker + config.StudentsCount * config.EnrollmentsPerStudent * 2)
            .RuleFor(e => e.StudentId, f => f.Random.Number(1, config.StudentsCount))
            .RuleFor(e => e.CourseInstanceId, f => f.Random.Number(1, config.CoursesCount * config.CourseInstancesPerCourse))
            .RuleFor(e => e.EnrollmentDate, f => f.Date.Past(10))
            .RuleFor(e => e.Grade, f => f.PickRandom(grades));

        return faker.Generate(count);
    }
    
    public static List<MongoEnrollment> GenerateMongo(DataGeneratorConfig config, int count)
    {
        var dummyStudent = StudentsGenerator.Generate(1).First();
        var dummyCourseInstance = CourseInstancesGenerator.Generate(config, 1).First();
        var dummyCourse = CoursesGenerator.Generate(1).First();
        return GenerateDomain(config, count).Select(e => MongoEnrollment.FromDomain(e, dummyStudent, dummyCourseInstance, dummyCourse)).ToList();
    }
}
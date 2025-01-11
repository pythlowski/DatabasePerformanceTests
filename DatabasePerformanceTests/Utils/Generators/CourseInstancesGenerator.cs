using Bogus;
using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Utils.Generators.Models;

namespace DatabasePerformanceTests.Utils.Generators;

public class CourseInstancesGenerator
{
    public static List<CourseInstance> Generate(DataGeneratorConfig config, int? count = null)
    {
        var faker = new Faker<CourseInstance>()
            .RuleFor(ci => ci.Id, f => f.IndexFaker + 1)
            .RuleFor(ci => ci.CourseId, f => f.Random.Number(1, config.CoursesCount))
            .RuleFor(ci => ci.InstructorId, f => f.Random.Number(1, config.InstructorsCount))
            .RuleFor(ci => ci.AcademicYear, f => f.Date.Past(config.CourseInstancesPerCourse).Year)
            .RuleFor(ci => ci.Budget, f => f.Random.Number(0, 100_000_000));

        return faker.Generate(count ?? config.CoursesCount * config.CourseInstancesPerCourse);
    }
}
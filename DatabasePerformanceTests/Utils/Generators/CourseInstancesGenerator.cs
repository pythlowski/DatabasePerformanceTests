using Bogus;
using DatabasePerformanceTests.Data.Models.Domain;

namespace DatabasePerformanceTests.Utils.Generators;

public class CourseInstancesGenerator
{
    public List<CourseInstance> Generate(int coursesCount, int instructorsCount, int instancesPerCourse)
    {
        var faker = new Faker<CourseInstance>()
            .RuleFor(ci => ci.Id, f => f.IndexFaker + 1)
            .RuleFor(ci => ci.CourseId, f => f.Random.Number(1, coursesCount))
            .RuleFor(ci => ci.InstructorId, f => f.Random.Number(1, instructorsCount))
            .RuleFor(ci => ci.AcademicYear, f => f.Date.Past(instancesPerCourse).Year)
            .RuleFor(ci => ci.Budget, f => f.Random.Number(0, 100_000_000));

        return faker.Generate(coursesCount * instancesPerCourse);
    }

}
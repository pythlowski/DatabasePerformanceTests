using Bogus;
using DatabasePerformanceTests.Data.Models.Domain;

namespace DatabasePerformanceTests.Utils.Generators;

public class EnrollmentsGenerator
{
    public List<Enrollment> Generate(int studentsCount, int courseInstancesCount, int enrollmentsPerStudent)
    {
        var grades = new List<float> { 2, 3, 3.5f, 4, 4.5f, 5 };
        
        var faker = new Faker<Enrollment>()
            .RuleFor(e => e.Id, f => f.IndexFaker + 1)
            .RuleFor(e => e.StudentId, f => f.Random.Number(1, studentsCount))
            .RuleFor(e => e.CourseInstanceId, f => f.Random.Number(1, courseInstancesCount))
            .RuleFor(e => e.Grade, f => f.PickRandom(grades));

        return faker.Generate(studentsCount * enrollmentsPerStudent);
    }
}
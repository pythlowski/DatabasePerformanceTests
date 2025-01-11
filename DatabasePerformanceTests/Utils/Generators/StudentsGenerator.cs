using Bogus;
using DatabasePerformanceTests.Data.Models.Domain;

namespace DatabasePerformanceTests.Utils.Generators;

public class StudentsGenerator
{
    public static List<Student> Generate(int count)
    {
        var faker = new Faker<Student>()
            .RuleFor(s => s.Id, f => f.IndexFaker + 1)
            .RuleFor(s => s.FirstName, f => f.Name.FirstName())
            .RuleFor(s => s.LastName, f => f.Name.LastName())
            .RuleFor(s => s.BirthDate, f => f.Date.Between(DateTime.Now.AddYears(-30), DateTime.Now).AddYears(-18))
            .RuleFor(s => s.AdmissionYear, f => f.Date.Past(10).Year)
            .RuleFor(s => s.IsActive, f => f.Random.Bool());

        return faker.Generate(count);
    }
}
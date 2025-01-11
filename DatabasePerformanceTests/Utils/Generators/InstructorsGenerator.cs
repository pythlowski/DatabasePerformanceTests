using Bogus;
using DatabasePerformanceTests.Data.Models.Domain;

namespace DatabasePerformanceTests.Utils.Generators;

public class InstructorsGenerator
{
    public static List<Instructor> Generate(int count)
    {
        var faker = new Faker<Instructor>()
            .RuleFor(i => i.Id, f => f.IndexFaker + 1)
            .RuleFor(i => i.FirstName, f => f.Name.FirstName())
            .RuleFor(i => i.LastName, f => f.Name.LastName());

        return faker.Generate(count);
    }
}
using Bogus;
using DatabasePerformanceTests.Data.Models.Domain;

namespace DatabasePerformanceTests.Utils.Generators;

public class CoursesGenerator
{
    public static List<Course> Generate(int count)
    {
        var descriptors = new[]
        {
            "Introduction to", "Advanced", "Fundamentals of", "Principles of", "Applications of",
            "Essentials of", "Basics of", "Foundations of", "Overview of", "History of",
            "Modern", "Theoretical", "Experimental", "Special Topics in", "Workshop on",
            "Seminar on", "Techniques in", "Methods in", "Perspectives on", "Exploration of"
        };
        
        var subjects = new[]
        {
            "Mathematics", "Physics", "Computer Science", "Biology", "Chemistry",
            "Psychology", "Sociology", "Economics", "Philosophy", "Statistics",
            "Political Science", "Art History", "Environmental Science", "Anthropology", "Literature",
            "Engineering", "Astronomy", "Geography", "History", "Linguistics",
            "Cognitive Science", "Data Science", "Machine Learning", "Artificial Intelligence", "Robotics",
            "Cybersecurity", "Game Design", "Business Administration", "Finance", "Marketing",
            "Education", "Architecture", "Law", "Medicine", "Nursing",
            "Health Sciences", "Public Policy", "Criminology", "Theology", "Ethics",
            "Genetics", "Neuroscience", "Pharmacology", "Urban Planning", "Design Thinking",
            "Creative Writing", "Music Theory", "Visual Arts", "Drama", "Film Studies"
        };
        
        var semester = new[] { "I", "II", "III", "IV", "V" };

        var faker = new Faker<Course>()
            .RuleFor(c => c.Id, f => f.IndexFaker + 1)
            .RuleFor(c => c.Name, f => $"{f.PickRandom(descriptors)} {f.PickRandom(subjects)} {f.PickRandom(semester)}");

        return faker.Generate(count);
    }
}
using Bogus;
using DatabasePerformanceTests.Data.Models.Domain;

namespace DatabasePerformanceTests.Utils.Generators;

public class EnrollmentsGenerator
{
    public (List<Enrollment> Enrollments, Dictionary<int, Student> CourseInstanceStudentMap) Generate(List<Student> students, int courseInstancesCount, int enrollmentsPerStudent)
    {
        var grades = new List<float> { 2, 3, 3.5f, 4, 4.5f, 5 };
        var enrollments = new List<Enrollment>();
        var enrollmentIdToStudentMap = new Dictionary<int, Student>();
        Random random = new Random();
        
        var faker = new Faker<Enrollment>()
            .RuleFor(e => e.Id, f => f.IndexFaker + 1)
            .RuleFor(e => e.CourseInstanceId, f => f.Random.Number(1, courseInstancesCount))
            .RuleFor(e => e.EnrollmentDate, f => f.Date.Past(10))
            .RuleFor(e => e.Grade, f => f.PickRandom(grades));

        for (int i = 0; i < students.Count * enrollmentsPerStudent; i++)
        {
            var randomStudent = students[random.Next(students.Count)];
            var enrollment = faker.Generate();
            enrollment.StudentId = randomStudent.Id;
            enrollments.Add(enrollment);

            enrollmentIdToStudentMap[enrollment.Id] = randomStudent;
        }

        return (enrollments, enrollmentIdToStudentMap);
    }
}
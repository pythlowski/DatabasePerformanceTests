using Bogus;
using DatabasePerformanceTests.Data.Models.Domain;

namespace DatabasePerformanceTests.Utils.Generators;

public class EnrollmentsGenerator
{
    public (List<Enrollment> Enrollments, Dictionary<int, List<(Student, Enrollment)>> CourseInstanceStudentMap) Generate(List<Student> students, int courseInstancesCount, int enrollmentsPerStudent)
    {
        var grades = new List<float> { 2, 3, 3.5f, 4, 4.5f, 5 };
        var enrollments = new List<Enrollment>();
        var courseInstanceStudentMap = new Dictionary<int, List<(Student, Enrollment)>>();
        Random random = new Random();
        
        for (int i = 1; i <= students.Count; i++)
        {
            courseInstanceStudentMap[i] = new List<(Student, Enrollment)>();
        }
        
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

            if (!courseInstanceStudentMap.ContainsKey(enrollment.CourseInstanceId))
            {
                courseInstanceStudentMap[enrollment.CourseInstanceId] = new List<(Student, Enrollment)>();
            }
            courseInstanceStudentMap[enrollment.CourseInstanceId].Add((randomStudent, enrollment));
        }

        return (enrollments, courseInstanceStudentMap);
    }
}
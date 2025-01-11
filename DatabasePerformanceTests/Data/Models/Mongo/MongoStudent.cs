using DatabasePerformanceTests.Data.Models.Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabasePerformanceTests.Data.Models.Mongo;

public class MongoStudent
{
    [BsonId]
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public int AdmissionYear { get; set; }
    public bool IsActive { get; set; }
    public List<MongoStudentEnrolledCourses> EnrolledCourses { get; set; }

    public static MongoStudent FromDomain(Student student, List<CourseInstance> enrolledCourseInstances, List<Course> courses, List<Instructor> instructors)
    {
        return new MongoStudent()
        {
            Id = $"student-{student.Id}",
            FirstName = student.FirstName,
            LastName = student.LastName,
            BirthDate = student.BirthDate,
            AdmissionYear = student.AdmissionYear,
            IsActive = student.IsActive,
            EnrolledCourses = enrolledCourseInstances.Select(ci => new MongoStudentEnrolledCourses
            {
                CourseInstanceId = ci.Id,
                CourseId = ci.CourseId,
                CourseName = courses.Where(c => c.Id == ci.CourseId).Select(c => c.Name).FirstOrDefault(),
                InstructorId = ci.InstructorId,
                InstructorLastName = instructors.Where(i => i.Id == ci.InstructorId).Select(i => i.LastName).FirstOrDefault()
            }).ToList()
        };
    }
}

public class MongoStudentEnrolledCourses
{
    public int CourseInstanceId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public int InstructorId { get; set; }
    public string InstructorLastName { get; set; }

}
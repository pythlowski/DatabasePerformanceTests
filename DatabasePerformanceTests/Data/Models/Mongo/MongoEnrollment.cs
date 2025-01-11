using DatabasePerformanceTests.Data.Models.Domain;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabasePerformanceTests.Data.Models.Mongo;

public class MongoEnrollment : IEnrollment
{
    [BsonId]
    public int Id { get; set; }
    public MongoEnrollmentStudent Student { get; set; }
    public MongoEnrollmentCourse Course { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public float Grade { get; set; }

    public static MongoEnrollment FromDomain(Enrollment enrollment, Student student, CourseInstance courseInstance, Course course)
    {
        return new MongoEnrollment
        {
            Id = enrollment.Id,
            Student = new MongoEnrollmentStudent
            {
                StudentId = $"student-{student.Id}",
                FirstName = student.FirstName,
                LastName = student.LastName,
                IsActive = student.IsActive
            },
            Course = new MongoEnrollmentCourse
            {
                CourseInstanceId = $"course-instance-{courseInstance.Id}",
                CourseId = $"course-{course.Id}",
                CourseName = course.Name,
                Budget = courseInstance.Budget
            },
            EnrollmentDate = enrollment.EnrollmentDate,
            Grade = enrollment.Grade
        };
    }
}

public class MongoEnrollmentStudent
{
    public string StudentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsActive { get; set; }
}

public class MongoEnrollmentCourse
{
    public string CourseInstanceId { get; set; }
    public string CourseId { get; set; }
    public string CourseName { get; set; }
    public long Budget { get; set; }
}
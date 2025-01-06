using DatabasePerformanceTests.Data.Models.Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabasePerformanceTests.Data.Models.Mongo;

public class MongoCourseInstance
{
    [BsonId] 
    public ObjectId Id { get; set; }
    public int AcademicYear { get; set; }
    public long Budget { get; set; }
    public MongoCourseInstanceCourse Course { get; set; }
    public string InstructorId { get; set; }
    public List<MongoCourseInstanceStudent> EnrolledStudents { get; set; }

    public static MongoCourseInstance FromDomain(CourseInstance courseInstance, Course course, Instructor instructor,
        List<(Student, Enrollment)> enrolledStudents)
    {
        return new MongoCourseInstance
        {
            Id = ObjectId.GenerateNewId(),
            AcademicYear = courseInstance.AcademicYear,
            Budget = courseInstance.Budget,
            Course = new MongoCourseInstanceCourse
            {
                CourseId = $"course-{course.Id}",
                Name = course.Name
            },
            InstructorId = $"instructor-{instructor.Id}",
            EnrolledStudents = enrolledStudents is null 
                ? new List<MongoCourseInstanceStudent>()
                : enrolledStudents.Select(data => new MongoCourseInstanceStudent
                {
                    StudentId = $"student-{data.Item1.Id}",
                    IsActive = data.Item1.IsActive,
                    LastName = data.Item1.LastName,
                    EnrollmentDate = data.Item2.EnrollmentDate,
                    EnrollmentId = data.Item2.Id
                }).ToList()
        };
    }
}

public class MongoCourseInstanceCourse
{
    public string CourseId { get; set; }
    public string Name { get; set; }
}

public class MongoCourseInstanceStudent
{
    public string StudentId { get; set; }
    public bool IsActive { get; set; }
    public string LastName { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public int EnrollmentId { get; set; }
}
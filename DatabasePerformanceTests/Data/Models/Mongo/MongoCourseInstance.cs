using DatabasePerformanceTests.Data.Models.Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabasePerformanceTests.Data.Models.Mongo;

public class MongoCourseInstance
{
    [BsonId]
    public string Id { get; set; }
    public MongoCourseInstanceCourse Course { get; set; }
    public MongoCourseInstanceInstructor Instructor { get; set; }
    public int AcademicYear { get; set; }
    public long Budget { get; set; }

    public static MongoCourseInstance FromDomain(CourseInstance courseInstance, Course course, Instructor instructor)
    {
        return new MongoCourseInstance
        {
            Id = $"course-instance-{courseInstance.Id}",
            Course = new MongoCourseInstanceCourse
            {
                CourseId = $"course-{course.Id}",
                Name = course.Name
            },
            Instructor = new MongoCourseInstanceInstructor
            {
                InstructorId = $"instructor-{instructor.Id}",
                FirstName = instructor.FirstName,
                LastName = instructor.LastName
            },
            AcademicYear = courseInstance.AcademicYear,
            Budget = courseInstance.Budget
        };
    }
}

public class MongoCourseInstanceCourse
{
    public string CourseId { get; set; }
    public string Name { get; set; }
}

public class MongoCourseInstanceInstructor
{
    public string InstructorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
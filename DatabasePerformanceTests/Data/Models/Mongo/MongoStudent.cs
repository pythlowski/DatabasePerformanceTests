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

    public static MongoStudent FromDomain(Student student)
    {
        return new MongoStudent()
        {
            Id = $"student-{student.Id}",
            FirstName = student.FirstName,
            LastName = student.LastName,
            BirthDate = student.BirthDate,
            AdmissionYear = student.AdmissionYear,
            IsActive = student.IsActive
        };
    }
}
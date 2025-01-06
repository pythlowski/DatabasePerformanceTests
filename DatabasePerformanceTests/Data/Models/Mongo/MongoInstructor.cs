using DatabasePerformanceTests.Data.Models.Domain;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabasePerformanceTests.Data.Models.Mongo;

public class MongoInstructor
{
    [BsonId]
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public static MongoInstructor FromDomain(Instructor instructor)
    {
        return new MongoInstructor()
        {
            Id = $"instructor-{instructor.Id}",            
            FirstName = instructor.FirstName,
            LastName = instructor.LastName
        };
    }
}
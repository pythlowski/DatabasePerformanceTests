using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Data.Models.Mongo;

namespace DatabasePerformanceTests.Data.Models.Results;

public class StudentBaseResult
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public static StudentBaseResult FromMongo(MongoStudent student)
    {
        return new StudentBaseResult
        {
            Id = int.Parse(student.Id.Split('-')[1]),
            FirstName = student.FirstName,
            LastName = student.LastName
        };
    }

    public static StudentBaseResult FromDomain(Student student)
    {
        return new StudentBaseResult
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName
        };
    }
}
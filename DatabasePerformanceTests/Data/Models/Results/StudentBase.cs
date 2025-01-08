using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Data.Models.Mongo;

namespace DatabasePerformanceTests.Data.Models.Results;

public class StudentBase
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public static StudentBase FromMongo(MongoStudent student)
    {
        return new StudentBase
        {
            Id = int.Parse(student.Id.Split('-')[1]),
            FirstName = student.FirstName,
            LastName = student.LastName
        };
    }

    public static StudentBase FromDomain(Student student)
    {
        return new StudentBase
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName
        };
    }
}
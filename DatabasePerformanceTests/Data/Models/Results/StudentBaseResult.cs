using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Data.Models.Mongo;

namespace DatabasePerformanceTests.Data.Models.Results;

public class StudentBaseResult
{
    public StudentBaseResult(){}
    public StudentBaseResult(string mongoId, string firstName, string lastName)
    {
        Id = int.Parse(mongoId.Split('-')[1]);
        FirstName = firstName;
        LastName = lastName;
    }
    public StudentBaseResult(int id, string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
    }

    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public static StudentBaseResult FromMongo(MongoStudent student)
    {
        return new StudentBaseResult(student.Id, student.FirstName, student.LastName);
    }

    public static StudentBaseResult FromDomain(Student student)
    {
        return new StudentBaseResult(student.Id, student.FirstName, student.LastName);
    }
}
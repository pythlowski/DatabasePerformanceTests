using DatabasePerformanceTests.Data.Models.Mongo;

namespace DatabasePerformanceTests.Data.Models.Results;

public class StudentDetailsResult : StudentBaseResult
{
    public StudentDetailsResult(){}
    public StudentDetailsResult(string mongoId, string firstName, string lastName, DateTime birthDate, int admissionYear, bool isActive) : base(mongoId, firstName, lastName)
    {
        BirthDate = birthDate;
        AdmissionYear = admissionYear;
        IsActive = isActive;
    }

    public StudentDetailsResult(int id, string firstName, string lastName, DateTime birthDate, int admissionYear, bool isActive) : base(id, firstName, lastName)
    {
        BirthDate = birthDate;
        AdmissionYear = admissionYear;
        IsActive = isActive;
    }

    public DateTime BirthDate { get; set; }
    public int AdmissionYear { get; set; }
    public bool IsActive { get; set; }
    
    public static StudentDetailsResult FromMongo(MongoStudent student)
    {
        if (student is null)
            return new StudentDetailsResult();
        
        return new StudentDetailsResult(
            student.Id,
            student.FirstName,
            student.LastName,
            student.BirthDate,
            student.AdmissionYear,
            student.IsActive
        );
    }
}
using DatabasePerformanceTests.Data.Models.Mongo;

namespace DatabasePerformanceTests.Data.Models.Results;

public class StudentDetailsResult : StudentBaseResult
{
    public DateTime BirthDate { get; set; }
    public int AdmissionYear { get; set; }
    public bool IsActive { get; set; }
    
    public static StudentDetailsResult FromMongo(MongoStudent student)
    {
        var baseStudent = (StudentDetailsResult)StudentBaseResult.FromMongo(student); // TODO make it out of constructors
        baseStudent.BirthDate = student.BirthDate;
        baseStudent.AdmissionYear = student.AdmissionYear;
        baseStudent.IsActive = student.IsActive;
        return baseStudent;
    }
}
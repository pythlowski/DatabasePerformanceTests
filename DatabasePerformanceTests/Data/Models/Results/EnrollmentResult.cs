using DatabasePerformanceTests.Data.Models.Mongo;

namespace DatabasePerformanceTests.Data.Models.Results;

public class EnrollmentResult
{
    public int EnrollmentId { get; set; }
    public string StudentFirstName { get; set; }
    public string StudentLastName { get; set; }

    public static EnrollmentResult FromMongo(MongoEnrollment enrollment)
    {
        return new EnrollmentResult
        {
            EnrollmentId = enrollment.Id,
            StudentFirstName = enrollment.Student.FirstName,
            StudentLastName = enrollment.Student.LastName
        };
    }
}
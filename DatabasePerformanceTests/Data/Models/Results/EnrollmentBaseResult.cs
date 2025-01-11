using DatabasePerformanceTests.Data.Models.Mongo;

namespace DatabasePerformanceTests.Data.Models.Results;

public class EnrollmentBaseResult
{
    public int EnrollmentId { get; set; }
    public string StudentFirstName { get; set; }
    public string StudentLastName { get; set; }

    public static EnrollmentBaseResult FromMongo(MongoEnrollment enrollment)
    {
        return new EnrollmentBaseResult
        {
            EnrollmentId = enrollment.Id,
            StudentFirstName = enrollment.Student.FirstName,
            StudentLastName = enrollment.Student.LastName
        };
    }
}
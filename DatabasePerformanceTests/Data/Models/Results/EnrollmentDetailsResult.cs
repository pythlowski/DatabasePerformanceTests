namespace DatabasePerformanceTests.Data.Models.Results;

public class EnrollmentDetailsResult : EnrollmentBaseResult
{
    public string CourseName { get; set; }
    public int CourseBudget { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public float Grade { get; set; }
}
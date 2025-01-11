namespace DatabasePerformanceTests.Data.Models.Results;

public class StudentDetailsResult : StudentBaseResult
{
    public DateTime BirthDate { get; set; }
    public int AdmissionYear { get; set; }
    public bool IsActive { get; set; }
}
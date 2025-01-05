namespace DatabasePerformanceTests.Data.Models.Domain;

public class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int AdmissionYear { get; set; }
    public bool IsActive { get; set; }
}
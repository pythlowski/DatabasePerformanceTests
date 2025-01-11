using DatabasePerformanceTests.Data.Models.Results;

namespace DatabasePerformanceTests.Data.Operations.Interfaces;

public interface IStudentsOperations
{
    Task<List<StudentBaseResult>> SelectStudentsOrderedByIdAsync(int limit);
}
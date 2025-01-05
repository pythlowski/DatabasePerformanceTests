namespace DatabasePerformanceTests.Utils.Generators.Models;

public class DataGeneratorConfig
{ 
     public int StudentsCount { get; set; }
     public int InstructorsCount { get; set; }
     public int CoursesCount { get; set; }
     public int CourseInstancesPerCourse { get; set; }
     public int EnrollmentsPerStudent { get; set; }
}
using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Utils.Generators.Models;
using Microsoft.Data.SqlClient;

namespace DatabasePerformanceTests.Data.Contexts;

public class MssqlDbContext : AbstractDbContext
{
    public MssqlDbContext(string connectionString)
        : base(connectionString)
    {
    }
    
    private SqlConnection GetAdminConnection(string databaseName)
    {
        return new SqlConnection(
            new SqlConnectionStringBuilder(ConnectionString) { InitialCatalog = databaseName }.ToString()
        );
    }

    public async Task ExecuteSqlNonQueryAsync(string databaseName, string query)
    {
        await using var adminConnection = GetAdminConnection(databaseName);
        await adminConnection.OpenAsync();

        using var cmd = adminConnection.CreateCommand();
        cmd.CommandText = query;
        await cmd.ExecuteNonQueryAsync();
    }

    
    public override async Task CreateDatabaseAsync()
    {
        await ExecuteSqlNonQueryAsync("master", $"CREATE DATABASE [{DatabaseName}]");
        Console.WriteLine($"MSSQL database '{DatabaseName}' created.");
    }
    
    public override async Task CreateTablesAsync()
    {
        await ExecuteSqlNonQueryAsync(DatabaseName, @"
        CREATE TABLE students (
            StudentId INT IDENTITY(1,1) PRIMARY KEY,
            FirstName NVARCHAR(100) NOT NULL,
            LastName NVARCHAR(100) NOT NULL,
            AdmissionYear INT NOT NULL,
            IsActive BIT NOT NULL DEFAULT 1
        );

        CREATE TABLE Courses (
            CourseId INT IDENTITY(1,1) PRIMARY KEY,
            CourseName NVARCHAR(200) NOT NULL
        );

        CREATE TABLE Instructors (
            InstructorId INT IDENTITY(1,1) PRIMARY KEY,
            FirstName NVARCHAR(100) NOT NULL,
            LastName NVARCHAR(100) NOT NULL
        );

        CREATE TABLE CourseInstances (
            CourseInstanceId INT IDENTITY(1,1) PRIMARY KEY,
            CourseId INT NOT NULL,
            InstructorId INT NOT NULL,
            AcademicYear INT NOT NULL,
            Budget INT
        );

        CREATE TABLE Enrollments (
            EnrollmentId INT IDENTITY(1,1) PRIMARY KEY,
            StudentId INT NOT NULL,
            CourseInstanceId INT NOT NULL,
            Grade DECIMAL(3, 2)
        );
    ");
        Console.WriteLine($"MSSQL tables created.");
    }

    public override Task PopulateDatabaseAsync(GeneratedData data)
    {
        throw new NotImplementedException();
    }

    public override Task RestoreDatabaseAsync(GeneratedData data)
    {
        throw new NotImplementedException();
    }

    public override async Task DropDatabaseAsync()
    {
        await using var adminConnection = GetAdminConnection("master");
        await adminConnection.OpenAsync();

        using var cmd = adminConnection.CreateCommand();
        cmd.CommandText = $"        ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{DatabaseName}]";
        await cmd.ExecuteNonQueryAsync();
        Console.WriteLine($"MSSQL database '{DatabaseName}' dropped.");
    }
}
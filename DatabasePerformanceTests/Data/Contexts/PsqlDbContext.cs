using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Utils.Generators.Models;
using Npgsql;

namespace DatabasePerformanceTests.Data.Contexts;

public class PsqlDbContext : AbstractDbContext
{
    public PsqlDbContext(string connectionString)
        : base(connectionString)
    {
    }
    
    private NpgsqlConnection GetAdminConnection(string databaseName)
    {
        return new NpgsqlConnection(
            new NpgsqlConnectionStringBuilder(ConnectionString) { Database = databaseName }.ToString()
        );
    }
    
    public override async Task CreateDatabaseAsync()
    {
        await ExecuteSqlNonQueryAsync("postgres", $"CREATE DATABASE \"{DatabaseName}\"");
        Console.WriteLine($"PostgreSQL database '{DatabaseName}' created.");

    }

    public async Task ExecuteSqlNonQueryAsync(string databaseName, string query)
    {
        try
        {
            await using var adminConnection = GetAdminConnection(databaseName);
            await adminConnection.OpenAsync();

            using var cmd = adminConnection.CreateCommand();
            cmd.CommandText = query;
            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public override async Task CreateTablesAsync()
    {
        await ExecuteSqlNonQueryAsync(DatabaseName, @"
            CREATE TABLE students (
                StudentId SERIAL PRIMARY KEY, 
                FirstName VARCHAR(100) NOT NULL,
                LastName VARCHAR(100) NOT NULL,
                AdmissionYear INT NOT NULL,
                IsActive BOOLEAN NOT NULL DEFAULT TRUE
            );

            CREATE TABLE Courses (
                CourseId SERIAL PRIMARY KEY,
                CourseName VARCHAR(200) NOT NULL
            );

            CREATE TABLE Instructors (
                InstructorId SERIAL PRIMARY KEY,
                FirstName VARCHAR(100) NOT NULL,
                LastName VARCHAR(100) NOT NULL
            );

            CREATE TABLE CourseInstances (
                CourseInstanceId SERIAL PRIMARY KEY,
                CourseId INT NOT NULL,
                InstructorId INT NOT NULL,
                AcademicYear INT NOT NULL,
                Budget INT
            );

            CREATE TABLE Enrollments (
                EnrollmentId SERIAL PRIMARY KEY,
                StudentId INT NOT NULL,
                CourseInstanceId INT NOT NULL,
                Grade DECIMAL(3, 2)
            );
        ");
        Console.WriteLine($"PostgreSQL tables created.");
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
        await ExecuteSqlNonQueryAsync("postgres", $"DROP DATABASE {DatabaseName} WITH (FORCE);");
        Console.WriteLine($"PostgreSQL database '{DatabaseName}' dropped.");
    }
}
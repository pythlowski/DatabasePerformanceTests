using DatabasePerformanceTests.Data.Models.Domain;
using DatabasePerformanceTests.Utils;
using DatabasePerformanceTests.Utils.Database.Models.Enums;
using DatabasePerformanceTests.Utils.Generators.Models;
using Npgsql;

namespace DatabasePerformanceTests.Data.Contexts;

public class PsqlDbContext : AbstractDbContext, ISqlDbContext
{    
    private NpgsqlConnection _transactionConnection;
    private NpgsqlTransaction _transaction;

    public PsqlDbContext(string connectionString)
        : base(connectionString)
    {
    }
    
    public override DatabaseSystem DatabaseSystem => DatabaseSystem.Postgres;

    private NpgsqlConnection Connection(string databaseName)
    {
        return new NpgsqlConnection(
            new NpgsqlConnectionStringBuilder(ConnectionString) { Database = databaseName }.ToString()
        );
    }
    
    public async Task ExecuteNonQueryAsync(string query, bool useCurrentTransaction = false)
    {
        if (useCurrentTransaction)
        {
            using var cmd = new NpgsqlCommand(query, _transactionConnection, _transaction);
            await cmd.ExecuteNonQueryAsync();
        }
        else
        {
            await using var connection = Connection(DatabaseName);
            await connection.OpenAsync();
            
            using var cmd = new NpgsqlCommand(query, connection);
            await cmd.ExecuteNonQueryAsync();
        }
    }
    
    public async Task<object?> ExecuteScalarAsync(string query, bool useCurrentTransaction = false)
    {
        if (useCurrentTransaction)
        {
            using var cmd = new NpgsqlCommand(query, _transactionConnection, _transaction);
            return await cmd.ExecuteScalarAsync();
        }
        else
        {
            await using var connection = Connection(DatabaseName);
            await connection.OpenAsync();
            
            using var cmd = new NpgsqlCommand(query, connection);
            return await cmd.ExecuteScalarAsync();
        }
    }
    
    public async Task<List<Dictionary<string, object>>> ExecuteReaderAsync(string query, bool useCurrentTransaction = false)
    {
        var results = new List<Dictionary<string, object>>();
        NpgsqlCommand command;
        
        if (useCurrentTransaction)
        {
            command = new NpgsqlCommand(query, _transactionConnection, _transaction);
            
        }
        else
        {
            await using var connection = Connection(DatabaseName);
            await connection.OpenAsync();
            
            command = new NpgsqlCommand(query, connection);
        }

        await using (command)
        {
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.GetValue(i);
                }

                results.Add(row);
            }
        }

        return results;
    }
    
    public override async Task CreateDatabaseAsync()
    {
        await using var connection = Connection("postgres");
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand($"CREATE DATABASE \"{DatabaseName}\"", connection);
        await command.ExecuteNonQueryAsync();
        Logger.Log($"PostgreSQL database '{DatabaseName}' created.");
    }
    
    public override async Task CreateTablesAsync()
    {
        var query = @"
            CREATE TABLE ""Students"" (
                ""StudentId"" SERIAL PRIMARY KEY, 
                ""FirstName"" VARCHAR(100) NOT NULL,
                ""LastName"" VARCHAR(100) NOT NULL,
                ""BirthDate"" DATE NOT NULL,
                ""AdmissionYear"" INT NOT NULL,
                ""IsActive"" BOOLEAN NOT NULL DEFAULT TRUE
            );

            CREATE TABLE ""Courses"" (
                ""CourseId"" SERIAL PRIMARY KEY,
                ""CourseName"" VARCHAR(200) NOT NULL
            );

            CREATE TABLE ""Instructors"" (
                ""InstructorId"" SERIAL PRIMARY KEY,
                ""FirstName"" VARCHAR(100) NOT NULL,
                ""LastName"" VARCHAR(100) NOT NULL
            );

            CREATE TABLE ""CourseInstances"" (
                ""CourseInstanceId"" SERIAL PRIMARY KEY,
                ""CourseId"" INT NOT NULL,
                ""InstructorId"" INT NOT NULL,
                ""AcademicYear"" INT NOT NULL,
                ""Budget"" BIGINT
            );

            CREATE TABLE ""Enrollments"" (
                ""EnrollmentId"" SERIAL PRIMARY KEY,
                ""StudentId"" INT NOT NULL,
                ""CourseInstanceId"" INT NOT NULL,
                ""EnrollmentDate"" DATE NOT NULL,
                ""Grade"" DECIMAL(3, 2)
            );
        ";
        await using var connection = Connection(DatabaseName);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(query, connection);
        await command.ExecuteNonQueryAsync();
        
        Logger.Log($"PostgreSQL tables created.");
    }

    public override async Task PopulateDatabaseAsync(GeneratedData data)
    {
        int batchSize = 1000;
        await using var connection = Connection(DatabaseName);
        await connection.OpenAsync();

        for (int i = 0; i < data.Students.Count; i += batchSize)
        {
            var batch = data.Students.GetRange(i, Math.Min(batchSize, data.Students.Count - i));
            var query = "INSERT INTO \"Students\" (\"StudentId\", \"FirstName\", \"LastName\", \"BirthDate\", \"AdmissionYear\", \"IsActive\") VALUES " +
                        string.Join(",\n", batch.ConvertAll(student =>
                            $"({student.Id}, '{EscapeString(student.FirstName)}', '{EscapeString(student.LastName)}', '{student.BirthDate.ToString("yyyy-MM-dd")}', {student.AdmissionYear}, {student.IsActive})"));
            await using var command = new NpgsqlCommand(query, connection);
            await command.ExecuteNonQueryAsync();
        }

        for (int i = 0; i < data.Courses.Count; i += batchSize)
        {
            var batch = data.Courses.GetRange(i, Math.Min(batchSize, data.Courses.Count - i));
            var query = "INSERT INTO \"Courses\" (\"CourseId\", \"CourseName\") VALUES " +
                        string.Join(",\n", batch.ConvertAll(course =>
                            $"({course.Id}, '{EscapeString(course.Name)}')"));
            await using var command = new NpgsqlCommand(query, connection);
            await command.ExecuteNonQueryAsync();
        }

        for (int i = 0; i < data.Instructors.Count; i += batchSize)
        {
            var batch = data.Instructors.GetRange(i, Math.Min(batchSize, data.Instructors.Count - i));
            var query = "INSERT INTO \"Instructors\" (\"InstructorId\", \"FirstName\", \"LastName\") VALUES " +
                        string.Join(",\n", batch.ConvertAll(instructor =>
                            $"({instructor.Id}, '{EscapeString(instructor.FirstName)}', '{EscapeString(instructor.LastName)}')"));
            await using var command = new NpgsqlCommand(query, connection);
            await command.ExecuteNonQueryAsync();
        }

        for (int i = 0; i < data.CourseInstances.Count; i += batchSize)
        {
            var batch = data.CourseInstances.GetRange(i, Math.Min(batchSize, data.CourseInstances.Count - i));
            var query = "INSERT INTO \"CourseInstances\" (\"CourseInstanceId\", \"CourseId\", \"InstructorId\", \"AcademicYear\", \"Budget\") VALUES " +
                        string.Join(",\n", batch.ConvertAll(instance =>
                            $"({instance.Id}, {instance.CourseId}, {instance.InstructorId}, {instance.AcademicYear}, {instance.Budget})"));
            await using var command = new NpgsqlCommand(query, connection);
            await command.ExecuteNonQueryAsync();
        }

        for (int i = 0; i < data.Enrollments.Count; i += batchSize)
        {
            var batch = data.Enrollments.GetRange(i, Math.Min(batchSize, data.Enrollments.Count - i));
            var query = "INSERT INTO \"Enrollments\" (\"EnrollmentId\", \"StudentId\", \"CourseInstanceId\", \"EnrollmentDate\", \"Grade\") VALUES " +
                        string.Join(",\n", batch.ConvertAll(enrollment =>
                            $"({enrollment.Id}, {enrollment.StudentId}, {enrollment.CourseInstanceId}, '{enrollment.EnrollmentDate.ToString("yyyy-MM-dd")}', {enrollment.Grade.ToString(System.Globalization.CultureInfo.InvariantCulture)})"));
            await using var command = new NpgsqlCommand(query, connection);
            await command.ExecuteNonQueryAsync();
        }
    }

    public static string EscapeString(string input)
    {
        return input.Replace("'", "''");
    }

    public override async Task DropDatabaseAsync()
    {
        await using var connection = Connection("postgres");
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand($"DROP DATABASE {DatabaseName} WITH (FORCE);", connection);
        await command.ExecuteNonQueryAsync();
        Logger.Log($"PostgreSQL database '{DatabaseName}' dropped.");
    }

    private async Task OpenTransactionConnectionAsync()
    {
        _transactionConnection = Connection(DatabaseName); 
        await _transactionConnection.OpenAsync();
    }

    public override async Task StartTransactionAsync()
    {
        await OpenTransactionConnectionAsync();
        _transaction = await _transactionConnection.BeginTransactionAsync();
    }

    public override async Task CommitTransactionAsync()
    {
        await _transaction.CommitAsync();
        _transaction.Dispose();
        _transaction = null;
        await CloseTransactionConnectionAsync();
    }

    public override async Task RollbackTransactionAsync()
    {
        await _transaction.RollbackAsync();
        _transaction.Dispose();
        _transaction = null;
        await CloseTransactionConnectionAsync();
    }

    private async Task CloseTransactionConnectionAsync()
    {
        await _transactionConnection.CloseAsync();
        _transactionConnection.Dispose();
        _transactionConnection = null;
    }
}
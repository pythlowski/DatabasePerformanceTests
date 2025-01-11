using System.Data;
using DatabasePerformanceTests.Utils;
using DatabasePerformanceTests.Utils.Config.Enums;
using DatabasePerformanceTests.Utils.Generators.Models;
using Microsoft.Data.SqlClient;

namespace DatabasePerformanceTests.Data.Contexts;

public class MssqlDbContext : AbstractDbContext, ISqlDbContext
{    
    private SqlConnection _transactionConnection;
    private SqlTransaction _transaction;

    public MssqlDbContext(string connectionString, string databaseName)
        : base(connectionString, databaseName)
    {
    }
    
    public override DatabaseSystem DatabaseSystem => DatabaseSystem.MsSql;

    private SqlConnection Connection(string databaseName)
    {
        return new SqlConnection(
            new SqlConnectionStringBuilder(ConnectionString) { InitialCatalog = databaseName }.ToString()
        );
    }
    
    public async Task ExecuteNonQueryAsync(string query, bool useCurrentTransaction = false)
    {
        if (useCurrentTransaction)
        {
            using var cmd = new SqlCommand(query, _transactionConnection, _transaction);
            await cmd.ExecuteNonQueryAsync();
        }
        else
        {
            await using var connection = Connection(DatabaseName);
            await connection.OpenAsync();
            
            using var cmd = new SqlCommand(query, connection);
            await cmd.ExecuteNonQueryAsync();
        }
    }
    
    public async Task<object?> ExecuteScalarAsync(string query, bool useCurrentTransaction = false)
    {
        if (useCurrentTransaction)
        {
            using var cmd = new SqlCommand(query, _transactionConnection, _transaction);
            return await cmd.ExecuteScalarAsync();
        }
        else
        {
            await using var connection = Connection(DatabaseName);
            await connection.OpenAsync();
            
            using var cmd = new SqlCommand(query, connection);
            return await cmd.ExecuteScalarAsync();
        }
    }

    public async Task<List<Dictionary<string, object>>> ExecuteReaderAsync(string query, bool useCurrentTransaction = false)
    {
        var results = new List<Dictionary<string, object>>();
        SqlCommand command;
        
        if (useCurrentTransaction)
        {
            command = new SqlCommand(query, _transactionConnection, _transaction);
            
        }
        else
        {
            await using var connection = Connection(DatabaseName);
            await connection.OpenAsync();
            
            command = new SqlCommand(query, connection);
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
        await using var connection = Connection("master");
        await connection.OpenAsync();
        await using var command = new SqlCommand($"CREATE DATABASE [{DatabaseName}]", connection);
        await command.ExecuteNonQueryAsync();
        
        Logger.Log($"MSSQL database '{DatabaseName}' created.");
    }
    
    public override async Task CreateTablesAsync()
    {
        string query = @"
            CREATE TABLE Students (
                StudentId INT IDENTITY(1,1) PRIMARY KEY,
                FirstName NVARCHAR(100) NOT NULL,
                LastName NVARCHAR(100) NOT NULL,
                BirthDate DATE NOT NULL,
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
                Budget BIGINT
            );

            CREATE TABLE Enrollments (
                EnrollmentId INT IDENTITY(1,1) PRIMARY KEY,
                StudentId INT NOT NULL,
                CourseInstanceId INT NOT NULL,
                EnrollmentDate DATE NOT NULL,
                Grade DECIMAL(3, 2)
            );
        ";
        
        await using var connection = Connection(DatabaseName);
        await connection.OpenAsync();
        await using var command = new SqlCommand(query, connection);
        await command.ExecuteNonQueryAsync();
        
        Logger.Log($"MSSQL tables created.");
    }

    public override async Task PopulateDatabaseAsync(GeneratedData data)
    {
        await using var connection = Connection(DatabaseName);
        await connection.OpenAsync();

        Logger.Log("MSSQL Inserting students...");
        var studentTable = new DataTable();
        studentTable.Columns.Add("StudentId", typeof(int));
        studentTable.Columns.Add("FirstName", typeof(string));
        studentTable.Columns.Add("LastName", typeof(string));
        studentTable.Columns.Add("BirthDate", typeof(DateTime));
        studentTable.Columns.Add("AdmissionYear", typeof(int));
        studentTable.Columns.Add("IsActive", typeof(bool));

        foreach (var student in data.Students)
        {
            studentTable.Rows.Add(student.Id, student.FirstName, student.LastName, student.BirthDate, student.AdmissionYear, student.IsActive);
        }

        using var studentBulkCopy = new SqlBulkCopy(connection)
        {
            DestinationTableName = "Students"
        };

        studentBulkCopy.ColumnMappings.Add("StudentId", "StudentId");
        studentBulkCopy.ColumnMappings.Add("FirstName", "FirstName");
        studentBulkCopy.ColumnMappings.Add("LastName", "LastName");
        studentBulkCopy.ColumnMappings.Add("BirthDate", "BirthDate");
        studentBulkCopy.ColumnMappings.Add("AdmissionYear", "AdmissionYear");
        studentBulkCopy.ColumnMappings.Add("IsActive", "IsActive");

        await studentBulkCopy.WriteToServerAsync(studentTable);

        Logger.Log("MSSQL Inserting courses...");
        var courseTable = new DataTable();
        courseTable.Columns.Add("CourseId", typeof(int));
        courseTable.Columns.Add("CourseName", typeof(string));

        foreach (var course in data.Courses)
        {
            courseTable.Rows.Add(course.Id, course.Name);
        }

        using var courseBulkCopy = new SqlBulkCopy(connection)
        {
            DestinationTableName = "Courses"
        };

        courseBulkCopy.ColumnMappings.Add("CourseId", "CourseId");
        courseBulkCopy.ColumnMappings.Add("CourseName", "CourseName");

        await courseBulkCopy.WriteToServerAsync(courseTable);

        Logger.Log("MSSQL Inserting instructors...");
        var instructorTable = new DataTable();
        instructorTable.Columns.Add("InstructorId", typeof(int));
        instructorTable.Columns.Add("FirstName", typeof(string));
        instructorTable.Columns.Add("LastName", typeof(string));

        foreach (var instructor in data.Instructors)
        {
            instructorTable.Rows.Add(instructor.Id, instructor.FirstName, instructor.LastName);
        }

        using var instructorBulkCopy = new SqlBulkCopy(connection)
        {
            DestinationTableName = "Instructors"
        };

        instructorBulkCopy.ColumnMappings.Add("InstructorId", "InstructorId");
        instructorBulkCopy.ColumnMappings.Add("FirstName", "FirstName");
        instructorBulkCopy.ColumnMappings.Add("LastName", "LastName");

        await instructorBulkCopy.WriteToServerAsync(instructorTable);

        Logger.Log("MSSQL Inserting course instances...");
        var courseInstanceTable = new DataTable();
        courseInstanceTable.Columns.Add("CourseInstanceId", typeof(int));
        courseInstanceTable.Columns.Add("CourseId", typeof(int));
        courseInstanceTable.Columns.Add("InstructorId", typeof(int));
        courseInstanceTable.Columns.Add("AcademicYear", typeof(int));
        courseInstanceTable.Columns.Add("Budget", typeof(int));

        foreach (var instance in data.CourseInstances)
        {
            courseInstanceTable.Rows.Add(instance.Id, instance.CourseId, instance.InstructorId, instance.AcademicYear, instance.Budget);
        }

        using var courseInstanceBulkCopy = new SqlBulkCopy(connection)
        {
            DestinationTableName = "CourseInstances"
        };

        courseInstanceBulkCopy.ColumnMappings.Add("CourseInstanceId", "CourseInstanceId");
        courseInstanceBulkCopy.ColumnMappings.Add("CourseId", "CourseId");
        courseInstanceBulkCopy.ColumnMappings.Add("InstructorId", "InstructorId");
        courseInstanceBulkCopy.ColumnMappings.Add("AcademicYear", "AcademicYear");
        courseInstanceBulkCopy.ColumnMappings.Add("Budget", "Budget");

        await courseInstanceBulkCopy.WriteToServerAsync(courseInstanceTable);

        Logger.Log("Inserting enrollments...");
        var enrollmentTable = new DataTable();
        enrollmentTable.Columns.Add("EnrollmentId", typeof(int));
        enrollmentTable.Columns.Add("StudentId", typeof(int));
        enrollmentTable.Columns.Add("CourseInstanceId", typeof(int));
        enrollmentTable.Columns.Add("EnrollmentDate", typeof(DateTime));
        enrollmentTable.Columns.Add("Grade", typeof(decimal));

        foreach (var enrollment in data.Enrollments)
        {
            enrollmentTable.Rows.Add(enrollment.Id, enrollment.StudentId, enrollment.CourseInstanceId, enrollment.EnrollmentDate, enrollment.Grade);
        }

        using var enrollmentBulkCopy = new SqlBulkCopy(connection)
        {
            DestinationTableName = "Enrollments"
        };

        enrollmentBulkCopy.ColumnMappings.Add("EnrollmentId", "EnrollmentId");
        enrollmentBulkCopy.ColumnMappings.Add("StudentId", "StudentId");
        enrollmentBulkCopy.ColumnMappings.Add("CourseInstanceId", "CourseInstanceId");
        enrollmentBulkCopy.ColumnMappings.Add("EnrollmentDate", "EnrollmentDate");
        enrollmentBulkCopy.ColumnMappings.Add("Grade", "Grade");

        await enrollmentBulkCopy.WriteToServerAsync(enrollmentTable);
        
        Logger.Log("MSSQL finished inserting.");
    }

    public override async Task CreateIndexesAsync()
    {
        await using var connection = Connection(DatabaseName);
        await connection.OpenAsync();
        await using var command = new SqlCommand(@"
            CREATE INDEX idxStudentsLastName ON Students (LastName);
            CREATE INDEX idxStudentsActiveLastNameDate ON Students (IsActive, LastName);

            CREATE INDEX idxCoursesCourseId ON Courses (CourseId);

            CREATE INDEX idxInstructorsInstructorId ON Instructors (InstructorId);

            CREATE INDEX idxCourseInstancesCourseId ON CourseInstances (CourseId);
            CREATE INDEX idxCourseInstancesInstructorId ON CourseInstances (InstructorId);
            CREATE INDEX idxCourseInstancesCourseInstructor ON CourseInstances (CourseId, InstructorId);

            CREATE INDEX idxEnrollmentsStudentId ON Enrollments (StudentId);
            CREATE INDEX idxEnrollmentsCourseInstanceId ON Enrollments (CourseInstanceId);
            CREATE INDEX idxEnrollmentsEnrollmentDate ON Enrollments (EnrollmentDate);
            CREATE INDEX idxEnrollmentsStudentCourse ON Enrollments (StudentId, CourseInstanceId);
        ", connection);
        await command.ExecuteNonQueryAsync();
        Logger.Log("MSSQL indexes created.");
    }

    public override async Task DropDatabaseAsync()
    {
        await using var connection = Connection("master");
        await connection.OpenAsync();
        await using var command = new SqlCommand($"ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{DatabaseName}]", connection);
        await command.ExecuteNonQueryAsync();
        
        Logger.Log($"MSSQL database '{DatabaseName}' dropped.");
    }

    private async Task OpenTransactionConnectionAsync()
    {
        _transactionConnection = Connection(DatabaseName); 
        await _transactionConnection.OpenAsync();
    }

    public override async Task StartTransactionAsync()
    {
        await OpenTransactionConnectionAsync();
        _transaction = _transactionConnection.BeginTransaction(IsolationLevel.ReadCommitted);
    }
    
    public override async Task CommitTransactionAsync()
    {
        await Task.Run(() => _transaction.Commit());
        _transaction.Dispose();
        _transaction = null;
        await CloseTransactionConnectionAsync();
    }

    public override async Task RollbackTransactionAsync()
    {
        await Task.Run(() => _transaction.Rollback());
        _transaction.Dispose();
        _transaction = null;
        await CloseTransactionConnectionAsync();
    }

    public override async Task ClearCacheAsync()
    {
        await ExecuteNonQueryAsync("DBCC FREEPROCCACHE; DBCC DROPCLEANBUFFERS;");
    }

    private async Task CloseTransactionConnectionAsync()
    {
        if (_transactionConnection != null && _transactionConnection.State == ConnectionState.Open)
        {
            await _transactionConnection.CloseAsync();
            _transactionConnection.Dispose();
            _transactionConnection = null;
        }
    }
}
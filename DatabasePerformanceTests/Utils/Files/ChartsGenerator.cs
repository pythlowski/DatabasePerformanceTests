using DatabasePerformanceTests.Data.Operations;
using DatabasePerformanceTests.Utils.Config.Enums;
using DatabasePerformanceTests.Utils.Tests.Models;
using ScottPlot;

namespace DatabasePerformanceTests.Utils.Files;

public class ChartsGenerator
{
    private static readonly string CHARTS_DIRECTORY_NAME = "Plots";
    private static readonly string X_LABEL_N = "Liczba N";
    private static readonly string Y_LABEL_TIME = "Czas (ms)";
    

    private static readonly Dictionary<DatabaseSystem, DatabaseSystemChartProperties> _systemChartProperties = new()
    {
        {
            DatabaseSystem.Mongo, 
            new DatabaseSystemChartProperties { Name = "MongoDB", Color = new Color(0, 255, 0) }
        },
        {
            DatabaseSystem.Postgres,
            new DatabaseSystemChartProperties { Name = "PostgreSQL", Color = new Color(0, 0, 255) }
        },
        {
            DatabaseSystem.MsSql,
            new DatabaseSystemChartProperties { Name = "MS SQL Server", Color = new Color(255, 0, 0) }
        }
    };
        
    public void GenerateCharts(List<OperationResults> results, string outputDirectory)
    {
        List<ChartData> charts = new();
        
        charts.Add(new( 
            fileName:OperationType.BulkInsertEnrollments.ToString(),
            plot:GenerateLineChartForAllSystemsWithManyDataSizes(results, OperationType.BulkInsertEnrollments, 
                chartTitle:"Masowe dodawanie zapisów", xLabel:X_LABEL_N)
        ));
        
        charts.Add(new( 
            fileName:OperationType.DeleteEnrollments.ToString(),
            plot:GenerateLineChartForAllSystemsWithManyDataSizes(results, OperationType.DeleteEnrollments, 
                chartTitle:"Usuwanie zapisów", xLabel:X_LABEL_N)
        ));
        
        charts.Add(new( 
            fileName:OperationType.UpdateEnrollments.ToString(),
            plot:GenerateLineChartForAllSystemsWithManyDataSizes(results, OperationType.UpdateEnrollments, 
                chartTitle:"Aktualizacja zapisów", xLabel:X_LABEL_N)
        ));
        
        charts.Add(new( 
            fileName:OperationType.SelectStudentById.ToString(),
            plot:GenerateBarChartForAllDatabases(results, OperationType.SelectStudentById, 
                chartTitle:"Pobieranie studenta po ID")
        ));
        
        charts.Add(new( 
            fileName:OperationType.SelectCourseInstancesByStudentId.ToString(),
            plot:GenerateBarChartForAllDatabases(results, OperationType.SelectCourseInstancesByStudentId, 
                chartTitle:"Pobieranie listy instancji kursów po ID studenta")
        ));
        
        charts.Add(new( 
            fileName:OperationType.SelectStudentsOrderedById.ToString(),
            plot:GenerateLineChartForAllSystemsWithManyDataSizes(results, OperationType.SelectStudentsOrderedById, 
                chartTitle:"Pobieranie listy studentów posortowanych po ID", xLabel:X_LABEL_N)
        ));
        
        charts.Add(new( 
            fileName:OperationType.SelectEnrollmentsOrderedById.ToString(),
            plot:GenerateLineChartForAllSystemsWithManyDataSizes(results, OperationType.SelectEnrollmentsOrderedById, 
                chartTitle:"Pobieranie listy zapisów posortowanych po ID", xLabel:X_LABEL_N)
        ));
        
        charts.Add(new( 
            fileName:OperationType.SelectEnrollmentsFilteredByIsActive.ToString(),
            plot:GenerateBarChartForAllDatabases(results, OperationType.SelectEnrollmentsFilteredByIsActive, 
                chartTitle:"Pobieranie listy zapisów filtrowanych po wartości logicznej")
        ));
        
        charts.Add(new( 
            fileName:OperationType.SelectEnrollmentsFilteredByEnrollmentDate.ToString(),
            plot:GenerateBarChartForAllDatabases(results, OperationType.SelectEnrollmentsFilteredByEnrollmentDate, 
                chartTitle:"Pobieranie listy zapisów filtrowanych po zakresie dat")
        ));
        
        charts.Add(new( 
            fileName:OperationType.SelectEnrollmentsFilteredByBudget.ToString(),
            plot:GenerateBarChartForAllDatabases(results, OperationType.SelectEnrollmentsFilteredByBudget, 
                chartTitle:"Pobieranie listy zapisów filtrowanych po zakresie wartości liczbowej")
        ));
        
        charts.Add(new( 
            fileName:OperationType.SelectEnrollmentsFilteredByStudentsLastName.ToString(),
            plot:GenerateBarChartForAllDatabases(results, OperationType.SelectEnrollmentsFilteredByStudentsLastName, 
                chartTitle:"Pobieranie listy zapisów filtrowanych po wartości tekstowej typu zawiera")
        ));
        
        charts.Add(new( 
            fileName:OperationType.SelectEnrollmentsWithManyFilters.ToString(),
            plot:GenerateBarChartForAllDatabases(results, OperationType.SelectEnrollmentsWithManyFilters, 
                chartTitle:"Pobieranie listy zapisów filtrowanych po wielu wartościach")
        ));
        
        charts.Add(new( 
            fileName:OperationType.SelectEnrollmentsWithPagination.ToString(),
            plot:GenerateLineChartForAllSystemsWithManyDataSizes(results, OperationType.SelectEnrollmentsWithPagination, 
                chartTitle:"Pobieranie listy zapisów z paginacją", xLabel:"Numer strony N")
        ));
        
        charts.Add(new( 
            fileName:OperationType.SelectEnrollmentsWithManySortParameters.ToString(),
            plot:GenerateLineChartForAllSystemsWithManyDataSizes(results, OperationType.SelectEnrollmentsWithManySortParameters, 
                chartTitle:"Pobieranie listy zapisów sortowanych po wielu wartościach", xLabel:X_LABEL_N)
        ));
        
        charts.AddRange(GenerateSortingComparisonCharts(results));
        charts.AddRange(GenerateComparisonChartsForEachDatabase(
            results, 
            new ()
            {
                new OperationChartLine(OperationType.SelectStudentsOrderedById, "Sortowanie po ID"),
                new OperationChartLine(OperationType.SelectEnrollmentsWithManySortParameters, "Wiele parametrów sortowania"),
            },
            "Porównianie sortowania"
            )
        );
        charts.AddRange(GenerateComparisonChartsForEachDatabase(
            results, 
            new ()
            {
                new OperationChartLine(OperationType.SelectEnrollmentsFilteredByIsActive, "Filtrowanie po wartości logicznej"),
                new OperationChartLine(OperationType.SelectEnrollmentsFilteredByEnrollmentDate, "Filtrowanie po zakresie dat"),
                new OperationChartLine(OperationType.SelectEnrollmentsFilteredByBudget, "Filtrowanie po zakresie wartości liczbowych"),
                new OperationChartLine(OperationType.SelectEnrollmentsFilteredByStudentsLastName, "Filtrowanie po zawieraniu wartości tekstowej"),
                new OperationChartLine(OperationType.SelectEnrollmentsWithManyFilters, "Złożone filtrowanie"),
            },
            "Porównianie fitrowania"
            )
        );
        charts.AddRange(GenerateComparisonChartsForEachDatabase(
            results, 
            new ()
            {
                new OperationChartLine(OperationType.BulkInsertEnrollments, "Insert"),
                new OperationChartLine(OperationType.UpdateEnrollments, "Update"),
                new OperationChartLine(OperationType.DeleteEnrollments, "Delete"),
                new OperationChartLine(OperationType.SelectEnrollmentsOrderedById, "Select"),
            },
            "Porównianie operacji CRUD"
            )
        );
        
        string fileDirectory = GetFileDirectory(outputDirectory);
        Directory.CreateDirectory(fileDirectory);

        foreach (var chartData in charts)
        {
            string filePath = Path.Combine(fileDirectory, chartData.FileName + ".png");
            chartData.Plot.SavePng(filePath, 900, 600);
        }
    }

    private IEnumerable<ChartData> GenerateSortingComparisonCharts(List<OperationResults> results)
    {
        var orderByIdResults = results
            .Where(r => r.OperationType == OperationType.SelectEnrollmentsOrderedById)
            .OrderBy(r => r.DataSize);

        var manySortParametersResults = results
            .Where(r => r.OperationType == OperationType.SelectEnrollmentsWithManySortParameters)
            .OrderBy(r => r.DataSize);

        var orderByIdDataSizes = orderByIdResults.Select(r => r.DataSize);
        var manySortParametersResultsDataSizes = manySortParametersResults.Select(r => r.DataSize);
        var commonDataSizes = orderByIdDataSizes.Intersect(manySortParametersResultsDataSizes).ToArray();
        
        foreach (var databaseSystem in _systemChartProperties.Keys)
        {
            Plot plt = new();
            
            var orderByIdTimes = commonDataSizes.Select(dataSize => orderByIdResults
                .Where(r => r.DataSize == dataSize)
                .Select(r => r.Results[databaseSystem].Average)
                .FirstOrDefault()
            ).ToArray();
            
            var orderByIdScatter = plt.Add.Scatter(commonDataSizes, orderByIdTimes);;
            orderByIdScatter.LegendText = "Sortowanie po ID";
            
            var manySortParametersTimes = commonDataSizes.Select(dataSize => manySortParametersResults
                .Where(r => r.DataSize == dataSize)
                .Select(r => r.Results[databaseSystem].Average)
                .FirstOrDefault()
            ).ToArray();
            
            var manySortParametersScatter = plt.Add.Scatter(commonDataSizes, manySortParametersTimes);
            manySortParametersScatter.LegendText = "Wiele parametrów sortowania";

            plt.Title($"Porównianie sortowania - {_systemChartProperties[databaseSystem].Name}");
            plt.XLabel(X_LABEL_N);
            plt.YLabel(Y_LABEL_TIME);

            yield return new( 
                fileName:$"SortingComparison_{databaseSystem}",
                plot:plt
            );
        }
    }
    
    private IEnumerable<ChartData> GenerateComparisonChartsForEachDatabase(
        List<OperationResults> results,
        List<OperationChartLine> operationLines,
        string chartTitle)
    {
        var resultsByOperationType = operationLines
            .ToDictionary(
                line => line.OperationType,
                line => results
                    .Where(r => r.OperationType == line.OperationType)
                    .OrderBy(r => r.DataSize)
            );

        var commonDataSizes = resultsByOperationType
            .Values
            .Select(operationResults => operationResults.Select(r => r.DataSize))
            .Aggregate((current, next) => current.Intersect(next))
            .ToArray();

        foreach (var databaseSystem in _systemChartProperties.Keys)
        {
            Plot plt = new();

            foreach (var operationLine in operationLines)
            {
                var operationResults = resultsByOperationType[operationLine.OperationType];
                var times = commonDataSizes.Select(dataSize => operationResults
                    .Where(r => r.DataSize == dataSize)
                    .Select(r => r.Results[databaseSystem].Average)
                    .FirstOrDefault()
                ).ToArray();

                var scatter = plt.Add.Scatter(commonDataSizes, times);
                scatter.LegendText = operationLine.Label;
            }

            plt.Title($"{chartTitle} - {_systemChartProperties[databaseSystem].Name}");
            plt.XLabel(X_LABEL_N);
            plt.YLabel(Y_LABEL_TIME);

            yield return new ChartData(
                fileName: $"Comparison_{chartTitle.Replace(' ', '_')}_{databaseSystem}",
                plot: plt
            );
        }
    }

    private Plot GenerateBarChartForAllDatabases(List<OperationResults> results, OperationType operationType, string chartTitle)
    {
        var operationResults = results
            .Where(r => r.OperationType == operationType)
            .FirstOrDefault() ?? throw new InvalidDataException();
        
        Plot plt = new();

        foreach (var databaseSystem in _systemChartProperties.Keys)
        {
            var bar = plt.Add.Bars(new[] { operationResults.Results[databaseSystem].Average });
            bar.LegendText = _systemChartProperties[databaseSystem].Name;
            bar.Color = _systemChartProperties[databaseSystem].Color;
        }
        
        plt.ShowLegend(Alignment.UpperLeft);
        return plt;
    }

    private Plot GenerateLineChartForAllSystemsWithManyDataSizes(List<OperationResults> results, 
        OperationType operationType, string chartTitle, string xLabel)
    {
        var operationResults = results
            .Where(r => r.OperationType == operationType)
            .OrderBy(r => r.DataSize);
        
        var dataSizes = operationResults.Select(r => r.DataSize).ToArray();

        Plot plt = new();
        
        foreach (var databaseSystem in _systemChartProperties.Keys)
        {
            var times = operationResults
                .Select(r => r.Results[databaseSystem].Average)
                .ToArray();
            
            var scatter = plt.Add.Scatter(dataSizes, times, _systemChartProperties[databaseSystem].Color);;
            scatter.LegendText = _systemChartProperties[databaseSystem].Name;
        }
        
        plt.Title(chartTitle);
        plt.XLabel(xLabel);
        plt.YLabel(Y_LABEL_TIME);
        
        return plt;
    }

    private static string GetFileDirectory(string outputDirectory)
    {
        return Path.Combine(
            FilesManager.GetFileDirectory(outputDirectory, CHARTS_DIRECTORY_NAME), 
            FilesManager.GetFileNameDate()
        );
    }
}

public class DatabaseSystemChartProperties
{
    public string Name { get; set; }
    public Color Color { get; set; }
}

public class ChartData
{
    public ChartData(string fileName, Plot plot)
    {
        Plot = plot;
        FileName = fileName;
    }

    public Plot Plot { get; set; }
    public string FileName { get; set; }
}

public class OperationChartLine
{
    public OperationType OperationType { get; set; }
    public string Label { get; set; }

    public OperationChartLine(OperationType operationType, string label)
    {
        OperationType = operationType;
        Label = label;
    }
}
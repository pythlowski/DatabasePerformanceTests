using DatabasePerformanceTests.Data.Operations;
using DatabasePerformanceTests.Utils.Config.Enums;
using DatabasePerformanceTests.Utils.Files;
using DatabasePerformanceTests.Utils.Tests.Models;
using ScottPlot;

namespace DatabasePerformanceTests.Utils.Statistics;

public class ChartsGenerator
{
    private static readonly string CHARTS_DIRECTORY_NAME = "Plots";
    private static readonly string X_LABEL_N = "Rozmiar danych";
    private static readonly string Y_LABEL_TIME = "Czas (ms)";
    

    private static readonly Dictionary<DatabaseSystem, DatabaseSystemChartProperties> _systemChartProperties = new()
    {
        {
            DatabaseSystem.Postgres,
            new DatabaseSystemChartProperties { Name = "PostgreSQL", Color = new Color(0, 0, 255) }
        },
        {
            DatabaseSystem.MsSql,
            new DatabaseSystemChartProperties { Name = "MS SQL Server", Color = new Color(255, 0, 0) }
        },
        {
            DatabaseSystem.Mongo, 
            new DatabaseSystemChartProperties { Name = "MongoDB", Color = new Color(0, 255, 0) }
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
        
        charts.AddRange(GenerateComparisonChartsForEachDatabase(
            results, 
            new ()
            {
                new OperationChartLine(OperationType.SelectEnrollmentsOrderedById, "Sortowanie po ID"),
                new OperationChartLine(OperationType.SelectEnrollmentsWithManySortParameters, "Wiele parametrów sortowania"),
            },
            "Porównanie sortowania"
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
            "Porównanie filtrowania",
            isBarChart: true
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

        foreach (var (chartData, index) in charts.Select((c, i) => (c, i)))
        {
            string filePath = Path.Combine(fileDirectory, $"{index+1}." + chartData.FileName + ".png");
            chartData.Plot.SavePng(filePath, 900, 600);
        }
    }
    
    private IEnumerable<ChartData> GenerateComparisonChartsForEachDatabase(
        List<OperationResults> results,
        List<OperationChartLine> operationLines,
        string chartTitle,
        bool isBarChart = false)
    {
        var resultsByOperationType = operationLines
            .ToDictionary(
                line => line.OperationType,
                line => results
                    .Where(r => r.OperationType == line.OperationType)
                    .OrderBy(r => r.DataSize)
            );

        var commonDataSizes = resultsByOperationType.Values
            .Select(operationResults => 
                operationResults
                    .Where(r => r.Results.All(dbr => dbr.Value.Average > 0))
                    .Select(r => r.DataSize)
            )
            .Aggregate((current, next) => current.Intersect(next))
            .ToArray();

        foreach (var databaseSystem in _systemChartProperties.Keys)
        {
            Plot myPlot = new();
            
            foreach (var (operationLine, index) in operationLines.Select((o, i) => (o, i)))
            {
                var operationResults = resultsByOperationType[operationLine.OperationType];
                var times = commonDataSizes.Select(dataSize => operationResults
                    .Where(r => r.DataSize == dataSize)
                    .Select(r => r.Results[databaseSystem].Average)
                    .FirstOrDefault()
                ).ToArray();
                
                if (isBarChart)
                {
                    myPlot.Add.Bar(position:index+1, value:times.First());
                }
                else
                {
                    var scatter = myPlot.Add.Scatter(commonDataSizes, times);
                    scatter.LegendText = operationLine.Label;
                    scatter.LineWidth = 3;
                }
            }

            myPlot.Title($"{chartTitle} - {_systemChartProperties[databaseSystem].Name}");
            myPlot.YLabel(Y_LABEL_TIME);

            if (isBarChart)
            {
                myPlot.Axes.Margins(bottom: 0);
                Tick[] ticks = operationLines.Select((line, index) => 
                    new Tick(index+1, line.Label.Replace(' ', '\n'))).ToArray();
                myPlot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
                myPlot.Axes.Bottom.MajorTickStyle.Length = 0;
                myPlot.ShowLegend(Alignment.UpperLeft);
            }
            else
            {
                myPlot.XLabel(X_LABEL_N);
            }

            yield return new ChartData(
                fileName: $"Comparison_{chartTitle.Replace(' ', '_')}_{databaseSystem}",
                plot: myPlot
            );
        }
    }

    private Plot GenerateBarChartForAllDatabases(List<OperationResults> results, OperationType operationType, string chartTitle)
    {
        var operationResults = results
            .FirstOrDefault(r => r.OperationType == operationType) ?? throw new InvalidDataException();
        
        Plot myPlot = new();

        foreach (var (databaseSystem, index) in _systemChartProperties.Keys.Select((ds, i) => (ds, i)))
        {
            var bar = myPlot.Add.Bar(position:index+1, value:operationResults.Results[databaseSystem].Average);
            // bar.LegendText = _systemChartProperties[databaseSystem].Name;
            bar.Color = _systemChartProperties[databaseSystem].Color;
        }
        
        myPlot.Title(chartTitle);
        myPlot.YLabel(Y_LABEL_TIME);
        
        myPlot.Axes.Margins(bottom: 0);
        Tick[] ticks = _systemChartProperties.Select((p, index) => 
            new Tick(index+1, p.Value.Name)).ToArray();
        myPlot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
        myPlot.Axes.Bottom.MajorTickStyle.Length = 0;        
        
        return myPlot;
    }

    private Plot GenerateLineChartForAllSystemsWithManyDataSizes(List<OperationResults> results, 
        OperationType operationType, string chartTitle, string xLabel)
    {
        var operationResults = results
            .Where(r => r.OperationType == operationType 
                        && r.Results.All(r => r.Value.Average > 0)
            )
            .OrderBy(r => r.DataSize);
        
        var dataSizes = operationResults.Select(r => r.DataSize).ToArray();

        Plot myPlot = new();
        
        foreach (var databaseSystem in _systemChartProperties.Keys)
        {
            var times = operationResults
                .Select(r => r.Results[databaseSystem].Average)
                .ToArray();
            
            var scatter = myPlot.Add.Scatter(dataSizes, times, _systemChartProperties[databaseSystem].Color);
            scatter.LegendText = _systemChartProperties[databaseSystem].Name;
            scatter.LineWidth = 3;
        }
        
        myPlot.Title(chartTitle);
        myPlot.XLabel(xLabel);
        myPlot.YLabel(Y_LABEL_TIME);
        myPlot.ShowLegend(Alignment.UpperLeft);

        return myPlot;
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
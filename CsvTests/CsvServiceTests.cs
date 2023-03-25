namespace CsvTests;

using System.IO.Abstractions;
using CsvHelperSandbox;
using FluentAssertions;

public class CsvServiceTests
{
    [Fact]
    public async void can_capture_csv_import()
    {
        // Arrange
        var headerConfig = new List<SupportedImportField>
        {
            new("ColumnOne", "Special Column 1"),
            new("ColumnTwo", "Special Column 2")
        };
        var csvText =
            @$"Special Column 1,Special Column 2
Vacuum,Drink
Apple,Candle";
        var memoryStream = await CreateMemoryStreamFromCsvString(csvText);
        var sut = new CsvService(new FileSystem());

        // Act
        var response = await sut.ImportFromCsv(memoryStream, headerConfig, true, CancellationToken.None);

        // Assert
        var first = response.FirstOrDefault();
        first.Should().Contain("Vacuum,Drink");
        
        var second = response.Skip(1).FirstOrDefault();
        second.Should().Contain("Apple,Candle");
    }
    
    [Fact]
    public async void can_capture_csv_import_with_no_header()
    {
        // Arrange
        var headerConfig = new List<SupportedImportField>
        {
            new("ColumnOne", "Column B"),
            new("ColumnTwo", "Column C")
        };
        var csvText =
            @$",Vacuum,Drink
,Apple,Candle";
        var memoryStream = await CreateMemoryStreamFromCsvString(csvText);
        var sut = new CsvService(new FileSystem());

        // Act
        var response = await sut.ImportFromCsv(memoryStream, headerConfig, false, CancellationToken.None);

        // Assert
        var first = response.FirstOrDefault();
        first.Should().Contain(",Vacuum,Drink");
        
        var second = response.Skip(1).FirstOrDefault();
        second.Should().Contain(",Apple,Candle");
    }
    
    [Fact]
    public async Task UploadResults_ShouldWriteCsvFileAndCallUploader()
    {
        // Arrange
        var headerConfig = new List<SupportedImportField>
        {
            new("ColumnOne", "Special Column 1"),
            new("ColumnTwo", "Special Column 2")
        };
    
        var rowsToUpload = new List<CsvService.RowToUpload>
        {
            new() { ColumnOne = "Value1", ColumnTwo = "Value2" },
            new() { ColumnOne = "Value3", ColumnTwo = "Value4" }
        };
    
        var expectedCsvText =
            @$"Special Column 1,Special Column 2,CustomHeaderOne
Value1,Value2,Custom Note
Value3,Value4,Custom Note
";

        var fileSystem = new FileSystem();
        var sut = new CsvService(fileSystem);
    
        // Act
        await sut.UploadResults(rowsToUpload, headerConfig, CancellationToken.None);
    
        // Assert
        foreach (var textFile in fileSystem.Directory.GetFiles(".", "*.csv"))
        {
            var text = await fileSystem.File.ReadAllTextAsync(textFile);
            text.Should().Be(expectedCsvText);
        }
    }

    private static async Task<MemoryStream> CreateMemoryStreamFromCsvString(string csvText)
    {
        var memoryStream = new MemoryStream();
        var streamWriter = new StreamWriter(memoryStream);
        await streamWriter.WriteAsync(csvText);
        await streamWriter.FlushAsync();
        memoryStream.Position = 0;
        return memoryStream;
    }
}
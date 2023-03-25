namespace CsvHelperSandbox;

using System.Globalization;
using System.IO.Abstractions;
using CsvHelper;
using CsvHelper.Configuration;

public sealed class CsvService
{
    private readonly IFileSystem _fileSystem;
    
    public CsvService(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public async Task<List<string>> ImportFromCsv(MemoryStream memoryStream,
        List<SupportedImportField> headerConfig, 
        bool hasHeader, 
        CancellationToken cancellationToken = default)
    {
        using var streamReader = new StreamReader(memoryStream);
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = hasHeader
        };

        using var csv = new CsvReader(streamReader, csvConfig);
        var map = new DefaultClassMap<SupportedImportFieldsConfiguration>();
        if (hasHeader)
        {
            map.Map(headerConfig.GetMap());
        }
        else
        {
            map.Map(headerConfig.GetMapForIndex());
        }
        csv.Context.RegisterClassMap(map);
        
        var listOfRawRecords = new List<string>();
        while (await csv.ReadAsync())
        {
            var importRow = csv.GetRecord<SupportedImportFieldsConfiguration>();
            var rawRecord = csv.Parser.RawRecord;
            
            // do things with that row
            listOfRawRecords.Add(rawRecord);
        }
        return listOfRawRecords;
    }
    
    public class RowToUpload : SupportedImportFieldsConfiguration
    {
    }
    
    public async Task UploadResults(List<RowToUpload> rowsToUpload, 
        List<SupportedImportField> headerConfig, 
        CancellationToken cancellationToken = default)
    {
        
        var tempDirectory = _fileSystem.Directory.CreateDirectory(Path.Combine("basepath", Guid.NewGuid().ToString())).FullName;
        var tempFilePath = Path.Combine(tempDirectory, "temp.csv");
        await using var writer = new StreamWriter(tempFilePath);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        
        AddHeaderWithCustomColumns(headerConfig, csv);
        await csv.NextRecordAsync();
        
        var map = new DefaultClassMap<RowToUpload>();
        map.Map(headerConfig.GetMap());
        csv.Context.RegisterClassMap(map);
        
        foreach (var record in rowsToUpload)
        {
            csv.WriteRecord(record);
            csv.WriteField("Custom Note");
            await csv.NextRecordAsync();
        }
    }

    private static void AddHeaderWithCustomColumns(List<SupportedImportField> headerConfig, CsvWriter csv)
    {
        headerConfig.ForEach(x => csv.WriteField(x.ColumnLabel));
        csv.WriteField("CustomHeaderOne");
    }
}
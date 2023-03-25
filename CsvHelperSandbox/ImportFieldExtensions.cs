namespace CsvHelperSandbox;

using System.Reflection;

public static class ImportFieldExtensions
{
    public static Dictionary<string, string> GetMap(this List<SupportedImportField> columnMappings)
    {
        var myDictionary = new Dictionary<string, string>();
        foreach (var columnMapping in columnMappings)
        {
            var propertyInfo = typeof(SupportedImportFieldsConfiguration).GetProperty(columnMapping.ColumnKey,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo != null)
            {
                myDictionary.Add(columnMapping.ColumnKey, columnMapping.ColumnLabel);
            }
        }

        return myDictionary;
    }
    
    public static Dictionary<string, int> GetMapForIndex(this List<SupportedImportField> columnMappings)
    {
        var myDictionary = new Dictionary<string, int>();
        foreach (var columnMapping in columnMappings)
        {
            var propertyInfo = typeof(SupportedImportFieldsConfiguration).GetProperty(columnMapping.ColumnKey,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo != null)
            {
                myDictionary.Add(columnMapping.ColumnKey, ExcelHelper.GetColumnIndex(columnMapping.ColumnLabel));
            }
        }

        return myDictionary;
    }
}
namespace CsvHelperSandbox;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ImportHeaderValue : Attribute
{
    public ImportHeaderValue(string fieldName)
    {
        FieldName = fieldName;
    }

    public string FieldName { get; set; }
}

public sealed class SupportedImportField
{
    public SupportedImportField(string columnKey, string columnLabel)
    {
        // var supportedFields = SupportedImportFieldsConfiguration.GetSupportedFields();
        // if (supportedFields.All(x => x.ColumnKey != columnKey))
        // {
        //     throw new Exception($"The field {columnKey} is not supported");
        // }
        
        ColumnKey = columnKey;
        ColumnLabel = columnLabel;
    }

    public string ColumnKey { get; private set; }
    public string ColumnLabel { get; private set; }
}

public class SupportedImportFieldsConfiguration
{
    private static readonly IReadOnlyList<SupportedImportField> SupportedFields = Initialize();

    public static IEnumerable<SupportedImportField> GetSupportedFields()
    {
        return SupportedFields;
    }

    private static List<SupportedImportField> Initialize()
    {
        var supportedFields = new List<SupportedImportField>();
        var properties =  typeof(SupportedImportFieldsConfiguration).GetProperties();

        foreach (var property in properties)
        {
            var attributes = property.GetCustomAttributes(typeof(ImportHeaderValue), false);
            if (attributes.Length == 0)
            {
                continue;
            }

            var supportFieldAttribute = (ImportHeaderValue)attributes[0];
            supportedFields.Add(new SupportedImportField(property.Name, supportFieldAttribute.FieldName));
        }

        return supportedFields;
    }

    [ImportHeaderValue("Column 1")]
    public string? ColumnOne { get; set; }

    [ImportHeaderValue("Column 2")]
    public string? ColumnTwo { get; set; }
}
namespace CsvHelperSandbox;

public static class ExcelHelper
{
    public static int GetColumnIndex(string columnName)
    {
        if (string.IsNullOrEmpty(columnName))
            throw new ArgumentNullException(nameof(columnName), "Invalid column name parameter");
        
        columnName = columnName.Replace(" ", "");
        var hasColumn = columnName.Contains("column", StringComparison.InvariantCultureIgnoreCase);
        if (hasColumn)
        {
            columnName = columnName.Replace("column", "", StringComparison.InvariantCultureIgnoreCase);
        }

        return GetExcelColumnNumber(columnName);
    }
    
    private static int GetExcelColumnNumber(string columnName)
    {
        columnName = columnName.ToUpperInvariant();

        var sum = 0;
        foreach (var character in columnName)
        {
            if (char.IsDigit(character))
                throw new ArgumentNullException("Invalid column name parameter on character " + character);

            sum *= 26;
            sum += (character - 'A' + 1);
        }

        return sum - 1;
    }
}
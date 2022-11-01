namespace Incubation.AzConf.Common;
public static class ExcelFile
{
    public static List<Dictionary<string, Object>> Read(MemoryStream ms, string sheetName, bool header = true)
    {
        using (var doc = SpreadsheetDocument.Open(ms, true))
        {
            var wbPart = doc.WorkbookPart;
            Worksheet Worksheet = GetWorksheetByName(wbPart, sheetName);
            SheetData sheetData = Worksheet.GetFirstChild<SheetData>();
            SharedStringTable stringTable = wbPart.SharedStringTablePart.SharedStringTable;
            return ReadDataFrom(stringTable, sheetData, header);
        }
    }
    private static List<Dictionary<string, Object>> ReadDataFrom(SharedStringTable stringTable, SheetData sheetData, bool header)
    {
        IEnumerable<Row> rows = sheetData.Descendants<Row>();
        var result = new List<Dictionary<string, Object>>();
        List<string> headers = null;
        if (header)
        {
            var row = rows.FirstOrDefault();
            if (row != null)
            {
                var cells = row.Descendants<Cell>();
                headers = GetObject(stringTable, cells, headers).Values.Select(x => x.ToString()).ToList();
                rows = rows.Skip(1);
            }
        }
        foreach (Row row in rows)
        {
            var cells = row.Descendants<Cell>();
            result.Add(GetObject(stringTable, cells, headers));
        }
        return result;
    }

    private static Worksheet GetWorksheetByName(WorkbookPart wbPart, string sheetName)
    {
        var relationShipId = wbPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name.Equals(sheetName))?.Id;
        if (relationShipId == null) { throw new Exception($"{sheetName} workshee is not available in uploaded Excel file"); }
        return ((WorksheetPart)wbPart.GetPartById(relationShipId)).Worksheet;
    }
    private static Dictionary<string, Object> GetObject(SharedStringTable stringTable, IEnumerable<Cell> cells, List<string> headers = null)
    {
        var count = cells.Count();
        Dictionary<string, Object> result = new Dictionary<string, object>();
        for (var i = 0; i < count; i++)
        {
            var cell = cells.ElementAt(i);
            var value = cell.CellValue.InnerXml;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                value = stringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            var key = headers == null ? $"{i}" : headers[i];
            result[key] = value;
        }
        return result;
    }
}
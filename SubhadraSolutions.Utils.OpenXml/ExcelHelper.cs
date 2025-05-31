using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Data.Annotations;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SubhadraSolutions.Utils.OpenXml;

public static class ExcelHelper
{
    public static void AddSheet(DataTable table, SpreadsheetDocument workbook)
    {
        uint sheetId = 1;

        var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
        var sheetData = new SheetData();

        var sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
        var relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

        if (sheets.Elements<Sheet>().Any())
        {
            sheetId =
                sheets.Elements<Sheet>().Max(s => s.SheetId.Value) + 1;
        }

        var sheet = new Sheet { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
        sheets.Append(sheet);

        var headerRow = new Row();

        var dataTypes = new CellValues[table.Columns.Count];

        for (var i = 0; i < table.Columns.Count; i++)
        {
            var column = table.Columns[i];
            var cell = new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(column.ColumnName)
            };
            headerRow.AppendChild(cell);

            dataTypes[i] = GetDataType(column.DataType);
        }

        sheetData.AppendChild(headerRow);

        foreach (DataRow dsrow in table.Rows)
        {
            var newRow = new Row();

            for (var i = 0; i < table.Columns.Count; i++)
            {
                var val = dsrow[i];
                var cell = new Cell
                {
                    DataType = dataTypes[i],
                    CellValue = new CellValue(val?.ToString())
                };
                newRow.AppendChild(cell);
            }

            sheetData.AppendChild(newRow);
        }

        sheetPart.Worksheet = new Worksheet(sheetData);
    }

    public static int AddWorksheetPart(IEnumerable items, string sheetName, WorkbookPart workbookPart,
        AttributesLookup attributesLookup = null)
    {
        var elementType = items.GetType().GetEnumerableItemType();
        var method = typeof(ExcelHelper)
            .GetMethod(nameof(AddWorksheetPartCore), BindingFlags.Static | BindingFlags.NonPublic)
            .MakeGenericMethod(elementType);
        return (int)method.Invoke(null, [items, sheetName, workbookPart, attributesLookup]);
    }

    public static int AddWorksheetPart<T>(IEnumerable<T> items, string sheetName, WorkbookPart workbookPart,
        AttributesLookup attributesLookup = null)
    {
        return AddWorksheetPartCore(items, sheetName, workbookPart, attributesLookup);
    }

    public static int ExportToExcel(IEnumerable items, Stream stream, string sheetName = "Sheet1",
        AttributesLookup attributesLookup = null)
    {
        var elementType = items.GetType().GetEnumerableItemType();
        var method = typeof(ExcelHelper)
            .GetMethod(nameof(ExportToExcelCore), BindingFlags.Static | BindingFlags.NonPublic)
            .MakeGenericMethod(elementType);
        return (int)method.Invoke(null, [items, stream, sheetName, attributesLookup]);
    }

    public static int ExportToExcel<T>(IEnumerable<T> items, Stream stream, string sheetName = "Sheet1",
        AttributesLookup attributesLookup = null)
    {
        return ExportToExcelCore(items, stream, sheetName, attributesLookup);
    }

    public static void ExportToExcel(DataTable table, string outputFileName)
    {
        using var workbook = SpreadsheetDocument.Create(outputFileName, SpreadsheetDocumentType.Workbook);
        var workbookPart = workbook.AddWorkbookPart();
        workbook.WorkbookPart.Workbook = new Workbook
        {
            Sheets = new Sheets()
        };
        AddSheet(table, workbook);
    }
    public static void ExportToExcel(DataSet dataSet, string outputFileName)
    {
        using var workbook = SpreadsheetDocument.Create(outputFileName, SpreadsheetDocumentType.Workbook);
        var workbookPart = workbook.AddWorkbookPart();
        workbook.WorkbookPart.Workbook = new Workbook
        {
            Sheets = new Sheets()
        };
        foreach (DataTable table in dataSet.Tables)
        {
            AddSheet(table, workbook);
        }
    }

    public static string GetReference(int columnIndex, int rowIndex)
    {
        return $"${(char)('A' + columnIndex)}${(rowIndex + 1).ToString()}";
    }

    public static string GetReference(int columnIndex, int rowIndex, string sheetName)
    {
        var reference = GetReference(columnIndex, rowIndex);
        return sheetName + "!" + reference;
    }

    [DynamicallyInvoked]
    private static int AddWorksheetPartCore<T>(IEnumerable<T> items, string sheetName, WorkbookPart workbookPart,
        AttributesLookup attributesLookup = null)
    {
        var sheetPart = workbookPart.AddNewPart<WorksheetPart>();
        var sheets = workbookPart.Workbook.GetFirstChild<Sheets>();

        var sheet = new Sheet { Name = sheetName };
        sheetPart.Worksheet = BuildWorksheet(items, out var itemsCount, attributesLookup);

        uint sheetId = 1;

        if (sheets.Elements<Sheet>().Any())
        {
            sheetId = sheets.Elements<Sheet>().Max(s => s.SheetId.Value) + 1;
        }

        sheet.Id = workbookPart.GetIdOfPart(sheetPart);
        sheet.SheetId = sheetId;

        sheets.Append(sheet);

        return itemsCount;
    }

    private static Worksheet BuildWorksheet<T>(IEnumerable<T> items, out int itemsCount,
        AttributesLookup attributesLookup = null)
    {
        var properties = attributesLookup.GetSortedPublicProperties<T>();
        var propertiesCount = properties.Count;

        var headerRow = new Row();

        var dataTypes = new CellValues[propertiesCount];

        for (var i = 0; i < propertiesCount; i++)
        {
            var property = properties[i];
            var title = property.GetMemberDisplayName(true);
            var cell = new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(title)
            };
            headerRow.AppendChild(cell);
            dataTypes[i] = GetDataType(property.PropertyType);
        }

        var sheetData = new SheetData();
        sheetData.AppendChild(headerRow);
        var action = PropertiesToStringValuesHelper.BuildActionForToStringArray<T>(properties);
        var stringValues = new string[properties.Count];

        itemsCount = 0;

        foreach (var item in items)
        {
            action(item, stringValues);
            var newRow = new Row();

            for (var i = 0; i < propertiesCount; i++)
            {
                var cell = new Cell
                {
                    DataType = dataTypes[i],
                    CellValue = new CellValue(stringValues[i])
                };
                newRow.AppendChild(cell);
            }

            sheetData.AppendChild(newRow);
            itemsCount++;
        }

        return new Worksheet(sheetData);
    }

    [DynamicallyInvoked]
    private static int ExportToExcelCore<T>(IEnumerable<T> items, Stream stream, string sheetName,
        AttributesLookup attributesLookup)
    {
        using var spreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);
        var workbookPart = spreadsheetDocument.AddWorkbookPart();

        workbookPart.Workbook = new Workbook
        {
            Sheets = new Sheets()
        };

        return AddWorksheetPart(items, sheetName, workbookPart, attributesLookup);
    }

    public static DataSet GetDataFromExcelFile(string excelFile)
    {
        using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(excelFile, false))
        {
            var ds = new DataSet();

            WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
            var sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
            foreach (var sheet in sheets)
            {
                string relationshipId = sheet.Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                Worksheet workSheet = worksheetPart.Worksheet;
                SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                IEnumerable<Row> rows = sheetData.Descendants<Row>();
                var isFirstRow = true;
                DataTable table = null;
                //this will also include your header row...
                foreach (Row row in rows)
                {
                    if (isFirstRow)
                    {
                        table = new DataTable(sheet.Name);
                        foreach (Cell cell in rows.ElementAt(0))
                        {
                            table.Columns.Add(GetCellValue(spreadSheetDocument, cell));
                        }
                        isFirstRow = false;
                        continue;
                    }
                    DataRow tempRow = table.NewRow();
                    var values = row.Descendants<Cell>();
                    var count = values.Count();
                    count = Math.Min(count, table.Columns.Count);
                    for (int i = 0; i < count; i++)
                    {
                        tempRow[i] = GetCellValue(spreadSheetDocument, values.ElementAt(i));
                    }
                    table.Rows.Add(tempRow);
                }
                if (table != null)
                {
                    ds.Tables.Add(table);
                }
            }
            return ds;
        }
    }

    private static string GetCellValue(SpreadsheetDocument document, Cell cell)
    {
        SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
        string value = cell.CellValue?.InnerXml;
        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
        {
            return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
        }
        else
        {
            return value;
        }
    }

    private static CellValues GetDataType(Type type)
    {
        if (type == typeof(string) || type == typeof(char) || type == typeof(char?))
        {
            return CellValues.String;
        }

        if (type.IsNumericType())
        {
            return CellValues.Number;
        }

        //if (ReflectionHelper.IsDateOrTimeType(type))
        //{
        //    return CellValues.Date;
        //}

        if (type == typeof(bool) || type == typeof(bool?))
        {
            return CellValues.Boolean;
        }

        return CellValues.String;
    }
}
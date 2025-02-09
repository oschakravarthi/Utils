using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SubhadraSolutions.Utils.Abstractions;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SubhadraSolutions.Utils.OpenXml.Excel;

public class ExcelBuilder : AbstractDisposable
{
    private readonly SpreadsheetDocument document;
    private readonly MemoryStream memoryStream;

    public ExcelBuilder()
    {
        memoryStream = new MemoryStream();
        document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook);
        document.AddWorkbookPart();

        document.WorkbookPart.Workbook = new Workbook
        {
            Sheets = new Sheets()
        };
    }

    public int AddWorksheetPart(IEnumerable items, string sheetName)
    {
        return ExcelHelper.AddWorksheetPart(items, sheetName, document.WorkbookPart);
    }

    public int AddWorksheetPart<T>(IEnumerable<T> items, string sheetName)
    {
        return ExcelHelper.AddWorksheetPart(items, sheetName, document.WorkbookPart);
    }

    public void Build(Stream stream)
    {
        document.Dispose();
        memoryStream.Seek(0, SeekOrigin.Begin);
        memoryStream.CopyTo(stream);
        stream.Flush();
    }

    protected override void Dispose(bool disposing)
    {
        document.Dispose();
        memoryStream.Dispose();
    }
}
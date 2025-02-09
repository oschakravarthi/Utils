namespace SubhadraSolutions.Utils.Reporting;

public interface IReportWriter
{
    void WriteReport(IDocumentBuilder documentBuilder, string reportType);
}
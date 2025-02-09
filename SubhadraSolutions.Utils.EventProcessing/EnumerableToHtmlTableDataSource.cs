using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Web;

namespace SubhadraSolutions.Utils.EventProcessing;

public class EnumerableToHtmlTableDataSource : IDataSource<string>
{
    public static string DefaultBasePath { get; set; }
    public string BasePath { get; set; }
    public IDataSource<object> DataSource { get; set; }
    public bool WriteDocumentRoot { get; set; }

    public string GetData()
    {
        var data = DataSource.GetData();
        var converter = new EnumerableToHtmlTableDataConverter
        {
            BasePath = BasePath ?? DefaultBasePath,
            WriteDocumentRoot = WriteDocumentRoot
        };
        return converter.Convert(data);
    }
}
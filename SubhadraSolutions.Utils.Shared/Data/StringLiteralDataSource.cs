namespace SubhadraSolutions.Utils.Data;

public class StringLiteralDataSource : IDataSource<string>
{
    public string Data { get; set; }

    public string GetData()
    {
        return Data;
    }
}
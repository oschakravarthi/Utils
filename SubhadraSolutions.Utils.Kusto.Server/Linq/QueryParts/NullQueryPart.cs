namespace SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;

public sealed class NullQueryPart : AbstractQueryPart, IQueryPart
{
    private NullQueryPart()
    {
    }

    public static NullQueryPart Instance { get; } = new();

    public override string GetQuery()
    {
        return null;
    }
}
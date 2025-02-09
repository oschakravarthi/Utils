namespace SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;

public sealed class EmptyQueryPart : AbstractQueryPart, IQueryPart
{
    private EmptyQueryPart()
    {
    }

    public static EmptyQueryPart Instance { get; } = new();

    public override string GetQuery()
    {
        return string.Empty;
    }
}
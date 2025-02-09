namespace SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;

public class LiteralQueryPart(string part) : AbstractQueryPart
{
    public override string GetQuery()
    {
        return part;
    }
}
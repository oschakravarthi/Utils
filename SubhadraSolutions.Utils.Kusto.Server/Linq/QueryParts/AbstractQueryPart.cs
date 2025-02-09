using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;

public abstract class AbstractQueryPart : IQueryPart
{
    public abstract string GetQuery();

    public virtual void PopulateSetQueryParts(List<IQueryPart> setQueryParts)
    {
    }

    public override string ToString()
    {
        return GetQuery();
    }
}
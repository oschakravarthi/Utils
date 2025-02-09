using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;

public interface IQueryPart
{
    string GetQuery();

    void PopulateSetQueryParts(List<IQueryPart> setQueryParts);
}
using System.Collections.Generic;
using System.Text;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;

public class CompositeQueryPart : AbstractQueryPart
{
    private readonly List<IQueryPart> innerParts = [];

    public CompositeQueryPart()
    {
    }

    public CompositeQueryPart(params IQueryPart[] innerParts)
    {
        this.innerParts.AddRange(innerParts);
    }

    public void AddChild(IQueryPart queryPart)
    {
        innerParts.Add(queryPart);
    }

    public override string GetQuery()
    {
        var sb = new StringBuilder();

        foreach (var part in innerParts)
        {
            var cmd = part.GetQuery();
            sb.Append(cmd);
        }

        return sb.ToString();
    }

    public override void PopulateSetQueryParts(List<IQueryPart> setQueryParts)
    {
        foreach (var part in innerParts)
        {
            part.PopulateSetQueryParts(setQueryParts);
        }
    }
}
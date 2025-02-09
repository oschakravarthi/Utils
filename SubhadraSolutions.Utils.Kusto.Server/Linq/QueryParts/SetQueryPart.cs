using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;

internal class SetQueryPart : AbstractQueryPart
{
    public SetQueryPart(IQueryPart inner, bool isTemplate, QueryContext context)
    {
        Inner = inner;
        IsTemplate = isTemplate;

        if (!isTemplate)
        {
            Name = "set" + context.Sequencer.Next;
        }
    }

    public IQueryPart Inner { get; }

    public bool IsTemplate { get; }

    public string Name { get; internal set; }

    public override string GetQuery()
    {
        if (IsTemplate)
        {
            return Inner.GetQuery();
        }

        return Name;
    }

    public string GetQueryWithSetDefinition()
    {
        if (IsTemplate)
        {
            return Inner.GetQuery();
        }

        return $"let {Name}={Inner.GetQuery()};";
    }

    public override void PopulateSetQueryParts(List<IQueryPart> setQueryParts)
    {
        Inner.PopulateSetQueryParts(setQueryParts);
        if (!IsTemplate)
        {
            setQueryParts.Add(this);
        }
    }
}
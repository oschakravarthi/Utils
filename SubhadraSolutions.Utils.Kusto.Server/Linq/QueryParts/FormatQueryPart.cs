using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;

public class FormatQueryPart : AbstractQueryPart
{
    private readonly IQueryPart[] templateArguments;
    private readonly IQueryPart templateQueryPart;

    public FormatQueryPart(string templateQueryPart, params IQueryPart[] templateArguments)
    {
        this.templateQueryPart = new LiteralQueryPart(templateQueryPart);
        this.templateArguments = templateArguments;
    }

    public FormatQueryPart(IQueryPart templateQueryPart, params IQueryPart[] templateArguments)
    {
        this.templateQueryPart = templateQueryPart;
        this.templateArguments = templateArguments;
    }

    public override string GetQuery()
    {
        var args = new string[templateArguments.Length];

        for (var i = 0; i < templateArguments.Length; i++) args[i] = templateArguments[i].GetQuery();

        var queryTemplate = templateQueryPart.GetQuery();
        return string.Format(queryTemplate, args);
    }

    public override void PopulateSetQueryParts(List<IQueryPart> setQueryParts)
    {
        for (var i = 0; i < templateArguments.Length; i++) templateArguments[i].PopulateSetQueryParts(setQueryParts);
    }
}
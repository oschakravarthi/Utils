using SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using SubhadraSolutions.Utils.Kusto.Shared.Linq;
using SubhadraSolutions.Utils.Linq.Expressions;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

internal partial class KustoExpressionVisitor
{
    private Expression HandleMakeSeriesMethod(MethodCallExpression node)
    {
        if (node.Method.DeclaringType != typeof(KustoQueryableExtensions))
        {
            return null;
        }

        switch (node.Method.Name)
        {
            case nameof(KustoQueryableExtensions.MakeSeries):
                {
                    var onMember = node
                        .Arguments[2]
                        .GetMember();

                    var expressions = new QueryPartExpression[node.Arguments.Count];

                    for (var i = 0; i < node.Arguments.Count; i++)
                        expressions[i] = (QueryPartExpression)Visit(node.Arguments[i]);

                    var queryPart = new FormatQueryPart(
                        "{0}\r\n| make-series Aggregate={1} on {2} step {3} by By={4}\n| project Aggregate, By, On=" +
                        onMember.Name, expressions[0].QueryPart, expressions[1].QueryPart, expressions[2].QueryPart,
                        expressions[3].QueryPart, expressions[4].QueryPart, expressions[5].QueryPart);
                    var shouldSkipProjectingMerge = node.Arguments[5].IfLambdaAndIsOneOfTheParametersTheBody();

                    if (!shouldSkipProjectingMerge)
                    {
                        queryPart = new FormatQueryPart("{0}\r\n| project {1}", queryPart, expressions[5].QueryPart);
                    }

                    return new QueryPartExpression(node, queryPart);
                }
            case nameof(KustoQueryableExtensions.SeriesFitLine):
                {
                    var expression = (QueryPartExpression)Visit(node.Arguments[0]);
                    var member = node.Arguments[1].GetMember();
                    var queryPart =
                        new FormatQueryPart(
                            "{0}\r\n| extend(RSquare, Slope, Variance, RVariance, Interception, LineFit) = series_fit_line(" +
                            member.Name + ")", expression.QueryPart);
                    var shouldSkipProjectingMerge = node.Arguments[2].IfLambdaAndIsOneOfTheParametersTheBody();

                    if (!shouldSkipProjectingMerge)
                    {
                        var mergeExpression = (QueryPartExpression)Visit(node.Arguments[2]);
                        queryPart = new FormatQueryPart("{0}\r\n| project {1}", queryPart, mergeExpression.QueryPart);
                    }

                    return new QueryPartExpression(node, queryPart);
                }
            case nameof(KustoQueryableExtensions.SeriesStats):
                {
                    var expression = (QueryPartExpression)Visit(node.Arguments[0]);
                    var member = node.Arguments[1].GetMember();
                    var queryPart = new FormatQueryPart(
                        "{0}\r\n| extend(SeriesStatsReadsMin, SeriesStatsReadsMinIdx, SeriesStatsReadsMax, SeriesStatsReadsMaxIdx, SeriesStatsReadsAvg, SeriesStatsReadsStDev, SeriesStatsReadsVariance) = series_stats(" +
                        member.Name + ")", expression.QueryPart);
                    var shouldSkipProjectingMerge = node.Arguments[2].IfLambdaAndIsOneOfTheParametersTheBody();

                    if (!shouldSkipProjectingMerge)
                    {
                        var mergeExpression = (QueryPartExpression)Visit(node.Arguments[2]);
                        queryPart = new FormatQueryPart("{0}\r\n| project {1}", queryPart, mergeExpression.QueryPart);
                    }

                    return new QueryPartExpression(node, queryPart);
                }

            case nameof(KustoQueryableExtensions.SeriesOutliers):
                {
                    var expression = (QueryPartExpression)Visit(node.Arguments[0]);
                    var member = node.Arguments[1].GetMember();
                    // var temp = "__outliers" + GeneralHelper.Identity.ToString();
                    var queryPart = new FormatQueryPart("{0}\r\n| extend Outlier=series_outliers(" + member.Name + ")",
                        expression.QueryPart);
                    var shouldSkipProjectingMerge = node.Arguments[2].IfLambdaAndIsOneOfTheParametersTheBody();

                    if (!shouldSkipProjectingMerge)
                    {
                        var mergeExpression = (QueryPartExpression)Visit(node.Arguments[2]);
                        queryPart = new FormatQueryPart("{0}\r\n| project {1}", queryPart, mergeExpression.QueryPart);
                    }

                    return new QueryPartExpression(node, queryPart);
                }
        }

        return null;
    }
}
//using SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;
//using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
//using SubhadraSolutions.Utils.Kusto.Shared.Linq;
//using SubhadraSolutions.Utils.Data;
//using SubhadraSolutions.Utils.Linq;
//using SubhadraSolutions.Utils.Linq.Expressions;
//using SubhadraSolutions.Utils.Reflection;
//using System.Linq.Expressions;
//using System.Text;

//namespace SubhadraSolutions.Utils.Kusto.Server.Linq
//{
//    internal partial class KustoExpressionVisitor
//    {
//        private Expression HandleKustoQueryableMethods(MethodCallExpression node)
//        {
//            if (node.Method.DeclaringType != typeof(KustoQueryableExtensions)&& node.Method.DeclaringType != typeof(QueryableExtensions))
//            {
//                return null;
//            }

//            switch (node.Method.Name)
//            {
//                //case nameof(KustoQueryableExtensions.ToDynamic):
//                //    {
//                //        var part = Visit(node.Arguments[0]);
//                //        var partExp = part as QueryPartExpression;
//                //        return new QueryPartExpression(node, new FormatQueryPart("todynamic({0})", partExp.QueryPart));
//                //    }

//                case nameof(QueryableExtensions.Summarize):
//                    if (node.Arguments.Count == 4)
//                    {
//                        LambdaExpression mergeLambda = node.Arguments[3].GetLambda();

//                        ParameterExpression byParameter = mergeLambda.Parameters[0];
//                        ParameterExpression summaryParameter = mergeLambda.Parameters[1];
//                        bool isByPrimitive = byParameter.IsPrimitiveParameterExpression();
//                        bool isSummaryPrimitive = summaryParameter.IsPrimitiveParameterExpression();

//                        string template = "{0}\r\n| summarize " + (isSummaryPrimitive ? (summaryParameter.Name + "=") : null) + "{1} by " + (isByPrimitive ? (byParameter.Name + "=") : null) + "{2}";

//                        Expression part1 = Visit(node.Arguments[0]);

//                        Expression summaryPart = Visit(node.Arguments[2]);
//                        Expression byPart = Visit(node.Arguments[1]);
//                        bool shouldSkipProjectingMerge = node.Arguments[3].IfLambdaAndIsOneOfTheParametersTheBody();

//                        if (!shouldSkipProjectingMerge)
//                        {
//                            template += "\r\n| project {3}";
//                        }

//                        Expression mergePart = Visit(node.Arguments[3]);
//                        var part1Exp = (QueryPartExpression)part1;
//                        var summaryPartExp = (QueryPartExpression)summaryPart;
//                        var byPartExp = (QueryPartExpression)byPart;
//                        var mergePartExp = (QueryPartExpression)mergePart;
//                        return new QueryPartExpression(node, new FormatQueryPart(template, part1Exp.QueryPart, summaryPartExp.QueryPart, byPartExp.QueryPart, mergePartExp.QueryPart));
//                    }
//                    else
//                    {
//                        LambdaExpression summaryLambda = node.Arguments[1].GetLambda();
//                        ParameterExpression summaryParameter = summaryLambda.Parameters[0];
//                        bool isSummaryPrimitive = summaryLambda.ReturnType.IsPrimitiveOrExtendedPrimitive();

//                        string template = "{0}\r\n| summarize " + (isSummaryPrimitive ? (summaryParameter.Name + "=") : null) + "{1}";
//                        Expression part1 = Visit(node.Arguments[0]);
//                        var part1Exp = (QueryPartExpression)part1;
//                        Expression part2 = Visit(node.Arguments[1]);
//                        var part2Exp = (QueryPartExpression)part2;

//                        return new QueryPartExpression(node, new FormatQueryPart(template, part1Exp.QueryPart, part2Exp.QueryPart));
//                    }

//                case nameof(KustoQueryableExtensions.DatePartSummarize):
//                    {
//                        string dateTimePropertyName = ((ConstantExpression)node.Arguments[1]).Value as string;
//                        string measurePropertyName = ((ConstantExpression)node.Arguments[2]).Value as string;
//                        string datePart = ((ConstantExpression)node.Arguments[3]).Value as string;
//                        AggregationType aggregationType = (AggregationType)((ConstantExpression)node.Arguments[4]).Value;
//                        string method = aggregationType == AggregationType.Average ? "avg" : aggregationType.ToString().ToLower();

//                        string literal = string.Format("\r\n| summarize MeasureValue={0}(MeasureValue) by DatePart", method);

//                        Expression part = Visit(node.Arguments[0]);
//                        var partExp = (QueryPartExpression)part;
//                        return new QueryPartExpression(node, new CompositeQueryPart(partExp.QueryPart, new LiteralQueryPart(literal)));
//                    }

//                //case nameof(KustoQueryableExtensions.AppendQueryLiteral):
//                //    {
//                //        var queryLiteral = (node.Arguments[1] as ConstantExpression).Value as string;
//                //        var part = Visit(node.Arguments[0]);
//                //        var partExp = part as QueryPartExpression;
//                //        return new QueryPartExpression(node, new CompositeQueryPart(partExp.QueryPart, new LiteralQueryPart(queryLiteral)));
//                //    }
//                case nameof(KustoQueryableExtensions.MultiOrderBy):
//                    {
//                        Expression part = Visit(node.Arguments[0]);
//                        var partExp = (QueryPartExpression)part;

//                        var constExp = (ConstantExpression)node.Arguments[1];
//                        var sortInfos = (PropertySortingInfo[])constExp.Value;
//                        if (sortInfos.Length == 0)
//                        {
//                            return partExp;
//                        }

//                        var sb = new StringBuilder("\r\n| order by ");

//                        for (int i = 0; i < sortInfos.Length; i++)
//                        {
//                            if (i > 0)
//                            {
//                                sb.Append(", ");
//                            }

//                            PropertySortingInfo info = sortInfos[i];
//                            sb.AppendFormat("{0} {1}", info.PropertyName, info.SortingOrder == SortingOrder.Ascending ? "asc" : "desc");
//                        }

//                        return new QueryPartExpression(node, new CompositeQueryPart(partExp.QueryPart, new LiteralQueryPart(sb.ToString())));
//                    }

//                case nameof(KustoQueryableExtensions.ToScalar):
//                    {
//                        Expression part = Visit(node.Arguments[0]);
//                        var partExp = (QueryPartExpression)part;
//                        return new QueryPartExpression(node, new FormatQueryPart("toscalar({0})", partExp.QueryPart));
//                    }

//                case nameof(KustoQueryableExtensions.ProjectAway):
//                    {
//                        Expression part = Visit(node.Arguments[0]);
//                        var constExpression = ExpressionHelper.GetTypedExpression<ConstantExpression>(node.Arguments[1]);
//                        var propertiesToExclude = (string[])constExpression.Value;
//                        if (propertiesToExclude.Length == 0)
//                        {
//                            return part;
//                        }
//                        var partExp = (QueryPartExpression)part;
//                        var sb = new StringBuilder("| project-away ");
//                        for (int i = 0; i < propertiesToExclude.Length; i++)
//                        {
//                            var property = propertiesToExclude[i];
//                            if (i > 0)
//                            {
//                                sb.Append(",");
//                            }
//                            sb.Append(property);
//                        }
//                        return new QueryPartExpression(node, new FormatQueryPart("{0}\r\n{1}", partExp.QueryPart, new LiteralQueryPart(sb.ToString())));
//                    }
//            }

//            return null;
//        }
//    }
//}
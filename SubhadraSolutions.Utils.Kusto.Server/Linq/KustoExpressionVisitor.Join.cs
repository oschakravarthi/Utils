using SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using SubhadraSolutions.Utils.Kusto.Shared.Linq;
using SubhadraSolutions.Utils.Linq.Expressions;
using SubhadraSolutions.Utils.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

internal partial class KustoExpressionVisitor
{
    private Expression HandleJoinMethod(MethodCallExpression node)
    {
        if (node.Method.DeclaringType != typeof(Queryable) &&
            node.Method.DeclaringType != typeof(KustoQueryableExtensions))
        {
            return null;
        }

        switch (node.Method.Name)
        {
            case nameof(Queryable.Join):
                var joinKind = JoinKind.Inner;
                if (node.Arguments.Count > 5)
                {
                    var arg = node.Arguments[5];

                    if (arg is ConstantExpression { Value: JoinKind kind })
                    {
                        joinKind = kind;
                    }
                }

                var visitor0 = new KustoExpressionVisitor(queryContext);
                var part0 = visitor0.Visit(node.Arguments[0]);
                var part0Exp = (QueryPartExpression)part0;

                var visitor2 = new KustoExpressionVisitor(queryContext);
                var part2 = visitor2.Visit(node.Arguments[2]);
                var part2Exp = (QueryPartExpression)part2;

                // var queryTemplate0 = new FormatQueryPart("{0}\r\n| extend {1}", new ReferenceQueryPart(part0Exp.QueryPart), new ReferenceQueryPart(part2Exp.QueryPart));
                var queryTemplate0 = new FormatQueryPart("{0}\r\n| extend {1}", part0Exp.QueryPart, part2Exp.QueryPart);

                var visitor1 = new KustoExpressionVisitor(queryContext);
                var part1 = visitor1.Visit(node.Arguments[1]);
                var part1Exp = (QueryPartExpression)part1;

                var visitor3 = new KustoExpressionVisitor(queryContext);
                var part3 = visitor3.Visit(node.Arguments[3]);
                var part3Exp = (QueryPartExpression)part3;

                // var queryTemplate1 = new FormatQueryPart("{0}\r\n| extend {1}", new ReferenceQueryPart(part1Exp.QueryPart), new ReferenceQueryPart(part3Exp.QueryPart));
                var queryTemplate1 = new FormatQueryPart("{0}\r\n| extend {1}", part1Exp.QueryPart, part3Exp.QueryPart);

                var part0SetPart = part0Exp.QueryPart as SetQueryPart;
                var part1SetPart = part1Exp.QueryPart as SetQueryPart;

                // IQueryPart set0 = part0SetPart == null ? queryTemplate0 : new SetQueryPart(queryTemplate0);
                // IQueryPart set1 = part1SetPart == null ? queryTemplate1 : new SetQueryPart(queryTemplate1);

                var set0 = new SetQueryPart(queryTemplate0, part0SetPart?.IsTemplate == true, queryContext);
                var set1 = new SetQueryPart(queryTemplate1, part1SetPart?.IsTemplate == true, queryContext);
                // if (!set0.IsTemplate)
                // {
                //    this.queryContext.AddSetQueryPart(set0);
                // }
                // if (!set1.IsTemplate)
                // {
                //    this.queryContext.AddSetQueryPart(set1);
                // }

                var sb = new StringBuilder();
                // TODO:
                sb.Append("{0} \r\n| join kind=").Append(joinKind.ToString().ToLower()).Append(" ({1}) on ");

                // var keyType = ((node.Arguments[2] as UnaryExpression).Operand as LambdaExpression).ReturnType;
                // if (!ReflectionHelper.IsPrimitiveOrExtendedPrimitive(keyType))
                // {
                //    var keyProperties = keyType.GetProperties();
                //    for (int i = 0; i < keyProperties.Length; i++)
                //    {
                //        var keyProperty = keyProperties[i];
                //        if (i > 0)
                //        {
                //            sb.Append(", ");
                //        }
                //        sb.Append(string.Format("$left.{0}==$right.{0}", keyProperty.Name));
                //    }
                // }
                // else
                // {
                //    sb.Append("$left==$right");
                // }
                var clauseLeft = Visit(node.Arguments[2]);
                var clauseRight = Visit(node.Arguments[3]);

                var splitsLeft = ((QueryPartExpression)clauseLeft).QueryPart.GetQuery().Split(",");
                var splitsRight = ((QueryPartExpression)clauseRight).QueryPart.GetQuery().Split(",");
                for (var i = 0; i < splitsLeft.Length; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }

                    sb.AppendFormat("$left.{0}==$right.{1}", splitsLeft[i].Trim(), splitsRight[i].Trim());
                }

                var shouldSkipProjecting = node.Arguments[4].IfLambdaAndIsOneOfTheParametersTheBody();
                if (!shouldSkipProjecting)
                {
                    sb.Append(" | project {2}");
                }

                var lambda = node.Arguments[4].GetLambda();
                var parameterType = lambda.Parameters[0].Type;

                var hash = new HashSet<string>();
                if (!parameterType.IsPrimitiveOrExtendedPrimitive())
                {
                    var properties = parameterType.GetProperties();

                    for (var i = 0; i < properties.Length; i++)
                    {
                        var property = properties[i];
                        hash.Add(property.Name);
                    }
                }

                queryContext.ExistingMembers.Add(lambda.Parameters[1], hash);
                var projects = Visit(node.Arguments[4]);
                queryContext.ExistingMembers.Remove(lambda.Parameters[1]);

                var projectsExp = (QueryPartExpression)projects;
                // var finalQueryTemplate = new FormatQueryPart(sb.ToString(), new ReferenceQueryPart(set0), new ReferenceQueryPart(set1), projectsExp.QueryPart);
                var finalQueryTemplate = new FormatQueryPart(sb.ToString(), set0, set1, projectsExp.QueryPart);
                var isTemplate = part0SetPart?.IsTemplate == true || part1SetPart?.IsTemplate == true;

                var setQueryPart = new SetQueryPart(finalQueryTemplate, isTemplate, queryContext);
                // if (!setQueryPart.IsTemplate)
                // {
                //    this.queryContext.AddSetQueryPart(setQueryPart);
                // }
                return new QueryPartExpression(node, setQueryPart);
        }

        return null;
    }
}
using SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using SubhadraSolutions.Utils.Kusto.Shared.Linq;
using SubhadraSolutions.Utils.Linq;
using SubhadraSolutions.Utils.Linq.Expressions;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

internal partial class KustoExpressionVisitor(QueryContext queryContext) : ExpressionVisitor
{
    private readonly Dictionary<Expression, Expression> visitedExpressions = new(ExpressionEqualityComparer.Instance);

    public override Expression Visit(Expression node)
    {
#if DEBUG || DEV_PPE

        var exp = QueryPartExpression.GetActualExpression(node);
        var returned = base.Visit(exp);
        if (returned == null)
        {
            throw new Exception();
        }

        return returned;
#else
            if (visitedExpressions.TryGetValue(node, out Expression returned))
            {
                return returned;
            }
            //visitedExpressions.Add(node);
            //var parent = queryContext.Stack.Peek();
            //var treeNode = parent.AddChild(node);
            //queryContext.Stack.Push(treeNode);

            var exp = node is not QueryPartExpression partExpression ? node : partExpression.Expression;

            //queryContext.VisitedExpressions.Add(exp);
            if (!visitedExpressions.TryGetValue(exp, out returned))
            {
                returned = base.Visit(exp);
                visitedExpressions.Add(exp, returned);
            }
            //queryContext.Stack.Pop();
            if (returned == null)
            {
            }
            return returned;
#endif
    }

    protected override Expression VisitConditional(ConditionalExpression node)
    {
        var expTest = Visit(node.Test);
        var expTrue = Visit(node.IfTrue);
        var expFalse = Visit(node.IfFalse);

        if (((QueryPartExpression)expTrue).QueryPart == NullQueryPart.Instance)
        {
            expTrue = new QueryPartExpression(node.IfTrue,
                new LiteralQueryPart(GetQueryPartToReturnNullFromKusto(node.Type)));
        }

        if (((QueryPartExpression)expFalse).QueryPart == NullQueryPart.Instance)
        {
            expFalse = new QueryPartExpression(node.IfFalse,
                new LiteralQueryPart(GetQueryPartToReturnNullFromKusto(node.Type)));
        }

        var expTestPartExp = (QueryPartExpression)expTest;
        var expTruePartExp = (QueryPartExpression)expTrue;
        var expFalsePartExp = (QueryPartExpression)expFalse;

        return new QueryPartExpression(node,
            new FormatQueryPart("iff({0}, {1}, {2})", expTestPartExp.QueryPart, expTruePartExp.QueryPart,
                expFalsePartExp.QueryPart));
    }

    private static string GetQueryPartToReturnNullFromKusto(Type returnType)
    {
        if (returnType == typeof(string))
        //TODO: Need to check how to return null string from kusto
        {
            return "\"\"";
        }

        var method = KustoHelper.GetKustoScalarTypeName(returnType);
        return $"{method}(null)";
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        var returned = base.VisitParameter(node);
        var isPrimitiveParameterExpression = node.IsPrimitiveParameterExpression();

        if (isPrimitiveParameterExpression)
        {
            if (!queryContext.ParameterAliases.TryGetValue(node, out var name))
            {
                name = node.Name;
            }

            return new QueryPartExpression(returned, new LiteralQueryPart(name));
        }
        else
        {
            if (queryContext.ParameterAliases.TryGetValue(node, out var name))
            {
                return new QueryPartExpression(returned, new LiteralQueryPart(name));
            }
        }

        return new QueryPartExpression(returned, EmptyQueryPart.Instance);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        return HandleBinary(node);
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        if (node.Value is IQueryable queryable)
        {
            return queryable.Expression;
        }

        var nodeValue = node.Value;
        if (nodeValue == null)
        {
            return new QueryPartExpression(node, NullQueryPart.Instance);
        }

        if (node.Type.IsEnum)
        {
            var enumIntValue = (int)node.Value;
            return new QueryPartExpression(node, new LiteralQueryPart(enumIntValue.ToString()));
        }

        if (node.Type == typeof(bool) || node.Type == typeof(bool?))
        {
            var parameterPart = new ParameterQueryPart((bool)nodeValue, queryContext);
            return new QueryPartExpression(node, parameterPart);
        }

        if (node.Type == typeof(int) || node.Type == typeof(int?))
        {
            var parameterPart = new ParameterQueryPart((int)nodeValue, queryContext);
            return new QueryPartExpression(node, parameterPart);
        }

        if (node.Type == typeof(long) || node.Type == typeof(long?))
        {
            var parameterPart = new ParameterQueryPart((long)nodeValue, queryContext);
            return new QueryPartExpression(node, parameterPart);
        }

        if (node.Type == typeof(double) || node.Type == typeof(double?))
        {
            var parameterPart = new ParameterQueryPart((double)nodeValue, queryContext);
            return new QueryPartExpression(node, parameterPart);
        }

        if (node.Type == typeof(float) || node.Type == typeof(float?))
        {
            var parameterPart = new ParameterQueryPart((double)nodeValue, queryContext);
            return new QueryPartExpression(node, parameterPart);
        }

        if (node.Type == typeof(string))
        {
            var value = (string)nodeValue;
            return new QueryPartExpression(node, new ParameterQueryPart(value, queryContext));
        }

        if (node.Type == typeof(Guid) || node.Type == typeof(Guid?))
        {
            var value = (Guid)nodeValue;
            return new QueryPartExpression(node, new ParameterQueryPart(value, queryContext));
        }

        if (node.Type == typeof(char) || node.Type == typeof(char?))
        {
            var value = (char)nodeValue;
            return new QueryPartExpression(node, new ParameterQueryPart(value, queryContext));
        }

        if (node.Type.IsDateType())
        {
            var parameterPart = new ParameterQueryPart((DateTime)nodeValue, queryContext);
            return new QueryPartExpression(node, parameterPart);
        }

        if (node.Type == typeof(TimeSpan) || node.Type == typeof(TimeSpan?))
        {
            var parameterPart = new ParameterQueryPart((TimeSpan)nodeValue, queryContext);
            return new QueryPartExpression(node, parameterPart);
        }

        //if (node.Value.GetType().FullName.Contains("OData"))
        //{
        //    string s = node.Value.GetType().GetProperty("Property").GetValue(nodeValue).ToString();
        //    return new QueryPartExpression(node, new LiteralQueryPart(s));
        //}

        if (nodeValue is IEnumerable enumerable)
        {
            return HandleEnumerable(node, enumerable);
        }

        return new QueryPartExpression(node, new LiteralQueryPart(node.ToString()));
    }

    private Expression HandleEnumerable(Expression node, IEnumerable enumerable)
    {
        var sb = new StringBuilder();
        var parts = new List<IQueryPart>();
        var i = 0;
        foreach (var val in enumerable)
        {
            if (i > 0)
            {
                sb.Append(", ");
            }

            sb.Append($"{"{"}{i.ToString()}{"}"}");
            var part = Visit(Expression.Constant(val));
            var partExp = (QueryPartExpression)part;
            parts.Add(partExp.QueryPart);

            i++;
        }

        return new QueryPartExpression(node, new FormatQueryPart(sb.ToString(), parts.ToArray()));
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression.NodeType == ExpressionType.Parameter)
        {
            var memberName = GetMemberName(node.Member);

            if (queryContext.ExistingMembers.TryGetValue(node.Expression, out var existing) &&
                existing.Contains(memberName))
            {
                memberName += "1";
            }

            return new QueryPartExpression(node, new LiteralQueryPart(memberName));
        }

        if (node.NodeType == ExpressionType.MemberAccess)
        {
            if (node.Expression.NodeType == ExpressionType.Convert)
            {
                var unary = node.Expression as UnaryExpression;
                return Visit(Expression.MakeMemberAccess(unary.Operand, node.Member));
            }

            var partObject = (QueryPartExpression)Visit(node.Expression);

            if (partObject.Expression is ConstantExpression objExp)
            {
                var obj = objExp.Value;
                var value = node.Member.GetMemberValue(obj);
                return Visit(Expression.Constant(value));
            }
        }

        var returned = HandleStringMemberExpression(node);
        if (returned != null)
        {
            return returned;
        }

        returned = HandleDateTimeMemberExpression(node);
        if (returned != null)
        {
            return returned;
        }

        returned = HandleTimeSpanMemberExpression(node);
        if (returned != null)
        {
            return returned;
        }

        returned = HandleMathMemberExpression(node);
        if (returned != null)
        {
            return returned;
        }

        var part = Visit(node.Expression);
        var partExp = (QueryPartExpression)part;
        return new QueryPartExpression(node, partExp.QueryPart);
    }

    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        return Visit(node.Body);
    }

    protected override Expression VisitMemberInit(MemberInitExpression node)
    {
        var sb = new StringBuilder();
        var parts = new List<IQueryPart>();
        var indexer = 0;
        var mvExpands = new List<string>();

        for (var i = 0; i < node.Bindings.Count; i++)
        {
            var binding = node.Bindings[i];

            if (binding.BindingType == MemberBindingType.Assignment)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                var memberName = binding.Member.Name;
                var assignment = binding as MemberAssignment;

                if (HandleMVExpand(assignment.Expression, out var mvExpandMemberName, out var assignedFrom))
                {
                    mvExpands.Add(mvExpandMemberName);
                }

                sb.Append(memberName);

                if (memberName != assignedFrom)
                {
                    sb.Append("={").Append(indexer).Append('}');
                    parts.Add(new LiteralQueryPart(assignedFrom));
                    indexer++;
                }
            }
        }

        var query = BuildMVExpandQueryPrefix(mvExpands, sb.ToString());

        if (node.Bindings.Count > 0)
        {
            return new QueryPartExpression(node, new FormatQueryPart(query, parts.ToArray()));
        }

        return new QueryPartExpression(node, new LiteralQueryPart(string.Empty));
    }

    protected override Expression VisitNew(NewExpression node)
    {
        var mvExpands = new List<string>();
        var count = node.Arguments.Count;
        var parts = new IQueryPart[count];
        var sb = new StringBuilder();

        var parameters = node.Constructor.GetParameters();
        if (node.Members == null && parameters[0].Name == "Date")
        {
        }

        for (var i = 0; i < count; i++)
        {
            if (i > 0)
            {
                sb.Append(", ");
            }

            var arg = node.Arguments[i];

            if (HandleMVExpand(arg, out var mvExpandMemberName, out var assignedFrom))
            {
                mvExpands.Add(mvExpandMemberName);
            }

            parts[i] = new LiteralQueryPart(assignedFrom);

            if (node.Members != null)
            {
                var member = node.Members[i];

                if (member.Name != assignedFrom)
                {
                    sb.Append(member.Name);
                    sb.Append('=');
                }
            }
            //else
            //{
            //    var parameter = parameters[i];
            //    sb.Append(parameter.Name);
            //    sb.Append('=');
            //}

            sb.Append('{').Append(i).Append('}');
        }

        var query = BuildMVExpandQueryPrefix(mvExpands, sb.ToString());

        return new QueryPartExpression(node, new FormatQueryPart(query, parts));
    }

    private static string BuildMVExpandQueryPrefix(IList<string> mvExpands, string suffixQuery)
    {
        if (mvExpands.Count == 0)
        {
            return suffixQuery;
        }

        var sb1 = new StringBuilder();
        sb1.Append("| mv-expand ");

        for (var i = 0; i < mvExpands.Count; i++)
        {
            if (i > 0)
            {
                sb1.Append(',');
            }

            sb1.Append(mvExpands[i]);
        }

        return sb1 + "\r\n| project " + suffixQuery;
    }

    private bool HandleMVExpand(Expression expression, out string mvExpandMemberName, out string assignedFrom)
    {
        if (expression is MethodCallExpression { Method.Name: nameof(KustoQueryableExtensions.MVExpand) } callExp)
        {
            var mvExpandMember = callExp.Arguments[0].GetMember() as PropertyInfo;
            mvExpandMemberName = mvExpandMember.Name;

            var type = mvExpandMember.PropertyType.GetGenericArguments()[0];
            var kustoMethod = KustoHelper.GetKustoMethodToConvertFromDynamicTo(type);
            assignedFrom = kustoMethod == null
                ? mvExpandMember.Name
                : $"{kustoMethod}({mvExpandMember.Name})";
            return true;
        }

        var part = Visit(expression);
        var partExp = (QueryPartExpression)part;

        mvExpandMemberName = null;
        if (partExp.QueryPart == NullQueryPart.Instance)
        {
        }

        assignedFrom = partExp.QueryPart.GetQuery();
        return false;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.DeclaringType == typeof(QueryableExtensions))
        {
            switch (node.Method.Name)
            {
                case nameof(QueryableExtensions.RebasePreviousDateTimeRangeWindow):
                case nameof(QueryableExtensions.Cache):
                    return Visit(node.Arguments[0]);
            }
        }

        var returned = HandleQueryableMethods(node);
        if (returned != null)
        {
            return returned;
        }

        returned = HandleOperatorOverloadedMethod(node);
        if (returned != null)
        {
            return returned;
        }

        returned = HandleAggregateMethod(node);
        if (returned != null)
        {
            return returned;
        }

        returned = HandleDateTimeMethod(node);
        if (returned != null)
        {
            return returned;
        }

        returned = HandleTimeSpanMethod(node);
        if (returned != null)
        {
            return returned;
        }

        returned = HandleStringMethod(node);
        if (returned != null)
        {
            return returned;
        }

        returned = HandleDistinct(node);
        if (returned != null)
        {
            return returned;
        }

        returned = HandleJoinMethod(node);
        if (returned != null)
        {
            return returned;
        }

        returned = HandleUnionAndConcatMethods(node);
        if (returned != null)
        {
            return returned;
        }

        returned = HandleFormatMethod(node);
        if (returned != null)
        {
            return returned;
        }

        returned = HandleMathMethos(node);
        if (returned != null)
        {
            return returned;
        }

        returned = HandleKustoQueryableMethods(node);
        if (returned != null)
        {
            return returned;
        }

        returned = HandlePackMethod(node);
        if (returned != null)
        {
            return returned;
        }

        returned = HandleWindowFunctions(node);
        if (returned != null)
        {
            return returned;
        }

        returned = HandleMakeSeriesMethod(node);
        return returned;
    }

    private Expression HandleQueryableMethods(MethodCallExpression node)
    {
        if (node.Method.DeclaringType != typeof(Queryable) &&
            node.Method.DeclaringType != typeof(QueryableExtensions))
        {
            return null;
        }

        var args = new IQueryPart[2];

        switch (node.Method.Name)
        {
            case nameof(Queryable.First):
            case nameof(Queryable.FirstOrDefault):
                return HandleFirstAndFirstOrDefault(node);

            case nameof(Queryable.GroupBy):
                throw new KustoExpressionException(
                    "GroupBy is deprecated and no longer supported. Please use Summarize instead.");
            case nameof(Queryable.Where):
            case nameof(Queryable.Select):
            case nameof(Queryable.Take):
            case nameof(Queryable.Skip):
            case nameof(Queryable.OrderBy):
            case nameof(Queryable.ThenBy):
            case nameof(Queryable.OrderByDescending):
            case nameof(Queryable.ThenByDescending):
            case nameof(QueryableExtensions.Comment):
                var part1 = Visit(node.Arguments[0]);
                var part2 = Visit(node.Arguments[1]);

                var part1Exp = (QueryPartExpression)part1;
                var part2Exp = (QueryPartExpression)part2;
                args[0] = part1Exp.QueryPart;
                args[1] = part2Exp.QueryPart;
                break;

            default:
                return null;
        }

        string template = null;

        switch (node.Method.Name)
        {
            case nameof(Queryable.Where):
                template = "{0}\r\n| where {1}";
                break;

            case nameof(Queryable.Select):
                var lambda = node.Arguments[1].GetLambda();
                var parameter = lambda.Parameters[0];
                var isPrimitive = lambda.ReturnType.IsPrimitiveOrExtendedPrimitive();

                template = "{0}";

                var arg1Query = args[1].GetQuery();
                arg1Query = arg1Query.TrimStart('|', ' ');
                if (arg1Query.StartsWith("mv-expand", StringComparison.InvariantCultureIgnoreCase))
                {
                    template += " {1}";
                }
                else
                {
                    var shouldSkipProjecting = node.Arguments[1].IfLambdaAndIsOneOfTheParametersTheBody();

                    if (!shouldSkipProjecting)
                    {
                        var entityType = lambda.ReturnType;
                        var newExpression = lambda.Body as NewExpression;
                        var args1sb = new StringBuilder();
                        var args1 = new List<IQueryPart>();
                        if (newExpression != null)
                        {
                            var parameters = newExpression.Constructor.GetParameters();
                            for (var i = 0; i < newExpression.Arguments.Count; i++)
                            {
                                var part = Visit(newExpression.Arguments[i]);
                                var partExp = (QueryPartExpression)part;
                                var query = partExp.QueryPart.GetQuery();

                                var param = parameters[i];

                                if (param.Name == query)
                                {
                                    args1.Add(new LiteralQueryPart(param.Name));
                                }
                                else
                                {
                                    args1.Add(new FormatQueryPart("{0}={1}", new LiteralQueryPart(param.Name),
                                        partExp.QueryPart));
                                }

                                if (i > 0)
                                {
                                    args1sb.Append(',');
                                }

                                args1sb.Append("{" + i + "}");
                                //args[0] = part1Exp.QueryPart;
                            }

                            args[1] = new FormatQueryPart(args1sb.ToString(), args1.ToArray());
                        }
                        else
                        {
                            if (lambda.Body is MemberInitExpression memberInitExpression)
                            {
                                for (var i = 0; i < memberInitExpression.Bindings.Count; i++)
                                {
                                    var binding = memberInitExpression.Bindings[i] as MemberAssignment;
                                    var part = Visit(binding.Expression);
                                    var partExp = (QueryPartExpression)part;
                                    var query = partExp.QueryPart.GetQuery();

                                    var member = binding.Member;

                                    if (member.Name == query)
                                    {
                                        args1.Add(new LiteralQueryPart(member.Name));
                                    }
                                    else
                                    {
                                        args1.Add(new FormatQueryPart("{0}={1}", new LiteralQueryPart(member.Name),
                                            partExp.QueryPart));
                                    }

                                    if (i > 0)
                                    {
                                        args1sb.Append(',');
                                    }

                                    args1sb.Append("{" + i + "}");
                                }

                                args[1] = new FormatQueryPart(args1sb.ToString(), args1.ToArray());
                            }
                            //else
                            //{
                            //    var memberExpression = lambda.Body as MemberExpression;
                            //    if (memberExpression != null)
                            //    {
                            //        var part = Visit(memberExpression.Expression);
                            //        var partExp = (QueryPartExpression)part;
                            //        var query = partExp.QueryPart.GetQuery();

                            //        var member = memberExpression.Member;
                            //        if (member.Name == query)
                            //        {
                            //            args1.Add(new LiteralQueryPart(member.Name));
                            //        }
                            //        else
                            //        {
                            //            args1.Add(new FormatQueryPart("{0}={1}", new LiteralQueryPart(member.Name), partExp.QueryPart));
                            //        }
                            //        args1sb.Append("{0}");
                            //        args[1] = new FormatQueryPart(args1sb.ToString(), args1.ToArray());
                            //    }
                            //    //var memberAccessExpression = lambda.Body as propertyacc;
                            //}
                        }

                        template += "\r\n| project " + (isPrimitive ? parameter.Name + "=" : null) + "{1}";
                    }
                }

                break;

            case nameof(Queryable.Take):
                template = "{0}\r\n| take {1}";
                break;

            case nameof(Queryable.Skip):
                template = "{0}\r\n";
                var rowNumberColumnName = "__ROWNUMBER" + GeneralHelper.Identity;
                template += $"|\r\nextend {rowNumberColumnName} = row_number() ";
                template += $"|\r\nwhere {rowNumberColumnName} > ";
                template += $"|\r\nproject-away {rowNumberColumnName}";
                template += "{1}";
                break;

            case nameof(Queryable.OrderBy):
            case nameof(Queryable.ThenBy):
                template = "{0}\r\n| order by {1} asc";
                break;

            case nameof(Queryable.OrderByDescending):
            case nameof(Queryable.ThenByDescending):
                template = "{0}\r\n| order by {1} desc";
                break;

            case nameof(QueryableExtensions.Comment):
                template = "{0}\r\n// {1}\r\n";
                break;
        }

        if (args[1] == EmptyQueryPart.Instance)
        {
            return new QueryPartExpression(node, args[0]);
        }

        return new QueryPartExpression(node, new FormatQueryPart(template, args));
    }

    private Expression HandleFirstAndFirstOrDefault(MethodCallExpression node)
    {
        var part1 = Visit(node.Arguments[0]);

        var part1Exp = (QueryPartExpression)part1;
        const string template = "{0}\r\n| take 1";
        return new QueryPartExpression(node, new FormatQueryPart(template, part1Exp.QueryPart));
    }

    protected override Expression VisitNewArray(NewArrayExpression node)
    {
        var sb = new StringBuilder();
        var count = node.Expressions.Count;
        var parts = new IQueryPart[count];

        for (var i = 0; i < count; i++)
        {
            if (i > 0)
            {
                sb.Append(", ");
            }

            sb.Append($"{"{"}{i.ToString()}{"}"}");
            if (i > 0)
            {
                sb.Append(", ");
            }

            sb.Append($"{"{"}{i.ToString()}{"}"}");
            var part = Visit(node.Expressions[i]);
            var partExp = (QueryPartExpression)part;
            parts[i] = partExp.QueryPart;
        }

        return new QueryPartExpression(node, new FormatQueryPart(sb.ToString(), parts));
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
        return HandleUnary(node);
    }

    private static string GetMemberName(MemberInfo member)
    {
        var attribute = member.GetCustomAttribute<ColumnAttribute>();
        if (attribute == null)
        {
            return member.Name;
        }

        if (string.IsNullOrWhiteSpace(attribute.Name))
        {
            return member.Name;
        }

        return attribute.Name;
    }
}
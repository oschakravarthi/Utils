using SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using SubhadraSolutions.Utils.Kusto.Shared.Linq;
using System;
using System.Linq.Expressions;
using System.Text;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

internal partial class KustoExpressionVisitor
{
    private static Expression HandleMathMemberExpression(MemberExpression node)
    {
        if (node.Member.DeclaringType != typeof(Math) && node.Member.DeclaringType != typeof(MathF))
        {
            return null;
        }

        return null;
    }

    private Expression HandleMathMethos(MethodCallExpression node)
    {
        if (node.Method.DeclaringType != typeof(Math) && node.Method.DeclaringType != typeof(MathF))
        {
            return null;
        }

        // case nameof(Math.Cot):
        // case nameof(Math.Degrees):
        // case nameof(Math.Exp10):
        // case nameof(Math.Exp2):
        // case nameof(Math.Gamma):
        // case nameof(Math.IsFinite):
        // case nameof(Math.IsInf):
        // case nameof(Math.IsNaN):
        // case nameof(Math.LogGamma):
        // case nameof(Math.Not):
        // case nameof(Math.Radians):
        // case nameof(Math.Rand):
        // case nameof(Math.Range):
        // case nameof(Math.welch_test):
        switch (node.Method.Name)
        {
            case nameof(Math.Abs):
            case nameof(Math.Acos):
            case nameof(Math.Asin):
            case nameof(Math.Atan):
            case nameof(Math.Atan2):
            case nameof(Math.Ceiling):
            case nameof(Math.Cos):
            case nameof(Math.Exp):
            case nameof(Math.Floor):
            case nameof(Math.Log):
            case nameof(Math.Log10):
            case nameof(Math.Log2):
            case nameof(Math.PI):
            case nameof(Math.Pow):
            case nameof(Math.Round):
            case nameof(Math.Sign):
            case nameof(Math.Sin):
            case nameof(Math.Sqrt):
            case nameof(Math.Tan):
                return HandleStandardMathMethod(node);

            case nameof(Math.Min):
                return HandleStandardMathMethod(node, "min_of");

            case nameof(Math.Max):
                return HandleStandardMathMethod(node, "max_of");

            case nameof(Math.Acosh):
            case nameof(Math.Asinh):
            case nameof(Math.Atanh):
            case nameof(Math.BigMul):
            case nameof(Math.BitDecrement):
            case nameof(Math.BitIncrement):
            case nameof(Math.Cbrt):
            case nameof(Math.Clamp):
            case nameof(Math.CopySign):
            case nameof(Math.Cosh):
            case nameof(Math.DivRem):
            case nameof(Math.E):
            case nameof(Math.FusedMultiplyAdd):
            case nameof(Math.ILogB):
            case nameof(Math.MaxMagnitude):
            case nameof(Math.MinMagnitude):
            case nameof(Math.ScaleB):
            case nameof(Math.Sinh):
            case nameof(Math.Tanh):
            case nameof(Math.Tau):
            case nameof(Math.Truncate):
                throw new KustoExpressionException("This function is not supported.");
        }

        return null;
    }

    private Expression HandleStandardMathMethod(MethodCallExpression node, string methodName = null)
    {
        if (methodName == null)
        {
            methodName = node.Method.Name.ToLower();
        }

        var args = new IQueryPart[node.Arguments.Count];
        var sb = new StringBuilder();
        sb.Append(methodName).Append('(');

        for (var i = 0; i < node.Arguments.Count; i++)
        {
            if (i > 0)
            {
                sb.Append(", ");
            }

            sb.Append('{').Append(i).Append('}');
            var arg = Visit(node.Arguments[i]);
            var part = (QueryPartExpression)arg;
            args[i] = part.QueryPart;
        }

        sb.Append(')');
        return new QueryPartExpression(node, new FormatQueryPart(sb.ToString(), args));
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace SubhadraSolutions.Utils.Linq.Expressions;

public class GenericExpressionVisitor
{
    private readonly Dictionary<Expression, Expression> visitedExpressions = new(ExpressionEqualityComparer.Instance);

    public virtual Expression Visit(Expression exp)
    {
        if (exp == null)
        {
            return null;
        }

        if (!visitedExpressions.TryGetValue(exp, out var returned))
        {
            returned = VisitCore(exp);
            visitedExpressions.Add(exp, returned);
        }

        return returned;
    }

    protected static BinaryExpression UpdateBinary(BinaryExpression b, Expression left, Expression right,
        Expression conversion, bool isLiftedToNull, MethodInfo method)
    {
        if (left != b.Left || right != b.Right || conversion != b.Conversion || method != b.Method ||
            isLiftedToNull != b.IsLiftedToNull)
        {
            if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
            {
                return Expression.Coalesce(left, right, conversion as LambdaExpression);
            }

            return Expression.MakeBinary(b.NodeType, left, right, isLiftedToNull, method);
        }

        return b;
    }

    protected static ConditionalExpression UpdateConditional(ConditionalExpression c, Expression test,
        Expression ifTrue, Expression ifFalse)
    {
        if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
        {
            return Expression.Condition(test, ifTrue, ifFalse);
        }

        return c;
    }

    protected static InvocationExpression UpdateInvocation(InvocationExpression iv, Expression expression,
        IEnumerable<Expression> args)
    {
        if (args != iv.Arguments || expression != iv.Expression)
        {
            return Expression.Invoke(expression, args);
        }

        return iv;
    }

    protected static LambdaExpression UpdateLambda(LambdaExpression lambda, Type delegateType, Expression body,
        IEnumerable<ParameterExpression> parameters)
    {
        if (body != lambda.Body || parameters != lambda.Parameters || delegateType != lambda.Type)
        {
            return Expression.Lambda(delegateType, body, parameters);
        }

        return lambda;
    }

    protected static ListInitExpression UpdateListInit(ListInitExpression init, NewExpression nex,
        IEnumerable<ElementInit> initializers)
    {
        if (nex != init.NewExpression || initializers != init.Initializers)
        {
            return Expression.ListInit(nex, initializers);
        }

        return init;
    }

    protected static MemberExpression UpdateMemberAccess(MemberExpression m, Expression expression, MemberInfo member)
    {
        if (expression != m.Expression || member != m.Member)
        {
            return Expression.MakeMemberAccess(expression, member);
        }

        return m;
    }

    protected static MemberAssignment UpdateMemberAssignment(MemberAssignment assignment, MemberInfo member,
        Expression expression)
    {
        if (expression != assignment.Expression || member != assignment.Member)
        {
            return Expression.Bind(member, expression);
        }

        return assignment;
    }

    protected static MemberInitExpression UpdateMemberInit(MemberInitExpression init, NewExpression nex,
        IEnumerable<MemberBinding> bindings)
    {
        if (nex != init.NewExpression || bindings != init.Bindings)
        {
            return Expression.MemberInit(nex, bindings);
        }

        return init;
    }

    protected static MemberListBinding UpdateMemberListBinding(MemberListBinding binding, MemberInfo member,
        IEnumerable<ElementInit> initializers)
    {
        if (initializers != binding.Initializers || member != binding.Member)
        {
            return Expression.ListBind(member, initializers);
        }

        return binding;
    }

    protected static MemberMemberBinding UpdateMemberMemberBinding(MemberMemberBinding binding, MemberInfo member,
        IEnumerable<MemberBinding> bindings)
    {
        if (bindings != binding.Bindings || member != binding.Member)
        {
            return Expression.MemberBind(member, bindings);
        }

        return binding;
    }

    protected static MethodCallExpression UpdateMethodCall(MethodCallExpression m, Expression obj, MethodInfo method,
        IEnumerable<Expression> args)
    {
        if (obj != m.Object || method != m.Method || args != m.Arguments)
        {
            return Expression.Call(obj, method, args);
        }

        return m;
    }

    protected static NewExpression UpdateNew(NewExpression nex, ConstructorInfo constructor,
        IEnumerable<Expression> args, IEnumerable<MemberInfo> members)
    {
        if (args != nex.Arguments || constructor != nex.Constructor || members != nex.Members)
        {
            if (nex.Members != null)
            {
                return Expression.New(constructor, args, members);
            }

            return Expression.New(constructor, args);
        }

        return nex;
    }

    protected static NewArrayExpression UpdateNewArray(NewArrayExpression na, Type arrayType,
        IEnumerable<Expression> expressions)
    {
        if (expressions != na.Expressions || na.Type != arrayType)
        {
            if (na.NodeType == ExpressionType.NewArrayInit)
            {
                return Expression.NewArrayInit(arrayType.GetElementType(), expressions);
            }

            return Expression.NewArrayBounds(arrayType.GetElementType(), expressions);
        }

        return na;
    }

    protected static TypeBinaryExpression UpdateTypeIs(TypeBinaryExpression b, Expression expression, Type typeOperand)
    {
        if (expression != b.Expression || typeOperand != b.TypeOperand)
        {
            return Expression.TypeIs(expression, typeOperand);
        }

        return b;
    }

    protected static UnaryExpression UpdateUnary(UnaryExpression u, Expression operand, Type resultType,
        MethodInfo method)
    {
        if (u.Operand != operand || u.Type != resultType || u.Method != method)
        {
            return Expression.MakeUnary(u.NodeType, operand, resultType, method);
        }

        return u;
    }

    protected virtual Expression VisitBinary(BinaryExpression b)
    {
        var left = Visit(b.Left);
        var right = Visit(b.Right);
        var conversion = Visit(b.Conversion);
        return UpdateBinary(b, left, right, conversion, b.IsLiftedToNull, b.Method);
    }

    protected virtual MemberBinding VisitBinding(MemberBinding binding)
    {
        switch (binding.BindingType)
        {
            case MemberBindingType.Assignment:
                return VisitMemberAssignment((MemberAssignment)binding);

            case MemberBindingType.MemberBinding:
                return VisitMemberMemberBinding((MemberMemberBinding)binding);

            case MemberBindingType.ListBinding:
                return VisitMemberListBinding((MemberListBinding)binding);

            default:
                throw new Exception($"Unhandled binding type '{binding.BindingType}'");
        }
    }

    protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
    {
        List<MemberBinding> list = null;

        for (int i = 0, n = original.Count; i < n; i++)
        {
            var b = VisitBinding(original[i]);

            if (list != null)
            {
                list.Add(b);
            }
            else if (b != original[i])
            {
                list = new List<MemberBinding>(n);

                for (var j = 0; j < i; j++) list.Add(original[j]);

                list.Add(b);
            }
        }

        return list ?? (IEnumerable<MemberBinding>)original;
    }

    protected virtual Expression VisitConditional(ConditionalExpression c)
    {
        var test = Visit(c.Test);
        var ifTrue = Visit(c.IfTrue);
        var ifFalse = Visit(c.IfFalse);
        return UpdateConditional(c, test, ifTrue, ifFalse);
    }

    protected virtual Expression VisitConstant(ConstantExpression c)
    {
        if (c.Value is Expression exp)
        {
            exp = Visit(exp);
            return Expression.Constant(exp);
        }

        return c;
    }

    protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
    {
        var arguments = VisitExpressionList(initializer.Arguments);

        if (arguments != initializer.Arguments)
        {
            return Expression.ElementInit(initializer.AddMethod, arguments);
        }

        return initializer;
    }

    protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
    {
        List<ElementInit> list = null;

        for (int i = 0, n = original.Count; i < n; i++)
        {
            var init = VisitElementInitializer(original[i]);

            if (list != null)
            {
                list.Add(init);
            }
            else if (init != original[i])
            {
                list = new List<ElementInit>(n);

                for (var j = 0; j < i; j++) list.Add(original[j]);

                list.Add(init);
            }
        }

        return list ?? (IEnumerable<ElementInit>)original;
    }

    protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
    {
        if (original != null)
        {
            List<Expression> list = null;

            for (int i = 0, n = original.Count; i < n; i++)
            {
                var p = Visit(original[i]);

                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);

                    for (var j = 0; j < i; j++) list.Add(original[j]);

                    list.Add(p);
                }
            }

            if (list != null)
            {
                return list.AsReadOnly();
            }
        }

        return original;
    }

    protected virtual Expression VisitInvocation(InvocationExpression iv)
    {
        IEnumerable<Expression> args = VisitExpressionList(iv.Arguments);
        var expr = Visit(iv.Expression);
        return UpdateInvocation(iv, expr, args);
    }

    protected virtual Expression VisitLambda(LambdaExpression lambda)
    {
        var body = Visit(lambda.Body);
        return UpdateLambda(lambda, lambda.Type, body, lambda.Parameters);
    }

    protected virtual Expression VisitListInit(ListInitExpression init)
    {
        var n = VisitNew(init.NewExpression);
        var initializers = VisitElementInitializerList(init.Initializers);
        return UpdateListInit(init, n, initializers);
    }

    protected virtual Expression VisitMemberAccess(MemberExpression m)
    {
        var exp = Visit(m.Expression);
        return UpdateMemberAccess(m, exp, m.Member);
    }

    protected virtual Expression VisitMemberAndExpression(MemberInfo member, Expression expression)
    {
        return Visit(expression);
    }

    protected virtual ReadOnlyCollection<Expression> VisitMemberAndExpressionList(
        ReadOnlyCollection<MemberInfo> members, ReadOnlyCollection<Expression> original)
    {
        if (original != null)
        {
            List<Expression> list = null;

            for (int i = 0, n = original.Count; i < n; i++)
            {
                var p = VisitMemberAndExpression(members?[i], original[i]);

                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);

                    for (var j = 0; j < i; j++) list.Add(original[j]);

                    list.Add(p);
                }
            }

            if (list != null)
            {
                return list.AsReadOnly();
            }
        }

        return original;
    }

    protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
    {
        var e = Visit(assignment.Expression);
        return UpdateMemberAssignment(assignment, assignment.Member, e);
    }

    protected virtual Expression VisitMemberInit(MemberInitExpression init)
    {
        var n = VisitNew(init.NewExpression);
        var bindings = VisitBindingList(init.Bindings);
        return UpdateMemberInit(init, n, bindings);
    }

    protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
    {
        var initializers = VisitElementInitializerList(binding.Initializers);
        return UpdateMemberListBinding(binding, binding.Member, initializers);
    }

    protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
    {
        var bindings = VisitBindingList(binding.Bindings);
        return UpdateMemberMemberBinding(binding, binding.Member, bindings);
    }

    protected virtual Expression VisitMethodCall(MethodCallExpression m)
    {
        var obj = Visit(m.Object);
        IEnumerable<Expression> args = VisitExpressionList(m.Arguments);
        return UpdateMethodCall(m, obj, m.Method, args);
    }

    protected virtual NewExpression VisitNew(NewExpression nex)
    {
        IEnumerable<Expression> args = VisitMemberAndExpressionList(nex.Members, nex.Arguments);
        return UpdateNew(nex, nex.Constructor, args, nex.Members);
    }

    protected virtual Expression VisitNewArray(NewArrayExpression na)
    {
        IEnumerable<Expression> exprs = VisitExpressionList(na.Expressions);
        return UpdateNewArray(na, na.Type, exprs);
    }

    protected virtual Expression VisitParameter(ParameterExpression p)
    {
        return p;
    }

    protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
    {
        var expr = Visit(b.Expression);
        return UpdateTypeIs(b, expr, b.TypeOperand);
    }

    protected virtual Expression VisitUnary(UnaryExpression u)
    {
        var operand = Visit(u.Operand);
        return UpdateUnary(u, operand, u.Type, u.Method);
    }

    protected virtual Expression VisitUnknown(Expression expression)
    {
        throw new Exception($"Unhandled expression type: '{expression.NodeType}'");
    }

    private Expression VisitCore(Expression exp)
    {
        exp = exp.GetTheActualExpressionIfDecorated();

        switch (exp.NodeType)
        {
            case ExpressionType.Negate:
            case ExpressionType.NegateChecked:
            case ExpressionType.Not:
            case ExpressionType.Convert:
            case ExpressionType.ConvertChecked:
            case ExpressionType.ArrayLength:
            case ExpressionType.Quote:
            case ExpressionType.TypeAs:
            case ExpressionType.UnaryPlus:
                return VisitUnary((UnaryExpression)exp);

            case ExpressionType.Add:
            case ExpressionType.AddChecked:
            case ExpressionType.Subtract:
            case ExpressionType.SubtractChecked:
            case ExpressionType.Multiply:
            case ExpressionType.MultiplyChecked:
            case ExpressionType.Divide:
            case ExpressionType.Modulo:
            case ExpressionType.And:
            case ExpressionType.AndAlso:
            case ExpressionType.Or:
            case ExpressionType.OrElse:
            case ExpressionType.LessThan:
            case ExpressionType.LessThanOrEqual:
            case ExpressionType.GreaterThan:
            case ExpressionType.GreaterThanOrEqual:
            case ExpressionType.Equal:
            case ExpressionType.NotEqual:
            case ExpressionType.Coalesce:
            case ExpressionType.ArrayIndex:
            case ExpressionType.RightShift:
            case ExpressionType.LeftShift:
            case ExpressionType.ExclusiveOr:
            case ExpressionType.Power:
                return VisitBinary((BinaryExpression)exp);

            case ExpressionType.TypeIs:
                return VisitTypeIs((TypeBinaryExpression)exp);

            case ExpressionType.Conditional:
                return VisitConditional((ConditionalExpression)exp);

            case ExpressionType.Constant:
                return VisitConstant((ConstantExpression)exp);

            case ExpressionType.Parameter:
                return VisitParameter((ParameterExpression)exp);

            case ExpressionType.MemberAccess:
                return VisitMemberAccess((MemberExpression)exp);

            case ExpressionType.Call:
                return VisitMethodCall((MethodCallExpression)exp);

            case ExpressionType.Lambda:
                return VisitLambda((LambdaExpression)exp);

            case ExpressionType.New:
                return VisitNew((NewExpression)exp);

            case ExpressionType.NewArrayInit:
            case ExpressionType.NewArrayBounds:
                return VisitNewArray((NewArrayExpression)exp);

            case ExpressionType.Invoke:
                return VisitInvocation((InvocationExpression)exp);

            case ExpressionType.MemberInit:
                return VisitMemberInit((MemberInitExpression)exp);

            case ExpressionType.ListInit:
                return VisitListInit((ListInitExpression)exp);

            default:
                return VisitUnknown(exp);
        }
    }
}
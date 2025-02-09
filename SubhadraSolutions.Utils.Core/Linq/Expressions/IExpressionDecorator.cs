using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Linq.Expressions;

public interface IExpressionDecorator
{
    public Expression Expression { get; }
}
using System.Linq;

namespace SubhadraSolutions.Utils.Linq;

public interface IQueryableAction
{
    IQueryable Apply(IQueryable input);
}
using Aqua.Dynamic;
using Remote.Linq.Expressions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.ApiClient.Contracts;

public interface ILinqDataProvider
{
    ValueTask<DynamicObject> PostAsync(string url, Expression expression);

    ValueTask<DynamicObject> PostAsync(string url, Expression expression, CancellationToken cancellationToken);

    ValueTask<DynamicObject> PostAsync(string url, Expression expression, IDictionary<string, string> headers);

    ValueTask<DynamicObject> PostAsync(string url, Expression expression, IDictionary<string, string> headers, CancellationToken cancellationToken);
}
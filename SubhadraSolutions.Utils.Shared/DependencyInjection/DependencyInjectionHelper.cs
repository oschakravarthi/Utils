using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SubhadraSolutions.Utils.DependencyInjection;

public static class DependencyInjectionHelper
{
    public static ServiceCollection CloneServiceCollection(this IServiceCollection serviceCollection)
    {
        var cloned = new ServiceCollection();
        foreach (var serviceDescriptor in serviceCollection)
        {
            cloned.Add(serviceDescriptor);
        }

        return cloned;
    }
}
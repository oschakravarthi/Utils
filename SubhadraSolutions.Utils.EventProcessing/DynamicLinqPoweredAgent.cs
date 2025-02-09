using Microsoft.Extensions.DependencyInjection;
using SubhadraSolutions.Utils.EventProcessing.Agents;
using SubhadraSolutions.Utils.Linq;
using System;
using System.Collections.Concurrent;

namespace SubhadraSolutions.Utils.EventProcessing;

public class DynamicLinqPoweredAgent(IServiceProvider serviceProvider) : AbstractAgent(serviceProvider
    .GetRequiredService<IEventAggregator>())
{
    private readonly ConcurrentDictionary<Type, Delegate> compiledExpressions = new();

    public string Expression { get; set; }

    protected override void OnEvent(object sender, IEvent e)
    {
        var compiledExpression = compiledExpressions.GetOrAdd(e.GetType(), CompileExpression);
        var newPayload = compiledExpression.DynamicInvoke(e, serviceProvider);

        var newEvent = EventProcessingHelper.BuildGenericEvent(Topic, newPayload, e.Id);
        eventAggregator.PublishEvent(newEvent);
    }

    private Delegate CompileExpression(Type payloadType)
    {
        return DynamicLinqHelper.CompileDynamicLinqExpression(Expression, new Tuple<string, Type>("e", payloadType),
            new Tuple<string, Type>("serviceProvider", typeof(IServiceProvider)));
    }
}
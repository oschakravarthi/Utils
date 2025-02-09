using SubhadraSolutions.Utils.Contracts;
using System;

namespace SubhadraSolutions.Utils.EventProcessing;

public interface IEvent : IIdentity
{
    DateTime OccuredOn { get; }
    object PayloadObject { get; }
    public Guid? SourceEventId { get; }
    string Topic { get; }
}
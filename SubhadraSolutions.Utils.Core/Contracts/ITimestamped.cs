using System;

namespace SubhadraSolutions.Utils.Contracts;

public interface ITimestamped
{
    DateTime Timestamp { get; }
}
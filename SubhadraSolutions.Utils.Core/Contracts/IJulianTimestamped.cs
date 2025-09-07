using System;

namespace SubhadraSolutions.Utils.Contracts;

public interface IJulianTimestamped
{
    JulianDateTime Timestamp { get; }
}
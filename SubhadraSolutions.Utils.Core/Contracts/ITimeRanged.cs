using System;

namespace SubhadraSolutions.Utils.Contracts;

public interface ITimeRanged
{
    DateTime GetFromTimestamp();

    DateTime GetUptoTimestamp();
}
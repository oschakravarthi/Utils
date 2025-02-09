using System;

namespace SubhadraSolutions.Utils.Contracts;

public interface IToLocal<out T>
{
    T ToLocal(TimeZoneInfo timezone);
}
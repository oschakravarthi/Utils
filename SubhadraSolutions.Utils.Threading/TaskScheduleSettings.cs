using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Threading;

public sealed class TaskScheduleSettings
{
    public bool PerformTaskOnStart { get; set; }

    public List<TimeOnly> Timings { get; } = [];
}
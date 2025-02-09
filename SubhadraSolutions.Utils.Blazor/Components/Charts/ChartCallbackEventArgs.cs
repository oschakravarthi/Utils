using System;

namespace SubhadraSolutions.Utils.Blazor.Components.Charts;

internal sealed class ChartCallbackEventArgs(object selectedObject, string seriesName) : EventArgs
{
    public object SelectedObject { get; } = selectedObject;
    public string SeriesName { get; } = seriesName;
}
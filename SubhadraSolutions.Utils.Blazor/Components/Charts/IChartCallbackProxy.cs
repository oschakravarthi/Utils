using System;

namespace SubhadraSolutions.Utils.Blazor.Components.Charts;

internal interface IChartCallbackProxy
{
    event EventHandler<ChartCallbackEventArgs> OnObjectSelected;

    object GetCallbackAction();
}
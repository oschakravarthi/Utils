using ApexCharts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using System;

namespace SubhadraSolutions.Utils.Blazor.Components.Charts;

internal sealed class ChartCallbackHandler<T> : IChartCallbackProxy where T : class
{
    public event EventHandler<ChartCallbackEventArgs> OnObjectSelected;

    public object GetCallbackAction()
    {
        return RuntimeHelpers.TypeCheck(EventCallback.Factory.Create(this, (Action<SelectedData<T>>)DataPointSelected));
    }

    private void DataPointSelected(SelectedData<T> selected)
    {
        if (selected != null)
        {
            OnObjectSelected?.Invoke(this, new ChartCallbackEventArgs(selected.DataPoint.X, selected.Series.Name));
        }
        // selected.DataPoint.X
    }
}
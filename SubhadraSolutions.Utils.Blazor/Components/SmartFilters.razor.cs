using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Blazor.Components;

public partial class SmartFilters<T>
{
    [Parameter]
    public IDictionary<string, List<T>> Items { get; set; }

    private Dictionary<string, List<T>> selectedItems = [];

    [Parameter]
    public EventCallback<Dictionary<string, T[]>> OnFilterChanged { get; set; }

    private bool canFireEvents = false;

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender)
        {
            this.canFireEvents = true;
            InvokeSelectionChanged();
        }
    }

    private void SelectedChipsChanged(string key, MudChip<T>[] selectedChips)
    {
        if (!this.selectedItems.TryGetValue(key, out var values))
        {
            values = [];
            this.selectedItems.Add(key, values);
        }
        else
        {
            values.Clear();
        }
        var hasSelection = selectedChips?.Length > 0 && selectedChips.Length != this.Items.Count;
        if (hasSelection)
        {
            foreach (var chip in selectedChips)
            {
                if (chip is MudChip<T> gChip)
                {
                    values.Add(gChip.Value);
                }
            }
        }
        //else
        //{
        //    StateHasChanged();
        //}

        InvokeSelectionChanged();
    }

    private void InvokeSelectionChanged()
    {
        if (!this.canFireEvents)
        {
            return;
        }
        var arg = new Dictionary<string, T[]>();
        foreach (var kvp in this.Items)
        {
            var key = kvp.Key;
            if (this.selectedItems.TryGetValue(key, out var selectedValues))
            {
                if (selectedValues?.Count > 0 && selectedValues.Count != kvp.Value.Count)
                {
                    arg.Add(key, selectedValues.ToArray());
                }
            }
        }
        OnFilterChanged.InvokeAsync(arg);
    }

    private bool IsSelected(string key, T value)
    {
        if (!this.selectedItems.TryGetValue(key, out var values))
        {
            return true;
        }

        return values == null || values.Count == 0 || values.Contains(value);
    }
}
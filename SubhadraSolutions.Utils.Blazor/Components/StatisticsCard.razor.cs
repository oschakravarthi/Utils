using Microsoft.AspNetCore.Components;
using MudBlazor;
using SubhadraSolutions.Utils.Reflection;
using SubhadraSolutions.Utils.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Blazor.Components;

public partial class StatisticsCard : AbstractSmartComponent
{
    private Color color = Color.Primary;

    private string icon = null;

    private bool isInDataFetchingMode;

    private double? numericValue;

    private string stringValue;

    public StatisticsCard()
    {
        Outlined = true;
    }

    [Parameter] public string Documentation { get; set; }

    [Parameter] public bool? IsPercentage { get; set; }

    [Parameter] public bool? IsPositiveValuePositive { get; set; }

    [Parameter] public IEnumerable<NavigationInfo> Navigations { get; set; }

    [Parameter] public string Subtitle { get; set; }

    [Parameter] public string Title { get; set; }

    [Parameter] public object Value { get; set; }

    protected override async Task ResetAsync()
    {
        stringValue = null;

        if (Value == null)
        {
            return;
        }

        var obj = Value;
        if (TaskHelper.IsValueTask(Value))
        {
            isInDataFetchingMode = true;
            obj = await TaskHelper.GetObjectFromValueTask(Value).ConfigureAwait(false);
            isInDataFetchingMode = false;
        }

        var asString = obj.ToString();

        if (obj.GetType().IsNumericType())
        {
            var asDouble = Convert.ToDouble(asString);
            numericValue = asDouble;
            stringValue = NumberHelper.MinifyDouble(asDouble);
        }
        else
        {
            numericValue = null;
            stringValue = asString;
            if (obj is DateTime dateTime)
            {
                stringValue = dateTime.ToString("MMMM dd yyyy", DateTimeFormatInfo.InvariantInfo);
            }
            else
            {
                if (obj is TimeSpan timeSpan)
                {
                    stringValue = timeSpan.ToString(@"dd\.hh\:mm\:ss") + " days";
                }
            }
        }

        color = GetColor();
        icon = GetIcon();
    }

    private Color GetColor()
    {
        if (IsPercentage == null || !IsPercentage.Value)
        {
            return Color.Primary;
        }

        if (IsPositiveValuePositive == null)
        {
            return Color.Primary;
        }

        if (numericValue is null or double.NaN)
        {
            return Color.Primary;
        }

        var val = numericValue.Value;
        var isPP = IsPositiveValuePositive == true;
        if (val == 0)
        {
            return Color.Primary;
        }

        if (val > 0 || double.IsPositiveInfinity(val))
        {
            return isPP ? Color.Success : Color.Error;
        }

        return isPP ? Color.Error : Color.Success;
    }

    private string GetIcon()
    {
        if (IsPositiveValuePositive == null)
        {
            return null;
        }

        if (numericValue is null or double.NaN)
        {
            return null;
        }

        var val = numericValue.Value;
        if (val == 0)
        {
            return MudBlazor.Icons.Material.Filled.TrendingFlat;
        }

        if (val > 0 || double.IsPositiveInfinity(val))
        {
            return MudBlazor.Icons.Material.Filled.TrendingUp;
        }

        return MudBlazor.Icons.Material.Filled.TrendingDown;
    }
}
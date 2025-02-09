using Microsoft.AspNetCore.Components;
using MudBlazor;
using SubhadraSolutions.Utils.Blazor.Extensions;
using System;
using System.Reflection;

namespace SubhadraSolutions.Utils.Blazor.Components;

public class ExtendedDateRangePicker : MudDateRangePicker
{
    private static readonly FieldInfo _elementReferenceEndField = typeof(MudRangeInput<string>).GetField("_elementReferenceEnd", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo _elementReferenceStartField = typeof(MudRangeInput<string>).GetField("_elementReferenceStart", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo _firstDateFieldInfo = typeof(MudDateRangePicker).GetField("_firstDate", BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly FieldInfo _rangeInputField = typeof(MudDateRangePicker).GetField("_rangeInput", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo _secondDateFieldInfo = typeof(MudDateRangePicker).GetField("_secondDate", BindingFlags.NonPublic | BindingFlags.Instance);

    [Parameter]
    public int? MaxNumberOfDaysAllowed { get; set; }

    public void SelectLastNDays(int n)
    {
        DateTime from = MaxDate.Value.Date.AddDays(-(n - 1));
        DateTime upto = MaxDate.Value.Date;
        _firstDateFieldInfo.SetValue(this, from);
        _secondDateFieldInfo.SetValue(this, upto);
        StateHasChanged();
    }

    protected override string GetDayClasses(int month, DateTime day)
    {
        string style = base.GetDayClasses(month, day);
        return style;
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        FixAccessibilityCore();
    }

    private void FixAccessibilityCore()
    {
        var rangeInput = _rangeInputField.GetValue(this);
        if (rangeInput != null)
        {
            var elementReferenceStart = (ElementReference)_elementReferenceStartField.GetValue(rangeInput);
            var elementReferenceEnd = (ElementReference)_elementReferenceEndField.GetValue(rangeInput);

            elementReferenceStart.SetAttribute("aria-label", AdornmentAriaLabel);
            elementReferenceEnd.SetAttribute("aria-label", AdornmentAriaLabel);
        }
    }
}
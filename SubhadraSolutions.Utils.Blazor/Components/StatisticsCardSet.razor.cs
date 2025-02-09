using Microsoft.AspNetCore.Components;
using SubhadraSolutions.Utils.Reflection;
using SubhadraSolutions.Utils.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Blazor.Components;

public partial class StatisticsCardSet : AbstractSmartComponent
{
    private object actualValue;

    private bool isInDataFetchingMode;

    [Parameter] public object Value { get; set; }

    protected override async Task ResetAsync()
    {
        actualValue = null;

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

        actualValue = obj;
    }

    private List<KeyValuePair<string, object>> GetPropertiesAndValues()
    {
        var result = new List<KeyValuePair<string, object>>();
        var properties = actualValue.GetType().GetPublicProperties(true);
        for (var i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            var propertyTitle = property.GetMemberDisplayName(true);
            var propertyValue = property.GetValue(actualValue);
            result.Add(new KeyValuePair<string, object>(propertyTitle, propertyValue));
        }

        return result;
    }
}
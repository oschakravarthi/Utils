using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;

namespace SubhadraSolutions.Utils.Blazor.Extensions;

public static class ElementReferenceExtensions
{
    private static readonly PropertyInfo jsRuntimeProperty =
        typeof(WebElementReferenceContext).GetProperty("JSRuntime", BindingFlags.Instance | BindingFlags.NonPublic);

    public static IJSRuntime GetJSRuntime(this ElementReference elementReference)
    {
        if (elementReference.Context is not WebElementReferenceContext context)
        {
            return null;
        }

        return (IJSRuntime)jsRuntimeProperty.GetValue(context);
    }

    public static void SetAttribute(this ElementReference elementReference, string attributeName, object attributeValue)
    {
        elementReference.GetJSRuntime()
            ?.InvokeVoidAsync("setAttribute", elementReference, attributeName, attributeValue);
    }
}
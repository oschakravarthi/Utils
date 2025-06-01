//using MudBlazor;
//using SubhadraSolutions.Utils.Blazor.Extensions;
//using System.Reflection;

//namespace SubhadraSolutions.Utils.Blazor.Components;

//public class ExtendedAutocomplete<T> : MudAutocomplete<T>
//{
//    private void FixAccessibilityCore()
//    {
//        var _elementReferencetField = GetType().BaseType
//            .GetField("_elementReference", BindingFlags.Instance | BindingFlags.NonPublic);

//        var inputElement = (MudInput<T>)_elementReferencetField.GetValue(this);
//        if (inputElement != null)
//        {
//            var elementReference = inputElement.ElementReference;
//            elementReference.SetAttribute("autocomplete", "off");
//            elementReference.SetAttribute("aria-label", AdornmentAriaLabel);
//        }
//    }
//}
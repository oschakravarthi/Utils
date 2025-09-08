using MudBlazor;

namespace SubhadraSolutions.Utils.Blazor.Helpers
{
    public static class MudBlazorHelper
    {
        public static readonly Converter<object> StringConverter = new Converter<object>
        {
            SetFunc = value => value.ToString(),
            GetFunc = text => text.ToString(),
        };
    }
}

using Microsoft.JSInterop;
using System;

namespace SubhadraSolutions.Utils.Blazor
{
    public class ActionInfo<T>
    {
        public string Tooltip { get; set; }
        public string Icon { get; set; }
        public Action<T, IJSRuntime> Action { get; set; }
    }
}
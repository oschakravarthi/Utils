using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SubhadraSolutions.Utils.Blazor.Components
{
    public class ExtendedMudSelect<T> : MudSelect<T>
    {
        /// <summary>
        /// The color of the icon when <see cref="Icon"/> is set.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Inherit"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.Appearance)]
        public Color IconColor { get; set; } = Color.Inherit;
    }
}

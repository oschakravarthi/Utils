using Microsoft.AspNetCore.Components;
using MudBlazor;
using SubhadraSolutions.Utils.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Blazor.Components
{
    public partial class DynamicInputFormContent
    {
        [Parameter]
        public FieldSchema Schema { get; set; }

        [Parameter]
        public Dictionary<string, object> Model { get; set; }
    }
}

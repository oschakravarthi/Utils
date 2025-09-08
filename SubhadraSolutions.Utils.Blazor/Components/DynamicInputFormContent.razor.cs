using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using Newtonsoft.Json.Schema;

namespace SubhadraSolutions.Utils.Blazor.Components
{
    public partial class DynamicInputFormContent
    {
        [Parameter]
        public JSchema Schema { get; set; }
       
        [Parameter]
        public Dictionary<string, object> Model { get; set; }
    }
}

using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Blazor.Components
{
    public partial class StringKeyValuePairsTableComponent
    {
        [Parameter]
        public IEnumerable<StringKeyValuePair> Items { get; set; }
    }
}

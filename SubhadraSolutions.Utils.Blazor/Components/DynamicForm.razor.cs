using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SubhadraSolutions.Utils.Core.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Blazor.Components
{
    public partial class DynamicForm
    {
        private MudForm _form;
        private Dictionary<string, object> model = new();

        [Parameter]
        public FieldSchema Schema { get; set; }

        private async Task HandleSubmit()
        {
            await _form.Validate();

            if (_form.IsValid)
            {
                //// Example: serialize submitted values
                //var json = JsonSerializer.Serialize(model);
                //Console.WriteLine(json);
            }
        }
    }
}

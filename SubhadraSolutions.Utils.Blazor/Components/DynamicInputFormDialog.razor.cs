using Microsoft.AspNetCore.Components;
using MudBlazor;
using SubhadraSolutions.Utils.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Blazor.Components
{
    public partial class DynamicInputFormDialog
    {
       [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        private void Close()
        {
            MudDialog.Close(DialogResult.Ok(model));
        }
        private MudForm _form;
        private Dictionary<string, object> model = new();

        [Parameter]
        public FieldSchema Schema { get; set; }

        private async Task HandleSubmit()
        {
            await _form.Validate();

            if (_form.IsValid)
            {
                Close();
                //// Example: serialize submitted values
                //var json = JsonSerializer.Serialize(model);
                //Console.WriteLine(json);
            }
        }
    }
}

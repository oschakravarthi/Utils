using Microsoft.AspNetCore.Components;
using MudBlazor;
using SubhadraSolutions.Utils.Core.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Blazor.Components
{
    public partial class DynamicInputFormDialog
    {
        private static readonly Converter<object> StringConverter = new Converter<object>
        {
            SetFunc = value => value.ToString(),
            GetFunc = text => text.ToString(),
        };

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

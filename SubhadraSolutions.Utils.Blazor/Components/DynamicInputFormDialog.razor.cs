using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json.Schema;

namespace SubhadraSolutions.Utils.Blazor.Components;

public partial class DynamicInputFormDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    private void Close()
    {
        MudDialog.Close(DialogResult.Ok(this.Model));
    }
    private void Cancel()
    {
        MudDialog.Close(DialogResult.Cancel());
    }
    private MudForm _form;
    
    [Parameter]
    public Dictionary<string, object> Model { get; set; } = new();
    
    [Parameter]
    public JSchema Schema { get; set; }

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
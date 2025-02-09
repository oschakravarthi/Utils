using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor;
using System.Reflection;

namespace SubhadraSolutions.Utils.Blazor.Components;

public partial class GenericDialog<T>
{
    [Parameter] public T Component { get; set; }

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

    private void BuildRenderTreeCore(RenderTreeBuilder builder)
    {
        var componentType = typeof(T);
        var sequencer = new Sequencer();
        builder.OpenComponent(sequencer.Next, componentType);
        foreach (var property in componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!property.IsDefined(typeof(ParameterAttribute), true))
            {
                continue;
            }

            builder.AddAttribute(sequencer.Next, property.Name, property.GetValue(Component));
        }

        builder.CloseComponent();
    }

    private void Close()
    {
        MudDialog.Close(DialogResult.Ok(Component));
    }

    private RenderFragment RenderCoreComponents()
    {
        return BuildRenderTreeCore;
    }
}
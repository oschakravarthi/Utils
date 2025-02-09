using Microsoft.AspNetCore.Components;
using MudBlazor;
using SubhadraSolutions.Utils.Net.Http;
using System;

namespace SubhadraSolutions.Utils.Blazor.Components;

public partial class ExceptionViewerDialog : AbstractSmartComponent
{
    [Parameter] public Exception Exception { get; set; }

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

    private void Close()
    {
        MudDialog.Close(DialogResult.Ok(true));
    }

    private MarkupString GetExceptionBody()
    {
        if (Exception is HttpRequestExceptionEx serverException)
        {
            var response = (MarkupString)serverException.ServerResponse;
            return response;
        }

        return (MarkupString)Exception.ToString();
    }
}
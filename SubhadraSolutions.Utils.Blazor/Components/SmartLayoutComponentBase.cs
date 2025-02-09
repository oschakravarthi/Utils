using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Docs.Services;
using SubhadraSolutions.Utils.Logging;
using System;

namespace SubhadraSolutions.Utils.Blazor.Components;

public class SmartLayoutComponentBase : LayoutComponentBase, IDisposable
{
    [Inject] protected IDialogService DialogService { get; set; }
    [Inject] protected LayoutService LayoutService { get; set; }
    [Inject] protected NavigationManager NavigationManager { get; set; }
    [Inject] protected ISmartLogger SmartLogger { get; set; }
    [Inject] protected ISnackbar Snackbar { get; set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        LayoutService.MajorUpdateOccured -= LayoutServiceOnMajorUpdateOccured;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        LayoutService.MajorUpdateOccured += LayoutServiceOnMajorUpdateOccured;
        SmartLogger.OnExceptionLogged += SmartLogger_OnExceptionLogged;
    }

    private void LayoutServiceOnMajorUpdateOccured(object sender, EventArgs e)
    {
        StateHasChanged();
    }

    private void ShowExceptionDialog(Exception exception)
    {
        var parameters = new DialogParameters { { nameof(ExceptionViewerDialog.Exception), exception } };
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Medium,
            Position = DialogPosition.Center,
            BackdropClick = false
        };

        var dialog = DialogService.Show<ExceptionViewerDialog>("Exception", parameters, options);
    }

    private void SmartLogger_OnExceptionLogged(object sender, GenericEventArgs<Exception> e)
    {
        System.Console.WriteLine(e.Payload);
        ShowExceptionDialog(e.Payload);
    }
}
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SubhadraSolutions.Utils.Blazor.Helpers;
using SubhadraSolutions.Utils.Data.Annotations;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Blazor.Components;

public abstract class AbstractSmartComponentBase : MudComponentBase
{
    protected bool IsInAsyncCall { get; set; }

    [Inject] protected IServiceProvider ServiceProvider { get; set; }
    [Inject] protected IJSRuntime JSRuntime { get; set; }

    [Inject] protected NavigationManager NavManager { get; set; }

    [Inject]
    protected IDialogService DialogService { get; set; }

    [Parameter]
    public bool PrintView { get; set; }

    protected Dictionary<string, object> GetExistingParameters()
    {
        var dictionary = new Dictionary<string, object>();
        var type = GetType();
        var properties = type.GetProperties();
        for (var i = 0; i < properties.Length; i++)
        {
            var property = properties[i];
            if (property.DeclaringType == typeof(MudComponentBase))
            {
                continue;
            }

            if (!typeof(IComparable).IsAssignableFrom(property.PropertyType))
            {
                continue;
            }

            if (!property.IsDefined(typeof(ParameterAttribute), true))
            {
                continue;
            }

            if (property.IsDefined(typeof(ExcludeForComparisonAttribute), true))
            {
                continue;
            }

            var value = property.GetValue(this);
            dictionary.Add(property.Name, value);
        }

        return dictionary;
    }

    protected async void Navigate(NavigationInfo navigationInfo)
    {
        if (!EventCallback.Empty.Equals(navigationInfo.NavigationAction))
        {
            await navigationInfo.NavigationAction.InvokeAsync().ConfigureAwait(false);
            return;
        }

        await NavigateToUrl(navigationInfo.NavigationUrl, false, navigationInfo.Target == "_blank").ConfigureAwait(false);
    }

    protected Task NavigateToPage(Type toPageType, string suffix = null, bool forceLoad = false,
        bool inNewTab = false, IDictionary<string, object> parameters = null)
    {
        var query = BlazorHelper.BuildQueryForPageTranfer(this, toPageType, parameters, property => property.DeclaringType != typeof(MudComponentBase));
        if (suffix != null)
        {
            query += suffix;
        }

        return NavigateToUrl(query, forceLoad, inNewTab);
    }

    protected async Task NavigateToUrl(string url, bool forceLoad = false, bool inNewTab = false)
    {
        if (!inNewTab)
        {
            NavManager.NavigateTo(url, forceLoad);
        }
        else
        {
            await JSRuntime.InvokeAsync<object>("open", url, "_blank").ConfigureAwait(false);
        }
    }

    protected virtual void Nullify()
    {
    }

    protected override Task OnParametersSetAsync()
    {
        //await base.OnParametersSetAsync().ConfigureAwait(false);
        return ResetIfParametersAreChanged(false);
    }

    protected virtual void Reset()
    {
    }

    protected virtual Task ResetAsync()
    {
        return null;
        //Task.CompletedTask;
    }

    //protected async Task ResetIfParametersAreChanged(bool stateChanged)
    //{
    //    Nullify();

    //    var resetTask = ResetAsync();
    //    if (resetTask != null)
    //    {
    //        try
    //        {
    //            IsInAsyncCall = true;
    //            //if (resetTask.Status == TaskStatus.Created)
    //            //{
    //            //    resetTask.Start();
    //            //}
    //            await resetTask.ConfigureAwait(false);
    //        }
    //        finally
    //        {
    //            IsInAsyncCall = false;
    //        }
    //    }
    //    else
    //    {
    //        Reset();
    //    }

    //    if (stateChanged)
    //    {
    //        StateHasChanged();
    //    }
    //}

    protected async Task ResetIfParametersAreChanged(bool stateChanged)
    {
        Nullify();
        try
        {
            IsInAsyncCall = true;
            var resetTask = ResetAsync();
            if (resetTask != null)
            {

                //if (resetTask.Status == TaskStatus.Created)
                //{
                //    resetTask.Start();
                //}
                await resetTask.ConfigureAwait(false);

            }
            else
            {
                Reset();
            }
        }
        finally
        {
            IsInAsyncCall = false;
        }
        if (stateChanged)
        {
            StateHasChanged();
        }
    }

    public virtual string RenderHtml()
    {
        var type = this.GetType();
        var dictionary = new Dictionary<string, object>();
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (property.IsDefined<ParameterAttribute>(true, true))
            {
                var value = property.GetValue(this);
                dictionary.Add(property.Name, value);
            }
        }
        var templater = new Templater.Templater();
        templater.AddServiceProvider(this.ServiceProvider);

        var result = templater.RenderComponent(type, dictionary);

        return result;
    }
}
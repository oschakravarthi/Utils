using Microsoft.AspNetCore.Components;
using System;
using System.Linq;

namespace SubhadraSolutions.Utils.Blazor.Extensions;

public static class NavigationManagerExtensions
{
    /// <summary>
    ///     Gets the section part of the documentation page
    ///     Ex: /components/button;  "components" is the section
    /// </summary>
    public static string GetSection(this NavigationManager navMan)
    {
        // get the absolute path with out the base path
        var currentUri = navMan.Uri.Remove(0, navMan.BaseUri.Length - 1);

        return currentUri
            .Split("/", StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault();
    }

    /// <summary>
    ///     Determines if the current page is the base page
    /// </summary>
    public static bool IsHomePage(this NavigationManager navMan)
    {
        return navMan.Uri == navMan.BaseUri;
    }
}
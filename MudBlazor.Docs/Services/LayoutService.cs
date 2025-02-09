// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//using MudBlazor.Docs.Services.UserPreferences;

using System;

namespace MudBlazor.Docs.Services;

public class LayoutService
{
    private readonly MudTheme _defaultTheme = BuildTheme();
    private MudTheme _currentTheme;

    public MudTheme CurrentTheme
    {
        get => _currentTheme ?? _defaultTheme;
        private set => _currentTheme = value;
    }

    public bool IsDarkMode { get; private set; }
    public bool IsRTL { get; private set; }

    public event EventHandler MajorUpdateOccured;

    public void SetBaseTheme(MudTheme theme)
    {
        CurrentTheme = theme;
        OnMajorUpdateOccured();
    }

    public void SetDarkMode(bool value)
    {
        IsDarkMode = value;
    }

    public void ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
        OnMajorUpdateOccured();
    }

    public void ToggleRightToLeft()
    {
        IsRTL = !IsRTL;
        OnMajorUpdateOccured();
    }

    private static MudTheme BuildTheme()
    {
        var theme = Theme.Theme.DocsTheme();
        theme.PaletteLight.Success = Colors.Green.Darken1;
        theme.PaletteDark.Primary = "#8474ff";
        //theme.PaletteDark.Background = Colors.Shades.White;
        theme.PaletteLight.Background = Colors.Shades.White;
        return theme;
    }

    private void OnMajorUpdateOccured()
    {
        MajorUpdateOccured?.Invoke(this, EventArgs.Empty);
    }
}
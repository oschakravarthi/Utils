using MudBlazor;

namespace SubhadraSolutions.Utils.Blazor.Components;

public class ExtendedMudDataGrid<T> : MudDataGrid<T>
{
    public ExtendedMudDataGrid()
    {
        Elevation = 3;
        Outlined = false;
        Square = false;
    }
}
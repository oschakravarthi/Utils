using Microsoft.AspNetCore.Components;

namespace SubhadraSolutions.Utils.Blazor.Components;

public partial class Footer
{
    public Footer()
    {
        Elevation = 25;
        //var color = ColorPalettes.GetRandomLightColor();
        //var color = "#1772ba";
        //var color = "#F8F8F8";
        ////var color = "#F0F0F0";
        //this.Style = ($"background:{color};");
        //this.Class = "mud-theme-dark";

        this.Outlined = true;
    }

    [Inject]
    private ProductInfo ProductInfo { get; set; }

    [Inject]
    private CompanyInfo CompanyInfo { get; set; }
}
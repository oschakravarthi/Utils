using SubhadraSolutions.Utils.Text;
using System;

namespace SubhadraSolutions.Utils.Data.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class GridColumnAttribute : Attribute
{
    public GridColumnAttribute(bool filterable = true, TextAlignment textAlignment = TextAlignment.Left,
        string cssClass = null, string htmlTag = null)
    {
        this.Filterable = filterable;
        this.TextAlign = textAlignment;
        this.CssClass = cssClass;
        this.HtmlTag = htmlTag;
        //Width = 50;
    }

    public GridColumnAttribute()
    {
        Filterable = true;
        TextAlign = TextAlignment.Left;
    }

    public string HtmlTag { get; }
    public string CssClass { get; }
    public bool Filterable { get; }
    public TextAlignment TextAlign { get; }

    //public string Width { get; set; }
}
using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Properties;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Web;

namespace SubhadraSolutions.Utils.Web;

public static class HtmlHelper
{
    //public const string CellStyle = "border-width: 1px;padding: 8px;border-style: solid;border-color: #666666;background-color: #ffffff;text-align: left;";

    //public const string HeaderCellStyle = "border-width: 1px;padding: 8px;border-style: solid;border-color: #666666;background-color: #dedede;text-align: center;";

    //public const string NumericCellStyle = "border-width: 1px;padding: 8px;border-style: solid;border-color: #666666;background-color: #ffffff;text-align: right;";

    //public const string TableStyle = "font-family: verdana,arial,sans-serif;font-size:11px;color:#333333;border-width: 1px;border-color: #666666;border-collapse: collapse;";

    public static readonly MethodInfo WriteCSSMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteCSS), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteLinkMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteLink), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteObjectBeginMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteObjectBegin), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteObjectChildrenBeginMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteObjectChildrenBegin), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteObjectChildrenEndMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteObjectChildrenEnd), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteObjectEndMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteObjectEnd), BindingFlags.Public | BindingFlags.Static);

    //    writer.WriteLine("</head>");
    //}
    public static readonly MethodInfo WritePageHeaderMethod =
        typeof(HtmlHelper).GetMethod(nameof(WritePageHeader), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteRawMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteRaw), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteRootObjectBeginMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteRootObjectBegin), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteRootObjectEndMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteRootObjectEnd), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteSpaceMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteSpace), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteTableBeginMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteTableBegin), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteTableBodyBeginMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteTableBodyBegin), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteTableBodyEndMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteTableBodyEnd), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteTableCellBeginMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteTableCellBegin), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteTableCellEndMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteTableCellEnd), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteTableCellLabelMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteTableCellLabel), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteTableCellMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteTableCell), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteTableEndMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteTableEnd), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteTableHeaderBeginMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteTableHeaderBegin), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteTableHeaderCellMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteTableHeaderCell), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteTableHeaderEndMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteTableHeaderEnd), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteTableRowBeginMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteTableRowBegin), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteTableRowEndMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteTableRowEnd), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo WriteTextContentMethod =
        typeof(HtmlHelper).GetMethod(nameof(WriteTextContent), BindingFlags.Public | BindingFlags.Static);

    public static string HtmlTableCSS => Resources.HtmlTableCSS;

    [DynamicallyInvoked]
    public static void WriteCSS(this TextWriter writer, string style = null)
    {
        writer.WriteLine("<style>");
        if (style != null)
        {
            writer.WriteLine(style);
        }

        writer.WriteLine(HtmlTableCSS);
        writer.WriteLine("</style>");
    }

    [DynamicallyInvoked]
    public static void WriteLink(this TextWriter writer, string url, string text = null, string target = null)
    {
        if (string.IsNullOrEmpty(text))
        {
            text = url;
        }

        if (string.IsNullOrEmpty(target))
        {
            target = "_blabk";
        }

        writer.Write($"<a href=\"{url}\" target=\"{target}\">{text}</a>");
    }

    [DynamicallyInvoked]
    public static void WriteObjectBegin(this TextWriter writer, string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            writer.WriteLine("<li>");
        }
        else
        {
            writer.WriteLine("<li data-caption=\"" + name + "\">");
        }
    }

    [DynamicallyInvoked]
    public static void WriteObjectChildrenBegin(this TextWriter writer)
    {
        writer.WriteLine("<ul>");
    }

    [DynamicallyInvoked]
    public static void WriteObjectChildrenEnd(this TextWriter writer)
    {
        writer.WriteLine("</ul>");
    }

    [DynamicallyInvoked]
    public static void WriteObjectEnd(this TextWriter writer)
    {
        writer.WriteLine("</li>");
    }

    //    writer.WriteLine("<script src=\"resources/table.js\"></script>");
    //    writer.WriteLine("<link rel=\"stylesheet\" href=\"resources/table.css\" media=\"all\">");
    //    writer.Write("<link rel=\"stylesheet\" href=\"https://metroui.org.ua/css/site.css\">");
    //    WriteCSS(writer);
    //    //writer.WriteLine("<script type=\"text/javascript\" src=\"resources/table.js\"></script>");
    //    //writer.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"resources/table.css\" media=\"all\">");
    //    //WriteStyles(writer);
    [DynamicallyInvoked]
    public static void WritePageHeader(this TextWriter writer, string title, string style = null)
    {
        writer.WriteLine("<head>");
        if (!string.IsNullOrWhiteSpace(title))
        {
            writer.WriteLine("<title>" + title + "</title>");
        }

        writer.WriteCSS(style);
        writer.WriteLine("</head>");
    }

    //public static readonly MethodInfo WritePageHeaderMethod = typeof(HtmlHelper).GetMethod(nameof(WritePageHeader), BindingFlags.Public | BindingFlags.Static);
    //[DynamicallyInvoked]
    //public static void WritePageHeader(TextWriter writer, string title)
    //{
    //    writer.WriteLine("<head>");
    //    if (!string.IsNullOrWhiteSpace(title))
    //    {
    //        writer.WriteLine("<title>" + title + "</title>");
    //    }
    //    writer.WriteLine("<meta charset=\"utf-8\">");
    //    writer.WriteLine("<link rel=\"stylesheet\" href=\"https://cdn.metroui.org.ua/v4.3.2/css/metro-all.min.css\">");
    //    writer.WriteLine("<script src=\"https://cdn.metroui.org.ua/v4.3.2/js/metro.min.js\"></script>");
    [DynamicallyInvoked]
    public static void WriteRaw(this TextWriter writer, string raw)
    {
        writer.Write(raw);
    }

    //public static void WriteStyles(TextWriter writer, string tableStyle, string headerCellStyle, string numericCellStyle, string cellStyle)
    //{
    //    writer.Write("<style type=\"text/css\">\r\n");
    //    writer.Write("table.tbl\r\n{\t" + tableStyle + "\r\n}\r\n");
    //    writer.Write("th.h\r\n{\r\n\t" + headerCellStyle + "\r\n}\r\n");
    //    writer.Write("td.n\r\n{\r\n\t" + numericCellStyle + "\r\n}\r\n");
    //    writer.Write("td.c\r\n{\r\n\t" + cellStyle + "\r\n}\r\n");
    //    writer.Write("</style>\r\n");
    //}
    //public static void WriteStyles(TextWriter writer)
    //{
    //    WriteStyles(writer, TableStyle, HeaderCellStyle, NumericCellStyle, CellStyle);
    //}
    [DynamicallyInvoked]
    public static void WriteRootObjectBegin(this TextWriter writer, string name)
    {
        writer.WriteLine("<div class=\"example\">");
        writer.WriteLine("<ul data-role=\"treeview\">");
        writer.WriteObjectBegin(name);
    }

    [DynamicallyInvoked]
    public static void WriteRootObjectEnd(this TextWriter writer)
    {
        writer.WriteObjectEnd();
        writer.WriteLine("</ul>");
        writer.WriteLine("</div>");
    }

    public static void WriteSortableTableHeaderCell(this TextWriter writer, string column, Type columnType)
    {
        var style = "default";
        if (columnType.IsNumericType())
        {
            style = "numeric";
        }
        else
        {
            if (columnType == typeof(DateTime))
            {
                style = "date";
            }
        }

        writer.WriteLine("<th class=\"h table-sortable:" + style + " table-sortable" + "\" title=\"Click to sort\">" +
                         column + "</th>\r\n");
    }

    [DynamicallyInvoked]
    public static void WriteSpace(this TextWriter writer)
    {
        writer.Write("<span> </span>");
    }

    [DynamicallyInvoked]
    public static void WriteTableBegin(this TextWriter writer)
    {
        writer.WriteLine("<table class=\"tbl table-autosort\" cellspacing=\"0\">");
    }

    [DynamicallyInvoked]
    public static void WriteTableBodyBegin(this TextWriter writer)
    {
        writer.WriteLine("<tbody>");
    }

    [DynamicallyInvoked]
    public static void WriteTableBodyEnd(this TextWriter writer)
    {
        writer.WriteLine("\r\n</tbody>");
    }

    [DynamicallyInvoked]
    public static void WriteTableCell(this TextWriter writer, object value, bool isNumeric = false,
        bool isMultiline = false, bool isError = false, string href = null)
    {
        writer.WriteTableCellBegin(isNumeric, isMultiline, isError);
        if (value != null)
        {
            var valueString = value.ToString();
            if (!string.IsNullOrEmpty(valueString))
            {
                //if (isMultiline)
                //{
                //    writer.Write("<textarea data-role=\"textarea\">");
                //}
                var hasHref = !string.IsNullOrEmpty(href);
                if (hasHref)
                {
                    writer.Write($"<a href=\"{href}\">");
                }

                if (isNumeric)
                {
                    writer.Write(valueString);
                }
                else
                {
                    valueString = valueString.Trim();
                    valueString = HttpUtility.HtmlEncode(valueString);
                    valueString = valueString.Replace(Environment.NewLine, "</p><p>");
                    writer.Write("<p>");
                    writer.Write(valueString);
                    writer.Write("</p>");
                }

                if (hasHref)
                {
                    writer.Write("</a>");
                }
                //if (isMultiline)
                //{
                //    writer.Write("</textarea>");
                //}
            }
        }

        writer.WriteTableCellEnd();
    }

    [DynamicallyInvoked]
    public static void WriteTableCellBegin(this TextWriter writer, bool? isNumeric = null, bool isMultiline = false,
        bool isError = false, int rowspan = 1, int colspan = 1)
    {
        var text = $"<td class=\"{(isNumeric == null ? "g" : isNumeric.Value ? "n" : "c")}\"";
        if (rowspan != 1)
        {
            text += $" rowspan=\"{rowspan}\"";
        }

        if (colspan != 1)
        {
            text += $" colspan=\"{colspan}\"";
        }

        text += ">";
        writer.Write(text);
    }

    [DynamicallyInvoked]
    public static void WriteTableCellEnd(this TextWriter writer)
    {
        writer.Write("</td>");
    }

    //public static void WriteIndent(TextWriter writer, int indent = 0)
    //{
    //    indent = Math.Max(0, indent);
    //    for(int i=0;i<indent;i++)
    //    {
    //        writer.Write("\t");
    //    }
    //}
    [DynamicallyInvoked]
    public static void WriteTableCellLabel(this TextWriter writer, string value)
    {
        writer.Write("<td class=\"c\">");
        writer.Write(value);
        writer.WriteLine("</td>");
    }

    [DynamicallyInvoked]
    public static void WriteTableEnd(this TextWriter writer)
    {
        writer.WriteLine("</table>");
    }

    [DynamicallyInvoked]
    public static void WriteTableHeaderBegin(this TextWriter writer)
    {
        writer.Write("<thead style=\"display:table-header-group\">");
    }

    [DynamicallyInvoked]
    public static void WriteTableHeaderCell(this TextWriter writer, string column)
    {
        writer.Write("<th class=\"h\">");
        writer.Write(column);
        writer.WriteLine("</th>");
    }
    public static void WriteTableHeaderCells(this TextWriter w, params string[] headers)
    {
        foreach (var header in headers)
        {
            w.WriteTableHeaderCell(header);
        }
    }

    [DynamicallyInvoked]
    public static void WriteTableHeaderEnd(this TextWriter writer)
    {
        writer.WriteLine("</thead>");
    }

    public static void WriteTableHeaderFilterCell(this TextWriter writer, int index)
    {
        writer.WriteLine("<th class=\"h\"><input class=\"filter\" name='filter" +
                         index.ToString(CultureInfo.InvariantCulture) +
                         "' width=\"100%\" onfocus=\"hideWatermark(this);\" onblur=\"showWatermark(this);\" onkeyup=\"DelayedOnKeyUp(this,1000);\"></th>");
    }

    [DynamicallyInvoked]
    public static void WriteTableRowBegin(this TextWriter writer)
    {
        writer.Write("<tr>");
    }

    [DynamicallyInvoked]
    public static void WriteTableRowEnd(this TextWriter writer)
    {
        writer.WriteLine("</tr>");
    }

    [DynamicallyInvoked]
    public static void WriteTextContent(this TextWriter writer, string content, bool multiline = false)
    {
        if (!multiline)
        {
            writer.Write("<span>");
            writer.Write(content);
            writer.Write("</span>");
            return;
        }

        writer.Write("<textarea data-role=\"textarea\">");
        writer.Write(content);
        writer.Write("</textarea>");
    }

    //public static void WriteResources(string targetDirectory)
    //{
    //    targetDirectory = targetDirectory.Trim().TrimEnd(Path.DirectorySeparatorChar);
    //    var resourcesDirectory = targetDirectory + Path.DirectorySeparatorChar + "resources";
    //    if (!Directory.Exists(resourcesDirectory))
    //    {
    //        Directory.CreateDirectory(resourcesDirectory);
    //    }
    //    Resources.filter.Save(resourcesDirectory + Path.DirectorySeparatorChar + "filter.png");
    //    Resources.sort_asc.Save(resourcesDirectory + Path.DirectorySeparatorChar + "sort_asc.gif");
    //    Resources.sort_desc.Save(resourcesDirectory + Path.DirectorySeparatorChar + "sort_desc.gif");
    //    Resources.sortable.Save(resourcesDirectory + Path.DirectorySeparatorChar + "sortable.gif");
    //    Resources.sorted_down.Save(resourcesDirectory + Path.DirectorySeparatorChar + "sorted_down.gif");
    //    Resources.sorted_up.Save(resourcesDirectory + Path.DirectorySeparatorChar + "sorted_up.gif");

    //    File.WriteAllText(resourcesDirectory + Path.DirectorySeparatorChar + "table.css", Resources.table_css);
    //    File.WriteAllText(resourcesDirectory + Path.DirectorySeparatorChar + "table.js", Resources.table_js);

    //}

    //public static void WriteDocType(TextWriter writer)
    //{
    //    writer.WriteLine("<!DOCTYPE html>");
    //}
    //    public static void WriteTableHeader(TextWriter writer, IEnumerable<string> columns)
    //{
    //    writer.WriteLine("<tr>");
    //    foreach (var column in columns)
    //    {
    //        writer.WriteLine("<th>");
    //        writer.WriteLine(column);
    //        writer.WriteLine("<th>");
    //    }
    //    writer.WriteLine("</tr>");
    //}
}
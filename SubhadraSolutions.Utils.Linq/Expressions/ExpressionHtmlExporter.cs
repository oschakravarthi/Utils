using SubhadraSolutions.Utils.Collections.Generic;
using SubhadraSolutions.Utils.Linq.Properties;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Web;

namespace SubhadraSolutions.Utils.Linq.Expressions;

public static class ExpressionHtmlExporter
{
    public static void ExportAsHtml(Expression expression, string file)
    {
        var tree = ExpressionTreeExporter.ExportAsTree(expression);
        ExportAsHtml(tree, file);
    }

    public static void ExportAsHtml(Tree<Expression> tree, string file)
    {
        ExportAsHtml(tree.Root, file);
    }

    public static void ExportAsHtml(TreeNode<Expression> node, string file)
    {
        using var sw = new StreamWriter(file);
        ExportExpression(node, sw);
    }

    private static void ExportExpression(TreeNode<Expression> node, TextWriter writer)
    {
        writer.WriteLine(Resources.ExpressionTreeHtmlPrefix);
        writer.WriteLine("<ul>");
        Write(node, writer);
        writer.WriteLine("</ul>");
        writer.Write("</div></body></html>");
    }

    private static void Write(TreeNode<Expression> node, TextWriter writer)
    {
        var expression = node.Value;

        writer.WriteLine("<li>");
        writer.WriteLine("<span class=\"tf-nc\">");
        WriteExpressionBox(expression, writer);
        writer.WriteLine("</span>");

        if (node.Nodes.Count > 0)
        {
            writer.WriteLine("<ul>");

            for (var i = 0; i < node.Nodes.Count; i++)
            {
                var childNode = node.Nodes[i];
                Write(childNode, writer);
            }

            writer.WriteLine("</ul>");
        }

        writer.WriteLine("</li>");
    }

    private static void WriteExpressionBox(Expression expression, TextWriter writer)
    {
        var actualExpression = expression.GetTheActualExpressionIfDecorated();

        var type = actualExpression.GetType();
        var properties = type.GetProperties();
        Array.Sort(properties, (a, b) => a.Name.CompareTo(b.Name));

        // writer.WriteLine("<table class=\"styled-table\">");
        writer.WriteLine("<table class=\"tbl\">");
        writer.WriteLine(
            $"<thead class=\"h\"><tr><th>{HttpUtility.HtmlEncode(type.Name)}</th></tr><tr><th>{HttpUtility.HtmlEncode(expression.ToString())}</th></tr></thead>");
        writer.WriteLine("<tr><td>");
        writer.WriteLine("<table class=\"tbl\">");

        foreach (var property in properties)
        // if (!typeof(Expression).IsAssignableFrom(property.PropertyType))
        {
            var propertyValue = property.GetValue(actualExpression);
            var isNumericType = property.PropertyType.IsNumericType();
            writer.WriteLine(
                $"<tr><td class=\"c_nowrap\">{property.Name}</td><td class=\"{(isNumericType ? "n" : "c")}\">{(propertyValue == null ? null : HttpUtility.HtmlEncode(propertyValue.ToString()))}</td></tr>");
        }

        writer.WriteLine("</table>");
        writer.WriteLine("</td></tr>");
        writer.WriteLine("</table>");
    }

    // private static void WriteExpressionBox(Expression expression, TextWriter writer)
    // {
    //    writer.Write("hi");
    // }
}
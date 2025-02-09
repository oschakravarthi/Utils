using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor;
using SubhadraSolutions.Utils.Blazor.Components;
using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Data.Annotations;
using SubhadraSolutions.Utils.Reflection;
using SubhadraSolutions.Utils.Text.Encoding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SubhadraSolutions.Utils.Blazor.Helpers;

public static class DataGridHelper
{
    private static readonly MethodInfo BuildColumnTemplateCoreTemplateMethod =
        typeof(DataGridHelper).GetMethod(nameof(BuildColumnTemplateCore), BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo BuildSequenceColumnTemplateCoreTemplateMethod =
        typeof(DataGridHelper).GetMethod(nameof(BuildSequenceColumnTemplateCore),
            BindingFlags.NonPublic | BindingFlags.Static);

    //private static readonly MethodInfo toMappedEnumerableTemplateMethod = typeof(CollectionsHelper).GetMethods(BindingFlags.Public | BindingFlags.Static).First(m => m.Name == nameof(CollectionsHelper.ToMappedEnumerable) && m.GetParameters().Length == 2);

    public static int DefaultColumnWidth { get; set; } = 100;

    public static void AddDataGrid(RenderTreeBuilder builder, Sequencer sequencer, object data,
        bool addSequenceColumn = true, int pageSize = 50, AttributesLookup attributesLookup = null,
        bool kmNumberFormat = false, bool simpleGridView = false, object childRowContent = null, object templateColumnContent = null)
    {
        if (sequencer == null)
        {
            sequencer = new Sequencer();
        }

        //data = RemapDataIfRequired(data);
        var entityType = data.GetType().GetEnumerableItemType();

        if (attributesLookup == null)
        {
            attributesLookup = new AttributesLookup(entityType);
        }

        builder.OpenComponent(sequencer.Next, typeof(ExtendedMudDataGrid<>).MakeGenericType(entityType));
        builder.AddAttribute(sequencer.Next, nameof(MudDataGrid<object>.Items), data);
        builder.AddAttribute(sequencer.Next, nameof(MudDataGrid<object>.ReadOnly), true);
        builder.AddAttribute(sequencer.Next, nameof(MudDataGrid<object>.RowsPerPage), pageSize);
        builder.AddAttribute(sequencer.Next, nameof(MudDataGrid<object>.Dense), true);
        builder.AddAttribute(sequencer.Next, nameof(MudDataGrid<object>.Hover), true);
        builder.AddAttribute(sequencer.Next, nameof(MudDataGrid<object>.Bordered), true);
        builder.AddAttribute(sequencer.Next, nameof(MudDataGrid<object>.Striped), true);
        builder.AddAttribute(sequencer.Next, nameof(MudDataGrid<object>.Filterable), !simpleGridView);
        builder.AddAttribute(sequencer.Next, nameof(MudDataGrid<object>.FilterMode),
            DataGridFilterMode.ColumnFilterRow);
        builder.AddAttribute(sequencer.Next, nameof(MudDataGrid<object>.ColumnResizeMode), ResizeMode.Column);
        //builder.AddAttribute(sequencer.Next, nameof(MudDataGrid<object>.ShowMenuIcon), !simpleGridView);
        builder.AddAttribute(sequencer.Next, nameof(MudDataGrid<object>.ShowMenuIcon), false);
        builder.AddAttribute(sequencer.Next, nameof(MudDataGrid<object>.FixedHeader), true);
        builder.AddAttribute(sequencer.Next, nameof(MudDataGrid<object>.FilterCaseSensitivity),
            DataGridFilterCaseSensitivity.CaseInsensitive);
        builder.AddAttribute(sequencer.Next, nameof(MudDataGrid<object>.FixedHeader), true);

        //builder.AddAttribute(sequencer.Next, nameof(MudDataGrid<object>.FixedFooter), true);
        builder.AddAttribute(sequencer.Next, nameof(MudDataGrid<object>.Columns),
            (RenderFragment)(b =>
                AddColumns(b, null, entityType, addSequenceColumn, attributesLookup, kmNumberFormat, childRowContent, templateColumnContent)));
        if (childRowContent != null)
        {
            builder.AddAttribute(sequencer.Next, "ChildRowContent", childRowContent);
        }
        builder.AddAttribute(sequencer.Next, nameof(MudDataGrid<object>.PagerContent), (RenderFragment)(b =>
        {
            b.OpenComponent(sequencer.Next, typeof(MudDataGridPager<>).MakeGenericType(entityType));
            b.CloseComponent();
        }));
        builder.CloseComponent();
    }

    public static void AddTemplateColumn(RenderTreeBuilder builder, Sequencer sequencer, Type entityType, object templateColumnContent)
    {
        builder.OpenComponent(sequencer.Next, typeof(TemplateColumn<>).MakeGenericType(entityType));
        builder.AddComponentParameter(sequencer.Next, nameof(TemplateColumn<object>.Filterable), false);
        builder.AddComponentParameter(sequencer.Next, nameof(TemplateColumn<object>.Sortable), false);
        builder.AddComponentParameter(sequencer.Next, nameof(TemplateColumn<object>.CellTemplate), templateColumnContent);
        builder.CloseComponent();
    }

    private static void AddHierarchyColumn(RenderTreeBuilder builder, Sequencer sequencer, Type entityType)
    {
        var columnType = typeof(HierarchyColumn<>).MakeGenericType(entityType);
        builder.OpenComponent(sequencer.Next, columnType);
        builder.CloseComponent();
    }

    private static void AddColumn(RenderTreeBuilder builder, Sequencer sequencer, Type entityType,
        PropertyInfo property, IDictionary<string, PropertyInfo> propertiesLookup, string title,
        GridColumnAttribute gridColumnAttribute, int? width, List<NavigationAttribute> navigationAttributes,
        AttributesLookup attributesLookup, bool kmNumberFormat)
    {
        var columnType = typeof(PropertyColumn<,>).MakeGenericType(entityType, property.PropertyType);
        var formatString = property.GetFormatString(attributesLookup);
        var propertyType = property.PropertyType;
        //var isValidPropertyName = CSharpIdentifiers.IsValidParsedIdentifier(property.Name);
        var isFilterable = /*isValidPropertyName && */ gridColumnAttribute?.Filterable != false;
        var isPropertyTypeEnumerable = !propertyType.IsPrimitiveOrExtendedPrimitive() && propertyType.IsEnumerableType();

        var propertyAccessor = entityType.BuildPropertyAccessorAsLambda(property);
        builder.OpenComponent(sequencer.Next, columnType);
        builder.AddAttribute(sequencer.Next, nameof(PropertyColumn<object, object>.Property), propertyAccessor);
        builder.AddAttribute(sequencer.Next, nameof(Column<object>.Title), title);
        builder.AddAttribute(sequencer.Next, nameof(Column<object>.Sortable), /*isValidPropertyName && */
            property.PropertyType.IsPrimitiveOrExtendedPrimitive());
        //builder.AddAttribute(sequencer.Next, nameof(Column<object>.Resizable), true);
        builder.AddAttribute(sequencer.Next, nameof(Column<object>.Filterable), isFilterable);
        builder.AddAttribute(sequencer.Next, nameof(Column<object>.ShowFilterIcon), isFilterable);

        //if (width != null)
        //{
        //    builder.AddAttribute(sequencer.Next, "Width", width + "%");
        //}
        //if (gridColumnAttribute != null)
        //{
        //    builder.AddAttribute(sequencer.Next, "TextAlign", gridColumnAttribute.TextAlign);
        //}
        //else
        //{
        //    if (property.PropertyType.IsNumericType())
        //    {
        //        builder.AddAttribute(sequencer.Next, "TextAlign", TextAlign.Right);
        //    }
        //}
        if (property.PropertyType.IsPrimitiveOrExtendedPrimitive())
        {
            var sortDirectionAttribute = attributesLookup.GetCustomAttribute<SortAttribute>(property, true);
            if (sortDirectionAttribute != null)
            {
                builder.AddAttribute(sequencer.Next, nameof(Column<object>.InitialDirection),
                    sortDirectionAttribute.SortOrder == SortingOrder.Descending
                        ? SortDirection.Descending
                        : SortDirection.Ascending);
            }
        }

        if (gridColumnAttribute?.HtmlTag == null)
        {
            if (gridColumnAttribute?.CssClass != null)
            {
                builder.AddAttribute(sequencer.Next, nameof(Column<object>.CellClass), gridColumnAttribute.CssClass);
            }
        }

        var cssValuePropertyNames = attributesLookup
            .GetLinkedMembersWithGivenAttribute<CSSClassValueAttribute>(property).Where(propertiesLookup.ContainsKey)
            .ToList();
        var linkedNavigationPropertyNames = attributesLookup
            .GetLinkedMembersWithGivenAttribute<NavigationAttribute>(property).Where(propertiesLookup.ContainsKey)
            .ToList();

        if (gridColumnAttribute?.HtmlTag != null || propertyType == typeof(bool) || propertyType == typeof(bool?) || formatString != null || (kmNumberFormat && propertyType.IsNumericType()) || isPropertyTypeEnumerable ||
            (navigationAttributes?.Count > 0) || cssValuePropertyNames.Count > 0 ||
            linkedNavigationPropertyNames.Count > 0)
        {
            try
            {
                var columnTemplate = BuildColumnTemplate(null, entityType, property, navigationAttributes, linkedNavigationPropertyNames,
                        cssValuePropertyNames, attributesLookup, kmNumberFormat, formatString, gridColumnAttribute);

                builder.AddAttribute(sequencer.Next, nameof(Column<object>.CellTemplate), columnTemplate);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }
        else
        {
            builder.AddAttribute(sequencer.Next, nameof(Column<object>.CellTemplate), (RenderFragment)null);
        }
        builder.CloseComponent();
    }

    private static void AddColumns(RenderTreeBuilder builder, Sequencer sequencer, Type entityType,
        bool addSequenceColumn, AttributesLookup attributesLookup, bool kmNumberFormat, object childRowContent, object templateColumnContent)
    {
        if (sequencer == null)
        {
            sequencer = new Sequencer();
        }

        if (addSequenceColumn)
        {
            //AddSequenceColumn(builder, sequencer, entityType, data);
        }
        if (childRowContent != null)
        {
            AddHierarchyColumn(builder, sequencer, entityType);
        }
        var properties = entityType.GetSortedPublicProperties(attributesLookup);
        var propertiesToDisplay = new List<PropertyInfo>();

        foreach (var property in properties)
        {
            if (attributesLookup.IsDefined<CSSClassValueAttribute>(property, true))
            {
                continue;
            }

            propertiesToDisplay.Add(property);
        }

        var finalPropertiesToDisplay = new List<PropertyInfo>(propertiesToDisplay);
        foreach (var property in propertiesToDisplay)
        {
            var linkedProperties = attributesLookup.GetLinkedMembers(property);
            foreach (var linkedProperty in linkedProperties)
            {
                for (var i = 0; i < finalPropertiesToDisplay.Count; i++)
                    if (finalPropertiesToDisplay[i].Name == linkedProperty)
                    {
                        finalPropertiesToDisplay.RemoveAt(i);
                        break;
                    }
            }
        }

        var gridColumnAttributes = new GridColumnAttribute[finalPropertiesToDisplay.Count];
        var columnTitles = new string[finalPropertiesToDisplay.Count];
        for (var i = 0; i < finalPropertiesToDisplay.Count; i++)
        {
            var property = finalPropertiesToDisplay[i];
            var gridColumnAttribute = attributesLookup.GetCustomAttribute<GridColumnAttribute>(property, true);
            gridColumnAttributes[i] = gridColumnAttribute;
            //int width = DefaultColumnWidth;
            var title = property.GetMemberDisplayName(true, attributesLookup);
            columnTitles[i] = title;
        }

        var propertiesLookup = properties.ToDictionary(x => x.Name);
        for (var i = 0; i < finalPropertiesToDisplay.Count; i++)
        {
            var property = finalPropertiesToDisplay[i];
            var width = 0;
            var navigationAttributes = GetNavigationAttributes(property, attributesLookup);
            AddColumn(builder, sequencer, entityType, property, propertiesLookup, columnTitles[i],
                gridColumnAttributes[i], width, navigationAttributes, attributesLookup, kmNumberFormat);
        }
        if (templateColumnContent != null)
        {
            AddTemplateColumn(builder, sequencer, entityType, templateColumnContent);
        }
    }

    private static void AddSequenceColumn(RenderTreeBuilder builder, Sequencer sequencer, Type entityType, IList data)
    {
        builder.OpenComponent(sequencer.Next, typeof(Column<>).MakeGenericType(entityType));

        builder.AddAttribute(sequencer.Next, nameof(Column<object>.Title), "#");
        builder.AddAttribute(sequencer.Next, nameof(Column<object>.StickyLeft), true);

        builder.AddAttribute(sequencer.Next, nameof(Column<object>.Sortable), true);
        builder.AddAttribute(sequencer.Next, nameof(Column<object>.Resizable), false);
        builder.AddAttribute(sequencer.Next, nameof(Column<object>.Filterable), false);
        builder.AddAttribute(sequencer.Next, nameof(Column<object>.ShowFilterIcon), false);
        builder.AddAttribute(sequencer.Next, nameof(Column<object>.CellTemplate),
            BuildSequenceColumnTemplate(sequencer, data, entityType));

        builder.CloseComponent();
    }

    private static object BuildColumnTemplate(Sequencer sequencer, Type entityType, PropertyInfo property,
        List<NavigationAttribute> navigationAttributes, List<string> linkedNavigationPropertyNames,
        List<string> cssValuePropertyNames, AttributesLookup attributesLookup, bool kmNumberFormat, string formatString, GridColumnAttribute gridColumnAttribute)
    {
        var method = BuildColumnTemplateCoreTemplateMethod.MakeGenericMethod(entityType);
        return method.Invoke(null,
            [
                sequencer, property, navigationAttributes, linkedNavigationPropertyNames, cssValuePropertyNames,
                attributesLookup, kmNumberFormat, formatString, gridColumnAttribute
            ]);
    }

    [DynamicallyInvoked]
    private static RenderFragment<CellContext<T>> BuildColumnTemplateCore<T>(Sequencer sequencer, PropertyInfo property,
        List<NavigationAttribute> navigationAttributes, List<string> linkedNavigationPropertyNames,
        List<string> cssValuePropertyNames, AttributesLookup attributesLookup, bool kmNumberFormat, string formatString, GridColumnAttribute gridColumnAttribute)
    {
        if (sequencer == null)
        {
            sequencer = new Sequencer();
        }

        var propertyType = property.PropertyType;
        var isPropertyTypeEnumerable = !propertyType.IsPrimitiveOrExtendedPrimitive() && propertyType.IsEnumerableType();

        Func<T, string[]> cssFunc = null;

        if (cssValuePropertyNames?.Count > 0)
        {
            cssFunc = PropertiesToStringValuesHelper.BuildFuncForToStringArray<T>(cssValuePropertyNames);
        }

        if (!isPropertyTypeEnumerable || propertyType == typeof(bool) || propertyType == typeof(bool?))
        {
            var toStringFunc = property.BuildPropertyGetter<T>(kmNumberFormat, attributesLookup);
            List<KeyValuePair<Func<T, string>, List<NavigationAttribute>>> navigations = null;

            if (linkedNavigationPropertyNames?.Count > 0)
            {
                navigations = [];
                foreach (var linkedNavigationPropertyName in linkedNavigationPropertyNames)
                {
                    var func = PropertiesToStringValuesHelper.BuildFuncForToString<T>(new List<string>
                        { linkedNavigationPropertyName });
                    var linkedMember = attributesLookup.GetMember(linkedNavigationPropertyName);
                    var attributes = attributesLookup.GetCustomAttributes<NavigationAttribute>(linkedMember).ToList();
                    navigations.Add(new KeyValuePair<Func<T, string>, List<NavigationAttribute>>(func, attributes));
                }
            }

            return cellContext =>
            {
                var record = cellContext.Item;
                if (record == null)
                {
                    return null;
                }

                return builder =>
                {
                    var valueAsString = toStringFunc(record);

                    if (string.IsNullOrWhiteSpace(valueAsString))
                    {
                        return;
                    }

                    if (propertyType == typeof(bool) || propertyType == typeof(bool?))
                    {
                        builder.OpenComponent(sequencer.Next, typeof(MudCheckBox<bool>));
                        builder.AddAttribute(sequencer.Next, "Disabled", true);
                        builder.AddAttribute(sequencer.Next, "Value", valueAsString == "True");
                        builder.CloseComponent();
                        return;
                    }

                    if (formatString != null)
                    {
                        valueAsString = string.Format(formatString, valueAsString);
                    }

                    string cssClass = null;
                    if (cssFunc != null)
                    {
                        cssClass = GetCssValueForTemplate(cssFunc, record);
                    }

                    var navigationInfos = new List<NavigationAttributeInfo>();

                    if (navigationAttributes != null && !string.IsNullOrEmpty(valueAsString))
                    {
                        foreach (var navigationAttribute in navigationAttributes)
                        {
                            navigationInfos.Add(new NavigationAttributeInfo(navigationAttribute, valueAsString));
                        }
                    }

                    if (navigations != null)
                    {
                        foreach (var navigation in navigations)
                        {
                            var val = navigation.Key(record);
                            if (!string.IsNullOrEmpty(val))
                            {
                                foreach (var attrib in navigation.Value)
                                {
                                    navigationInfos.Add(new NavigationAttributeInfo(attrib, val));
                                }
                            }
                        }
                    }

                    BuildTemplateCore(builder, sequencer, valueAsString, navigationInfos, cssClass, gridColumnAttribute);
                };
            };
        }

        {
            var func = PropertiesToStringValuesHelper.BuildGetValueAsStringEnumerableFunc<T>(property);
            //Type elementType = ReflectionHelper.GetEnumerableItemType(propertyType);

            return cellContext =>
            {
                return builder =>
                {
                    var record = cellContext.Item;
                    string cssClass = null;

                    if (cssFunc != null)
                    {
                        cssClass = GetCssValueForTemplate(cssFunc, record);
                    }

                    var strings = func(record);
                    var i = 0;

                    foreach (var valueAsString in strings)
                    {
                        if (string.IsNullOrWhiteSpace(valueAsString))
                        {
                            continue;
                        }

                        if (i > 0)
                        {
                            builder.OpenElement(sequencer.Next, "span");
                            builder.AddContent(sequencer.Next, " • ");
                            builder.CloseElement();
                        }

                        var navigationInfos = new List<NavigationAttributeInfo>();

                        if (navigationAttributes != null && !string.IsNullOrEmpty(valueAsString))
                        {
                            foreach (var navigationAttribute in navigationAttributes)
                            {
                                navigationInfos.Add(new NavigationAttributeInfo(navigationAttribute, valueAsString));
                            }
                        }

                        BuildTemplateCore(builder, sequencer, valueAsString, navigationInfos, cssClass, gridColumnAttribute);
                        i++;
                    }
                };
            };
        }
    }

    private static object BuildSequenceColumnTemplate(Sequencer sequencer, IList data, Type entityType)
    {
        var method = BuildSequenceColumnTemplateCoreTemplateMethod.MakeGenericMethod(entityType);
        return method.Invoke(null, [sequencer, data]);
    }

    [DynamicallyInvoked]
    private static RenderFragment<CellContext<T>> BuildSequenceColumnTemplateCore<T>(Sequencer sequencer, IList<T> data)
    {
        if (sequencer == null)
        {
            sequencer = new Sequencer();
        }

        return context => builder => builder.AddContent(sequencer.Next, data.IndexOf(context.Item) + 1);
    }

    private static void BuildTemplateCore(RenderTreeBuilder builder, Sequencer sequencer, string valueAsString,
        List<NavigationAttributeInfo> navigationInfos, string cssClass, GridColumnAttribute gridColumnAttribute)
    {
        if (string.IsNullOrEmpty(valueAsString))
        {
            return;
        }
        var tag = gridColumnAttribute?.HtmlTag ?? "span";

        builder.OpenElement(sequencer.Next, tag);
        builder.AddAttribute(sequencer.Next, "style", "white-space:normal " + gridColumnAttribute?.CssClass);

        if (navigationInfos?.Count > 0)
        {
            var i = 0;
            foreach (var navigationInfo in navigationInfos)
            {
                if (i > 0)
                {
                    builder.OpenElement(sequencer.Next, "span");
                    builder.AddContent(sequencer.Next, " ");
                    builder.CloseElement();
                }

                var valueForPath = navigationInfo.Value;
                var attrib = navigationInfo.NavigationAttribute;
                if (attrib.Encode)
                {
                    valueForPath = Base64Encoding.Encode(valueForPath);
                }

                if (attrib.LinkTemplate != null)
                {
                    var path = string.Format(attrib.LinkTemplate, valueForPath);
                    var linkLabel = navigationInfo.Value;

                    builder.OpenComponent<MudLink>(sequencer.Next);
                    //builder.AddAttribute(sequencer.Next, nameof(MudLink.Typo), Typo.body2);
                    if (cssClass != null)
                    {
                        builder.AddAttribute(sequencer.Next, nameof(MudLink.Class), "cssClass");
                    }

                    if (i > 0)
                    {
                        linkLabel = attrib.Name;

                        if (string.IsNullOrEmpty(linkLabel))
                        {
                            linkLabel = "Link" + i;
                        }

                        linkLabel = "[" + linkLabel + "]";
                        builder.AddAttribute(sequencer.Next, nameof(MudLink.Color), Color.Secondary);
                    }

                    builder.AddAttribute(sequencer.Next, nameof(MudLink.Href), path);
                    if (attrib.Target != null)
                    {
                        builder.AddAttribute(sequencer.Next, nameof(MudLink.Target), attrib.Target);
                    }

                    builder.AddAttribute(sequencer.Next, nameof(MudLink.ChildContent),
                        (RenderFragment)(b => { b.AddContent(sequencer.Next, linkLabel); }));
                    builder.CloseComponent();
                }
                else
                {
                    if (bool.TryParse(valueForPath, out var shouldDisplay))
                    {
                        if (shouldDisplay)
                        {
                            builder.OpenElement(sequencer.Next, "span");
                            if (i == 0)
                            {
                                if (cssClass != null)
                                {
                                    builder.AddAttribute(sequencer.Next, "class", cssClass);
                                }
                            }

                            var label = attrib.Name;

                            if (string.IsNullOrEmpty(label))
                            {
                                label = "Label" + i;
                            }

                            builder.AddAttribute(sequencer.Next, "class", "appEmptyNavigationLink");
                            builder.AddAttribute(sequencer.Next, "Title", label);
                            builder.AddContent(sequencer.Next, label);
                            builder.CloseElement();
                        }
                    }
                }

                i++;
            }

            builder.CloseElement();
            return;
        }

        if (cssClass != null)
        {
            builder.OpenElement(sequencer.Next, "span");
            builder.AddAttribute(sequencer.Next, "class", cssClass);
        }

        builder.AddContent(sequencer.Next, valueAsString);
        if (cssClass != null)
        {
            builder.CloseElement();
        }

        builder.CloseElement();
    }

    private static string GetCssValueForTemplate<T>(Func<T, string[]> cssClassesFunc, T obj)
    {
        if (cssClassesFunc == null)
        {
            return null;
        }

        var cssClasses = cssClassesFunc(obj);
        var cssClass = string.Join(" ", cssClasses);

        if (string.IsNullOrWhiteSpace(cssClass))
        {
            return null;
        }

        return cssClass;
    }

    private static List<NavigationAttribute> GetNavigationAttributes(PropertyInfo property,
        AttributesLookup attributesLookup)
    {
        var navigationAttributes = attributesLookup.GetCustomAttributes<NavigationAttribute>(property, true).ToList();

        if (navigationAttributes.Count == 0)
        {
            return null;
        }

        navigationAttributes.Sort(NavigationAttributeComparer.Instance);
        return navigationAttributes;
    }
}
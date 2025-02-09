namespace SubhadraSolutions.Utils.Data;

public class PropertySortingInfo(string propertyName, SortingOrder sortingOrder)
{
    public string PropertyName { get; } = propertyName;
    public SortingOrder SortingOrder { get; } = sortingOrder;
}
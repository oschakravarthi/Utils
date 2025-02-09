namespace SubhadraSolutions.Utils.Data.Annotations;

public class NavigationAttributeInfo
{
    public NavigationAttributeInfo(NavigationAttribute navigationAttribute, string value)
    {
        NavigationAttribute = navigationAttribute;
        Value = value;
    }

    public NavigationAttribute NavigationAttribute { get; }
    public string Value { get; }
}
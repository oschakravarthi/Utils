namespace SubhadraSolutions.Utils;

public struct StringKeyValuePair
{
    public StringKeyValuePair(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; }
    public string Value { get; }
}
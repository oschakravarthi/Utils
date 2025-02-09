namespace SubhadraSolutions.Utils;

public static class DynamicToStringHelperExtensions
{
    public static string ExportAsString<T>(this T obj)
    {
        return DynamicToStringHelper<T>.ExportAsString(obj);
    }

    public static string ExportObjectAsString(this object o)
    {
        if (o != null)
        {
            var t = typeof(DynamicToStringHelper<>).MakeGenericType(o.GetType());
            var method = t.GetMethod("ExportAsString", [o.GetType()]);
            var result = method.Invoke(null, [o]);
            return (string)result;
        }

        return null;
    }
}
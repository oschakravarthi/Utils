namespace SubhadraSolutions.Utils.Diagnostics.Tracing;

public class TraceChannelId
{
    private string _value;

    public static explicit operator string(TraceChannelId input)
    {
        return input?._value;
    }

    public static explicit operator TraceChannelId(string input)
    {
        return new TraceChannelId
        {
            _value = input
        };
    }
}
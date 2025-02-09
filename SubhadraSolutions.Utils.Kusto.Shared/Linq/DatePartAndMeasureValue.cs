namespace SubhadraSolutions.Utils.Kusto.Shared.Linq;

public class DatePartAndMeasureValue
{
    public DatePartAndMeasureValue()
    {
    }

    public DatePartAndMeasureValue(string datePart, double measureValue)
    {
        DatePart = datePart;
        MeasureValue = measureValue;
    }

    public string DatePart { get; set; }
    public double MeasureValue { get; set; }
}
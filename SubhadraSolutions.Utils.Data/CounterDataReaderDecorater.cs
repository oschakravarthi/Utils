using System.Data;

namespace SubhadraSolutions.Utils.Data;

public class CounterDataReaderDecorater : AbstractDataReaderDecorator
{
    public CounterDataReaderDecorater(IDataReader dataReader)
    {
        Actual = dataReader;
    }

    public int Count { get; private set; }

    public override bool Read()
    {
        var canRead = base.Read();
        if (canRead)
        {
            Count++;
        }

        return canRead;
    }
}
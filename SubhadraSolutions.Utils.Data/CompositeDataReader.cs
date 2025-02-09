using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Data;

public class CompositeDataReader : AbstractDataReaderDecorator
{
    private readonly IEnumerator<Task<IDataReader>> dataReadersEnumerator;
    private readonly bool stopReadingRemainingDataReadersIfRecordsFoundInPresentDataReader;
    private bool stopped;

    public CompositeDataReader(IEnumerable<Task<IDataReader>> dataReaders,
        bool stopReadingRemainingDataReadersIfRecordsFoundInPresentDataReader = false)
    {
        dataReadersEnumerator = dataReaders.GetEnumerator();
        this.stopReadingRemainingDataReadersIfRecordsFoundInPresentDataReader =
            stopReadingRemainingDataReadersIfRecordsFoundInPresentDataReader;
        if (dataReadersEnumerator.MoveNext())
        {
            var current = dataReadersEnumerator.Current.Result;
            Actual = stopReadingRemainingDataReadersIfRecordsFoundInPresentDataReader
                ? new CounterDataReaderDecorater(current)
                : current;
        }
    }

    public override bool Read()
    {
        if (stopped)
        {
            return false;
        }

        if (Actual == null)
        {
            return false;
        }

        var canRead = base.Read();
        if (canRead)
        {
            return canRead;
        }

        Actual.Close();

        if (stopReadingRemainingDataReadersIfRecordsFoundInPresentDataReader)
        {
            var decorator = (CounterDataReaderDecorater)Actual;
            if (decorator.Count != 0)
            {
                stopped = true;
                return false;
            }
        }

        if (!dataReadersEnumerator.MoveNext())
        {
            return false;
        }

        var current = dataReadersEnumerator.Current.Result;
        Actual = stopReadingRemainingDataReadersIfRecordsFoundInPresentDataReader
            ? new CounterDataReaderDecorater(current)
            : current;
        return Read();
    }
}
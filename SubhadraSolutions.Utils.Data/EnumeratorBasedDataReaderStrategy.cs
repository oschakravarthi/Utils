using SubhadraSolutions.Utils.Data.Common;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Data;

public class EnumeratorBasedDataReaderStrategy<T>(IEnumerator<T> enumerator, IList<FieldNameAndType> fields,
        PopulateItemArrayDelegate<T> populateItemArrayDelegate)
    : GenericDataReaderStrategy<T>(fields, delegate
    {
        if (enumerator.MoveNext())
        {
            return enumerator.Current;
        }

        return default;
    }, populateItemArrayDelegate, null)
    where T : class;
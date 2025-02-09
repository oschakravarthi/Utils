using SubhadraSolutions.Utils.Validation;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SubhadraSolutions.Utils.IO;

public class TextReaderEnumerable : IEnumerable<string>
{
    private readonly TextReader _reader;

    public TextReaderEnumerable(TextReader reader)
    {
        Guard.ArgumentShouldNotBeNull(reader, nameof(reader));
        _reader = reader;
    }

    public IEnumerator<string> GetEnumerator()
    {
        return new TextReaderEnumerator(_reader);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
using SubhadraSolutions.Utils.Abstractions;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SubhadraSolutions.Utils.IO;

public class TextReaderEnumerator(TextReader reader) : AbstractDisposable, IEnumerator<string>
{
    public string Current { get; private set; }

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        if (reader.Peek() != -1)
        {
            var line = reader.ReadLine();
            Current = string.IsNullOrEmpty(line) ? null : line;
            return true;
        }

        reader.Close();
        Current = null;
        return false;
    }

    public void Reset()
    {
    }

    protected override void Dispose(bool disposing)
    {
        reader.Dispose();
    }
}
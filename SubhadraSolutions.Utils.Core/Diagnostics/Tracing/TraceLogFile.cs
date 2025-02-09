using System.IO;

namespace SubhadraSolutions.Utils.Diagnostics.Tracing;

public class TraceLogFile
{
    private readonly FileInfo _fileInfo;

    internal TraceLogFile(string fileName)
    {
        _fileInfo = new FileInfo(fileName);
    }

    private TraceLogFile()
    {
    }

    public string FullName => _fileInfo.FullName;
    public string Name => _fileInfo.Name;

    public Stream GetStream()
    {
        return _fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    }
}
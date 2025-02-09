using System.Data;

namespace SubhadraSolutions.Utils.Data;

public interface IDataReaderDecorator : IDataReader
{
    IDataReader Actual { get; set; }
}
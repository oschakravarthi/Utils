namespace SubhadraSolutions.Utils.Collections.Generic;

public interface IDataStructureAdapter<T>
{
    int Capacity { get; }

    T this[int index] { get; set; }

    void Clear();
}
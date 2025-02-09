using SubhadraSolutions.Utils.Collections.Concurrent;
using SubhadraSolutions.Utils.Validation;
using System;
using System.Runtime.Serialization;

namespace SubhadraSolutions.Utils.Collections.Generic;

[Serializable]
[DataContract]
public class Grid<T>
{
    private readonly CompositeDictionary<int, T> _dictionary = new(true);

    public Grid(int numberOfRows, int numberOfColumns)
    {
        Guard.ArgumentShouldBeGreaterThan(1, numberOfRows, nameof(numberOfRows));
        Guard.ArgumentShouldBeGreaterThan(1, numberOfColumns, nameof(numberOfColumns));
        NumberOfRows = numberOfRows;
        NumberOfColumns = numberOfColumns;
    }

    public int NumberOfColumns { get; }
    public int NumberOfRows { get; }

    public void Add(int rowNumber, int columnNumber, T item)
    {
        Guard.ArgumentShouldBeInRange(1, NumberOfRows, rowNumber, nameof(rowNumber));
        Guard.ArgumentShouldBeInRange(1, NumberOfColumns, columnNumber, nameof(columnNumber));
        _dictionary.PerformAtomicOperation(() =>
        {
            if (_dictionary.TryGetValue(out var existingItem, rowNumber, columnNumber))
            {
                throw new Exception("The cell is already occupied");
            }

            _dictionary.AddOrUpdate(item, rowNumber, columnNumber);
        });
    }

    public void Remove(int rowNumber, int columnNumber)
    {
        Guard.ArgumentShouldBeInRange(1, NumberOfRows, rowNumber, nameof(rowNumber));
        Guard.ArgumentShouldBeInRange(1, NumberOfColumns, columnNumber, nameof(columnNumber));
        _dictionary.PerformAtomicOperation(() =>
        {
            if (!_dictionary.TryGetValue(out var existingItem, rowNumber, columnNumber))
            {
                throw new Exception("There is no item at the given cell");
            }

            _dictionary.RemoveIfExists(rowNumber, columnNumber);
        });
    }
}
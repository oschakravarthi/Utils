using System;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

[Serializable]
public class ChunkArray<T> : IEnumerable<T>, IDataStructureAdapter<T>
{
    private static readonly int OptimalChunkSize;
    private static int LargeObjectThreshold = (85000 - IntPtr.Size * 5) / IntPtr.Size;
    protected readonly int chunkSize;
    protected int capacity;
    protected List<T[]> ChunkArrays;
    protected T DefaultValue = default;

    static ChunkArray()
    {
        OptimalChunkSize = LargeObjectThreshold;
    }

    public ChunkArray(int capacity)
    {
        this.capacity = capacity;
        chunkSize = Math.Min(capacity, OptimalChunkSize);
        InitializeProtected();
    }

    public ChunkArray(int capacity, int chunkSize)
    {
        this.capacity = capacity;
        this.chunkSize = Math.Min(Math.Min(chunkSize, capacity), OptimalChunkSize);
        InitializeProtected();
    }

    public int ChunkSize => chunkSize;

    public int Capacity => capacity;

    public virtual T this[int index]
    {
        get
        {
            var x = index / chunkSize;
            var y = index % chunkSize;
            return ChunkArrays[x][y];
        }

        set
        {
            var x = index / chunkSize;
            var y = index % chunkSize;
            ChunkArrays[x][y] = value;
        }
    }

    public virtual void Clear()
    {
        foreach (var chunkArray in ChunkArrays)
        {
            for (var j = 0; j < chunkArray.Length; j++)
                chunkArray[j] = DefaultValue;
        }
    }

    public virtual IEnumerator<T> GetEnumerator()
    {
        return new DataStructureEnumerator<T>(this, 0, capacity);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static ChunkArray<T> CreateFromList(IList<T> items)
    {
        var ba = new ChunkArray<T>(items.Count);
        for (var i = 0; i < items.Count; i++) ba[i] = items[i];

        return ba;
    }

    public static T[] ToArray(ChunkArray<T> chunkArray)
    {
        var array = new T[chunkArray.capacity];
        for (var i = 0; i < chunkArray.capacity; i++) array[i] = chunkArray[i];

        return array;
    }

    public static TTarget[] ToCastedArray<TTarget>(ChunkArray<T> chunkArray) where TTarget : class
    {
        var array = new TTarget[chunkArray.capacity];
        for (var i = 0; i < chunkArray.capacity; i++) array[i] = chunkArray[i] as TTarget;

        return array;
    }

    public T[] ToArray()
    {
        return ToArray(this);
    }

    public TTarget[] ToCastedArray<TTarget>() where TTarget : class
    {
        return ToCastedArray<TTarget>(this);
    }

    protected void InitializeProtected()
    {
        var reminder = capacity % chunkSize;
        var numberOfChunks = capacity / chunkSize;
        if (reminder > 0)
        {
            numberOfChunks++;
        }

        ChunkArrays = new List<T[]>(numberOfChunks);
        for (var i = 0; i < numberOfChunks - 1; i++) ChunkArrays.Add(new T[chunkSize]);

        ChunkArrays.Add(new T[reminder > 0 ? reminder : chunkSize]);
    }
}
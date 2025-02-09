using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class SizedQueue<T>
{
    private readonly Queue<T> adaptedObject = new();

    public SizedQueue(int size)
    {
        if (size <= 0)
        {
            throw new ArgumentException(@"Queue size should be >0", nameof(size));
        }

        Size = size;
    }

    public int Count => adaptedObject.Count;

    public int Size { get; }

    public event EventHandler<SizedQueueOnFullEventArgs<T>> OnQueueFull;

    public T Dequeue()
    {
        return adaptedObject.Dequeue();
    }

    public void Enqueue(T item)
    {
        if (adaptedObject.Count == Size)
        {
            var oldItem = adaptedObject.Dequeue();
            if (OnQueueFull != null)
            {
                try
                {
                    OnQueueFull(this, new SizedQueueOnFullEventArgs<T>(oldItem));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        adaptedObject.Enqueue(item);
    }

    public void Enqueue(IEnumerable<T> enumerable)
    {
        foreach (var item in enumerable)
        {
            Enqueue(item);
        }
    }
}
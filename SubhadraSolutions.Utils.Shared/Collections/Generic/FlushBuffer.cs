using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.Threading;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SubhadraSolutions.Utils.Collections.Generic;

public sealed class FlushBuffer<T> : AbstractDisposable
{
    private readonly bool _keepOnlyUniqueItems;

    private readonly IntervalTask _scheduledTask;

    private readonly List<T> collection1 = [];

    private readonly List<T> collection2 = [];

    private readonly int? _bufferSize;

    private List<T> writingCollection;

    public FlushBuffer(int? bufferSize, long? flushInterval, bool keepOnlyUniqueItems)
    {
        writingCollection = collection1;
        _bufferSize = bufferSize;
        _keepOnlyUniqueItems = keepOnlyUniqueItems;
        if (flushInterval != null)
        {
            _scheduledTask = new IntervalTask(flushInterval.Value, false);
            _scheduledTask.AddAction(Flush, null);
            _scheduledTask.Start();
        }
    }

    public event EventHandler<FlushEventArgs<T>> BeforeFlush;

    public void Write(T obj)
    {
        var coll = writingCollection;
        if (!_keepOnlyUniqueItems || (_keepOnlyUniqueItems && !coll.Contains(obj)))
        {
            coll.Add(obj);
            if (_bufferSize != null && coll.Count >= _bufferSize.Value)
            {
                ThreadPool.QueueUserWorkItem(state => Flush(null), null);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        _scheduledTask?.Dispose();

        Flush(null);
    }

    private void Flush(object tag)
    {
        if (writingCollection.Count > 0)
        {
            var coll = writingCollection;
            writingCollection = writingCollection == collection1 ? collection2 : collection1;
            if (BeforeFlush != null)
            {
                BeforeFlush(this, new FlushEventArgs<T>(coll));
            }

            coll.Clear();
        }
    }
}
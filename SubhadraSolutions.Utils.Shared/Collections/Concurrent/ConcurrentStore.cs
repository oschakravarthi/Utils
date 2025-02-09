using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace SubhadraSolutions.Utils.Collections.Concurrent;

public sealed class ConcurrentStore<T> : AbstractDisposable
{
    private const int WaitTime = 10;

    private readonly StoreFlushEventArgs<T>[] args = new StoreFlushEventArgs<T>[2];

    private readonly T[][] buffers = new T[2][];
    private readonly int bufferSize;
    private readonly T defaultT = default;
    private readonly AutoResetEvent flushEvent = new(false);

    private readonly ThreadPriority flushingThreadPriority;

    private readonly AutoResetEvent forceFlushEvent = new(true);

    private readonly bool isReferenceType;

    private readonly long maxWaitTimeInTicks;

    private readonly ManualResetEvent[] writeEvents = new ManualResetEvent[2];

    private readonly int[] writingIndexes = new int[2];

    private readonly int[] writtenCounts = new int[2];
    private Thread flushingThread;
    private long flushStartedAt = -1;
    private bool isDisposeCalled;

    private bool isInitialized;

    private int lastFlushIndex = -1;

    private int writingBufferIndex;

    public ConcurrentStore(int bufferSize, TimeSpan periodicFlushInterval)
        : this(bufferSize, ThreadPriority.Normal, periodicFlushInterval)
    {
    }

    public ConcurrentStore(int bufferSize, ThreadPriority flushingThreadPriority, TimeSpan periodicFlushInterval)
        : this(bufferSize, flushingThreadPriority, periodicFlushInterval, -1)
    {
    }

    public ConcurrentStore(int bufferSize, ThreadPriority flushingThreadPriority, TimeSpan periodicFlushInterval,
        long maxWaitTime)
    {
        maxWaitTimeInTicks = maxWaitTime * TimeSpan.TicksPerMillisecond;
        isReferenceType = !typeof(T).IsValueType;
        PeriodicFlushInterval = periodicFlushInterval;
        this.flushingThreadPriority = flushingThreadPriority;
        writingIndexes[0] = -1;
        writingIndexes[1] = -1;

        writtenCounts[0] = 0;
        writtenCounts[1] = 0;

        this.bufferSize = bufferSize;
        writeEvents[0] = new ManualResetEvent(false);
        writeEvents[1] = new ManualResetEvent(true);
        ClearBufferAfterFlush = false;
    }

    public bool ClearBufferAfterFlush { get; set; }

    public string FlushingThreadName
    {
        get => flushingThread.Name;

        set => flushingThread.Name = value;
    }

    public long MaxWaitTime => maxWaitTimeInTicks / TimeSpan.TicksPerMillisecond;

    public TimeSpan PeriodicFlushInterval { get; set; }

    public event EventHandler<StoreFlushEventArgs<T>> OnStoreFlush;

    public bool Add(T item)
    {
        CheckAndThrowDisposedException();
        return CheckAndAdd(item);
    }

    public void ForceFlush()
    {
        CheckAndThrowDisposedException();
        ForceFlush(false);
    }

    protected override void Dispose(bool disposing)
    {
        ForceFlush(true);
        //if (disposing)
        //{
        //    flushEvent.Dispose();
        //    forceFlushEvent.Dispose();
        //    writeEvents[0].Dispose();
        //    writeEvents[1].Dispose();
        //}
    }

    private bool CheckAndAdd(T item)
    {
        var b = isInitialized;
        Thread.MemoryBarrier();
        if (!b)
        {
            lock (buffers)
            {
                b = isInitialized;
                Thread.MemoryBarrier();
                if (!b)
                {
                    buffers[0] = new T[bufferSize];
                    buffers[1] = new T[bufferSize];
                    args[0] = new StoreFlushEventArgs<T>(buffers[0]);
                    args[1] = new StoreFlushEventArgs<T>(buffers[1]);

                    flushingThread = new Thread(FlushBuffer) { Priority = flushingThreadPriority, IsBackground = true };
                    flushingThread.Start();

                    Thread.MemoryBarrier();
                    isInitialized = true;
                    Thread.MemoryBarrier();
                }
            }
        }

        return performAdd(item);
    }

    private void ClearAllBuffers()
    {
        ClearBuffer(buffers[0], 0, bufferSize - 1);
        ClearBuffer(buffers[1], 0, bufferSize - 1);
    }

    private void ClearBuffer(IList<T> buffer, int from, int to)
    {
        for (var i = from; i <= to; i++) buffer[i] = defaultT;
    }

    private void DoFlush()
    {
        try
        {
            var currentWritingBufferIndex = writingBufferIndex;
            Thread.MemoryBarrier();
            var wc = writtenCounts[currentWritingBufferIndex];
            var lfi = lastFlushIndex;
            if (wc == bufferSize)
            {
                var newWritingBufferIndex = currentWritingBufferIndex == 0 ? 1 : 0;
                writeEvents[newWritingBufferIndex].Reset();
                lastFlushIndex = -1;
                writtenCounts[newWritingBufferIndex] = 0;
                writingIndexes[newWritingBufferIndex] = -1;
                Thread.MemoryBarrier();
                writingBufferIndex = newWritingBufferIndex;
                writeEvents[currentWritingBufferIndex].Set();
            }
            else
            {
                lastFlushIndex = wc - 1;
            }

            if (OnStoreFlush != null)
            {
                try
                {
                    var isFlushed = false;
                    if (wc - lfi == bufferSize + 1)
                    {
                        OnStoreFlush(this, args[currentWritingBufferIndex]);
                        isFlushed = true;
                    }
                    else
                    {
                        if (wc - lfi >= 2)
                        {
                            OnStoreFlush(this,
                                new StoreFlushEventArgs<T>(buffers[currentWritingBufferIndex], lfi + 1, wc - 1));
                            isFlushed = true;
                        }
                    }

                    if (ClearBufferAfterFlush && isFlushed && isReferenceType)
                    {
                        ClearBuffer(buffers[currentWritingBufferIndex], lfi + 1, wc - 1);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToDetailedString());
                }
            }
        }
        catch (ThreadAbortException ex)
        {
            Debug.WriteLine(ex.ToDetailedString());
        }
    }

    private void FlushBuffer()
    {
        while (true)
        {
            flushStartedAt = -1;
            Thread.MemoryBarrier();
            flushEvent.WaitOne(PeriodicFlushInterval);
            flushStartedAt = SharedStopwatch.ElapsedTicks;
            Thread.MemoryBarrier();
            forceFlushEvent.Reset();
            DoFlush();
            flushStartedAt = -1;
            forceFlushEvent.Set();
            Thread.MemoryBarrier();
            var disposedCalled = isDisposeCalled;
            Thread.MemoryBarrier();
            if (disposedCalled)
            {
                if (isReferenceType)
                {
                    ClearAllBuffers();
                }

                break;
            }
        }
    }

    private void ForceFlush(bool isDispose)
    {
        forceFlushEvent.WaitOne();
        if (isDispose)
        {
            isDisposeCalled = true;
        }

        flushEvent.Set();
        forceFlushEvent.WaitOne();
    }

    private bool performAdd(T item)
    {
        int wi;
        int index;
        while (true)
        {
            Thread.MemoryBarrier();
            wi = writingBufferIndex;
            index = Interlocked.Increment(ref writingIndexes[wi]);
            if (index >= bufferSize || index < 0)
            {
                if (MaxWaitTime > -1)
                {
                    Thread.MemoryBarrier();
                    var l = flushStartedAt;
                    if (l != -1 && SharedStopwatch.ElapsedTicks - l > maxWaitTimeInTicks)
                    {
                        return false;
                    }
                }

                writeEvents[wi].WaitOne(WaitTime);
            }
            else
            {
                break;
            }
        }

        buffers[wi][index] = item;
        var wc = Interlocked.Increment(ref writtenCounts[wi]);
        if (wc == bufferSize)
        {
            flushEvent.Set();
        }

        return true;
    }
}
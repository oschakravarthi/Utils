//---------------------------------------------------------------------
// <copyright file="CabWorker.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// Part of the Deployment Tools Foundation project.
// </summary>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SubhadraSolutions.Utils.Compression.Cab;

internal abstract class CabWorker : IDisposable
{
    internal const string CabStreamName = "%%CAB%%";

    protected long currentArchiveBytesProcessed;
    protected string currentArchiveName;
    protected short currentArchiveNumber;
    protected long currentArchiveTotalBytes;
    protected long currentFileBytesProcessed;

    // Progress data
    protected string currentFileName;

    protected int currentFileNumber;
    protected long currentFileTotalBytes;
    protected short currentFolderNumber;
    protected long currentFolderTotalBytes;
    protected long fileBytesProcessed;
    protected short totalArchives;
    protected long totalFileBytes;
    protected int totalFiles;
    private readonly CabEngine cabEngine;
    private readonly IDictionary<string, short> cabNumbers;
    private readonly NativeMethods.ERF erf;
    private readonly HandleManager<Stream> streamHandles;
    private byte[] buf;
    private Stream cabStream;
    private GCHandle erfHandle;
    private Stream fileStream;
    private string nextCabinetName;
    private bool suppressProgressEvents;

    protected CabWorker(CabEngine cabEngine)
    {
        this.cabEngine = cabEngine;
        streamHandles = new HandleManager<Stream>();
        erf = new NativeMethods.ERF();
        erfHandle = GCHandle.Alloc(erf, GCHandleType.Pinned);
        cabNumbers = new Dictionary<string, short>(1);

        // 32K seems to be the size of the largest chunks processed by cabinet.dll.
        // But just in case, this buffer will auto-enlarge.
        buf = new byte[32768];
    }

    ~CabWorker()
    {
        Dispose(false);
    }

    public CabEngine CabEngine
    {
        get
        {
            return cabEngine;
        }
    }

    internal IDictionary<string, short> CabNumbers
    {
        get
        {
            return cabNumbers;
        }
    }

    internal Stream CabStream
    {
        get
        {
            return cabStream;
        }

        set
        {
            cabStream = value;
        }
    }

    internal NativeMethods.ERF Erf
    {
        get
        {
            return erf;
        }
    }

    internal GCHandle ErfHandle
    {
        get
        {
            return erfHandle;
        }
    }

    internal Stream FileStream
    {
        get
        {
            return fileStream;
        }

        set
        {
            fileStream = value;
        }
    }

    internal string NextCabinetName
    {
        get
        {
            return nextCabinetName;
        }

        set
        {
            nextCabinetName = value;
        }
    }

    internal HandleManager<Stream> StreamHandles
    {
        get
        {
            return streamHandles;
        }
    }

    internal bool SuppressProgressEvents
    {
        get
        {
            return suppressProgressEvents;
        }

        set
        {
            suppressProgressEvents = value;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    internal nint CabAllocMem(int byteCount)
    {
        nint memPointer = Marshal.AllocHGlobal((nint)byteCount);
        return memPointer;
    }

    internal int CabCloseStream(int streamHandle)
    {
        return CabCloseStreamEx(streamHandle, out int err, nint.Zero);
    }

    internal virtual int CabCloseStreamEx(int streamHandle, out int err, nint pv)
    {
        streamHandles.FreeHandle(streamHandle);
        err = 0;
        return 0;
    }

    internal void CabFreeMem(nint memPointer)
    {
        Marshal.FreeHGlobal(memPointer);
    }

    internal int CabOpenStream(string path, int openFlags, int shareMode)
    {
        return CabOpenStreamEx(path, openFlags, shareMode, out int err, nint.Zero);
    }

    internal virtual int CabOpenStreamEx(string path, int openFlags, int shareMode, out int err, nint pv)
    {
        path = path.Trim();
        Stream stream = cabStream;
        cabStream = new DuplicateStream(stream);
        int streamHandle = streamHandles.AllocHandle(stream);
        err = 0;
        return streamHandle;
    }

    internal int CabReadStream(int streamHandle, nint memory, int cb)
    {
        return CabReadStreamEx(streamHandle, memory, cb, out int err, nint.Zero);
    }

    internal virtual int CabReadStreamEx(int streamHandle, nint memory, int cb, out int err, nint pv)
    {
        Stream stream = streamHandles[streamHandle];
        int count = cb;
        if (count > buf.Length)
        {
            buf = new byte[count];
        }
        count = stream.Read(buf, 0, count);
        Marshal.Copy(buf, 0, memory, count);
        err = 0;
        return count;
    }

    internal int CabSeekStream(int streamHandle, int offset, int seekOrigin)
    {
        return CabSeekStreamEx(streamHandle, offset, seekOrigin, out int err, nint.Zero);
    }

    internal virtual int CabSeekStreamEx(int streamHandle, int offset, int seekOrigin, out int err, nint pv)
    {
        Stream stream = streamHandles[streamHandle];
        offset = (int)stream.Seek(offset, (SeekOrigin)seekOrigin);
        err = 0;
        return offset;
    }

    internal int CabWriteStream(int streamHandle, nint memory, int cb)
    {
        return CabWriteStreamEx(streamHandle, memory, cb, out int err, nint.Zero);
    }

    internal virtual int CabWriteStreamEx(int streamHandle, nint memory, int cb, out int err, nint pv)
    {
        Stream stream = streamHandles[streamHandle];
        int count = cb;
        if (count > buf.Length)
        {
            buf = new byte[count];
        }
        Marshal.Copy(memory, buf, 0, count);
        stream.Write(buf, 0, count);
        err = 0;
        return cb;
    }

    protected void CheckError(bool extracting)
    {
        if (Erf.Error)
        {
            throw new CabException(
                Erf.Oper,
                Erf.Type,
                CabException.GetErrorMessage(Erf.Oper, Erf.Type, extracting));
        }
    }

    /// <summary>
    /// Disposes of resources allocated by the cabinet engine.
    /// </summary>
    /// <param name="disposing">If true, the method has been called directly or indirectly by a user's code,
    /// so managed and unmanaged resources will be disposed. If false, the method has been called by the
    /// runtime from inside the finalizer, and only unmanaged resources will be disposed.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (cabStream != null)
            {
                cabStream.Close();
                cabStream = null;
            }

            if (fileStream != null)
            {
                fileStream.Close();
                fileStream = null;
            }
        }

        if (erfHandle.IsAllocated)
        {
            erfHandle.Free();
        }
    }

    protected void OnProgress(ArchiveProgressType progressType)
    {
        if (!suppressProgressEvents)
        {
            ArchiveProgressEventArgs e = new ArchiveProgressEventArgs(
                progressType,
                currentFileName,
                currentFileNumber >= 0 ? currentFileNumber : 0,
                totalFiles,
                currentFileBytesProcessed,
                currentFileTotalBytes,
                currentArchiveName,
                currentArchiveNumber,
                totalArchives,
                currentArchiveBytesProcessed,
                currentArchiveTotalBytes,
                fileBytesProcessed,
                totalFileBytes);
            CabEngine.ReportProgress(e);
        }
    }

    protected void ResetProgressData()
    {
        currentFileName = null;
        currentFileNumber = 0;
        totalFiles = 0;
        currentFileBytesProcessed = 0;
        currentFileTotalBytes = 0;
        currentFolderNumber = 0;
        currentFolderTotalBytes = 0;
        currentArchiveName = null;
        currentArchiveNumber = 0;
        totalArchives = 0;
        currentArchiveBytesProcessed = 0;
        currentArchiveTotalBytes = 0;
        fileBytesProcessed = 0;
        totalFileBytes = 0;
    }
}
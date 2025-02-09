//---------------------------------------------------------------------
// <copyright file="CabUnpacker.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// Part of the Deployment Tools Foundation project.
// </summary>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SubhadraSolutions.Utils.Compression.Cab;

internal class CabUnpacker : CabWorker
{
    // These delegates need to be saved as member variables
    // so that they don't get GC'd.
    private readonly NativeMethods.FDI.PFNALLOC fdiAllocMemHandler;

    private readonly NativeMethods.FDI.PFNCLOSE fdiCloseStreamHandler;
    private readonly NativeMethods.FDI.PFNFREE fdiFreeMemHandler;
    private readonly NativeMethods.FDI.PFNOPEN fdiOpenStreamHandler;
    private readonly NativeMethods.FDI.PFNREAD fdiReadStreamHandler;
    private readonly NativeMethods.FDI.PFNSEEK fdiSeekStreamHandler;
    private readonly NativeMethods.FDI.PFNWRITE fdiWriteStreamHandler;
    private IUnpackStreamContext context;
    private NativeMethods.FDI.Handle fdiHandle;
    private List<ArchiveFileInfo> fileList;

    private Predicate<string> filter;
    private int folderId;

    public CabUnpacker(CabEngine cabEngine)
        : base(cabEngine)
    {
        fdiAllocMemHandler = CabAllocMem;
        fdiFreeMemHandler = CabFreeMem;
        fdiOpenStreamHandler = CabOpenStream;
        fdiReadStreamHandler = CabReadStream;
        fdiWriteStreamHandler = CabWriteStream;
        fdiCloseStreamHandler = CabCloseStream;
        fdiSeekStreamHandler = CabSeekStream;

        fdiHandle = NativeMethods.FDI.Create(
            fdiAllocMemHandler,
            fdiFreeMemHandler,
            fdiOpenStreamHandler,
            fdiReadStreamHandler,
            fdiWriteStreamHandler,
            fdiCloseStreamHandler,
            fdiSeekStreamHandler,
            NativeMethods.FDI.CPU_80386,
            ErfHandle.AddrOfPinnedObject());
        if (Erf.Error)
        {
            int error = Erf.Oper;
            int errorCode = Erf.Type;
            ErfHandle.Free();
            throw new CabException(
                error,
                errorCode,
                CabException.GetErrorMessage(error, errorCode, true));
        }
    }

    public IList<ArchiveFileInfo> GetFileInfo(
        IUnpackStreamContext streamContext,
        Predicate<string> fileFilter)
    {
        if (streamContext == null)
        {
            throw new ArgumentNullException(nameof(streamContext));
        }

        lock (this)
        {
            context = streamContext;
            filter = fileFilter;
            NextCabinetName = string.Empty;
            fileList = [];
            bool tmpSuppress = SuppressProgressEvents;
            SuppressProgressEvents = true;
            try
            {
                for (short cabNumber = 0;
                     NextCabinetName != null;
                     cabNumber++)
                {
                    Erf.Clear();
                    CabNumbers[NextCabinetName] = cabNumber;

                    NativeMethods.FDI.Copy(
                        fdiHandle,
                        NextCabinetName,
                        string.Empty,
                        0,
                        CabListNotify,
                        nint.Zero,
                        nint.Zero);
                    CheckError(true);
                }

                List<ArchiveFileInfo> tmpFileList = fileList;
                fileList = null;
                return tmpFileList.AsReadOnly();
            }
            finally
            {
                SuppressProgressEvents = tmpSuppress;

                if (CabStream != null)
                {
                    context.CloseArchiveReadStream(
                        currentArchiveNumber,
                        currentArchiveName,
                        CabStream);
                    CabStream = null;
                }

                context = null;
            }
        }
    }

    public bool IsArchive(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        lock (this)
        {
            return IsCabinet(stream, out short id, out int folderCount, out int fileCount);
        }
    }

    public void Unpack(
        IUnpackStreamContext streamContext,
        Predicate<string> fileFilter)
    {
        lock (this)
        {
            IList<ArchiveFileInfo> files =
                GetFileInfo(streamContext, fileFilter);

            ResetProgressData();

            if (files != null)
            {
                totalFiles = files.Count;

                for (int i = 0; i < files.Count; i++)
                {
                    totalFileBytes += files[i].Length;
                    if (files[i].ArchiveNumber >= totalArchives)
                    {
                        int ta = files[i].ArchiveNumber + 1;
                        this.totalArchives = (short)ta;
                    }
                }
            }

            context = streamContext;
            fileList = null;
            NextCabinetName = string.Empty;
            folderId = -1;
            currentFileNumber = -1;

            try
            {
                for (short cabNumber = 0;
                     NextCabinetName != null;
                     cabNumber++)
                {
                    Erf.Clear();
                    CabNumbers[NextCabinetName] = cabNumber;

                    NativeMethods.FDI.Copy(
                        fdiHandle,
                        NextCabinetName,
                        string.Empty,
                        0,
                        CabExtractNotify,
                        nint.Zero,
                        nint.Zero);
                    CheckError(true);
                }
            }
            finally
            {
                if (CabStream != null)
                {
                    context.CloseArchiveReadStream(
                        currentArchiveNumber,
                        currentArchiveName,
                        CabStream);
                    CabStream = null;
                }

                if (FileStream != null)
                {
                    context.CloseFileWriteStream(currentFileName, FileStream, FileAttributes.Normal, DateTime.Now);
                    FileStream = null;
                }

                context = null;
            }
        }
    }

    internal override int CabCloseStreamEx(int streamHandle, out int err, nint pv)
    {
        Stream stream = DuplicateStream.OriginalStream(StreamHandles[streamHandle]);

        if (stream == DuplicateStream.OriginalStream(CabStream))
        {
            if (folderId != -3)  // -3 is a special folderId that requires re-opening the same cab
            {
                OnProgress(ArchiveProgressType.FinishArchive);
            }

            context.CloseArchiveReadStream(currentArchiveNumber, currentArchiveName, stream);

            currentArchiveName = NextCabinetName;
            currentArchiveBytesProcessed = currentArchiveTotalBytes = 0;

            CabStream = null;
        }
        return base.CabCloseStreamEx(streamHandle, out err, pv);
    }

    internal override int CabOpenStreamEx(string path, int openFlags, int shareMode, out int err, nint pv)
    {
        if (CabNumbers.ContainsKey(path))
        {
            Stream stream = CabStream;
            if (stream == null)
            {
                short cabNumber = CabNumbers[path];

                stream = context.OpenArchiveReadStream(cabNumber, path, CabEngine);
                if (stream == null)
                {
                    throw new FileNotFoundException(string.Format(CultureInfo.InvariantCulture, "Cabinet {0} not provided.", cabNumber));
                }
                currentArchiveName = path;
                currentArchiveNumber = cabNumber;
                if (totalArchives <= currentArchiveNumber)
                {
                    int ta = currentArchiveNumber + 1;
                    this.totalArchives = (short)ta;
                }
                currentArchiveTotalBytes = stream.Length;
                currentArchiveBytesProcessed = 0;

                if (folderId != -3)  // -3 is a special folderId that requires re-opening the same cab
                {
                    OnProgress(ArchiveProgressType.StartArchive);
                }
                CabStream = stream;
            }
            path = CabStreamName;
        }
        return base.CabOpenStreamEx(path, openFlags, shareMode, out err, pv);
    }

    internal override int CabReadStreamEx(int streamHandle, nint memory, int cb, out int err, nint pv)
    {
        int count = base.CabReadStreamEx(streamHandle, memory, cb, out err, pv);
        if (err == 0 && CabStream != null)
        {
            if (fileList == null)
            {
                Stream stream = StreamHandles[streamHandle];
                if (DuplicateStream.OriginalStream(stream) ==
                    DuplicateStream.OriginalStream(CabStream))
                {
                    currentArchiveBytesProcessed += cb;
                    if (currentArchiveBytesProcessed > currentArchiveTotalBytes)
                    {
                        currentArchiveBytesProcessed = currentArchiveTotalBytes;
                    }
                }
            }
        }
        return count;
    }

    internal override int CabWriteStreamEx(int streamHandle, nint memory, int cb, out int err, nint pv)
    {
        int count = base.CabWriteStreamEx(streamHandle, memory, cb, out err, pv);
        if (count > 0 && err == 0)
        {
            currentFileBytesProcessed += cb;
            fileBytesProcessed += cb;
            OnProgress(ArchiveProgressType.PartialFile);
        }
        return count;
    }

    /// <summary>
    /// Disposes of resources allocated by the cabinet engine.
    /// </summary>
    /// <param name="disposing">If true, the method has been called directly or indirectly by a user's code,
    /// so managed and unmanaged resources will be disposed. If false, the method has been called by the
    /// runtime from inside the finalizer, and only unmanaged resources will be disposed.</param>
    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing)
            {
                if (fdiHandle != null)
                {
                    fdiHandle.Dispose();
                    fdiHandle = null;
                }
            }
        }
        finally
        {
            base.Dispose(disposing);
        }
    }

    private static string GetFileName(NativeMethods.FDI.NOTIFICATION notification)
    {
        bool utf8Name = (notification.attribs & (ushort)FileAttributes.Normal) != 0;  // _A_NAME_IS_UTF

        // Non-utf8 names should be completely ASCII. But for compatibility with
        // legacy tools, interpret them using the current (Default) ANSI codepage.
        Encoding nameEncoding = utf8Name ? Encoding.UTF8 : Encoding.Default;

        // Find how many bytes are in the string.
        // Unfortunately there is no faster way.
        int nameBytesCount = 0;
        while (Marshal.ReadByte(notification.psz1, nameBytesCount) != 0)
        {
            nameBytesCount++;
        }

        byte[] nameBytes = new byte[nameBytesCount];
        Marshal.Copy(notification.psz1, nameBytes, 0, nameBytesCount);
        string name = nameEncoding.GetString(nameBytes);
        if (Path.IsPathRooted(name))
        {
            name = name.Replace("" + Path.VolumeSeparatorChar, "");
        }

        return name;
    }

    private int CabExtractCloseFile(NativeMethods.FDI.NOTIFICATION notification)
    {
        Stream stream = StreamHandles[notification.hf];
        StreamHandles.FreeHandle(notification.hf);

        //bool execute = (notification.attribs & (ushort) FileAttributes.Device) != 0;  // _A_EXEC

        string name = GetFileName(notification);

        FileAttributes attributes = (FileAttributes)notification.attribs &
                                    (FileAttributes.Archive | FileAttributes.Hidden | FileAttributes.ReadOnly | FileAttributes.System);
        if (attributes == 0)
        {
            attributes = FileAttributes.Normal;
        }
        CompressionEngine.DosDateAndTimeToDateTime(notification.date, notification.time, out DateTime lastWriteTime);

        stream.Flush();
        context.CloseFileWriteStream(name, stream, attributes, lastWriteTime);
        FileStream = null;

        long remainder = currentFileTotalBytes - currentFileBytesProcessed;
        currentFileBytesProcessed += remainder;
        fileBytesProcessed += remainder;
        OnProgress(ArchiveProgressType.FinishFile);
        currentFileName = null;

        return 1;  // Continue
    }

    private int CabExtractCopyFile(NativeMethods.FDI.NOTIFICATION notification)
    {
        if (notification.iFolder != folderId)
        {
            if (notification.iFolder != -3)  // -3 is a special folderId used when continuing a folder from a previous cab
            {
                if (folderId != -1) // -1 means we just started the extraction sequence
                {
                    currentFolderNumber++;
                }
            }
            folderId = notification.iFolder;
        }

        //bool execute = (notification.attribs & (ushort) FileAttributes.Device) != 0;  // _A_EXEC

        string name = GetFileName(notification);

        if (filter == null || filter(name))
        {
            currentFileNumber++;
            currentFileName = name;

            currentFileBytesProcessed = 0;
            currentFileTotalBytes = notification.cb;
            OnProgress(ArchiveProgressType.StartFile);

            CompressionEngine.DosDateAndTimeToDateTime(notification.date, notification.time, out DateTime lastWriteTime);

            Stream stream = context.OpenFileWriteStream(name, notification.cb, lastWriteTime);
            if (stream != null)
            {
                FileStream = stream;
                int streamHandle = StreamHandles.AllocHandle(stream);
                return streamHandle;
            }
            else
            {
                fileBytesProcessed += notification.cb;
                OnProgress(ArchiveProgressType.FinishFile);
                currentFileName = null;
            }
        }
        return 0;  // Continue
    }

    private int CabExtractNotify(NativeMethods.FDI.NOTIFICATIONTYPE notificationType, NativeMethods.FDI.NOTIFICATION notification)
    {
        switch (notificationType)
        {
            case NativeMethods.FDI.NOTIFICATIONTYPE.CABINET_INFO:
                {
                    if (NextCabinetName?.StartsWith("?", StringComparison.Ordinal) == true)
                    {
                        // We are just continuing the copy of a file that spanned cabinets.
                        // The next cabinet name needs to be preserved.
                        NextCabinetName = NextCabinetName.Substring(1);
                    }
                    else
                    {
                        string nextCab = Marshal.PtrToStringAnsi(notification.psz1);
                        NextCabinetName = nextCab.Length != 0 ? nextCab : null;
                    }
                    return 0;  // Continue
                }
            case NativeMethods.FDI.NOTIFICATIONTYPE.NEXT_CABINET:
                {
                    string nextCab = Marshal.PtrToStringAnsi(notification.psz1);
                    CabNumbers[nextCab] = notification.iCabinet;
                    NextCabinetName = "?" + NextCabinetName;
                    return 0;  // Continue
                }
            case NativeMethods.FDI.NOTIFICATIONTYPE.COPY_FILE:
                {
                    return CabExtractCopyFile(notification);
                }
            case NativeMethods.FDI.NOTIFICATIONTYPE.CLOSE_FILE_INFO:
                {
                    return CabExtractCloseFile(notification);
                }
        }
        return 0;
    }

    private int CabListNotify(NativeMethods.FDI.NOTIFICATIONTYPE notificationType, NativeMethods.FDI.NOTIFICATION notification)
    {
        switch (notificationType)
        {
            case NativeMethods.FDI.NOTIFICATIONTYPE.CABINET_INFO:
                {
                    string nextCab = Marshal.PtrToStringAnsi(notification.psz1);
                    NextCabinetName = nextCab.Length != 0 ? nextCab : null;
                    return 0;  // Continue
                }
            case NativeMethods.FDI.NOTIFICATIONTYPE.PARTIAL_FILE:
                {
                    // This notification can occur when examining the contents of a non-first cab file.
                    return 0;  // Continue
                }
            case NativeMethods.FDI.NOTIFICATIONTYPE.COPY_FILE:
                {
                    //bool execute = (notification.attribs & (ushort) FileAttributes.Device) != 0;  // _A_EXEC

                    string name = GetFileName(notification);

                    if (filter == null || filter(name))
                    {
                        if (fileList != null)
                        {
                            FileAttributes attributes = (FileAttributes)notification.attribs &
                                                        (FileAttributes.Archive | FileAttributes.Hidden | FileAttributes.ReadOnly | FileAttributes.System);
                            if (attributes == 0)
                            {
                                attributes = FileAttributes.Normal;
                            }
                            CompressionEngine.DosDateAndTimeToDateTime(notification.date, notification.time, out DateTime lastWriteTime);
                            long length = notification.cb;

                            CabFileInfo fileInfo = new CabFileInfo(
                                name,
                                notification.iFolder,
                                notification.iCabinet,
                                attributes,
                                lastWriteTime,
                                length);
                            fileList.Add(fileInfo);
                            currentFileNumber = fileList.Count - 1;
                            fileBytesProcessed += notification.cb;
                        }
                    }

                    totalFiles++;
                    totalFileBytes += notification.cb;
                    return 0;  // Continue
                }
        }
        return 0;
    }

    private bool IsCabinet(Stream cabStream, out short id, out int cabFolderCount, out int fileCount)
    {
        int streamHandle = StreamHandles.AllocHandle(cabStream);
        try
        {
            Erf.Clear();
            bool isCabinet = 0 != NativeMethods.FDI.IsCabinet(fdiHandle, streamHandle, out NativeMethods.FDI.CABINFO fdici);

            if (Erf.Error)
            {
                if ((NativeMethods.FDI.ERROR)Erf.Oper == NativeMethods.FDI.ERROR.UNKNOWN_CABINET_VERSION)
                {
                    isCabinet = false;
                }
                else
                {
                    throw new CabException(
                        Erf.Oper,
                        Erf.Type,
                        CabException.GetErrorMessage(Erf.Oper, Erf.Type, true));
                }
            }

            id = fdici.setID;
            cabFolderCount = fdici.cFolders;
            fileCount = fdici.cFiles;
            return isCabinet;
        }
        finally
        {
            StreamHandles.FreeHandle(streamHandle);
        }
    }
}
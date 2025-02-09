//---------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// Part of the Deployment Tools Foundation project.
// </summary>
//---------------------------------------------------------------------

using SubhadraSolutions.Utils.Runtime.InteropServices;
using System.Runtime.InteropServices;
using System.Security;

namespace SubhadraSolutions.Utils.Compression.Cab;

/// <summary>
/// Native DllImport methods and related structures and constants used for
/// cabinet creation and extraction via cabinet.dll.
/// </summary>
internal static class NativeMethods
{
    static NativeMethods()
    {
        InteropServicesHelper.EnsureWindowsPlatform();
    }

    /// <summary>
    /// A direct import of constants, enums, structures, delegates, and functions from fci.h.
    /// Refer to comments in fci.h for documentation.
    /// </summary>
    internal static class FCI
    {
        internal const int CPU_80386 = 1;
        internal const int MAX_CAB_PATH = 256;
        internal const int MAX_CABINET_NAME = 256;
        internal const int MAX_DISK = int.MaxValue;
        internal const int MAX_DISK_NAME = 256;
        internal const int MAX_FILENAME = 256;
        internal const int MAX_FOLDER = 0x7FFF8000;
        internal const int MIN_DISK = 32768;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate nint PFNALLOC(int cb);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int PFNCLOSE(int fileHandle, out int err, nint pv);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int PFNDELETE(string path, out int err, nint pv);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int PFNFILEPLACED(nint pccab, string path, long fileSize, int continuation, nint pv);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void PFNFREE(nint pv);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int PFNGETNEXTCABINET(nint pccab, uint cbPrevCab, nint pv);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int PFNGETOPENINFO(string path, out short date, out short time, out short pattribs, out int err, nint pv);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int PFNGETTEMPFILE(nint tempNamePtr, int tempNameSize, nint pv);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int PFNOPEN(string path, int oflag, int pmode, out int err, nint pv);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int PFNREAD(int fileHandle, nint memory, int cb, out int err, nint pv);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int PFNSEEK(int fileHandle, int dist, int seekType, out int err, nint pv);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int PFNSTATUS(STATUS typeStatus, uint cb1, uint cb2, nint pv);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int PFNWRITE(int fileHandle, nint memory, int cb, out int err, nint pv);

        /// <summary>
        /// Error codes that can be returned by FCI.
        /// </summary>
        internal enum ERROR : int
        {
            NONE,
            OPEN_SRC,
            READ_SRC,
            ALLOC_FAIL,
            TEMP_FILE,
            BAD_COMPR_TYPE,
            CAB_FILE,
            USER_ABORT,
            MCI_FAIL,
        }

        /// <summary>
        /// Reason for FCI status callback.
        /// </summary>
        internal enum STATUS : uint
        {
            FILE = 0,
            FOLDER = 1,
            CABINET = 2,
        }

        /// <summary>
        /// FCI compression algorithm types and parameters.
        /// </summary>
        internal enum TCOMP : ushort
        {
            MASK_TYPE = 0x000F,
            TYPE_NONE = 0x0000,
            TYPE_MSZIP = 0x0001,
            TYPE_QUANTUM = 0x0002,
            TYPE_LZX = 0x0003,
            BAD = 0x000F,

            MASK_LZX_WINDOW = 0x1F00,
            LZX_WINDOW_LO = 0x0F00,
            LZX_WINDOW_HI = 0x1500,
            SHIFT_LZX_WINDOW = 0x0008,

            MASK_QUANTUM_LEVEL = 0x00F0,
            QUANTUM_LEVEL_LO = 0x0010,
            QUANTUM_LEVEL_HI = 0x0070,
            SHIFT_QUANTUM_LEVEL = 0x0004,

            MASK_QUANTUM_MEM = 0x1F00,
            QUANTUM_MEM_LO = 0x0A00,
            QUANTUM_MEM_HI = 0x1500,
            SHIFT_QUANTUM_MEM = 0x0008,

            MASK_RESERVED = 0xE000,
        }

        [DllImport("cabinet.dll", EntryPoint = "FCIAddFile", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int AddFile(Handle hfci, string pszSourceFile, nint pszFileName, [MarshalAs(UnmanagedType.Bool)] bool fExecute, PFNGETNEXTCABINET pfnfcignc, PFNSTATUS pfnfcis, PFNGETOPENINFO pfnfcigoi, TCOMP typeCompress);

        [DllImport("cabinet.dll", EntryPoint = "FCICreate", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern Handle Create(nint perf, PFNFILEPLACED pfnfcifp, PFNALLOC pfna, PFNFREE pfnf, PFNOPEN pfnopen, PFNREAD pfnread, PFNWRITE pfnwrite, PFNCLOSE pfnclose, PFNSEEK pfnseek, PFNDELETE pfndelete, PFNGETTEMPFILE pfnfcigtf, [MarshalAs(UnmanagedType.LPStruct)] CCAB pccab, nint pv);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("cabinet.dll", EntryPoint = "FCIDestroy", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool Destroy(nint hfci);

        [DllImport("cabinet.dll", EntryPoint = "FCIFlushCabinet", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int FlushCabinet(Handle hfci, [MarshalAs(UnmanagedType.Bool)] bool fGetNextCab, PFNGETNEXTCABINET pfnfcignc, PFNSTATUS pfnfcis);

        [DllImport("cabinet.dll", EntryPoint = "FCIFlushFolder", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int FlushFolder(Handle hfci, PFNGETNEXTCABINET pfnfcignc, PFNSTATUS pfnfcis);

        /// <summary>
        /// Cabinet information structure used for FCI initialization and GetNextCabinet callback.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal class CCAB
        {
            internal int cb = MAX_DISK;
            internal int cbFolderThresh = MAX_FOLDER;
            internal int cbReserveCFHeader;
            internal int cbReserveCFFolder;
            internal int cbReserveCFData;
            internal int iCab;
            internal int iDisk;
            internal int fFailOnIncompressible;
            internal short setID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_DISK_NAME)] internal string szDisk = string.Empty;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_CABINET_NAME)] internal string szCab = string.Empty;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_CAB_PATH)] internal string szCabPath = string.Empty;
        }

        /// <summary>
        /// Ensures that the FCI handle is safely released.
        /// </summary>
        internal class Handle : SafeHandle
        {
            /// <summary>
            /// Creates a new unintialized handle. The handle will be initialized
            /// when it is marshalled back from native code.
            /// </summary>
            internal Handle()
                : base(nint.Zero, true)
            {
            }

            /// <summary>
            /// Checks if the handle is invalid. An FCI handle is invalid when it is zero.
            /// </summary>
            public override bool IsInvalid
            {
                get
                {
                    return handle == nint.Zero;
                }
            }

            /// <summary>
            /// Releases the handle by calling FDIDestroy().
            /// </summary>
            /// <returns>True if the release succeeded.</returns>
            protected override bool ReleaseHandle()
            {
                return Destroy(handle);
            }
        }
    }

    /// <summary>
    /// A direct import of constants, enums, structures, delegates, and functions from fdi.h.
    /// Refer to comments in fdi.h for documentation.
    /// </summary>
    internal static class FDI
    {
        internal const int CPU_80386 = 1;
        internal const int MAX_CAB_PATH = 256;
        internal const int MAX_CABINET_NAME = 256;
        internal const int MAX_DISK = int.MaxValue;
        internal const int MAX_DISK_NAME = 256;
        internal const int MAX_FILENAME = 256;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate nint PFNALLOC(int cb);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int PFNCLOSE(int hf);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void PFNFREE(nint pv);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int PFNNOTIFY(NOTIFICATIONTYPE fdint, NOTIFICATION fdin);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int PFNOPEN(string path, int oflag, int pmode);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int PFNREAD(int hf, nint pv, int cb);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int PFNSEEK(int hf, int dist, int seektype);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int PFNWRITE(int hf, nint pv, int cb);

        /// <summary>
        /// Error codes that can be returned by FDI.
        /// </summary>
        internal enum ERROR : int
        {
            NONE,
            CABINET_NOT_FOUND,
            NOT_A_CABINET,
            UNKNOWN_CABINET_VERSION,
            CORRUPT_CABINET,
            ALLOC_FAIL,
            BAD_COMPR_TYPE,
            MDI_FAIL,
            TARGET_FILE,
            RESERVE_MISMATCH,
            WRONG_CABINET,
            USER_ABORT,
        }

        /// <summary>
        /// Type of notification message for the FDI Notify callback.
        /// </summary>
        internal enum NOTIFICATIONTYPE : int
        {
            CABINET_INFO,
            PARTIAL_FILE,
            COPY_FILE,
            CLOSE_FILE_INFO,
            NEXT_CABINET,
            ENUMERATE,
        }

        [DllImport("cabinet.dll", EntryPoint = "FDICopy", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Copy(Handle hfdi, string pszCabinet, string pszCabPath, int flags, PFNNOTIFY pfnfdin, nint pfnfdid, nint pvUser);

        [DllImport("cabinet.dll", EntryPoint = "FDICreate", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern Handle Create([MarshalAs(UnmanagedType.FunctionPtr)] PFNALLOC pfnalloc, [MarshalAs(UnmanagedType.FunctionPtr)] PFNFREE pfnfree, PFNOPEN pfnopen, PFNREAD pfnread, PFNWRITE pfnwrite, PFNCLOSE pfnclose, PFNSEEK pfnseek, int cpuType, nint perf);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("cabinet.dll", EntryPoint = "FDIDestroy", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool Destroy(nint hfdi);

        [DllImport("cabinet.dll", EntryPoint = "FDIIsCabinet", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int IsCabinet(Handle hfdi, int hf, out CABINFO pfdici);

        /// <summary>
        /// Cabinet information structure filled in by FDI IsCabinet.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct CABINFO
        {
            internal int cbCabinet;
            internal short cFolders;
            internal short cFiles;
            internal short setID;
            internal short iCabinet;
            internal int fReserve;
            internal int hasprev;
            internal int hasnext;
        }

        /// <summary>
        /// Ensures that the FDI handle is safely released.
        /// </summary>
        internal class Handle : SafeHandle
        {
            /// <summary>
            /// Creates a new unintialized handle. The handle will be initialized
            /// when it is marshalled back from native code.
            /// </summary>
            internal Handle()
                : base(nint.Zero, true)
            {
            }

            /// <summary>
            /// Checks if the handle is invalid. An FDI handle is invalid when it is zero.
            /// </summary>
            public override bool IsInvalid
            {
                get
                {
                    return handle == nint.Zero;
                }
            }

            /// <summary>
            /// Releases the handle by calling FDIDestroy().
            /// </summary>
            /// <returns>True if the release succeeded.</returns>
            protected override bool ReleaseHandle()
            {
                try
                {
                    return Destroy(handle);
                }
                catch
                {
                }
                return true;
            }
        }

        /// <summary>
        /// Cabinet notification details passed to the FDI Notify callback.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal class NOTIFICATION
        {
            internal int cb;
            internal nint psz1;
            internal nint psz2;
            internal nint psz3;
            internal nint pv;

            internal nint hf_ptr;

            internal short date;
            internal short time;
            internal short attribs;
            internal short setID;
            internal short iCabinet;
            internal short iFolder;
            internal int fdie;

            // Unlike all the other file handles in FCI/FDI, this one is
            // actually pointer-sized. Use a property to pretend it isn't.
            internal int hf
            {
                get { return (int)hf_ptr; }
            }
        }
    }

    /// <summary>
    /// Error info structure for FCI and FDI.
    /// </summary>
    /// <remarks>Before being passed to FCI or FDI, this structure is
    /// pinned in memory via a GCHandle. The pinning is necessary
    /// to be able to read the results, since the ERF structure doesn't
    /// get marshalled back out after an error.</remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal class ERF
    {
        private int erfOper;
        private int erfType;
        private int fError;

        /// <summary>
        /// Gets or sets the cabinet error code.
        /// </summary>
        internal int Oper
        {
            get
            {
                return erfOper;
            }

            set
            {
                erfOper = value;
            }
        }

        /// <summary>
        /// Gets or sets the Win32 error code.
        /// </summary>
        internal int Type
        {
            get
            {
                return erfType;
            }

            set
            {
                erfType = value;
            }
        }

        /// <summary>
        /// GCHandle doesn't like the bool type, so use an int underneath.
        /// </summary>
        internal bool Error
        {
            get
            {
                return fError != 0;
            }

            set
            {
                fError = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Clears the error information.
        /// </summary>
        internal void Clear()
        {
            Oper = 0;
            Type = 0;
            Error = false;
        }
    }
}
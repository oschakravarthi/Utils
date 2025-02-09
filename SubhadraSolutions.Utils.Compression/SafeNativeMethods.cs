//---------------------------------------------------------------------
// <copyright file="SafeNativeMethods.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// Part of the Deployment Tools Foundation project.
// </summary>
//---------------------------------------------------------------------

using SubhadraSolutions.Utils.Runtime.InteropServices;
using System.Runtime.InteropServices;
using System.Security;

namespace SubhadraSolutions.Utils.Compression;

[SuppressUnmanagedCodeSecurity]
internal static class SafeNativeMethods
{
    static SafeNativeMethods()
    {
        InteropServicesHelper.EnsureWindowsPlatform();
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DosDateTimeToFileTime(
        short wFatDate, short wFatTime, out long fileTime);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool FileTimeToDosDateTime(
        ref long fileTime, out short wFatDate, out short wFatTime);
}
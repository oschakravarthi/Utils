/*
	Copyright (C) 2004 Model Matter, Inc.  Distributed under the GNU Lesser License V2.1

	Purpose:	Windows Installer Support
	Original Author:	Ian Schoen Mahr Mariano

	This library is free software; you can redistribute it and/or
	modify it under the terms of the GNU Lesser General Public
	License as published by the Free Software Foundation; either
	version 2.1 of the License, or (at your option) any later version.

	This library is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
	Lesser General Public License for more details.

	You should have received a copy of the GNU Lesser General Public
	License along with this library; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace SubhadraSolutions.Utils.MSI.Interop;

/// <summary>Enumeration of custom action types.</summary>
internal enum MsiCustomActionType : int
{
    // executable types

    /// <summary>Target = entry point name</summary>
    Dll = 0x00000001,

    /// <summary>Target = command line args</summary>
    Exe = 0x00000002,

    /// <summary>Target = text string to be formatted and set into property</summary>
    TextData = 0x00000003,

    /// <summary>Target = entry point name, null if none to call</summary>
    JScript = 0x00000005,

    /// <summary>Target = entry point name, null if none to call</summary>
    VBScript = 0x00000006,

    /// <summary>Target = property list for nested engine initialization</summary>
    Install = 0x00000007,

    // source of code

    /// <summary>Source = Binary.Name, data stored in stream</summary>
    BinaryData = 0x00000000,

    /// <summary>Source = File.File, file part of installation</summary>
    SourceFile = 0x00000010,

    /// <summary>Source = Directory.Directory, folder containing existing file</summary>
    Directory = 0x00000020,

    /// <summary>Source = Property.Property, full path to executable</summary>
    Property = 0x00000030,

    // return processing default is syncronous execution, process return code

    /// <summary>ignore action return status, continue running</summary>
    Continue = 0x00000040,

    /// <summary>run asynchronously</summary>
    Async = 0x00000080,

    // execution scheduling flags  default is execute whenever sequenced

    /// <summary>skip if UI sequence already run</summary>
    FirstSequence = 0x00000100,

    /// <summary>skip if UI sequence already run in same process</summary>
    OncePerProcess = 0x00000200,

    /// <summary>run on client only if UI already run on client</summary>
    ClientRepeat = 0x00000300,

    /// <summary>queue for execution within script</summary>
    InScript = 0x00000400,

    /// <summary>in conjunction with InScript: queue in Rollback script</summary>
    Rollback = 0x00000100,

    /// <summary>in conjunction with InScript: run Commit ops from script on success</summary>
    Commit = 0x00000200,

    // security context flag, default to impersonate as user, valid only if InScript

    /// <summary>no impersonation, run in system context</summary>
    NoImpersonate = 0x00000800,

    /// <summary>impersonate for per-machine installs on TS machines</summary>
    TSAware = 0x00004000,

    // script requires 64bit process

    /// <summary>script should run in 64bit process</summary>
    Type64BitScript = 0x00001000,

    /// <summary>don't record the contents of the Target field in the log file.</summary>
    HideTarget = 0x00002000,
}
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

using System;

namespace SubhadraSolutions.Utils.MSI.Interop;

/// <summary>Bit-flags for reinstallation.</summary>
[Flags]
public enum MsiReinstallMode : uint
{
    /// <summary>Reserved bit - currently ignored</summary>
    Repair = 0x00000001,

    /// <summary>Reinstall only if file is missing</summary>
    FileMissing = 0x00000002,

    /// <summary>Reinstall if file is missing, or older version</summary>
    FileOlderVersion = 0x00000004,

    /// <summary>Reinstall if file is missing, or equal or older version</summary>
    FileEqualVersion = 0x00000008,

    /// <summary>Reinstall if file is missing, or not exact version</summary>
    FileExact = 0x00000010,

    /// <summary>checksum executables, reinstall if missing or corrupt</summary>
    FileVerify = 0x00000020,

    /// <summary>Reinstall all files, regardless of version</summary>
    FileReplace = 0x00000040,

    /// <summary>insure required machine reg entries</summary>
    MachineData = 0x00000080,

    /// <summary>insure required user reg entries</summary>
    UserData = 0x00000100,

    /// <summary>validate shortcuts items</summary>
    Shortcut = 0x00000200,

    /// <summary>use re-cache source install package</summary>
    Package = 0x00000400,
}
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

/// <summary>Bit-flags for MSI validation.</summary>
[Flags]
public enum MsiValidationFlag : int
{
    /// <summary>Validate no properties.</summary>
    None = 0x00000000,

    /// <summary>Default language must match base database.</summary>
    Language = 0x00000001,

    /// <summary>Product must match base database.</summary>
    Product = 0x00000002,

    /// <summary>Check major version only.</summary>
    MajorVersion = 0x00000008,

    /// <summary>Check major and minor versions only.</summary>
    MinorVersion = 0x00000010,

    /// <summary>Check major, minor, and update versions.</summary>
    UpdateVersion = 0x00000020,

    /// <summary>Installed version &lt; base version.</summary>
    NewLessBaseVersion = 0x00000040,

    /// <summary>Installed version &lt;= base version.</summary>
    NewLessEqualBaseVersion = 0x00000080,

    /// <summary>Installed version = base version.</summary>
    NewEqualBaseVersion = 0x00000100,

    /// <summary>Installed version &gt;= base version.</summary>
    NewGreaterEqualBaseVersion = 0x00000200,

    /// <summary>Installed version &gt; base version.</summary>
    NewGreaterBaseVersion = 0x00000400,

    /// <summary>UpgradeCode must match base database.</summary>
    UpgradeCode = 0x00000800,
}
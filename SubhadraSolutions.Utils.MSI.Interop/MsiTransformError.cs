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

/// <summary>Bit-flag error conditions that should be suppressed when the transform is applied.</summary>
[Flags]
public enum MsiTransformError : int
{
    /// <summary>None of the following conditions.</summary>
    None = 0x00000000,

    /// <summary>Adding a row that already exists.</summary>
    AddExistingRow = 0x00000001,

    /// <summary>Deleting a row that doesn't exist.</summary>
    DelMissingRow = 0x00000002,

    /// <summary>Adding a table that already exists.</summary>
    AddExistingTable = 0x00000004,

    /// <summary>Deleting a table that doesn't exist.</summary>
    DelMissingTable = 0x00000008,

    /// <summary>Updating a row that doesn't exist.</summary>
    UpdateMissingRow = 0x00000010,

    /// <summary>Transform and database code pages do not match and neither code page is neutral.</summary>
    ChangeCodePage = 0x00000020,

    /// <summary>Create the temporary _TransformView table.</summary>
    ViewTransform = 0x00000100,
}
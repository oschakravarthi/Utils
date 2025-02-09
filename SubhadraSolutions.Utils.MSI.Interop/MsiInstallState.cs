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

/// <summary>Enumeration of MSI install states.</summary>
public enum MsiInstallState : int
{
    /// <summary>component disabled</summary>
    NotUsed = -7,

    /// <summary>configuration data corrupt</summary>
    BadConfig = -6,

    /// <summary>installation suspended or in progress</summary>
    Incomplete = -5,

    /// <summary>run from source, source is unavailable</summary>
    SourceAbsent = -4,

    /// <summary>return buffer overflow</summary>
    MoreData = -3,

    /// <summary>invalid function argument</summary>
    InvalidArg = -2,

    /// <summary>unrecognized product or feature</summary>
    Unknown = -1,

    /// <summary>broken</summary>
    Broken = 0,

    /// <summary>advertised feature</summary>
    Advertised = 1,

    /// <summary>component being removed (action state, not settable)</summary>
    Removed = 1,

    /// <summary>uninstalled (or action state absent but clients remain)</summary>
    Absent = 2,

    /// <summary>installed on local drive</summary>
    Local = 3,

    /// <summary>run from source, CD or net</summary>
    Source = 4,

    /// <summary>use default, local or source</summary>
    Default = 5,
}
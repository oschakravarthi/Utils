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

/// <summary>
/// Bit-flags defining an MSI feature's install state.
/// </summary>
[Flags]
public enum MsiFeatureInstallState : int
{
    /// <summary>The feature can be advertised.</summary>
    Advertised = 2,

    /// <summary>The feature can be absent.</summary>
    Absent = 4,

    /// <summary>The feature can be installed on the local drive.</summary>
    Local = 8,

    /// <summary>The feature can be configured to run from source, CD-ROM, or network.</summary>
    Source = 16,

    /// <summary>The feature can be configured to use the default location: local or source.</summary>
    Default = 32,
}
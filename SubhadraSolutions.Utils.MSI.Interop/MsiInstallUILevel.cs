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

/// <summary>Enumeration of internal MSI install UI levels.</summary>
public enum MsiInstallUILevel : uint
{
    /// <summary>UI level is unchanged</summary>
    NoChange = 0,

    /// <summary>default UI is used</summary>
    Default = 1,

    /// <summary>completely silent installation</summary>
    None = 2,

    /// <summary>simple progress and error handling</summary>
    Basic = 3,

    /// <summary>authored UI, wizard dialogs suppressed</summary>
    Reduced = 4,

    /// <summary>authored UI with wizards, progress, errors</summary>
    Full = 5,

    /// <summary>display success/failure dialog at end of install</summary>
    EndDialog = 0x80,

    /// <summary>display only progress dialog</summary>
    ProgressOnly = 0x40,

    /// <summary>do not display the cancel button in basic UI</summary>
    HideCancel = 0x20,

    /// <summary>force display of source resolution even if quiet</summary>
    SourceResOnly = 0x100,
}
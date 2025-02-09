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

/// <summary>
/// <para>Install message type for callback is a combination of the following:</para>
/// <para>A message box style:  MB_*, where MB_OK is the default</para>
/// <para>A message box icon type:  MB_ICON*, where no icon is the default</para>
/// <para>A default button:  MB_DEFBUTTON?, where MB_DEFBUTTON1 is the default</para>
/// <para>One of these flags an install message, no default.</para>
/// </summary>
public enum MsiInstallMessage : long
{
    /// <summary>premature termination, possibly fatal OOM</summary>
    FatalExit = 0x00000000,

    /// <summary>formatted error message</summary>
    Error = 0x01000000,

    /// <summary>formatted warning message</summary>
    Warning = 0x02000000,

    /// <summary>user request message</summary>
    User = 0x03000000,

    /// <summary>informative message for log</summary>
    Info = 0x04000000,

    /// <summary>list of files in use that need to be replaced</summary>
    FilesInUse = 0x05000000,

    /// <summary>request to determine a valid source location</summary>
    ResolveSource = 0x06000000,

    /// <summary>insufficient disk space message</summary>
    OutOfDiskSpace = 0x07000000,

    /// <summary>start of action: action name and description</summary>
    ActionStart = 0x08000000,

    /// <summary>formatted data associated with individual action item</summary>
    ActionData = 0x09000000,

    /// <summary>progress gauge info: units so far, total</summary>
    Progress = 0x0a000000,

    /// <summary>product info for dialog: language Id, dialog caption</summary>
    CommonData = 0x0b000000,

    /// <summary>sent prior to UI initialization, no string data</summary>
    Initialize = 0x0c000000,

    /// <summary>sent after UI termination, no string data</summary>
    Terminate = 0x0d000000,

    /// <summary>sent prior to display or authored dialog or wizard</summary>
    ShowDialog = 0x0e000000,
}
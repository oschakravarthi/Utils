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

/// <summary>Bit flags for use with <see cref="MsiInterop.MsiEnableLog"/> and <see cref="MsiInterop.MsiSetExternalUI"/>.</summary>
[Flags]
public enum MsiInstallLogMode : uint
{
    /// <summary>None.</summary>
    None = 0,

    /// <summary>Logs out of memory or fatal exit information.</summary>
    FatalExit = 1 << (int)(MsiInstallMessage.FatalExit >> 24),

    /// <summary>Logs the error messages.</summary>
    Error = 1 << (int)(MsiInstallMessage.Error >> 24),

    /// <summary>Logs the warning messages.</summary>
    Warning = 1 << (int)(MsiInstallMessage.Warning >> 24),

    /// <summary>Logs the user requests.</summary>
    User = 1 << (int)(MsiInstallMessage.User >> 24),

    /// <summary>Logs the status messages that are not displayed.</summary>
    Info = 1 << (int)(MsiInstallMessage.Info >> 24),

    /// <summary>Request to determine a valid source location.</summary>
    ResolveSource = 1 << (int)(MsiInstallMessage.ResolveSource >> 24),

    /// <summary>Indicates insufficient disk space.</summary>
    OutOfDiskSpace = 1 << (int)(MsiInstallMessage.OutOfDiskSpace >> 24),

    /// <summary>Logs the start of new installation actions.</summary>
    ActionStart = 1 << (int)(MsiInstallMessage.ActionStart >> 24),

    /// <summary>Logs the data record with the installation action.</summary>
    ActionData = 1 << (int)(MsiInstallMessage.ActionData >> 24),

    /// <summary>Logs the parameters for user-interface initialization.</summary>
    CommonData = 1 << (int)(MsiInstallMessage.CommonData >> 24),

    /// <summary>Logs the property values at termination.</summary>
    PropertyDump = 1 << (int)(MsiInstallMessage.Progress >> 24),

    /// <summary>Sends large amounts of information to a log file not generally useful to users. May be used for technical support.</summary>
    Verbose = 1 << (int)(MsiInstallMessage.Initialize >> 24),

    /// <summary>Sends extra debugging information, such as handle creation information, to the log file. <b>Windows XP/2000/98/95:  This feature is not supported.</b></summary>
    ExtraDebug = 1 << (int)(MsiInstallMessage.Terminate >> 24),

    /// <summary>external handler only</summary>
    Progress = 1 << (int)(MsiInstallMessage.Progress >> 24),

    /// <summary>external handler only</summary>
    Initialize = 1 << (int)(MsiInstallMessage.Initialize >> 24),

    /// <summary>external handler only</summary>
    Terminate = 1 << (int)(MsiInstallMessage.Terminate >> 24),

    /// <summary>external handler only</summary>
    ShowDialog = 1 << (int)(MsiInstallMessage.ShowDialog >> 24),

    /// <summary>All modes.</summary>
    ExternalUI = FatalExit | Error | Warning | User | ActionStart | ActionData |
                 CommonData | Progress | ShowDialog,
}
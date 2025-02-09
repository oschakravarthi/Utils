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

/// <summary>Enumeration of MSI run modes.</summary>
public enum MsiRunMode : int
{
    /// <summary>admin mode install, else product install</summary>
    Admin = 0,

    /// <summary>installing advertisements, else installing or updating product</summary>
    Advertise = 1,

    /// <summary>modifying an existing installation, else new installation</summary>
    Maintenance = 2,

    /// <summary>rollback is enabled</summary>
    RollbackEnabled = 3,

    /// <summary>log file active, enabled prior to install session</summary>
    LogEnabled = 4,

    /// <summary>spooling execute operations, else in determination phase</summary>
    Operations = 5,

    /// <summary>reboot needed after successful installation (settable)</summary>
    RebootAtEnd = 6,

    /// <summary>reboot needed to continue installation (settable)</summary>
    RebootNow = 7,

    /// <summary>installing files from cabinets and files using Media table</summary>
    Cabinet = 8,

    /// <summary>source LongFileNames suppressed via PID_MSISOURCE summary property</summary>
    SourceShortNames = 9,

    /// <summary>target LongFileNames suppressed via SHORTFILENAMES property</summary>
    TargetShortNames = 10,

    /// <summary>future use</summary>
    Reserved11 = 11,

    /// <summary>operating systems is Windows9?, else Windows NT</summary>
    Windows9x = 12,

    /// <summary>operating system supports demand installation</summary>
    ZawEnabled = 13,

    /// <summary>future use</summary>
    Reserved14 = 14,

    /// <summary>future use</summary>
    Reserved15 = 15,

    /// <summary>custom action call from install script execution</summary>
    Scheduled = 16,

    /// <summary>custom action call from rollback execution script</summary>
    Rollback = 17,

    /// <summary>custom action call from commit execution script</summary>
    Commit = 18,
}
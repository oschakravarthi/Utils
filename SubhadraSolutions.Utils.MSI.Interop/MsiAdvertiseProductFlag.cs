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
/// Internal class containing constants for the <c>script</c> parameter of <see cref="MsiInterop.MsiAdvertiseProduct"/> or <see cref="MsiInterop.MsiAdvertiseProductEx"/>.
/// This class cannot be inherited.
/// This class cannot be instantiated directly.
/// </summary>
internal sealed class MsiAdvertiseProductFlag
{
    #region	Constants (Static Fields)

    /// <summary>Set to advertise a per-machine installation of the product available to all users.</summary>
    public static readonly string MachineAssign = ((char)0).ToString();

    /// <summary>Set to advertise a per-user installation of the product available to a particular user.</summary>
    public static readonly string UserAssign = ((char)1).ToString();

    #endregion	Constants (Static Fields)

    #region	Construction / Destruction

    private MsiAdvertiseProductFlag()
    { }

    #endregion	Construction / Destruction
}
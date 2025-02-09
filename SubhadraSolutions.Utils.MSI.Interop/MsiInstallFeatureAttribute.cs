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

/// <summary>Bit flags install feature attributes enumeration.</summary>
[Flags]
public enum MsiInstallFeatureAttribute : int
{
    /// <summary>FavorLocal</summary>
    FavorLocal = 1 << 0,

    /// <summary>FavorSource</summary>
    FavorSource = 1 << 1,

    /// <summary>FollowParent</summary>
    FollowParent = 1 << 2,

    /// <summary>FavorAdvertise</summary>
    FavorAdvertise = 1 << 3,

    /// <summary>DisallowAdvertise</summary>
    DisallowAdvertise = 1 << 4,

    /// <summary>NounSupportedAdvertise</summary>
    NounSupportedAdvertise = 1 << 5,

    /// <summary>All attributes.</summary>
    All = FavorLocal | FavorSource | FollowParent | FavorAdvertise | DisallowAdvertise,
}
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

/// <summary>Enumeration of MSI feature cost tree options.</summary>
public enum MsiCostTree : int
{
    /// <summary>The feature only is included in the cost.</summary>
    SelfOnly = 0,

    /// <summary>The children of the indicated feature are included in the cost.</summary>
    Children = 1,

    /// <summary>The parent features of the indicated feature are included in the cost.</summary>
    Parents = 2,

    /// <summary>Reserved for future use.</summary>
    Reserved = 3,
}
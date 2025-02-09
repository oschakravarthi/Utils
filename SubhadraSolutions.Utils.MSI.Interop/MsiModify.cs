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

/// <summary>Enumeration of modification modes for <see cref="MsiInterop.MsiViewModify"/>.</summary>
public enum MsiModify : int
{
    /// <summary>reposition to current record primary key</summary>
    Seek = -1,

    /// <summary>refetch current record data</summary>
    Refresh = 0,

    /// <summary>insert new record, fails if matching key exists</summary>
    Insert = 1,

    /// <summary>update existing non-key data of fetched record</summary>
    Update = 2,

    /// <summary>insert record, replacing any existing record</summary>
    Assign = 3,

    /// <summary>update record, delete old if primary key edit</summary>
    Replace = 4,

    /// <summary>fails if record with duplicate key not identical</summary>
    Merge = 5,

    /// <summary>remove row referenced by this record from table</summary>
    Delete = 6,

    /// <summary>insert a temporary record</summary>
    InsertTemporary = 7,

    /// <summary>validate a fetched record</summary>
    Validate = 8,

    /// <summary>validate a new record</summary>
    ValidateNew = 9,

    /// <summary>validate field(s) of an incomplete record</summary>
    ValidateField = 10,

    /// <summary>validate before deleting record</summary>
    ValidateDelete = 11,
}
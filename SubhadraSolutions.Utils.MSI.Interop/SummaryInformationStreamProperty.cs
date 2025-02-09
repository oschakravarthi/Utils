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

/// <summary>Enumeration of MSI summary stream information property ids.</summary>
internal enum SummaryInformationStreamProperty : int
{
    /// <summary>Codepage</summary>
    Codepage = 1,

    /// <summary>Title</summary>
    Title = 2,

    /// <summary>Subject</summary>
    Subject = 3,

    /// <summary>Author</summary>
    Author = 4,

    /// <summary>Keywords</summary>
    Keywords = 5,

    /// <summary>Comments</summary>
    Comments = 6,

    /// <summary>Template</summary>
    Template = 7,

    /// <summary>LastSavedBy</summary>
    LastSavedBy = 8,

    /// <summary>RevisionNumber</summary>
    RevisionNumber = 9,

    /// <summary>LastPrinted</summary>
    LastPrinted = 11,

    /// <summary>CreateTime</summary>
    CreateTime = 12,

    /// <summary>LastSaveTime</summary>
    LastSaveTime = 13,

    /// <summary>PageCount</summary>
    PageCount = 14,

    /// <summary>WordCount</summary>
    WordCount = 15,

    /// <summary>CharacterCount</summary>
    CharacterCount = 16,

    /// <summary>CreatingApplication</summary>
    CreatingApplication = 18,

    /// <summary>Security</summary>
    Security = 19,
}
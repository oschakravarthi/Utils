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

/// <summary>Enumeration of MSI database errors.</summary>
public enum MsiDbError : int
{
    /// <summary>invalid argument</summary>
    InvalidArg = -3,

    /// <summary>buffer too small</summary>
    MoreData = -2,

    /// <summary>function error</summary>
    FunctionError = -1,

    /// <summary>no error</summary>
    NoError = 0,

    /// <summary>new record duplicates primary keys of existing record in table</summary>
    DuplicateKey = 1,

    /// <summary>non-nullable column, no null values allowed</summary>
    Required = 2,

    /// <summary>corresponding record in foreign table not found</summary>
    BadLink = 3,

    /// <summary>data greater than maximum value allowed</summary>
    Overflow = 4,

    /// <summary>data less than minimum value allowed</summary>
    Underflow = 5,

    /// <summary>data not a member of the values permitted in the set</summary>
    NotInSet = 6,

    /// <summary>invalid version string</summary>
    BadVersion = 7,

    /// <summary>invalid case, must be all upper-case or all lower-case</summary>
    BadCase = 8,

    /// <summary>invalid GUID</summary>
    BadGUID = 9,

    /// <summary>invalid wildcardfilename or use of wildcards</summary>
    BadWildcard = 10,

    /// <summary>bad identifier</summary>
    BadIdentifier = 11,

    /// <summary>bad language Id(s)</summary>
    BadLanguage = 12,

    /// <summary>bad filename</summary>
    BadFilename = 13,

    /// <summary>bad path</summary>
    BadPath = 14,

    /// <summary>bad conditional statement</summary>
    BadCondition = 15,

    /// <summary>bad format string</summary>
    BadFormatted = 16,

    /// <summary>bad template string</summary>
    BadTemplate = 17,

    /// <summary>bad string in DefaultDir column of Directory table</summary>
    BadDefaultDir = 18,

    /// <summary>bad registry path string</summary>
    BadRegPath = 19,

    /// <summary>bad string in CustomSource column of CustomAction table</summary>
    BadCustomSource = 20,

    /// <summary>bad property string</summary>
    BadProperty = 21,

    /// <summary>_Validation table missing reference to column</summary>
    MissingData = 22,

    /// <summary>Category column of _Validation table for column is invalid</summary>
    BadCategory = 23,

    /// <summary>table in KeyTable column of _Validation table could not be found/loaded</summary>
    BadKeyTable = 24,

    /// <summary>value in MaxValue column of _Validation table is less than value in MinValue column</summary>
    BadMaxMinValues = 25,

    /// <summary>bad cabinet name</summary>
    BadCabinet = 26,

    /// <summary>bad shortcut target</summary>
    BadShortcut = 27,

    /// <summary>string overflow (greater than length allowed in column def)</summary>
    StringOverflow = 28,

    /// <summary>invalid localization attribute (primary keys cannot be localized)</summary>
    BadLocalizedAttrib = 29,
}
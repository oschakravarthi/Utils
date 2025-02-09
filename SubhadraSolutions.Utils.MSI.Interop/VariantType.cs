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
/// Enumeration of variant types.
/// </summary>
public enum VariantType : uint
{
    Empty = 0,
    Null = 1,
    I2 = 2,
    I4 = 3,
    R4 = 4,
    R8 = 5,
    Currency = 6,
    Date = 7,
    BStr = 8,
    Error = 10,
    Bool = 11,
    Variant = 12,
    Decimal = 14,
    I1 = 16,
    UI1 = 17,
    UI2 = 18,
    UI4 = 19,
    I8 = 20,
    UI8 = 21,
    Int = 22,
    UInt = 23,
    LPStr = 30,
    LPWStr = 31,
    Filetime = 64,
    Blob = 65,
    Stream = 66,
    Storage = 67,
    StreamObject = 68,
    StoredObject = 69,
    BlobObject = 70,
    ClipFormat = 71,
    CLSID = 72,
    Vector = 0x1000,
    Array = 0x2000,
    ByRef = 0x4000,
    TypeMask = 0xfff,
}
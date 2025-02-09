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

/// <summary>Bitflags for MSI control attributes.</summary>
/// <remarks>Please refer to the MSDN Windows Installer documentation for more information.</remarks>
[Flags]
internal enum MsiControlAttribute : int
{
    /// <summary>If the Visible Control bit is set, the control is visible on the dialog box. If this bit is not set, the control is hidden on the dialog box. The visible or hidden state of the Visible control attribute can be later changed by a Control Event.</summary>
    Visible = 0x00000001,

    /// <summary>This attribute specifies if the given control is enabled or disabled. Most controls appear gray when disabled.</summary>
    Enabled = 0x00000002,

    /// <summary>If this bit is set, the control is displayed with a sunken, three dimensional look. The effect of this style bit is different on different controls and versions of Windows. On some controls it has no visible effect. If the system does not support the Sunken control attribute, the control is displayed in the default visual style. If this bit is not set, the control is displayed with the default visual style.</summary>
    Sunken = 0x00000004,

    /// <summary>The Indirect control attribute specifies whether the value displayed or changed by this control is referenced indirectly. If this bit is set, the control displays or changes the value of the property that has the identifier listed in the Property column of the Control table. If this bit is not set, the control displays or changes the value of the property in the Property column of the Control table.</summary>
    Indirect = 0x00000008,

    /// <summary>If this bit is set on a control, the associated property specified in the Property column of the Control table is an integer. If this bit is not set, the property is a string value.</summary>
    Integer = 0x00000010,

    /// <summary>If this style bit is set the text in the control is displayed in a right-to-left reading order.</summary>
    RTLRO = 0x00000020,

    /// <summary>If this style bit is set, text in the control is aligned to the right.</summary>
    RightAligned = 0x00000040,

    /// <summary>If this bit is set, the scroll bar is located on the left side of the control. If this bit is not set, the scroll bar is on the right side of the control.</summary>
    LeftScroll = 0x00000080,

    /// <summary>This is a combination of the right-to-left reading order <see cref="RTLRO"/>, the <see cref="RightAligned"/>, and <see cref="LeftScroll"/> attributes.</summary>
    BiDi = RTLRO | RightAligned | LeftScroll,

    //	text controls

    /// <summary>If the Transparent Control bit is set on a text control, the control is displayed transparently with the background showing through the control where there are no characters. If this bit is not set the text control is opaque.</summary>
    Transparent = 0x00010000,

    /// <summary>If this bit is set on a text control, the occurrence of the ampersand character in a text string is displayed as itself. If this bit is not set, then the character following the ampersand in the text string is displayed as an underscored character.</summary>
    NoPrefix = 0x00020000,

    /// <summary>If this bit is set the text in the control is displayed on a single line. If the text extends beyond the control's margins it is truncated and an ellipsis ("...") is inserted at the end to indicate the truncation. If this bit is not set, text wraps.</summary>
    NoWrap = 0x00040000,

    /// <summary>If this bit is set for a static text control, the control will automatically attempt to format the displayed text as a number representing a count of bytes. For proper formatting, the control's text must be set to a string representing a number expressed in units of 512 bytes. The displayed value will then be formatted in terms of kilobytes (KB), megabytes (MB), or gigabytes (GB), and displayed with the appropriate string representing the units.  Kb = Less than 20480.  Mb = Less than 20971520.  Gb = Less than 10737418240</summary>
    FormatSize = 0x00080000,

    /// <summary>If this bit flag is set, fonts are created using the user's default UI code page. If the bit flag is not set, fonts are created using the database code page.</summary>
    UsersLanguage = 0x00100000,

    //	edit controls

    /// <summary>If this bit is set on an Edit control, the installer creates a multiple line edit control with a vertical scroll bar.</summary>
    MultiLine = 0x00001000,

    /// <summary>PasswordInput</summary>
    PasswordInput = 0x00200000,

    //	progress bar

    /// <summary>If this bit is set on a ProgressBar control, the bar is drawn as a series of small rectangles in Microsoft Windows 95-style. If this bit is not set, the progress indicator bar is drawn as a single continuous rectangle.</summary>
    Progress95 = 0x00001000,

    //	volume select combo and directory combo

    /// <summary>If this bit is set, the control shows all the volumes involved in the current installation plus all the removable volumes. If this bit is not set, the control lists volumes in the current installation.</summary>
    RemovableVolume = 0x00010000,

    /// <summary>If the FixedVolume Control bit is set, the control shows all the volumes involved in the current installation plus all the fixed internal hard drives. If this bit is not set, the control lists the volumes in the current installation.</summary>
    FixedVolume = 0x00020000,

    /// <summary>If this bit is set, the control shows all the volumes involved in the current installation plus all the remote volumes. If this bit is not set, the control lists volumes in the current installation.</summary>
    RemoteVolume = 0x00040000,

    /// <summary>If the CDROMVolume Control bit is set, the control shows all the volumes in the current installation plus all the CD-ROM volumes. If this bit is not set, the control shows all the volumes in the current installation.</summary>
    CDRomVolume = 0x00080000,

    /// <summary>If this bit is set, the control shows all the volumes involved in the current installation plus all the RAM disk volumes. If this bit is not set the control lists volumes in the current installation.</summary>
    RAMDiskVolume = 0x00100000,

    /// <summary>If the FloppyVolume Control bit is set, the control shows all the volumes involved in the current installation plus all the floppy volumes. If this bit is not set, the control lists volumes in the current installation. </summary>
    FloppyVolume = 0x00200000,

    //	volume list controls

    /// <summary>ShowRollbackCost</summary>
    ShowRollbackCost = 0x00400000,

    //	list box / combo box
    /// <summary>If this bit is set, the items listed in the control are displayed in a specified order. If the bit is not set, items are displayed in alphabetical order.</summary>
    Sorted = 0x00010000,

    /// <summary>If the ComboList Control bit is set on a combo box, the edit field is replaced by a static text field. This prevents a user from entering a new value and requires the user to choose only one of the predefined values. If this bit is not set, the combo box has an edit field.</summary>
    ComboList = 0x00020000,

    //	picture buttons

    /// <summary>ImageHandle</summary>
    ImageHandle = 0x00010000,

    /// <summary>If this bit is set on a check box or a radio button group, the button is drawn with the appearance of a push button, but its logic stays the same. If the bit is not set, the controls are drawn in their usual style.</summary>
    PushLike = 0x00020000,

    /// <summary>If the Bitmap Control bit is set, the text in the control is replaced by a bitmap image. The Text column in the Control table is a foreign key into the Binary table. If this bit is not set, the text in the control is specified in the Text column of the Control table.</summary>
    Bitmap = 0x00040000,

    /// <summary>If this bit is set, text is replaced by an icon image and the Text column in the Control table is a foreign key into the Binary table. If this bit is not set, text in the control is specified in the Text column of the Control table.</summary>
    Icon = 0x00080000,

    /// <summary>If the FixedSize Control bit is set, the picture is cropped or centered in the control without changing its shape or size. If this bit is not set the picture is stretched to fit the control.</summary>
    FixedSize = 0x00100000,

    /// <summary>The first 16x16 image is loaded.</summary>
    IconSize16 = 0x00200000,

    /// <summary>The first 32x32 image is loaded.</summary>
    IconSize32 = 0x00400000,

    /// <summary>The first 48x48 image is loaded.</summary>
    IconSize48 = 0x00600000,

    //	radio buttons
    /// <summary>If this bit is set, the RadioButtonGroup has text and a border displayed around it. If the style bit is not set, the border is not displayed and no text is displayed on the group.</summary>
    HasBorder = 0x01000000,
}
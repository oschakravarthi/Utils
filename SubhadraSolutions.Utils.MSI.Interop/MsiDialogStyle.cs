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

/// <summary>Bitflags for MSI dialogs.</summary>
/// <remarks>Please refer to the MSDN Windows Installer documentation for more information.</remarks>
[Flags]
internal enum MsiDialogStyle : int
{
    /// <summary>If this bit is set the dialog is originally created as visible, otherwise it is hidden.</summary>
    Visible = 1,

    /// <summary>If this bit is set, the dialog box is modal, other dialogs of the same application cannot be put on top of it, and the dialog keeps the control while it is running. If this bit is not set, the dialog is modeless, other dialogs of the same application may be moved on top of it. After a modeless dialog is created and displayed, the user interface returns control to the installer. The installer then calls the user interface periodically to update the dialog and to give it a chance to process the messages. As soon as this is done, the control is returned to the installer.  <b>Note</b>  There should be no modeless dialogs in a wizard sequence, since this would return control to the installer, ending the wizard sequence prematurely.</summary>
    Modal = 2,

    /// <summary>If this bit is set, the dialog box can be minimized. This bit is ignored for modal dialog boxes, which cannot be minimized.</summary>
    Minimize = 4,

    /// <summary>If this style bit is set, the dialog box will stop all other applications and no other applications can take the focus. This state remains until the SysModal dialog is dismissed.</summary>
    SysModal = 8,

    /// <summary>Normally, when this bit is not set and a dialog box is created through DoAction, all other (typically modeless) dialogs are destroyed. If this bit is set, the other dialogs stay alive when this dialog box is created.</summary>
    KeepModeless = 16,

    /// <summary>If this bit is set, the dialog box periodically calls the installer. If the property changes, it notifies the controls on the dialog. This style can be used if there is a control on the dialog indicating disk space. If the user switches to another application, adds or removes files, or otherwise modifies available disk space, you can quickly implement the change using this style. Any dialog box relying on the OutOfDiskSpace property to determine whether to bring up a dialog must set the TrackDiskSpace Dialog Style Bit for the dialog to dynamically update space on the target volumes.</summary>
    TrackDiskSpace = 32,

    /// <summary>If this bit is set, the pictures on the dialog box are created with the custom palette (one per dialog received from the first control created). If the bit is not set, the pictures are rendered using a default palette.</summary>
    UseCustomPalette = 64,

    /// <summary>If this style bit is set the text in the dialog box is displayed in right-to-left-reading order.</summary>
    RTLRO = 128,

    /// <summary>If this style bit is set, the text is aligned on the right side of the dialog box.</summary>
    RightAligned = 256,

    /// <summary>If this style bit is set, the scroll bar is located on the left side of the dialog box.</summary>
    LeftScroll = 512,

    /// <summary>This is a combination of the right to left reading order <see cref="RTLRO"/>, the <see cref="RightAligned"/>, and the <see cref="LeftScroll"/> dialog style bits.</summary>
    BiDi = 896,

    /// <summary>If this bit is set, the dialog box is an error dialog.</summary>
    Error = 65536,
}
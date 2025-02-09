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

using SubhadraSolutions.Utils.Runtime.InteropServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SubhadraSolutions.Utils.MSI.Interop;

/// <summary>The <c>INSTALLUI_HANDLER</c> delegate defines a callback function that the installer calls for progress notification and error messages.</summary>
/// <remarks>
/// The <c>messageType</c> parameter specifies a combination of one message box style, one message box icon type, one default button, and one installation message type.
/// </remarks>
public delegate int MsiInstallUIHandler(int context,
    uint messageType, [MarshalAs(UnmanagedType.LPTStr)] string message);

/// <summary>
/// Internal class supporting direct MSI API.
/// This class cannot be inherited.
/// This class cannot be instantiated directly.
/// </summary>
/// <remarks>
/// <para>Supports the Windows Installer API 2.0.</para>
/// <para><b>Note</b>:  The following are omitted from this version:
/// Hashing and digital signature functions.</para>
/// <para>Please refer to the MSDN documention on the Windows Installer
/// for more information about the Windows Installer API.</para>
/// </remarks>
public sealed class MsiInterop
{
    static MsiInterop()
    {
        InteropServicesHelper.EnsureWindowsPlatform();
    }

    #region	Constants

    /// <summary>Maximum chars in feature name (same as string GUID)</summary>
    public const int MaxFeatureChars = 38;

    /// <summary>
    /// Type mask to extract the <see cref="MsiInstallMessage"/> in
    /// <see cref="MsiInstallUIHandler"/>s.
    /// </summary>
    public const uint MessageTypeMask = 0xff000000;

    /// <summary>Represents a "null" integer.</summary>
    public const int MsiNullInteger = int.MinValue;

    private const string MSI_LIB = "msi";
    //	0x80000000
    #endregion	Constants

    #region	Construction / Destruction

    private MsiInterop()
    { }

    #endregion	Construction / Destruction

    #region	Interop Methods
    #region	Installer Functions

    /// <summary>The <c>MsiAdvertiseProduct</c> function generates an advertise script or advertises a product to the computer. The <c>MsiAdvertiseProduct</c> function enables the installer to write to a script the registry and shortcut information used to assign or publish a product. The script can be written to be consistent with a specified platform by using <see cref="MsiAdvertiseProductEx"/>.</summary>
    /// <param name="path">The full path to the package of the product being advertised.</param>
    /// <param name="script">The full path to script file that will be created with the advertise information. To advertise the product locally to the computer, set <see cref="MsiAdvertiseProductFlag.MachineAssign"/> or <see cref="MsiAdvertiseProductFlag.UserAssign"/>.</param>
    /// <param name="transforms">A semicolon-delimited list of transforms to be applied. The list of transforms can be prefixed with the @ or | character to specify the secure caching of transforms. The @ prefix specifies secure-at-source transforms and the | prefix indicates secure full path transforms.  This parameter may be <c>null</c>.</param>
    /// <param name="langId">The installation language to use if the source supports multiple languages.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.CallNotImplemented"/>  This error is returned if an attempt is made to generate an advertise script on any platform other than Windows 2000 or Windows XP. Advertisement to the local computer is supported on all platforms.</para>
    /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiAdvertiseProduct(string path,
        string script, string transforms, ushort langId);

    /// <summary>The <c>MsiAdvertiseProductEx</c> function generates an advertise script or advertises a product to the computer. The <c>MsiAdvertiseProductEx</c> function enables the installer to write to a script the registry and shortcut information used to assign or publish a product. Provides the same functionality as <see cref="MsiAdvertiseProduct"/>. The script can be written to be consistent with a specified platform by using <c>MsiAdvertiseProductEx</c>.</summary>
    /// <param name="path">The full path to the package of the product being advertised.</param>
    /// <param name="script">The full path to script file that will be created with the advertise information. To advertise the product locally to the computer, set <see cref="MsiAdvertiseProductFlag.MachineAssign"/> or <see cref="MsiAdvertiseProductFlag.UserAssign"/>.</param>
    /// <param name="transforms">A semicolon-delimited list of transforms to be applied. The list of transforms can be prefixed with the @ or | character to specify the secure caching of transforms. The @ prefix specifies secure-at-source transforms and the | prefix indicates secure full path transforms.  This parameter may be <c>null</c>.</param>
    /// <param name="langId">The installation language to use if the source supports multiple languages.</param>
    /// <param name="platform">Bit flags that control for which platform the installer should create the script. This parameter is ignored if <c>script</c> is <c>null</c>. If <c>platform</c> is <see cref="PlatformArchitecture.Current"/>, then the script is created based on the current platform. This is the same functionality as <see cref="MsiAdvertiseProduct"/>. If <c>platform</c> is <see cref="PlatformArchitecture.X86"/> or <see cref="PlatformArchitecture.IA64"/>, the installer creates script for the specified platform.</param>
    /// <param name="options">Bit flags that specify extra advertisement options. Nonzero value is only available in Windows Installer versions shipped with Windows Server 2003 family and later and Windows XP Service Pack 1 and later.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.CallNotImplemented"/>  This error is returned if an attempt is made to generate an advertise script on any platform other than Windows 2000 or Windows XP. Advertisement to the local computer is supported on all platforms.</para>
    /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiAdvertiseProductEx(string path,
        string script, string transforms, ushort langId,
        PlatformArchitecture platform, MsiAdvertismentOptions options);

    /// <summary>For each product listed by the patch package as eligible to receive the patch, the MsiApplyPatch function invokes an installation and sets the PATCH property to the path of the patch package.</summary>
    /// <param name="patchPackage">A null-terminated string specifying the full path to the patch package. </param>
    /// <param name="installPackage">
    /// <para>If <c>installtype</c> is set to <see cref="MsiInstallType.NetworkImage"/>, this parameter is a null-terminated string that specifies a path to the product that is to be patched. The installer applies the patch to every eligible product listed in the patch package if <c>installPackage</c> is set to <c>null</c> and <c>installType</c> is set to <see cref="MsiInstallType.Default"/>.</para>
    /// <para>If <c>installtype</c> is <see cref="MsiInstallType.SingleInstance"/>, the installer applies the patch to the product specified by <c>installPackage</c>. In this case, other eligible products listed in the patch package are ignored and the <c>installPackage</c> parameter contains the null-terminated string representing the product code of the instance to patch. This type of installation requires the installer running Windows .NET Server 2003 family or Windows XP SP1.</para>
    /// </param>
    /// <param name="installType">
    /// <para>This parameter specifies the type of installation to patch.</para>
    /// <para><see cref="MsiInstallType.NetworkImage"/>  Specifies an administrative installation. In this case, <c>installPackage</c> must be set to a package path. A value of 1 for <see cref="MsiInstallType.NetworkImage"/> sets this for an administrative installation.</para>
    /// <para><see cref="MsiInstallType.Default"/>  Searches system for products to patch. In this case, szInstallPackage must be <c>null</c>.</para>
    /// <para><see cref="MsiInstallType.SingleInstance"/>  Patch the product specified by szInstallPackage. <c>installPackage</c> is the product code of the instance to patch. This type of installation requires the installer running Windows .NET Server 2003 family or Windows XP SP1.</para>
    /// </param>
    /// <param name="commandLine">A null-terminated string that specifies command line property settings.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.PatchPackageOpenFailed"/>  Patch package could not be opened.</para>
    /// <para><see cref="MsiError.PatchPackageInvalid"/>  The patch package is invalid.</para>
    /// <para><see cref="MsiError.PatchPackageUnsupported"/>  The patch package is not unsupported.</para>
    /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiApplyPatch(string patchPackage,
        string installPackage, MsiInstallType installType, string commandLine);

    /// <summary>The <c>MsiCloseAllHandles</c> function closes all open installation handles allocated by the current thread. This is a diagnostic function and should not be used for cleanup.</summary>
    /// <returns>This function returns 0 if all handles are closed. Otherwise, the function returns the number of handles open prior to its call.</returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB)]
    public static extern uint MsiCloseAllHandles();

    /// <summary>The <c>MsiCloseHandle</c> function closes an open installation handle.</summary>
    /// <param name="handle">Specifies any open installation handle.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid handle was passed to the function.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB)]
    public static extern MsiError MsiCloseHandle(int handle);

    /// <summary>The <c>MsiCollectUserInfo</c> function obtains and stores the user information and product ID from an installation wizard.</summary>
    /// <param name="product">Specifies the product code of the product for which the user information is collected.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
    /// </returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiCollectUserInfo(string product);

    /// <summary>The <c>MsiConfigureFeature</c> function configures the installed state for a product feature.</summary>
    /// <param name="product">Specifies the product code for the product to be configured.</param>
    /// <param name="feature">Specifies the feature ID for the feature to be configured.</param>
    /// <param name="installState">Specifies the installation state (<see cref="MsiInstallState"/>) for the feature.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
    /// </returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiConfigureFeature(string product,
        string feature, MsiInstallState installState);

    /// <summary>The <c>MsiConfigureProduct</c> function installs or uninstalls a product.</summary>
    /// <param name="product">Specifies the product code for the product to be configured.</param>
    /// <param name="level">Specifies how much of the product should be installed when installing the product to its default state. The <c>level</c> parameter will be ignored, and all features will be installed, if the <c>installState</c> parameter is set to any other value than <see cref="MsiInstallState.Default"/>.</param>
    /// <param name="installState">Specifies the installation state for the product.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
    /// </returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiConfigureProduct(string product,
        MsiInstallLevel level, MsiInstallState installState);

    /// <summary>The <c>MsiConfigureProductEx</c> function installs or uninstalls a product. A product command line may also be specified.</summary>
    /// <param name="product">Specifies the product code for the product to be configured.</param>
    /// <param name="level">Specifies how much of the product should be installed when installing the product to its default state. The <c>level</c> parameter will be ignored, and all features will be installed, if the <c>installState</c> parameter is set to any other value than <see cref="MsiInstallState.Default"/>.</param>
    /// <param name="installState">Specifies the installation state for the product.</param>
    /// <param name="commandLine">Specifies the command line property settings. This should be a list of the format <i>Property=Setting Property=Setting</i>.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
    /// </returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiConfigureProductEx(string product,
        MsiInstallLevel level, MsiInstallState installState, string commandLine);

    /// <summary>The <c>MsiCreateRecord</c> function creates a new record object with the specified number of fields. This function returns a handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
    /// <param name="count">Specifies the number of fields the record will have. The maximum number of fields in a record is limited to 65535.</param>
    /// <returns>
    /// <para>If the function succeeds, the return value is handle to a new record object.</para>
    /// <para>If the function fails, the return value is <c>IntPtr.Zero</c>.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB)]
    public static extern int MsiCreateRecord(uint count);

    /// <summary>The <c>MsiCreateTransformSummaryInfo</c> function creates summary information of an existing transform to include validation and error conditions. Execution of this function sets the error record, accessible through <see cref="MsiGetLastErrorRecord"/>.</summary>
    /// <param name="database">Handle to the database that contains the new database Summary Information.</param>
    /// <param name="databaseRef">Handle to the database that contains the original Summary Information.</param>
    /// <param name="transformFile">The name of the transform to which the Summary Information is added.</param>
    /// <param name="errorConditions">The error conditions that should be suppressed when the transform is applied.</param>
    /// <param name="validation">Specifies those properties to be validated to verify the transform can be applied to the database.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InstallPackageInvalid"/>  Reference to an invalid Windows Installer package.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.OpenFailed"/>  The transform storage file could not be opened.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiCreateTransformSummaryInfo(int database,
        int databaseRef, string transformFile, MsiTransformError errorConditions,
        MsiValidationFlag validation);

    /// <summary>The <c>MsiDatabaseApplyTransform</c> function applies a transform to a database.</summary>
    /// <param name="database">Handle to the database obtained from <see cref="MsiOpenDatabase"/> to the transform.</param>
    /// <param name="transformFile">Specifies the name of the transform file to apply.</param>
    /// <param name="errorConditions">Error conditions that should be suppressed.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InstallTransformFailure"/>  Reference to an invalid Windows Installer package.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.OpenFailed"/>  The transform storage file could not be opened.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiDatabaseApplyTransform(int database,
        string transformFile, MsiTransformError errorConditions);

    /// <summary>The <c>MsiDatabaseCommit</c> function commits changes to a database.</summary>
    /// <param name="database">Handle to the database obtained from <see cref="MsiOpenDatabase"/>.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiDatabaseCommit(int database);

    /// <summary>The <c>MsiDatabaseExport</c> function exports an installer table from an open database to a text archive file.</summary>
    /// <param name="database">Handle to the database obtained from <see cref="MsiOpenDatabase"/>.</param>
    /// <param name="table">Specifies the name of the table to export.</param>
    /// <param name="folder">Specifies the name of the folder that contains archive files.</param>
    /// <param name="fileName">Specifies the name of the exported table archive file.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadPathName"/>  An invalid path was passed to the function.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiDatabaseExport(int database,
        string table, string folder, string fileName);

    /// <summary>The <c>MsiDatabaseGenerateTransform</c> function generates a transform file of differences between two databases. A transform is a way of recording changes to a database without altering the original database. You can also use <c>MsiDatabaseGenerateTransform</c> to test whether two databases are identical without creating a transform.</summary>
    /// <param name="database">Handle to the database obtained from <see cref="MsiOpenDatabase"/> that includes the changes.</param>
    /// <param name="databaseRef">Handle to the database obtained from <see cref="MsiOpenDatabase"/> that does not include the changes.</param>
    /// <param name="transformFile">A null-terimated string specifying the name of the transform file being generated. This parameter can be <c>null</c>. If <c>transformFile</c> is <c>null</c>, you can use <c>MsiDatabaseGenerateTransform</c> to test whether two databases are identical without creating a transform. If the databases are identical, the function returns <see cref="MsiError.NoData"/>. If the databases are different the function returns <see cref="MsiError.NoError"/>.</param>
    /// <param name="reserved1">This is a reserved argument and must be set to 0.</param>
    /// <param name="reserved2">This is a reserved argument and must be set to 0.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.CreateFailed"/>  The storage for the transform file could not be created.</para>
    /// <para><see cref="MsiError.NoData"/>  If <c>transformFile</c> is <c>null</c>, this value is returned if the two databases are identical. No transform file is generated.</para>
    /// <para><see cref="MsiError.NoError"/>  If <c>transformFile</c> is <c>null</c>, this is returned if the two databases are different.</para>
    /// <para><see cref="MsiError.InstallTransformFailure"/>  The transform could not be generated.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiDatabaseGenerateTransform(int database,
        int databaseRef, string transformFile, int reserved1, int reserved2);

    /// <summary>The <c>MsiDatabaseGetPrimaryKeys</c> function returns a record containing the names of all the primary key columns for a specified table. This function returns a handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
    /// <param name="database">Handle to the database.</param>
    /// <param name="table">Specifies the name of the table from which to obtain primary key names.</param>
    /// <param name="record">Pointer to the handle of the record that holds the primary key names.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.InvalidTable"/>  An invalid table was passed to the function.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiDatabaseGetPrimaryKeys(int database,
        string table, out int record);

    /// <summary>The <c>MsiDatabaseImport</c> function imports an installer text archive table into an open database.</summary>
    /// <param name="database">Handle to the database obtained from <see cref="MsiOpenDatabase"/>.</param>
    /// <param name="folder">Specifies the path to the folder containing archive files.</param>
    /// <param name="fileName">Specifies the name of the file to import.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadPathName"/>  An invalid path was passed to the function.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiDatabaseImport(int database,
        string folder, string fileName);

    /// <summary>The <c>MsiDatabaseIsTablePersistent</c> function returns an enumeration describing the state of a particular table.</summary>
    /// <param name="database">Handle to the database to which the relevant table belongs.</param>
    /// <param name="table">Specifies the name of the relevant table.</param>
    /// <returns>The <see cref="MsiCondition"/> of the table.</returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiCondition MsiDatabaseIsTablePersistent(int database,
        string table);

    /// <summary>The <c>MsiDatabaseMerge</c> function merges two databases together, allowing duplicate rows.</summary>
    /// <param name="database">Handle to the database obtained from <see cref="MsiOpenDatabase"/>.</param>
    /// <param name="merge">Handle to the database obtained from <see cref="MsiOpenDatabase"/> to merge into the base database.</param>
    /// <param name="table">Specifies the name of the table to receive merge conflict information.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.DatatypeMismatch"/>  Schema difference between the two databases.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidTable"/>  An invalid table was passed to the function.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiDatabaseMerge(int database,
        int merge, string table);

    /// <summary>The <c>MsiDatabaseOpenView</c> function prepares a database query and creates a view object. This function returns a handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
    /// <param name="database">Handle to the database to which you want to open a view object.</param>
    /// <param name="query">Specifies a SQL query string for querying the database.</param>
    /// <param name="view">Pointer to a handle for the returned view.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadQuerySyntax"/>  An invalid SQL query string was passed to the function.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiDatabaseOpenView(int database,
        string query, out int view);

    /// <summary>The MsiDoAction function executes a built-in action, custom action, or user-interface wizard action.</summary>
    /// <param name="database">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>. </param>
    /// <param name="action">Specifies the action to execute.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.FunctionNotCalled"/>  The action was not found.</para>
    /// <para><see cref="MsiError.InstallFailure"/>  The action failed.</para>
    /// <para><see cref="MsiError.InstallSuspend"/>  The user suspended the installation.</para>
    /// <para><see cref="MsiError.InstallUserExit"/>  The user canceled the action.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.MoreData"/>  The action indicates that the remaining actions should be skipped.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiDoAction(int database, string action);

    /// <summary>The <c>MsiEnableLog</c> function sets the log mode for all subsequent installations that are initiated in the calling process.</summary>
    /// <param name="logMode">Specifies the log mode.  Can be a combination of <see cref="MsiInstallLogMode"/> flags.</param>
    /// <param name="logFile">Specifies the string that holds the full path to the log file. Entering a <c>null</c> disables logging, in which case <c>logMode</c> is ignored. If a path is supplied, then <c>logMode</c> must not be zero</param>
    /// <param name="logAttributes">Specifies how frequently the log buffer is to be flushed.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiEnableLog(MsiInstallLogMode logMode,
        string logFile, MsiLogAttribute logAttributes);

    /// <summary>The <c>MsiEnableUIPreview</c> function enables preview mode of the user interface to facilitate authoring of user-interface dialog boxes. This function returns a handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
    /// <param name="database">Handle to the database.</param>
    /// <param name="preview">Pointer to a returned handle for user-interface preview capability.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.OutOfMemory"/>  Out of memory.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiEnableUIPreview(int database,
        out int preview);

    /// <summary>The <c>MsiEnumClients</c> function enumerates the clients for a given installed component. The function retrieves one product code each time it is called.</summary>
    /// <param name="component">Specifies the component whose clients are to be enumerated.</param>
    /// <param name="index">Specifies the index of the client to retrieve. This parameter should be zero for the first call to the <c>MsiEnumClients</c> function and then incremented for subsequent calls. Because clients are not ordered, any new client has an arbitrary index. This means that the function can return clients in any order.</param>
    /// <param name="product">Pointer to a buffer that receives the product code. This buffer must be 39 characters long. The first 38 characters are for the GUID, and the last character is for the terminating null character.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.NoMoreItems"/>  There are no clients to return.</para>
    /// <para><see cref="MsiError.NotEnoughMemory"/>  The system does not have enough memory to complete the operation. Available with Windows Server 2003 family.</para>
    /// <para><see cref="MsiError.UnknownComponent"/>  The specified component is unknown.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiEnumClients(string component,
        uint index, string product);

    /// <summary>The <c>MsiEnumComponentCosts</c> function enumerates the disk-space per drive required to install a component. This information is needed to display the disk-space cost required for all drives in the user interface. The returned disk-space costs are expressed in multiples of 512 bytes.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="component">A null-terminated string specifying the component's name as it is listed in the Component column of the Component table. This parameter can be <c>null</c>. If <c>component</c> is <c>null</c> or an empty string, <c>MsiEnumComponentCosts</c> enumerates the total disk-space per drive used during the installation. In this case, <c>state</c> is ignored. The costs of the installer include those costs for caching the database in the secure folder as well as the cost to create the installation script. Note that the total disk-space used during the installation may be larger than the space used after the component is installed.</param>
    /// <param name="index">0-based index for drives. This parameter should be zero for the first call to the <c>MsiEnumComponentCosts</c> function and then incremented for subsequent calls.</param>
    /// <param name="state">Requested component state to be enumerated. If <c>component</c> is passed as <c>null</c> or an empty string, the installer ignores the <c>state</c> parameter.</param>
    /// <param name="drive">Buffer that holds the drive name including the null terminator. This is an empty string in case of an error.</param>
    /// <param name="driveSize">Pointer to a variable that specifies the size, in TCHARs, of the buffer pointed to by the <c>drive</c> parameter. This size should include the terminating null character. If the buffer provided is too small, the variable pointed to by <c>driveSize</c> contains the count of characters not including the null terminator.</param>
    /// <param name="cost">Cost of the component per drive expressed in multiples of 512 bytes. This value is 0 if an error has occurred. The value returned in <c>cost</c> is final disk-space used by the component after installation. If <c>component</c> is passed as <c>null</c> or an empty string, the installer sets the value at <c>cost</c> to 0.</param>
    /// <param name="tempCost">The component cost per drive for the duration of the installation, or 0 if an error occurred. The value in <c>tempCost</c> represents the temporary space requirements for the duration of the installation. This temporary space requirement is space needed only for the duration of the installation. This does not affect the final disk space requirement.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionNotCalled"/>  Costing is not complete.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.MoreData"/>  Buffer not large enough for the drive name.</para>
    /// <para><see cref="MsiError.NoMoreItems"/>  There are no more drives to return.</para>
    /// <para><see cref="MsiError.UnknownComponent"/>  The component is missing.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiEnumComponentCosts(int install,
        string component, uint index, MsiInstallState state, string drive,
        ref uint driveSize, out int cost, out int tempCost);

    /// <summary>The <c>MsiEnumComponentQualifiers</c> function enumerates the advertised qualifiers for the given component. This function retrieves one qualifier each time it is called.</summary>
    /// <param name="component">Specifies component whose qualifiers are to be enumerated.</param>
    /// <param name="index">Specifies the index of the qualifier to retrieve. This parameter should be zero for the first call to the <c>MsiEnumComponentQualifiers</c> function and then incremented for subsequent calls. Because qualifiers are not ordered, any new qualifier has an arbitrary index. This means that the function can return qualifiers in any order.</param>
    /// <param name="qualifier">Pointer to a buffer that receives the qualifier code.</param>
    /// <param name="qualifierSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>qualifier</c> parameter. On input, this size should include the terminating <c>null</c> character. On return, the value does not include the <c>null</c> character.</param>
    /// <param name="appData">Pointer to a buffer that receives the application registered data for the qualifier. This parameter can be <c>null</c>.</param>
    /// <param name="appDataSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>appData</c> parameter. On input, this size should include the terminating <c>null</c> character. On return, the value does not include the <c>null</c> character. This parameter can be <c>null</c> only if the <c>appData</c> parameter is <c>null</c>.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.MoreData"/>  A buffer is to small to hold the requested data.</para>
    /// <para><see cref="MsiError.NoMoreItems"/>  There are no clients to return.</para>
    /// <para><see cref="MsiError.NotEnoughMemory"/>  The system does not have enough memory to complete the operation. Available with Windows Server 2003 family.</para>
    /// <para><see cref="MsiError.UnknownComponent"/>  The specified component is unknown.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiEnumComponentQualifiers(string component,
        uint index, StringBuilder qualifier, ref uint qualifierSize,
        StringBuilder appData, ref int appDataSize);

    /// <summary>The <c>MsiEnumComponents</c> function enumerates the installed components for all products. This function retrieves one component code each time it is called.</summary>
    /// <param name="index">Specifies the index of the component to retrieve. This parameter should be zero for the first call to the <c>MsiEnumComponents</c> function and then incremented for subsequent calls. Because components are not ordered, any new component has an arbitrary index. This means that the function can return components in any order.</param>
    /// <param name="component">Pointer to a buffer that receives the component code. This buffer must be 39 characters long. The first 38 characters are for the GUID, and the last character is for the terminating <c>null</c> character.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.NoMoreItems"/>  There are no clients to return.</para>
    /// <para><see cref="MsiError.NotEnoughMemory"/>  The system does not have enough memory to complete the operation. Available with Windows Server 2003 family.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiEnumComponents(uint index, string component);

    /// <summary>The <c>MsiEnumFeatures</c> function enumerates the published features for a given product. This function retrieves one feature ID each time it is called.</summary>
    /// <param name="product">Null-terminated string specifying the product code of the product whose features are to be enumerated.</param>
    /// <param name="index">Specifies the index of the feature to retrieve. This parameter should be zero for the first call to the <c>MsiEnumFeatures</c> function and then incremented for subsequent calls. Because features are not ordered, any new feature has an arbitrary index. This means that the function can return features in any order.</param>
    /// <param name="feature">Pointer to a buffer that receives the feature ID. This parameter must be sized to hold a string of length <c><see cref="MaxFeatureChars"/> + 1</c>.</param>
    /// <param name="featureParent">Pointer to a buffer that receives the feature ID of the parent of the feature. This parameter must be sized to hold a string of length <c><see cref="MaxFeatureChars"/> + 1</c>. </param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.NoMoreItems"/>  There are no clients to return.</para>
    /// <para><see cref="MsiError.UnknownProduct"/>  The specified product is unknown.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiEnumFeatures(string product, uint index,
        string feature, string featureParent);

    /// <summary>The <c>MsiEnumPatches</c> function enumerates all of the patches that have been applied to a product. The function returns the patch code GUID for each patch that has been applied to the product and returns a list of transforms from each patch that apply to the product. Note that patches may have many transforms only some of which are applicable to a particular product. The list of transforms are returned in the same format as the value of the <c>TRANSFORMS</c> property.</summary>
    /// <param name="product">Specifies the product for which patches are to be enumerated.</param>
    /// <param name="index">Specifies the index of the patch to retrieve. This parameter should be zero for the first call to the <c>MsiEnumPatches</c> function and then incremented for subsequent calls. </param>
    /// <param name="patch">Pointer to a buffer that receives the patch's GUID. This argument is required.</param>
    /// <param name="transform">Pointer to a buffer that receives the list of transforms in the patch that are applicable to the product. This argument is required and cannot be <c>null</c>.</param>
    /// <param name="transformSize">Set to the number of characters copied to <c>transform</c> upon an unsuccessful return of the function. Not set for a successful return. On input, this is the full size of the buffer, including a space for a terminating <c>null</c> character. If the buffer passed in is too small, the count returned does not include the terminating <c>null</c> character.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.MoreData"/>  A buffer is too small to hold the requested data.</para>
    /// <para><see cref="MsiError.NoMoreItems"/>  There are no clients to return.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiEnumPatches(string product, uint index,
        string patch, StringBuilder transform, ref uint transformSize);

    /// <summary>The <c>MsiEnumProducts</c> function enumerates through all the products currently advertised or installed. Both per-user and per-machine installations and advertisements are enumerated.</summary>
    /// <param name="index">Specifies the index of the product to retrieve. This parameter should be zero for the first call to the <c>MsiEnumProducts</c> function and then incremented for subsequent calls. Because products are not ordered, any new product has an arbitrary index. This means that the function can return products in any order.</param>
    /// <param name="product">Pointer to a buffer that receives the product code. This buffer must be 39 characters long. The first 38 characters are for the GUID, and the last character is for the terminating null character.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.NoMoreItems"/>  There are no clients to return.</para>
    /// <para><see cref="MsiError.NotEnoughMemory"/>  The system does not have enough memory to complete the operation. Available with Windows Server 2003 family.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiEnumProducts(uint index, string product);

    /// <summary>The <c>MsiEnumRelatedProducts</c> function enumerates products with a specified upgrade code. This function lists the currently installed and advertised products that have the specified UpgradeCode property in their Property table.</summary>
    /// <param name="upgradeCode">The null-terminated string specifying the upgrade code of related products that the installer is to enumerate.</param>
    /// <param name="reserved">This parameter is reserved and must be 0.</param>
    /// <param name="index">The zero-based index into the registered products.</param>
    /// <param name="product">A buffer to receive the product code GUID. This buffer must be 39 characters long. The first 38 characters are for the GUID, and the last character is for the terminating <c>null</c> character.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.NoMoreItems"/>  There are no clients to return.</para>
    /// <para><see cref="MsiError.NotEnoughMemory"/>  The system does not have enough memory to complete the operation. Available with Windows Server 2003 family.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiEnumRelatedProducts(string upgradeCode,
        uint reserved, uint index, string product);

    /// <summary>The <c>MsiEvaluateCondition</c> function evaluates a conditional expression containing property names and values.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="condition">Specifies the conditional expression. This parameter must not be <c>null</c>.</param>
    /// <returns>The <see cref="MsiCondition"/>.</returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiCondition MsiEvaluateCondition(int install,
        string condition);

    /// <summary>The <c>MsiFormatRecord</c> function formats record field data and properties using a format string.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="record">Handle to the record to format. The template string must be stored in record field 0 followed by referenced data parameters.</param>
    /// <param name="result">Pointer to the buffer that receives the null terminated formatted string. Do not attempt to determine the size of the buffer by passing in a <c>null</c> for <c>result</c>. You can get the size of the buffer by passing in an empty string (for example ""). The function will then return <see cref="MsiError.MoreData"/> and <c>resultSize</c> will contain the required buffer size in TCHARs, not including the terminating null character. On return of <see cref="MsiError.Success"/>, <c>resultSize</c> contains the number of TCHARs written to the buffer, not including the terminating null character.</param>
    /// <param name="resultSize">Pointer to the variable that specifies the size, in TCHARs, of the buffer pointed to by the variable <c>result</c>. When the function returns <see cref="MsiError.Success"/>, this variable contains the size of the data copied to <c>result</c>, not including the terminating null character. If <c>result</c> is not large enough, the function returns <see cref="MsiError.MoreData"/> and stores the required size, not including the terminating null character, in the variable pointed to by <c>resultSize</c>.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.MoreData"/>  Buffer not large enough for the drive name.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiFormatRecord(int install, int record,
        StringBuilder result, ref uint resultSize);

    /// <summary>The <c>MsiGetActiveDatabase</c> function returns the active database for the installation. This function returns a read-only handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <returns>If the function succeeds, it returns a read-only handle to the database currently in use by the installer. If the function fails, the function returns <c>IntPtr.Zero</c>.</returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern int MsiGetActiveDatabase(int install);

    /// <summary>The <c>MsiGetComponentPath</c> function returns the full path to an installed component. If the key path for the component is a registry key then the registry key is returned.</summary>
    /// <param name="product">Specifies the product code for the client product.</param>
    /// <param name="component">Specifies the component ID of the component to be located.</param>
    /// <param name="path">Pointer to a variable that receives the path to the component. This parameter can be <c>null</c>. If the component is a registry key, the registry roots are represented numerically. If this is a registry subkey path, there is a backslash at the end of the Key Path. If this is a registry value key path, there is no backslash at the end. For example, a registry path of "HKEY_CURRENT_USER\SOFTWARE\Microsoft" would be returned as "01:\SOFTWARE\Microsoft\".</param>
    /// <param name="pathSize">
    /// <para>Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>path</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating <c>null</c> character.</para>
    /// <para>If <c>path</c> is <c>null</c>, <c>pathSize</c> can be <c>null</c>.</para>
    /// </param>
    /// <returns>The <see cref="MsiInstallState"/>.</returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiInstallState MsiGetComponentPath(
        string product, string component, StringBuilder path, ref uint pathSize);

    /// <summary>The <c>MsiGetComponentState</c> function obtains the state of a component.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="component">A null-terminated string specifying the component name within the product.</param>
    /// <param name="state">Receives the current installed state.</param>
    /// <param name="action">Receives the action taken during the installation.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.UnknownComponent"/>  An unknown component was requested.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiGetComponentState(int install,
        string component, out MsiInstallState state, out MsiInstallState action);

    /// <summary>The <c>MsiGetDatabaseState</c> function returns the state of the database.</summary>
    /// <param name="database">Handle to the database obtained from <see cref="MsiOpenDatabase"/>.</param>
    /// <returns>The <see cref="MsiDbState"/>.</returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiDbState MsiGetDatabaseState(int database);

    /// <summary>The <c>MsiGetFeatureCost</c> function returns the disk space required by a feature and its selected children and parent features.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="feature">Specifies the name of the feature.</param>
    /// <param name="costTree">Specifies the value the function uses to determine disk space requirements.</param>
    /// <param name="state">Specifies the installation state.</param>
    /// <param name="cost">Receives the disk space requirements in units of 512 bytes. This parameter must not be null.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.UnknownFeature"/>  An unknown feature was requested.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiGetFeatureCost(int install, string feature,
        MsiCostTree costTree, MsiInstallState state, out int cost);

    /// <summary>The <c>MsiGetFeatureInfo</c> function returns descriptive information for a feature.</summary>
    /// <param name="productHandle">Handle to the product that owns the feature. This handle is obtained from <see cref="MsiOpenProduct"/>.</param>
    /// <param name="feature">Specifies the feature code for the feature about which information should be returned.</param>
    /// <param name="attributes">Specifies the attribute flags.</param>
    /// <param name="title">Pointer to a buffer to receive the localized descriptive name of the feature.</param>
    /// <param name="titleSize">As input, the size of <c>title</c>. As output, the number of characters returned in <c>title</c>. On input, this is the full size of the buffer, including a space for a terminating <c>null</c> character. If the buffer passed in is too small, the count returned does not include the terminating <c>null</c> character.</param>
    /// <param name="help">Pointer to a buffer to receive the localized descriptive name of the feature.</param>
    /// <param name="helpSize">As input, the size of <c>help</c>. As output, the number of characters returned in <c>help</c>. On input, this is the full size of the buffer, including a space for a terminating <c>null</c> character. If the buffer passed in is too small, the count returned does not include the terminating <c>null</c> character.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  The product handle is invalid.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.MoreData"/>  A buffer is too small to hold the requested data.</para>
    /// <para><see cref="MsiError.UnknownFeature"/>  The feature is not known.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiGetFeatureInfo(int productHandle,
        string feature, MsiInstallFeatureAttribute attributes,
        StringBuilder title, ref uint titleSize, string help, ref uint helpSize);

    /// <summary>The <c>MsiGetFeatureState</c> function gets the requested state of a feature.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="feature">Specifies the feature name within the product.</param>
    /// <param name="state">Receives the current installed state.</param>
    /// <param name="action">Receives the action taken during the installation.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.UnknownFeature"/>  An unknown feature was requested.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiGetFeatureState(int install,
        string feature, out MsiInstallState state, out MsiInstallState action);

    /// <summary>The <c>MsiGetFeatureUsage</c> returns the usage metrics for a product feature.</summary>
    /// <param name="product">Specifies the product code for the product containing the feature.</param>
    /// <param name="feature">Specifies the feature code for the feature for which metrics are to be returned.</param>
    /// <param name="useCount">Indicates the number of times the feature has been used.</param>
    /// <param name="dateUsed">Specifies the date that the feature was last used. The date is in the MS-DOS date format.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.InstallFailure"/>  No usage information is available or the product or feature is invalid.</para>
    /// </returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiGetFeatureUsage(string product, string feature,
        out uint useCount, out ushort dateUsed);

    /// <summary>The <c>MsiGetFeatureValidStates</c> function returns a valid installation state.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="feature">Specifies the feature name within the product.</param>
    /// <param name="state">Receives the location to hold the valid installation states.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.UnknownFeature"/>  An unknown feature was requested.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiGetFeatureValidStates(int install,
        string feature, out MsiFeatureInstallState state);

    /// <summary>The <c>MsiGetFileVersion</c> returns the version string and language string in the format that the installer expects to find them in the database. If you just want version information, set <c>language</c> to <c>null</c> and <c>languageSize</c> to zero. If you just want language information, set <c>version</c> to <c>null</c> and <c>versionSize</c> to zero.</summary>
    /// <param name="path">Specifies the path to the file.</param>
    /// <param name="version">Returns the file version. Set to <c>null</c> for language information only.</param>
    /// <param name="versionSize">In and out buffer byte count. Set to 0 for language information only. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character.</param>
    /// <param name="language">Returns the file language. Set to <c>null</c> for version information only.</param>
    /// <param name="languageSize">In and out buffer byte count. Set to 0 for version information only. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.AccessDenied"/>  File could not be opened to get version information.</para>
    /// <para><see cref="MsiError.E_Fail"/>  Unexpected error.</para>
    /// <para><see cref="MsiError.FileInvalid"/>  File does not contain version information.</para>
    /// <para><see cref="MsiError.FileNotFound"/>  File does not exist.</para>
    /// <para><see cref="MsiError.InvalidData"/>  The version information is invalid.</para>
    /// </returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiGetFileVersion(string path, StringBuilder version,
        ref uint versionSize, StringBuilder language, ref uint languageSize);

    /// <summary>The <c>MsiGetLanguage</c> function returns the numeric language of the installation that is currently running.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <returns>If the function succeeds, the return value is the numeric LANGID for the install.  Can be <see cref="MsiError.InvalidHandle"/> if the function fails, or zero if the installation is not running.</returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern ushort MsiGetLanguage(int install);

    /// <summary>The <c>MsiGetLastErrorRecord</c> function returns the error record that was last returned for the calling process. This function returns a handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
    /// <returns>A handle to the error record. If the last function was successful, <c>MsiGetLastErrorRecord</c> returns an <c>IntPtr.Zero</c>.</returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern int MsiGetLastErrorRecord();

    /// <summary>The <c>MsiGetMode</c> function is used to determine whether the installer is currently running in a specified mode.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="mode">Specifies the run mode.</param>
    /// <returns><c>true</c> if the mode matches requested,</returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern bool MsiGetMode(int install, MsiRunMode mode);

    /// <summary>The <c>MsiGetPatchInfo</c> function returns information about a patch.</summary>
    /// <param name="patch">Specifies the patch code for the patch package.</param>
    /// <param name="attribute">Specifies the attribute to be retrieved.  (See <see cref="MsiInstallerProperty.LocalPackage"/>)</param>
    /// <param name="value">Pointer to a buffer that receives the property value. This parameter can be <c>null</c>.</param>
    /// <param name="valueSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>value</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.MoreData"/>  A buffer is too small to hold the requested data.</para>
    /// <para><see cref="MsiError.UnknownProduct"/>  The patch package is not installed.</para>
    /// <para><see cref="MsiError.UnknownProperty"/>  The property is unrecognized.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiGetPatchInfo(string patch, string attribute,
        StringBuilder value, ref uint valueSize);

    /// <summary>The <c>MsiGetProductCode</c> function returns the product code of an application by using the component code of an installed or advertised component of the application. During initialization, an application must determine under which product code it has been installed or advertised.</summary>
    /// <param name="component">This parameter specifies the component code of a component that has been installed by the application. This will be typically the component code of the component containing the executable file of the application.</param>
    /// <param name="product">Pointer to a buffer that receives the product code. This buffer must be 39 characters long. The first 38 characters are for the GUID, and the last character is for the terminating null character.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.InstallFailure"/>  The product code could not be determined.</para>
    /// <para><see cref="MsiError.UnknownComponent"/>  The specified component is unknown.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiGetProductCode(string component, string product);

    /// <summary>The <c>MsiGetProductInfo</c> function returns product information for published and installed products.</summary>
    /// <param name="product">Specifies the product code for the product.</param>
    /// <param name="property">Specifies the property to be retrieved. The properties in the following list can only be retrieved from applications that are already installed. Note that required properties are guaranteed to be available, but other properties are available only if that property has been set. See the indicated links to the installer properties for information about how each property is set.</param>
    /// <param name="value">Pointer to a buffer that receives the property value. This parameter can be <c>null</c>.</param>
    /// <param name="valueSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>value</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.MoreData"/>  A buffer is too small to hold the requested data.</para>
    /// <para><see cref="MsiError.UnknownProduct"/>  The patch package is not installed.</para>
    /// <para><see cref="MsiError.UnknownProperty"/>  The property is unrecognized.</para>
    /// </returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiGetProductInfo(string product, string property,
        StringBuilder value, ref uint valueSize);

    /// <summary>The <c>MsiGetProductInfoFromScript</c> function returns product information for a Windows Installer script file.</summary>
    /// <param name="scriptFile">A null-terminated string specifying the full path to the script file. The script file is the advertise script that was created by calling <see cref="MsiAdvertiseProduct"/> or <see cref="MsiAdvertiseProductEx"/>.</param>
    /// <param name="product">Points to a buffer that receives the product code. The buffer must be 39 characters long. The first 38 characters are for the product code GUID, and the last character is for the terminating null character.</param>
    /// <param name="langId">Points to a variable that receives the product language.</param>
    /// <param name="version">Points to a buffer that receives the product version. </param>
    /// <param name="name">Points to a buffer that receives the product name. The buffer includes a terminating null character.</param>
    /// <param name="nameSize">Points to a variable that specifies the size, in characters, of the buffer pointed to by the <c>name</c> parameter. This size should include the terminating null character. When the function returns, this variable contains the length of the string stored in the buffer. The count returned does not include the terminating null character. If the buffer is not large enough, the function returns <see cref="MsiError.MoreData"/>, and the variable contains the size of the string in characters, without counting the null character.</param>
    /// <param name="package">Points to a buffer that receives the package name. The buffer includes the terminating null character.</param>
    /// <param name="packageSize">Points to a variable that specifies the size, in characters, of the buffer pointed to by the <c>package</c> parameter. This size should include the terminating null character. When the function returns, this variable contains the length of the string stored in the buffer. The count returned does not include the terminating null character. If the buffer is not large enough, the function returns <see cref="MsiError.MoreData"/>, and the variable contains the size of the string in characters, without counting the null character.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.CallNotImplemented"/>  This function is only available on Windows 2000 and Windows XP.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.MoreData"/>  A buffer is too small to hold the requested data.</para>
    /// <para><see cref="MsiError.InstallFailure"/>  Could not get script information.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiGetProductInfoFromScript(string scriptFile,
        string product, ref ushort langId, ref uint version, StringBuilder name,
        ref uint nameSize, StringBuilder package, ref uint packageSize);

    /// <summary>The <c>MsiGetProductProperty</c> function retrieves product properties. These properties are in the product database.</summary>
    /// <param name="productHandle">Handle to the product obtained from <see cref="MsiOpenProduct"/>.</param>
    /// <param name="property">Specifies the property to retrieve. This is case-sensitive.</param>
    /// <param name="value">Pointer to a buffer that receives the property value. This parameter can be <c>null</c>.</param>
    /// <param name="valueSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>value</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  The product handle is invalid.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.MoreData"/>  A buffer is too small to hold the requested data.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiGetProductProperty(int productHandle,
        string property, StringBuilder value, ref uint valueSize);

    /// <summary>The MsiGetProperty function gets the value for an installer property.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="name">A null-terminated string that specifies the name of the property.</param>
    /// <param name="value">Pointer to the buffer that receives the null terminated string containing the value of the property. Do not attempt to determine the size of the buffer by passing in a <c>null</c> for <c>value</c>. You can get the size of the buffer by passing in an empty string (for example ""). The function will then return <see cref="MsiError.MoreData"/> and <c>valueSize</c> will contain the required buffer size in TCHARs, not including the terminating null character. On return of <see cref="MsiError.Success"/>, <c>valueSize</c> contains the number of TCHARs written to the buffer, not including the terminating null character.</param>
    /// <param name="valueSize">Pointer to the variable that specifies the size, in TCHARs, of the buffer pointed to by the variable <c>value</c>. When the function returns <see cref="MsiError.Success"/>, this variable contains the size of the data copied to <c>value</c>, not including the terminating null character. If <c>value</c> is not not large enough, the function returns <see cref="MsiError.MoreData"/> and stores the required size, not including the terminating null character, in the variable pointed to by <c>valueSize</c>.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.MoreData"/>  The provided buffer was too small to hold the entire value.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiGetProperty(int install, string name,
        StringBuilder value, ref uint valueSize);

    /// <summary>The <c>MsiGetShortcutTarget</c> function examines a shortcut and returns its product, feature name, and component if available.</summary>
    /// <param name="target">A null-terminated string specifying the full path to a shortcut.</param>
    /// <param name="product">A GUID for the product code of the shortcut. This string buffer must be 39 characters long. The first 38 characters are for the GUID, and the last character is for the terminating null character. This parameter can be null.</param>
    /// <param name="feature">The feature name of the shortcut. The string buffer must be <c><see cref="MaxFeatureChars"/> + 1</c> characters long. This parameter can be null.</param>
    /// <param name="component">A GUID of the component code. This string buffer must be 39 characters long. The first 38 characters are for the GUID, and the last character is for the terminating null character. This parameter can be null.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// </returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiGetShortcutTarget(string target,
        string product, string feature, string component);

    /// <summary>The <c>MsiGetSourcePath</c> function returns the full source path for a folder in the Directory table.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="folder"> null-terminated string that specifies a record of the Directory table. If the directory is a root directory, this can be a value from the DefaultDir column. Otherwise it must be a value from the Directory column.</param>
    /// <param name="path">Pointer to the buffer that receives the null terminated full source path. Do not attempt to determine the size of the buffer by passing in a <c>null</c> for <c>path</c>. You can get the size of the buffer by passing in an empty string (for example ""). The function will then return <see cref="MsiError.MoreData"/> and <c>pathSize</c> will contain the required buffer size in TCHARs, not including the terminating null character. On return of <see cref="MsiError.Success"/>, <c>pathSize</c> contains the number of TCHARs written to the buffer, not including the terminating null character.</param>
    /// <param name="pathSize">Pointer to the variable that specifies the size, in TCHARs, of the buffer pointed to by the variable <c>path</c>. When the function returns <see cref="MsiError.Success"/>, this variable contains the size of the data copied to <c>path</c>, not including the terminating null character. If <c>path</c> is not large enough, the function returns <see cref="MsiError.MoreData"/> and stores the required size, not including the terminating null character, in the variable pointed to by <c>pathSize</c>.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.Directory"/>  The directory specified was not found in the Directory table.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.MoreData"/>  The provided buffer was too small to hold the entire value.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiGetSourcePath(int install, string folder,
        StringBuilder path, ref uint pathSize);

    /// <summary>The <c>MsiGetSummaryInformation</c> function obtains a handle to the _SummaryInformation stream for an installer database. This function returns a handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="path">Specifies the path to the database. </param>
    /// <param name="updateCount">Specifies the maximum number of updated values.</param>
    /// <param name="summaryInfo">Pointer to the location from which to receive the summary information handle.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InstallPackageInvalid"/>  The installation package is invalid.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiGetSummaryInformation(int install,
        string path, uint updateCount, out int summaryInfo);

    /// <summary>The <c>MsiGetTargetPath</c> function returns the full target path for a folder in the Directory table.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="folder"> null-terminated string that specifies a record of the Directory table. If the directory is a root directory, this can be a value from the DefaultDir column. Otherwise it must be a value from the Directory column.</param>
    /// <param name="path">Pointer to the buffer that receives the null terminated full source path. Do not attempt to determine the size of the buffer by passing in a <c>null</c> for <c>path</c>. You can get the size of the buffer by passing in an empty string (for example ""). The function will then return <see cref="MsiError.MoreData"/> and <c>pathSize</c> will contain the required buffer size in TCHARs, not including the terminating null character. On return of <see cref="MsiError.Success"/>, <c>pathSize</c> contains the number of TCHARs written to the buffer, not including the terminating null character.</param>
    /// <param name="pathSize">Pointer to the variable that specifies the size, in TCHARs, of the buffer pointed to by the variable <c>path</c>. When the function returns <see cref="MsiError.Success"/>, this variable contains the size of the data copied to <c>path</c>, not including the terminating null character. If <c>path</c> is not large enough, the function returns <see cref="MsiError.MoreData"/> and stores the required size, not including the terminating null character, in the variable pointed to by <c>pathSize</c>.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.Directory"/>  The directory specified was not found in the Directory table.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.MoreData"/>  The provided buffer was too small to hold the entire value.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiGetTargetPath(int install, string folder,
        StringBuilder path, ref uint pathSize);

    /// <summary>The <c>MsiGetUserInfo</c> function returns the registered user information for an installed product.</summary>
    /// <param name="product">Specifies the product code for the product to be queried.</param>
    /// <param name="user">Pointer to a variable that receives the name of the user.</param>
    /// <param name="userSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>user</c> parameter. This size should include the terminating null character.</param>
    /// <param name="org">Pointer to a buffer that receives the organization name.</param>
    /// <param name="orgSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>org</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character.</param>
    /// <param name="serial">Pointer to a buffer that receives the product ID.</param>
    /// <param name="serialSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>serial</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character.</param>
    /// <returns>The <see cref="MsiUserInfoState"/> result.</returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiUserInfoState MsiGetUserInfo(string product,
        StringBuilder user, ref uint userSize, StringBuilder org, ref uint orgSize,
        StringBuilder serial, ref uint serialSize);

    /// <summary>The <c>MsiInstallMissingComponent</c> function installs files that are unexpectedly missing.</summary>
    /// <param name="product">Specifies the product code for the product that owns the component to be installed.</param>
    /// <param name="component">Identifies the component to be installed.</param>
    /// <param name="state">Specifies the way the component should be installed. </param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.InstallFailure"/>  The installation failed.</para>
    /// <para><see cref="MsiError.InstallSourceAbsent"/>  The source was unavailable.</para>
    /// <para><see cref="MsiError.InstallSuspend"/>  The installation was suspended.</para>
    /// <para><see cref="MsiError.InstallUserExit"/>  The user exited the installation.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.UnknownProduct"/>  The product code is unrecognized.</para>
    /// </returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiInstallMissingComponent(string product,
        string component, MsiInstallState state);

    /// <summary>The <c>MsiInstallMissingFile</c> function installs files that are unexpectedly missing.</summary>
    /// <param name="product">Specifies the product code for the product that owns the file to be installed.</param>
    /// <param name="file">Specifies the file to be installed.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.InstallFailure"/>  The installation failed.</para>
    /// <para><see cref="MsiError.InstallSourceAbsent"/>  The source was unavailable.</para>
    /// <para><see cref="MsiError.InstallSuspend"/>  The installation was suspended.</para>
    /// <para><see cref="MsiError.InstallUserExit"/>  The user exited the installation.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.UnknownProduct"/>  The product code is unrecognized.</para>
    /// </returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiInstallMissingFile(string product, string file);

    /// <summary>The MsiInstallProduct function installs or uninstalls a product.</summary>
    /// <param name="product">A null-terminated string that specifies the path to the package.</param>
    /// <param name="commandLine">A null-terminated string that specifies the command line property settings. This should be a list of the format <i>Property=Setting Property=Setting</i>.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiInstallProduct(string product, string commandLine);

    /// <summary>
    /// <para>The <c>MsiIsProductElevated</c> function checks whether the product is installed with elevated privileges. An application is called a "managed application" if elevated (system) privileges are used to install the application.</para>
    /// <para>Note that <c>MsiIsProductElevated</c> does not take into account policies such as AlwaysInstallElevated, but verifies that the local system owns the product's registry data.</para>
    /// </summary>
    /// <param name="product">The full product code GUID of the product. This parameter is required and may not be null or empty.</param>
    /// <param name="elevated">A pointer to a <see cref="bool"/> for the result. This may not be null.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.CallNotImplemented"/>  This function is only available on Windows 2000 and Windows XP.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiIsProductElevated(string product, out bool elevated);

    /// <summary>The <c>MsiLocateComponent</c> function returns the full path to an installed component without a product code. This function attempts to determine the product using <see cref="MsiGetProductCode"/>, but is not guaranteed to find the correct product for the caller. <see cref="MsiGetComponentPath"/> should always be called when possible.</summary>
    /// <param name="component">Specifies the component ID of the component to be located.</param>
    /// <param name="path">Pointer to a variable that receives the path to the component. The variable includes the terminating null character.</param>
    /// <param name="pathSize">
    /// <para>Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>path</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. Upon success of the <c>MsiLocateComponent</c> function, the variable pointed to by pcchBuf contains the count of characters not including the terminating null character. If the size of the buffer passed in is too small, the function returns <see cref="MsiInstallState.MoreData"/>.</para>
    /// <para>If <c>path</c> is <c>null</c>, pcchBuf can be 0.</para>
    /// </param>
    /// <returns>The <see cref="MsiInstallState"/>.</returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiInstallState MsiLocateComponent(string component,
        StringBuilder path, ref uint pathSize);

    /// <summary>The <c>MsiOpenDatabase</c> function opens a database file for data access. This function returns a handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
    /// <param name="path">Specifies the full path or relative path to the database file.</param>
    /// <param name="persist">Receives the full path to the file or the persistence mode.  You can use one of the constants from <see cref="MsiDbOpenPersistMode"/>.</param>
    /// <param name="handle">Pointer to the location of the returned database handle.</param>
    /// <returns>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiOpenDatabase(string path, MsiDbPersistMode persist,
        out int handle);

    /// <summary>The <c>MsiOpenPackage</c> function opens a package for use with the functions that access the product database. The <see cref="MsiCloseHandle"/> function must be called with the handle when the handle is no longer needed.</summary>
    /// <param name="path">Specifies the path to the package. </param>
    /// <param name="handle">Specifies the path to the package. </param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.InstallFailure"/>  The product could not be opened.</para>
    /// <para><see cref="MsiError.InstallRemoteProhibited"/>  Windows Installer does not permit installation from a Remote Desktop Connection.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiOpenPackage(string path, out int handle);

    /// <summary>The <c>MsiOpenPackageEx</c> function opens a package for use with functions that access the product database. The <see cref="MsiCloseHandle"/> function must be called with the handle when the handle is no longer needed.</summary>
    /// <param name="path">Specifies the path to the package.</param>
    /// <param name="options">The <see cref="MsiOpenPackageFlags"/> option.</param>
    /// <param name="handle">Pointer to a variable that receives the product handle.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.InstallFailure"/>  The product could not be opened.</para>
    /// <para><see cref="MsiError.InstallRemoteProhibited"/>  Windows Installer does not permit installation from a Remote Desktop Connection.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiOpenPackageEx(string path, MsiOpenPackageFlags options,
        out int handle);

    /// <summary>The <c>MsiOpenProduct</c> function opens a product for use with the functions that access the product database. The <see cref="MsiCloseHandle"/> function must be called with the handle when the handle is no longer needed.</summary>
    /// <param name="product">Specifies the product code of the product to be opened.</param>
    /// <param name="handle">Pointer to a variable that receives the product handle. </param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.InstallFailure"/>  The product could not be opened.</para>
    /// <para><see cref="MsiError.InstallRemoteProhibited"/>  Windows Installer does not permit installation from a Remote Desktop Connection.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiOpenProduct(string product, out int handle);

    /// <summary>The <c>MsiPreviewBillboard</c> function displays a billboard with the host control in the displayed dialog box.</summary>
    /// <param name="preview">Handle to the preview.</param>
    /// <param name="name">Specifies the name of the host control.</param>
    /// <param name="billboard">Specifies the name of the billboard to display.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiPreviewBillboard(int preview,
        string name, string billboard);

    /// <summary>The <c>MsiPreviewDialog</c> function displays a dialog box as modeless and inactive.</summary>
    /// <param name="preview">Handle to the preview.</param>
    /// <param name="dialog">Specifies the name of the dialog box to preview.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.FunctionNotCalled"/>  The function was not called.</para>
    /// <para><see cref="MsiError.InstallFailure"/>  The action failed.</para>
    /// <para><see cref="MsiError.InstallSuspend"/>  The user suspended the installation.</para>
    /// <para><see cref="MsiError.InstallUserExit"/>  The user canceled the action.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiPreviewDialog(int preview,
        string dialog);

    /// <summary>The <c>MsiProcessMessage</c> function sends an error record to the installer for processing.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="type">The <see cref="MsiInstallMessage"/>.</param>
    /// <param name="record">Handle to a record containing message format and data.</param>
    /// <returns>
    /// <para><b>-1</b> An invalid parameter or handle was supplied.</para>
    /// <para><b>0</b> No action was taken.</para>
    /// <para>A <see cref="DialogResult"/>.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern int MsiProcessMessage(int install,
        MsiInstallMessage type, int record);

    /// <summary>
    /// <para>The <c>MsiProvideAssembly</c> function returns the full path to a Windows Installer component containing an assembly. The function prompts for a source and performs any necessary installation. <c>MsiProvideAssembly</c> increments the usage count for the feature.</para>
    /// <para>This function is available starting with Windows Installer version 2.0.</para>
    /// </summary>
    /// <param name="assembly">The assembly's name as a string.</param>
    /// <param name="context">Set to <c>null</c> for global assemblies. For private assemblies, set <c>context</c> to the full path of the application configuration file (.cfg file) or to the full path of the executable file (.exe) of the application to which the assembly has been made private.</param>
    /// <param name="mode">Defines the installation mode.</param>
    /// <param name="info">Assembly information and assembly type.</param>
    /// <param name="path">Pointer to a variable that receives the path to the component. This parameter can be <c>null</c>.</param>
    /// <param name="pathSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>path</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character. </param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.FileNotFound"/>  The feature is absent or broken.</para>
    /// <para><see cref="MsiError.InstallFailure"/>  The product could not be opened.</para>
    /// <para><see cref="MsiError.InstallNotUsed"/>  The component being requested is disabled on the computer.</para>
    /// <para><see cref="MsiError.InstallSourceAbsent"/>  The source was unavailable.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.MoreData"/>  A buffer is too small to hold the requested data.</para>
    /// <para><see cref="MsiError.NotEnoughMemory"/>  The system does not have enough memory to complete the operation. Available with Windows Server 2003 family.</para>
    /// <para><see cref="MsiError.UnknownComponent"/>  The component ID does not specify a known component.</para>
    /// <para><see cref="MsiError.UnknownFeature"/>  The feature ID does not identify a known feature.</para>
    /// <para><see cref="MsiError.UnknownProduct"/>  The product code does not identify a known product.</para>
    /// <para><see cref="MsiInstallState.Unknown"/>  An unrecognized product or a feature name was passed to the function.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern uint MsiProvideAssembly(string assembly, string context,
        uint mode, MsiAssemblyInfo info, string path, ref int pathSize);

    /// <summary>The <c>MsiProvideComponent</c> function returns the full component path, performing any necessary installation. This function prompts for source if necessary and increments the usage count for the feature.</summary>
    /// <param name="product">Specifies the product code for the product that contains the feature with the necessary component.</param>
    /// <param name="feature">Specifies the feature ID of the feature with the necessary component.</param>
    /// <param name="component">Specifies the component code of the necessary component.</param>
    /// <param name="mode">Defines the installation mode.</param>
    /// <param name="path">Pointer to a variable that receives the path to the component. This parameter can be <c>null</c>.</param>
    /// <param name="pathSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>path</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character. </param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.FileNotFound"/>  The feature is absent or broken.</para>
    /// <para><see cref="MsiError.InstallFailure"/>  The product could not be opened.</para>
    /// <para><see cref="MsiError.InstallNotUsed"/>  The component being requested is disabled on the computer.</para>
    /// <para><see cref="MsiError.InstallSourceAbsent"/>  The source was unavailable.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.MoreData"/>  A buffer is too small to hold the requested data.</para>
    /// <para><see cref="MsiError.UnknownFeature"/>  The feature ID does not identify a known feature.</para>
    /// <para><see cref="MsiError.UnknownProduct"/>  The product code does not identify a known product.</para>
    /// <para><see cref="MsiInstallState.Unknown"/>  An unrecognized product or a feature name was passed to the function.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern uint MsiProvideComponent(string product, string feature,
        string component, uint mode, StringBuilder path, ref int pathSize);

    /// <summary>The <c>MsiProvideQualifiedComponent</c> function returns the full component path for a qualified component and performs any necessary installation. This function prompts for source if necessary, and increments the usage count for the feature.</summary>
    /// <param name="component">Specifies the component ID for the requested component. This may not be the GUID for the component itself, but rather a server that provides the correct functionality, as in the ComponentId column of the PublishComponent table.</param>
    /// <param name="qualifier">Specifies a qualifier into a list of advertising components (from PublishComponent Table).</param>
    /// <param name="mode">Defines the installation mode.</param>
    /// <param name="path">Pointer to a variable that receives the path to the component. This parameter can be <c>null</c>.</param>
    /// <param name="pathSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>path</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character. </param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FileNotFound"/>  The feature is absent or broken.</para>
    /// <para><see cref="MsiError.IndexAbsent"/>  The component qualifier is invalid or absent.</para>
    /// <para><see cref="MsiError.UnknownComponent"/>  The component ID does not specify a known component.</para>
    /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
    /// </returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiProvideQualifiedComponent(string component,
        string qualifier, uint mode, StringBuilder path, ref int pathSize);

    /// <summary>The <c>MsiProvideQualifiedComponent</c> function returns the full component path for a qualified component and performs any necessary installation. This function prompts for source if necessary, and increments the usage count for the feature.</summary>
    /// <param name="component">Specifies the component ID for the requested component. This may not be the GUID for the component itself, but rather a server that provides the correct functionality, as in the ComponentId column of the PublishComponent table.</param>
    /// <param name="qualifier">Specifies a qualifier into a list of advertising components (from PublishComponent Table).</param>
    /// <param name="mode">Defines the installation mode.</param>
    /// <param name="product">Specifies the product to match that has published the qualified component. If this is <c>null</c>, then this API works the same as <see cref="MsiProvideQualifiedComponent"/>.</param>
    /// <param name="unused1">Reserved. Must be zero.</param>
    /// <param name="unused2">Reserved. Must be zero.</param>
    /// <param name="path">Pointer to a variable that receives the path to the component. This parameter can be <c>null</c>.</param>
    /// <param name="pathSize">Pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <c>path</c> parameter. On input, this is the full size of the buffer, including a space for a terminating null character. If the buffer passed in is too small, the count returned does not include the terminating null character. </param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FileNotFound"/>  The feature is absent or broken.</para>
    /// <para><see cref="MsiError.IndexAbsent"/>  The component qualifier is invalid or absent.</para>
    /// <para><see cref="MsiError.UnknownComponent"/>  The component ID does not specify a known component.</para>
    /// <para>An error relating to an action, see <see cref="MsiError"/>.</para>
    /// </returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiProvideQualifiedComponentEx(string component,
        string qualifier, uint mode, string product, uint unused1, uint unused2,
        StringBuilder path, ref int pathSize);

    /// <summary>The <c>MsiQueryFeatureState</c> function returns the installed state for a product feature.</summary>
    /// <param name="product">Specifies the product code for the product that contains the feature of interest.</param>
    /// <param name="feature">Identifies the feature of interest.</param>
    /// <returns>The <see cref="MsiInstallState"/>.</returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiInstallState MsiQueryFeatureState(string product,
        string feature);

    /// <summary>The <c>MsiQueryProductState</c> function returns the installed state for a product.</summary>
    /// <param name="product">Specifies the product code for the product of interest.</param>
    /// <returns>The <see cref="MsiInstallState"/>.</returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiInstallState MsiQueryProductState(string product);

    /// <summary>The <c>MsiRecordClearData</c> function sets all fields in a record to <c>null</c>.</summary>
    /// <param name="record">Handle to the record.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiRecordClearData(int record);

    /// <summary>The <c>MsiRecordDataSize</c> function returns the length of a record field. The count does not include the terminating null character.</summary>
    /// <param name="record">Handle to the record.</param>
    /// <param name="field">Specifies a field of the record.</param>
    /// <returns>
    /// <para>The <c>MsiRecordDataSize</c> function returns 0 if the field is null, nonexistent, or an internal object pointer. The function also returns 0 if the handle is not a valid record handle.</para>
    /// <para>If the data is in integer format, the function returns sizeof(int).</para>
    /// <para>If the data is in string format, the function returns the character count (not including the null character).</para>
    /// <para>If the data is in stream format, the function returns the byte count.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern uint MsiRecordDataSize(int record, uint field);

    /// <summary>The <c>MsiRecordGetFieldCount</c> function returns the number of fields in a record.</summary>
    /// <param name="record">Handle to the record.</param>
    /// <returns>The count returned by the <c>MsiRecordGetFieldCount</c> parameter does not include field 0. Read access to fields beyond this count returns null values. Write access fails.</returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern uint MsiRecordGetFieldCount(int record);

    /// <summary>The <c>MsiRecordGetInteger</c> function returns the integer value from a record field.</summary>
    /// <param name="record">Handle to the record.</param>
    /// <param name="field">Specifies a field of the record.</param>
    /// <returns>The MsiRecordGetInteger function returns <see cref="MsiNullInteger"/> if the field is null or if the field is a string that cannot be converted to an integer.</returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern int MsiRecordGetInteger(int record, uint field);

    /// <summary>The <c>MsiRecordGetString</c> function returns the string value of a record field.</summary>
    /// <param name="record">Handle to the record.</param>
    /// <param name="field">Specifies a field of the record.</param>
    /// <param name="value">Pointer to the buffer that receives the null terminated string containing the value of the record field. Do not attempt to determine the size of the buffer by passing in a <c>null</c> for <c>value</c>. You can get the size of the buffer by passing in an empty string (for example ""). The function will then return <see cref="MsiError.MoreData"/> and <c>valueSize</c> will contain the required buffer size in TCHARs, not including the terminating null character. On return of <see cref="MsiError.Success"/>, <c>valueSize</c> contains the number of TCHARs written to the buffer, not including the terminating null character.</param>
    /// <param name="valueSize">Pointer to the variable that specifies the size, in TCHARs, of the buffer pointed to by the variable <c>value</c>. When the function returns <see cref="MsiError.Success"/>, this variable contains the size of the data copied to <c>value</c>, not including the terminating null character. If <c>value</c> is not large enough, the function returns <see cref="MsiError.MoreData"/> and stores the required size, not including the terminating null character, in the variable pointed to by <c>valueSize</c>.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.MoreData"/>  The provided buffer was too small to hold the entire value.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiRecordGetString(int record, uint field,
        StringBuilder value, ref uint valueSize);

    /// <summary>The <c>MsiRecordIsNull</c> function reports whether a record field is <c>null</c>.</summary>
    /// <param name="record">Handle to the record.</param>
    /// <param name="field">Specifies a field of the record.</param>
    /// <returns><c>true</c>, if the function succeeded and the field is null or the field does not exist; otherwise, The function succeeded and the field is not null or the record handle is invalid.</returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern bool MsiRecordIsNull(int record, uint field);

    /// <summary>The <c>MsiRecordReadStream</c> function reads bytes from a record stream field into a buffer.</summary>
    /// <param name="record">Handle to the record.</param>
    /// <param name="field">Specifies a field of the record.</param>
    /// <param name="buffer">A buffer to receive the stream field. You should ensure the destination buffer is the same size or larger than the source buffer.</param>
    /// <param name="bufferSize">Specifies the in and out buffer count. On input, this is the full size of the buffer. On output, this is the number of bytes that were actually written to the buffer.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidDataType"/>  The field is not a stream column.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiRecordReadStream(int record, uint field,
        StringBuilder buffer, ref uint bufferSize);

    /// <summary>The MsiRecordSetInteger function sets a record field to an integer field.</summary>
    /// <param name="record">Handle to the record.</param>
    /// <param name="field">Specifies a field of the record.</param>
    /// <param name="value">Specifies the value to which to set the field.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidField"/>  An invalid field of the record was supplied.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiRecordSetInteger(int record, uint field,
        int value);

    /// <summary>The <c>MsiRecordSetStream</c> function sets a record stream field from a file. Stream data cannot be inserted into temporary fields.</summary>
    /// <param name="record">Handle to the record.</param>
    /// <param name="field">Specifies a field of the record.</param>
    /// <param name="path">Specifies the path to the file containing the stream.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.BadPathName"/>  An invalid path was supplied.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiRecordSetStream(int record, uint field,
        string path);

    /// <summary>The <c>MsiRecordSetString</c> function copies a string into the designated field.</summary>
    /// <param name="record">Handle to the record.</param>
    /// <param name="field">Specifies a field of the record.</param>
    /// <param name="value">Specifies the string value of the field.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiRecordSetString(int record, uint field,
        string value);

    /// <summary>The <c>MsiReinstallFeature</c> function reinstalls features.</summary>
    /// <param name="product">Specifies the product code for the product containing the feature to be reinstalled.</param>
    /// <param name="feature">Identifies the feature to be reinstalled.</param>
    /// <param name="mode">Specifies what to install.  (See <see cref="MsiReinstallMode"/>.)</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InstallFailure"/>  The installation failed.</para>
    /// <para><see cref="MsiError.InstallServiceFailure"/>  The installation service could not be accessed.</para>
    /// <para><see cref="MsiError.InstallSuspend"/>  The installation was suspended.</para>
    /// <para><see cref="MsiError.InstallUserExit"/>  The user exited the installation.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.UnknownFeature"/>  The feature ID does not identify a known feature.</para>
    /// <para><see cref="MsiError.UnknownProduct"/>  The product code does not identify a known product.</para>
    /// </returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiReinstallFeature(string product,
        string feature, MsiReinstallMode mode);

    /// <summary>The <c>MsiReinstallProduct</c> function reinstalls products.</summary>
    /// <param name="product">Specifies the product code for the product containing the feature to be reinstalled.</param>
    /// <param name="mode">Specifies what to install.  (See <see cref="MsiReinstallMode"/>.)</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InstallFailure"/>  The installation failed.</para>
    /// <para><see cref="MsiError.InstallServiceFailure"/>  The installation service could not be accessed.</para>
    /// <para><see cref="MsiError.InstallSuspend"/>  The installation was suspended.</para>
    /// <para><see cref="MsiError.InstallUserExit"/>  The user exited the installation.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.UnknownFeature"/>  The feature ID does not identify a known feature.</para>
    /// <para><see cref="MsiError.UnknownProduct"/>  The product code does not identify a known product.</para>
    /// </returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiReinstallProduct(string product,
        MsiReinstallMode mode);

    /// <summary>The <c>MsiSequence</c> function executes another action sequence, as described in the specified table.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="table">Specifies the name of the table containing the action sequence.</param>
    /// <param name="mode">This parameter is currently unimplemented. It is reserved for future use and must be 0.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.FunctionNotCalled"/>  The function was not called.</para>
    /// <para><see cref="MsiError.InstallFailure"/>  The action failed.</para>
    /// <para><see cref="MsiError.InstallSuspend"/>  The user suspended the installation.</para>
    /// <para><see cref="MsiError.InstallUserExit"/>  The user canceled the action.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiSequence(int install, string table,
        int mode);

    /// <summary>The <c>MsiSetComponentState</c> function sets a component to the requested state.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="component">Specifies the name of the component.</param>
    /// <param name="state">Specifies the state to set.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InstallUserExit"/>  The user canceled the action.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.UnknownComponent"/>  An unknown component was requested.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiSetComponentState(int install, string component,
        MsiInstallState state);

    /// <summary>The <c>MsiSetExternalUI</c> function enables an external user-interface handler. This external UI handler is called before the normal internal user-interface handler. The external UI handler has the option to suppress the internal UI by returning a non-zero value to indicate that it has handled the messages.</summary>
    /// <param name="handler">The <see cref="MsiInstallUIHandler"/> handler delegate.</param>
    /// <param name="filter">Specifies which messages to handle using the external message handler. If the external handler returns a non-zero result, then that message will not be sent to the UI, instead the message will be logged if logging has been enabled. See <see cref="MsiEnableLog"/>.</param>
    /// <param name="context">Pointer to an application context that is passed to the callback function. This parameter can be used for error checking.</param>
    /// <returns>The return value is the previously set external handler, or <c>null</c> if there was no previously set handler.</returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiInstallUIHandler MsiSetExternalUI(
        [MarshalAs(UnmanagedType.FunctionPtr)]
        MsiInstallUIHandler handler, MsiInstallLogMode filter,
        int context);

    /// <summary>The <c>MsiSetFeatureAttributes</c> function can modify the default attributes of a feature at runtime. Note that the default attributes of features are authored in the Attributes column of the Feature table.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="feature">Specifies the feature name within the product.</param>
    /// <param name="attributes">Feature attributes specified at run time.  (Bit flags)</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.UnknownFeature"/>  The feature is not known.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiSetFeatureAttributes(int install,
        string feature, MsiInstallFeatureAttribute attributes);

    /// <summary>The <c>	MsiSetFeatureState</c> function sets a feature to a specified state.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="feature">Specifies the feature name within the product.</param>
    /// <param name="state">Specifies the state to set.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.UnknownFeature"/>  The feature is not known.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiSetFeatureState(int install, string feature,
        MsiInstallState state);

    /// <summary>The <c>MsiSetInstallLevel</c> function sets the installation level for a full product installation.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="level">Specifies the installation level.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiSetInstallLevel(int install, int level);

    /// <summary>The <c>MsiSetInternalUI</c> function enables the installer's internal user interface. Then this user interface is used for all subsequent calls to user-interface-generating installer functions in this process.</summary>
    /// <param name="level">Specifies the level of complexity of the user interface.</param>
    /// <param name="parentWindow">Pointer to a window. This window becomes the owner of any user interface created. A pointer to the previous owner of the user interface is returned. If this parameter is <c>null</c>, the owner of the user interface does not change.</param>
    /// <returns>The previous user interface level is returned. If an invalid <c>level</c> is passed, then <see cref="MsiInstallUILevel.NoChange"/> is returned.</returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiInstallUILevel MsiSetInternalUI(MsiInstallUILevel level,
        ref int parentWindow);

    /// <summary>The <c>MsiSetMode</c> function sets an internal engine Boolean state.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="mode">The <see cref="MsiRunMode"/>.  Only <see cref="MsiRunMode.RebootAtEnd"/> and <see cref="MsiRunMode.RebootNow"/> are supported.</param>
    /// <param name="state">Specifies the state to set to <c>true</c> or <c>false</c>.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.AccessDenied"/>  Access was denied.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiSetMode(int install, MsiRunMode mode,
        bool state);

    /// <summary>The <c>MsiSetProperty</c> function sets the value for an installation property.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="name">Specifies the name of the property.</param>
    /// <param name="value">Specifies the value of the property.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiSetProperty(int install, string name,
        string value);

    /// <summary>The <c>MsiSetTargetPath</c> function sets the full target path for a folder in the Directory table.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <param name="folder">Specifies the folder identifier. This is a primary key in the Directory table.</param>
    /// <param name="path">Specifies the full path for the folder, ending in a directory separator.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.Directory"/>  The directory specified was not found in the Directory table.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiSetTargetPath(int install, string folder,
        string path);

    /// <summary>The <c>MsiSourceListAddSource</c> function adds to the list of valid network sources in the source list.</summary>
    /// <param name="product">Specifies the product code.</param>
    /// <param name="user">User name for per-user installation; null or empty string for per-machine installation. On Windows NT, Windows 2000, or Windows XP, the user name should always be in the format of DOMAIN\USERNAME (or MACHINENAME\USERNAME for a local user).</param>
    /// <param name="reserved">Reserved for future use. This value must be set to 0.</param>
    /// <param name="source">Pointer to the string specifying the source.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.AccessDenied"/>  The user does not have the ability to add a source.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.BadUserName"/>  Could not resolve the user name.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InstallServiceFailure"/>  The installation service could not be accessed.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.UnknownProduct"/>  The product code does not identify a known product.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiSourceListAddSource(string product,
        string user, uint reserved, string source);

    /// <summary>The <c>MsiSourceListClearAll</c> function removes all network sources from the source list.</summary>
    /// <param name="product">Specifies the product code.</param>
    /// <param name="user">User name for per-user installation; null or empty string for per-machine installation. On Windows NT, Windows 2000, or Windows XP, the user name should always be in the format of DOMAIN\USERNAME (or MACHINENAME\USERNAME for a local user).</param>
    /// <param name="reserved">Reserved for future use. This value must be set to 0.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.AccessDenied"/>  The user does not have the ability to add a source.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.BadUserName"/>  Could not resolve the user name.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InstallServiceFailure"/>  The installation service could not be accessed.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.UnknownProduct"/>  The product code does not identify a known product.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiSourceListClearAll(string product,
        string user, uint reserved);

    /// <summary>The <c>MsiSourceListForceResolution</c> function forces the installer to search the source list for a valid product source the next time a source is needed. For example, when the installer performs an installation or reinstallation, or when it needs the path for a component that is set to run from source.</summary>
    /// <param name="product">Specifies the product code.</param>
    /// <param name="user">User name for per-user installation; null or empty string for per-machine installation. On Windows NT, Windows 2000, or Windows XP, the user name should always be in the format of DOMAIN\USERNAME (or MACHINENAME\USERNAME for a local user).</param>
    /// <param name="reserved">Reserved for future use. This value must be set to 0.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.AccessDenied"/>  The user does not have the ability to add a source.</para>
    /// <para><see cref="MsiError.BadConfiguration"/>  The configuration data is corrupt.</para>
    /// <para><see cref="MsiError.BadUserName"/>  Could not resolve the user name.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InstallServiceFailure"/>  The installation service could not be accessed.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.UnknownProduct"/>  The product code does not identify a known product.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiSourceListForceResolution(string product,
        string user, uint reserved);

    /// <summary>The <c>MsiSummaryInfoGetProperty</c> function gets a single property from the summary information.</summary>
    /// <param name="summaryInfo">Handle to summary information.</param>
    /// <param name="id">Specifies the property ID.</param>
    /// <param name="type">Receives the returned property type.</param>
    /// <param name="intValue">Receives the returned integer property data.</param>
    /// <param name="fileTime">Pointer to a file value.</param>
    /// <param name="value">Pointer to the buffer that receives the null terminated summary information property value. Do not attempt to determine the size of the buffer by passing in a <c>null</c> for <c>value</c>. You can get the size of the buffer by passing in an empty string (for example ""). The function will then return <see cref="MsiError.MoreData"/> and <c>valueSize</c> will contain the required buffer size in TCHARs, not including the terminating null character. On return of <see cref="MsiError.Success"/>, <c>valueSize</c> contains the number of TCHARs written to the buffer, not including the terminating null character. This parameter is an empty string if there are no errors.</param>
    /// <param name="valueSize">Pointer to the variable that specifies the size, in TCHARs, of the buffer pointed to by the variable <c>value</c>. When the function returns <see cref="MsiError.Success"/>, this variable contains the size of the data copied to <c>value</c>, not including the terminating null character. If <c>value</c> is not large enough, the function returns <see cref="MsiError.MoreData"/> and stores the required size, not including the terminating null character, in the variable pointed to by <c>valueSize</c>.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.MoreData"/>  The buffer passed in was too small to hold the entire value. </para>
    /// <para><see cref="MsiError.UnknownProperty"/>  The property is unknown.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiSummaryInfoGetProperty(int summaryInfo,
        uint id, out VariantType type, out int intValue, out FILETIME fileTime,
        StringBuilder value, ref int valueSize);

    /// <summary>The <c>MsiSummaryInfoGetPropertyCount</c> function returns the number of existing properties in the summary information stream.</summary>
    /// <param name="summaryInfo">Handle to summary information.</param>
    /// <param name="count">Location to receive the total property count.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiSummaryInfoGetPropertyCount(int summaryInfo,
        out int count);

    /// <summary>The <c>MsiSummaryInfoPersist</c> function writes changed summary information back to the summary information stream.</summary>
    /// <param name="summaryInfo">Handle to summary information.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiSummaryInfoPersist(int summaryInfo);

    /// <summary>The <c>MsiSummaryInfoSetProperty</c> function sets a single summary information property.</summary>
    /// <param name="summaryInfo">Handle to summary information.</param>
    /// <param name="id">Specifies the property to set.</param>
    /// <param name="type">Specifies the type of property to set.</param>
    /// <param name="intValue">Specifies the integer value.</param>
    /// <param name="fileTime">Specifies the file-time value.</param>
    /// <param name="value">Specifies the text value. </param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.DatatypeMismatch"/>  The data types were mismatched.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.UnknownProperty"/>  The property is unknown.</para>
    /// <para><see cref="MsiError.UnsupportedType"/>  The type is unsupported.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiSummaryInfoSetProperty(int summaryInfo,
        uint id, VariantType type, int intValue, FILETIME fileTime, string value);

    /// <summary>The <c>MsiUseFeature</c> function increments the usage count for a particular feature and indicates the installation state for that feature. This function should be used to indicate an application's intent to use a feature.</summary>
    /// <param name="product">Specifies the product code for the product that owns the feature to be used.</param>
    /// <param name="feature">Identifies the feature to be used.</param>
    /// <returns>The <see cref="MsiInstallState"/>.</returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiInstallState MsiUseFeature(string product, string feature);

    /// <summary>The <c>MsiUseFeatureEx</c> function increments the usage count for a particular feature and indicates the installation state for that feature. This function should be used to indicate an application's intent to use a feature.</summary>
    /// <param name="product">Specifies the product code for the product that owns the feature to be used.</param>
    /// <param name="feature">Identifies the feature to be used.</param>
    /// <param name="mode">This can be <see cref="MsiInstallMode.NoDetection"/>.</param>
    /// <param name="reserved">Reserved for future use. This value must be set to 0. </param>
    /// <returns>The <see cref="MsiInstallState"/>.</returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiInstallState MsiUseFeatureEx(string product, string feature,
        MsiInstallMode mode, uint reserved);

    /// <summary>The <c>MsiVerifyDiskSpace</c> function checks to see if sufficient disk space is present for the current installation.</summary>
    /// <param name="install">Handle to the installation provided to a DLL custom action or obtained through <see cref="MsiOpenPackage"/>, <see cref="MsiOpenPackageEx"/>, or <see cref="MsiOpenProduct"/>.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.DiskFull"/>  The disk is full.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiVerifyDiskSpace(int install);

    /// <summary>The <c>MsiVerifyPackage</c> function verifies that the given file is an installation package.</summary>
    /// <param name="path">Specifies the path and file name of the package.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The file is a package.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// <para><see cref="MsiError.PatchPackageInvalid"/>  The file is not a valid package.</para>
    /// <para><see cref="MsiError.PatchPackageOpenFailed"/>  The file could not be opened.</para>
    /// </returns>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiVerifyPackage(string path);

    #endregion	Installer Functions

    #region	Database Functions

    /// <summary>The <c>MsiViewClose</c> function releases the result set for an executed view. </summary>
    /// <param name="view">Handle to a view that is set to release.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiViewClose(int view);

    /// <summary>The <c>MsiViewExecute</c> function executes a SQL view query and supplies any required parameters. The query uses the question mark token to represent parameters as described in SQL Syntax. The values of these parameters are passed in as the corresponding fields of a parameter record.</summary>
    /// <param name="view">Handle to the view upon which to execute the query.</param>
    /// <param name="record">Handle to a record that supplies the parameters. This parameter contains values to replace the parameter tokens in the SQL query. It is optional, so hRecord can be <c>IntPtr.Zero</c>.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiViewExecute(int view, int record);

    /// <summary>The <c>MsiViewFetch</c> function fetches the next sequential record from the view. This function returns a handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
    /// <param name="view">Handle to the view to fetch from.</param>
    /// <param name="record">Pointer to the handle for the fetched record.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiViewFetch(int view, ref int record);

    /// <summary>The <c>MsiViewGetColumnInfo</c> function returns a record containing column names or definitions. This function returns a handle that should be closed using <see cref="MsiCloseHandle"/>.</summary>
    /// <param name="view">Handle to the view from which to obtain column information.</param>
    /// <param name="type">Specifies a flag indicating what type of information is needed.</param>
    /// <param name="record">Pointer to a handle to receive the column information data record.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiViewGetColumnInfo(int view,
        MsiColInfoType type, out int record);

    /// <summary>The <c>MsiViewGetError</c> function returns the error that occurred in the <see cref="MsiViewModify"/> function.</summary>
    /// <param name="view">Handle to the view.</param>
    /// <param name="columnNames">Pointer to the buffer that receives the null-terminated column name. Do not attempt to determine the size of the buffer by passing in a <c>null</c> for <c>columnNames</c>. You can get the size of the buffer by passing in an empty string (for example ""). The function will then return <see cref="MsiDbError.MoreData"/> and <c>columnNamesSize</c> will contain the required buffer size in TCHARs, not including the terminating null character. On return of <see cref="MsiDbError.NoError"/>, <c>columnNamesSize</c> contains the number of TCHARs written to the buffer, not including the terminating null character. This parameter is an empty string if there are no errors.</param>
    /// <param name="columnNamesSize">Pointer to the variable that specifies the size, in TCHARs, of the buffer pointed to by the variable <c>columnNames</c>. When the function returns <see cref="MsiDbError.NoError"/>, this variable contains the size of the data copied to <c>columnNames</c>, not including the terminating null character. If <c>columnNames</c> is not large enough, the function returns <see cref="MsiDbError.MoreData"/> and stores the required size, not including the terminating null character, in the variable pointed to by <c>columnNamesSize</c>.</param>
    /// <returns>The <see cref="MsiDbError"/></returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiDbError MsiViewGetError(int view,
        StringBuilder columnNames, ref uint columnNamesSize);

    /// <summary>The MsiViewModify function updates a fetched record.</summary>
    /// <param name="view">Handle to a view.</param>
    /// <param name="mode">Specifies the modify mode.</param>
    /// <param name="record">Handle to the record to modify.</param>
    /// <returns>
    /// <para><see cref="MsiError.Success"/>  The function succeeded.</para>
    /// <para><see cref="MsiError.FunctionFailed"/>  The function failed.</para>
    /// <para><see cref="MsiError.InvalidData"/>  A validation was requested and the data did not pass. For more information, call <see cref="MsiViewGetError"/>.</para>
    /// <para><see cref="MsiError.InvalidHandle"/>  An invalid or inactive handle was supplied.</para>
    /// <para><see cref="MsiError.InvalidHandleState"/>  The handle is in an invalid state.</para>
    /// <para><see cref="MsiError.InvalidParameter"/>  One of the parameters was invalid.</para>
    /// </returns>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    [DllImport(MSI_LIB, CharSet = CharSet.Auto)]
    public static extern MsiError MsiViewModify(int view, MsiModify mode,
        int record);

    #endregion	Database Functions
    #endregion	Interop Methods
}
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
/// Internal class containing constants for MSI installer properties.
/// This class cannot be inherited.
/// This class cannot be instantiated directly.
/// </summary>
public sealed class MsiInstallerProperty
{
    #region	Constants (Static Fields)
    #region	Component Location

    /// <summary>Initial action called after the installer is initialized.</summary>
    public const string ACTION = "ACTION";

    /// <summary>List of features (delimited by commas) to be installed in their default configuration.</summary>
    public const string ADDDEFAULT = "ADDDEFAULT";

    /// <summary>List of features (delimited by commas) to be installed locally.</summary>
    public const string ADDLOCAL = "ADDLOCAL";

    /// <summary>List of features (delimited by commas) to be run from source.</summary>
    public const string ADDSOURCE = "ADDSOURCE";

    /// <summary>List of properties (separated by semicolons) set during an administration installation.</summary>
    public const string AdminProperties = "AdminProperties";

    /// <summary>Full path to the directory containing administrative tools for an individual user.</summary>
    public const string AdminToolsFolder = "AdminToolsFolder";

    /// <summary>Set on Microsoft Windows NT/Windows 2000 if the user has administrator privileges.</summary>
    public const string AdminUser = "AdminUser";

    /// <summary>List of features (delimited by commas) to be advertised.</summary>
    public const string ADVERTISE = "ADVERTISE ";

    /// <summary>Indicates current installation follows a reboot invoked by the ForceReboot action.</summary>
    public const string AFTERREBOOT = "AFTERREBOOT";

    /// <summary>Determines where configuration information will be stored.</summary>
    public const string ALLUSERS = "ALLUSERS";

    /// <summary>Numeric processor level if running on an Alpha processor. </summary>
    public const string Alpha = "Alpha";

    /// <summary>Full path to the Application Data folder for the current user.</summary>
    public const string AppDataFolder = "AppDataFolder";

    /// <summary>URL of the update channel for the application.</summary>
    public const string ARPAUTHORIZEDCDFPREFIX = "ARPAUTHORIZEDCDFPREFIX";

    /// <summary>Provides Comments for the Add or Remove Programs on Control Panel.</summary>
    public const string ARPCOMMENTS = "ARPCOMMENTS";

    /// <summary>Provides Contact for the Add or Remove Programs on Control Panel.</summary>
    public const string ARPCONTACT = "ARPCONTACT";

    /// <summary>Internet address, or URL, for technical support.</summary>
    public const string ARPHELPLINK = "ARPHELPLINK";

    /// <summary>Technical support phone numbers.</summary>
    public const string ARPHELPTELEPHONE = "ARPHELPTELEPHONE";

    /// <summary>Fully qualified path to the application's primary folder.</summary>
    public const string ARPINSTALLLOCATION = "ARPINSTALLLOCATION";

    /// <summary>Disables functionality that would modify the product.</summary>
    public const string ARPNOMODIFY = "ARPNOMODIFY";

    /// <summary>Disables functionality that would remove the product.</summary>
    public const string ARPNOREMOVE = "ARPNOREMOVE";

    /// <summary>Disables the Repair button in the Programs wizard.</summary>
    public const string ARPNOREPAIR = "ARPNOREPAIR";

    /// <summary>Specifies the primary icon for the installation package.</summary>
    public const string ARPPRODUCTICON = "ARPPRODUCTICON";

    /// <summary>Provides a ReadMe for the Add or Remove Programs on Control Panel.</summary>
    public const string ARPREADME = "ARPREADME";

    /// <summary>Estimated size of the application in kilobytes.</summary>
    public const string ARPSIZE = "ARPSIZE";

    /// <summary>Prevents display of application in the Add or Remove Programs list.</summary>
    public const string ARPSYSTEMCOMPONENT = "ARPSYSTEMCOMPONENT";

    /// <summary>URL for an application's home page.</summary>
    public const string ARPURLINFOABOUT = "ARPURLINFOABOUT";

    /// <summary>URL for application-update information.</summary>
    public const string ARPURLUPDATEINFO = "ARPURLUPDATEINFO";

    /// <summary>AssignmentType</summary>
    public const string AssignmentType = "AssignmentType";

    /// <summary>Registry space in xilobytes required by the application. Used by AllocateRegistrySpace action.</summary>
    public const string AVAILABLEFREEREG = "AVAILABLEFREEREG";

    /// <summary>The width, in pixels, of the window borders.</summary>
    public const string BorderSide = "BorderSide";

    /// <summary>The height, in pixels, of the window borders.</summary>
    public const string BorderTop = "BorderTop";

    /// <summary>Height, in pixels, of normal caption area.</summary>
    public const string CaptionHeight = "CaptionHeight";

    /// <summary>The root path for any of the qualifying products for CCP.</summary>
    public const string CCP_DRIVE = "CCP_DRIVE";

    /// <summary>Number of adjacent color bits for each pixel.</summary>
    public const string ColorBits = "ColorBits";

    /// <summary>Full path to application data for all users.</summary>
    public const string CommonAppDataFolder = "CommonAppDataFolder";

    /// <summary>Full path to the predefined 64-bit Common Files folder.</summary>
    public const string CommonFiles64Folder = "CommonFiles64Folder";

    /// <summary>Full path to the Common Files folder for the current user.</summary>
    public const string CommonFilesFolder = "CommonFilesFolder";

    /// <summary>List of component IDs (delimited by commas) to be installed locally.</summary>
    public const string COMPADDLOCAL = "COMPADDLOCAL ";

    /// <summary>List of component IDs (delimited by commas) to run from source media.</summary>
    public const string COMPADDSOURCE = "COMPADDSOURCE ";

    /// <summary>Organization of user performing the installation.</summary>
    public const string COMPANYNAME = "COMPANYNAME";

    /// <summary>Computer name of the current system.</summary>
    public const string ComputerName = "ComputerName";

    /// <summary>Indicates whether disk space costing has completed.</summary>
    public const string CostingComplete = "CostingComplete";

    /// <summary>The current date.</summary>
    public const string Date = "Date";

    /// <summary>Default font style used for controls.</summary>
    public const string DefaultUIFont = "DefaultUIFont";

    /// <summary>Full path to the Desktop folder.</summary>
    public const string DesktopFolder = "DesktopFolder";

    /// <summary>Set to disable the generation certain shortcuts supporting installation-on-demand.</summary>
    public const string DISABLEADVTSHORTCUTS = "DISABLEADVTSHORTCUTS";

    /// <summary>Prevents the installer from registering media sources, such as a CD-ROMs, as valid sources for the product.</summary>
    public const string DISABLEMEDIA = "DISABLEMEDIA";

    /// <summary>Disables rollback for the current configuration.</summary>
    public const string DISABLEROLLBACK = "DISABLEROLLBACK";

    /// <summary>String displayed by a message box prompting for a disk.</summary>
    public const string DiskPrompt = "DiskPrompt";

    /// <summary>Top-level action initiated by the ExecuteAction action.</summary>
    public const string EXECUTEACTION = "EXECUTEACTION";

    /// <summary>Mode of execution performed by the installer.  (None, Script [default])</summary>
    public const string EXECUTEMODE = "EXECUTEMODE";

    /// <summary>Improves installation performance under specific OEM scenarios.</summary>
    public const string FASTOEM = "FASTOEM";

    /// <summary>Full path to the Favorites folder.</summary>
    public const string FavoritesFolder = "FavoritesFolder";

    /// <summary>List of file keys of files (delimited by commas) that are to be installed in their default configuration.</summary>
    public const string FILEADDDEFAULT = "FILEADDDEFAULT ";

    /// <summary>List of file keys of the files (delimited by commas) to be run locally.</summary>
    public const string FILEADDLOCAL = "FILEADDLOCAL ";

    /// <summary>List of file keys (delimited by commas)to be run from the source media.</summary>
    public const string FILEADDSOURCE = "FILEADDSOURCE ";

    /// <summary>Full path to the Fonts folder.</summary>
    public const string FontsFolder = "FontsFolder";

    /// <summary>HelpLink</summary>
    public const string HelpLink = "HelpLink";

    /// <summary>HelpTelephone</summary>
    public const string HelpTelephone = "HelpTelephone";

    /// <summary>InstallDate</summary>
    public const string InstallDate = "InstallDate";

    /// <summary>Indicates that a product is already installed.</summary>
    public const string Installed = "Installed";

    /// <summary>InstalledProductName</summary>
    public const string InstalledProductName = "InstalledProductName";

    /// <summary>Initial "level" at which features will be installed.  (1 - 32767)</summary>
    public const string INSTALLLEVEL = "INSTALLLEVEL";

    /// <summary>InstallLocation</summary>
    public const string InstallLocation = "InstallLocation";

    /// <summary>InstallSource</summary>
    public const string InstallSource = "InstallSource";

    /// <summary>InstanceType</summary>
    public const string InstanceType = "InstanceType";

    /// <summary>Numeric processor level if running on an Intel processor.</summary>
    public const string Intel = "Intel";

    /// <summary>Numeric processor level if running on an Itanium processor.</summary>
    public const string Intel64 = "Intel64";

    /// <summary>Set to 1 if the current installation is running from a package created through an administrative installation.</summary>
    public const string IsAdminPackage = "IsAdminPackage";

    /// <summary>Language</summary>
    public const string Language = "Language";

    /// <summary>Places units to the left of the number.</summary>
    public const string LeftUnit = "LeftUnit";

    /// <summary>UI level capped as Basic.</summary>
    public const string LIMITUI = "LIMITUI";

    /// <summary>Full path to directory that serves as a data repository for local (nonroaming) applications.</summary>
    public const string LocalAppDataFolder = "LocalAppDataFolder";

    /// <summary>LocalPackage</summary>
    public const string LocalPackage = "LocalPackage";

    /// <summary>List of action names that will be logged (separated only by semicolons and with no spaces.)</summary>
    public const string LOGACTION = "LOGACTION";

    /// <summary>User name for the user currently logged on.</summary>
    public const string LogonUser = "LogonUser";

    /// <summary>Name of the application's manufacturer. (Required.)</summary>
    public const string Manufacturer = "Manufacturer";

    /// <summary>This property must be set to the relative path if the installation package is not located at the root of the CD-ROM.</summary>
    public const string MEDIAPACKAGEPATH = "MEDIAPACKAGEPATH";

    /// <summary>The installer sets this property to 1 when the installation uses a media source, such as a CD-ROM.</summary>
    public const string MediaSourceDir = "MediaSourceDir";

    /// <summary>The installer does a CRC on files only if the MSICHECKCRCS property is set.</summary>
    public const string MSICHECKCRCS = "MSICHECKCRCS";

    /// <summary>List of properties (separated by semicolonsthat are prevented from being written into the log.</summary>
    public const string MsiHiddenProperties = "MsiHiddenProperties";

    /// <summary>The presence of this property indicates that a product code changing transform is registered to the product.</summary>
    public const string MSIINSTANCEGUID = "MSIINSTANCEGUID";

    /// <summary>On systems that support common language runtime assemblies, the installer sets the value of this property to the file version of fusion.dll. The installer does not set this property if the operating system does not support common language runtime assemblies. Only available with Windows Installer version 2.0 and later.</summary>
    public const string MsiNetAssemblySupport = "MsiNetAssemblySupport";

    /// <summary>This property indicates the installation of a new instance of a product with instance transforms.</summary>
    public const string MSINEWINSTANCE = "MSINEWINSTANCE";

    /// <summary>Set to prevent the installer from setting the DISABLEMEDIA property. Available with Windows Installer version 1.0.</summary>
    public const string MSINODISABLEMEDIA = "MSINODISABLEMEDIA";

    /// <summary>Indicates the Windows product type. Only available with Windows Installer version 2.0 and later versions.</summary>
    public const string MsiNTProductType = "MsiNTProductType";

    /// <summary>On Windows 2000 and later operating systems, the installer sets this property to 1 only if Microsoft BackOffice components are installed. Only available with Windows Installer version 2.0 and later versions.</summary>
    public const string MsiNTSuiteBackOffice = "MsiNTSuiteBackOffice";

    /// <summary>On Windows 2000 and later operating systems, the installer sets this property to 1 only if Windows 2000 DataCenter Server is installed. Only available with Windows Installer version 2.0 and later versions.</summary>
    public const string MsiNTSuiteDataCenter = "MsiNTSuiteDataCenter";

    /// <summary>On Windows 2000 and later operating systems, the installer sets this property to 1 only if Windows 2000 Advanced Server is installed. Only available with Windows Installer version 2.0 and later versions.</summary>
    public const string MsiNTSuiteEnterprise = "MsiNTSuiteEnterprise";

    /// <summary>On Windows 2000 and later operating systems, the installer sets this property to 1 only if the operating system is Workstation Personal (not Professional). Only available with Windows Installer version 2.0 and later versions.</summary>
    public const string MsiNTSuitePersonal = "MsiNTSuitePersonal";

    /// <summary>On Windows 2000 and later operating systems, the installer sets this property to 1 only if Microsoft Small Business Server is installed. Only available with Windows Installer version 2.0 and later versions.</summary>
    public const string MsiNTSuiteSmallBusiness = "MsiNTSuiteSmallBusiness";

    /// <summary>On Windows 2000 and later operating systems, the installer sets this property to 1 only if Microsoft Small Business Server is installed with the restrictive client license. Only available with Windows Installer version 2.0 and later.</summary>
    public const string MsiNTSuiteSmallBusinessRestricted = "MsiNTSuiteSmallBusinessRestricted";

    /// <summary>On Windows 2000 and later operating systems, the installer sets the MsiNTSuiteWebServer property to 1 if the web edition of the Windows  2003 Server family is installed. Only available with the Windows Server 2003 family release of the Windows Installer.</summary>
    public const string MsiNTSuiteWebServer = "MsiNTSuiteWebServer";

    /// <summary>On systems that support Win32 assemblies, the installer sets the value of this property to the file version of sxs.dll. The installer does not set this property if the operating system does not support Win32 assemblies. Only available with Windows Installer version 2.0 and later.</summary>
    public const string MsiWin32AssemblySupport = "MsiWin32AssemblySupport";

    /// <summary>Full path to the My Pictures folder.</summary>
    public const string MyPicturesFolder = "MyPicturesFolder";

    /// <summary>Suppresses the automatic setting of the COMPANYNAME property.</summary>
    public const string NOCOMPANYNAME = "NOCOMPANYNAME";

    /// <summary>Suppresses the automatic setting of the USERNAME property.</summary>
    public const string NOUSERNAME = "NOUSERNAME";

    /// <summary>Set if OLE supports the Windows Installer.</summary>
    public const string OLEAdvtSupport = "OLEAdvtSupport";

    /// <summary>The installer sets the OriginalDatabase property to the launched-from database, the database on the source, or the cached database.</summary>
    public const string OriginalDatabase = "OriginalDatabase";

    /// <summary>Insufficient disk space to accommodate the installation.</summary>
    public const string OutOfDiskSpace = "OutOfDiskSpace";

    /// <summary>Insufficient disk space with rollback turned off.</summary>
    public const string OutOfNoRbDiskSpace = "OutOfNoRbDiskSpace";

    /// <summary>PackageCode</summary>
    public const string PackageCode = "PackageCode";

    /// <summary>Setting this property applies a patch.</summary>
    public const string PATCH = "PATCH";

    /// <summary>The value of this property is written to the Revision Number Summary Property.</summary>
    public const string PATCHNEWPACKAGECODE = "PATCHNEWPACKAGECODE";

    /// <summary>The value of this property is written to the Comments Summary Property.</summary>
    public const string PATCHNEWSUMMARYCOMMENTS = "PATCHNEWSUMMARYCOMMENTS";

    /// <summary>The value of this property is written to the Subject Summary Property.</summary>
    public const string PATCHNEWSUMMARYSUBJECT = "PATCHNEWSUMMARYSUBJECT";

    /// <summary>Full path to the Personal folder for the current user.</summary>
    public const string PersonalFolder = "PersonalFolder";

    /// <summary>Size of the installed RAM in megabytes.</summary>
    public const string PhysicalMemory = "PhysicalMemory";

    /// <summary>Part of the Product ID entered by user.</summary>
    public const string PIDKEY = "PIDKEY";

    /// <summary>String used as a template for the PIDKEY property.</summary>
    public const string PIDTemplate = "PIDTemplate";

    /// <summary>Features are already selected.</summary>
    public const string Preselected = "Preselected";

    /// <summary>Allows the author to designate a "primary" folder for the installation. Used to determine the values for the PrimaryVolumePath, PrimaryVolumeSpaceAvailable, PrimaryVolumeSpaceRequired, and PrimaryVolumeSpaceRemaining properties.</summary>
    public const string PRIMARYFOLDER = "PRIMARYFOLDER";

    /// <summary>The Installer sets the value of this property to the path of the volume designated by the PRIMARYFOLDER property.</summary>
    public const string PrimaryVolumePath = "PrimaryVolumePath";

    /// <summary>The Installer sets the value of this property to a string representing the total number of bytes available on the volume referenced by the PrimaryVolumePath property.</summary>
    public const string PrimaryVolumeSpaceAvailable = "PrimaryVolumeSpaceAvailable";

    /// <summary>The Installer sets the value of this property to a string representing the total number of bytes remaining on the volume referenced by the PrimaryVolumePath property if all the currently selected features were installed.</summary>
    public const string PrimaryVolumeSpaceRemaining = "PrimaryVolumeSpaceRemaining";

    /// <summary>The Installer sets the value of this property to a string representing the total number of bytes required by all currently selected features on the volume referenced by the PrimaryVolumePath property.</summary>
    public const string PrimaryVolumeSpaceRequired = "PrimaryVolumeSpaceRequired";

    /// <summary>Runs an installation with elevated privileges.</summary>
    public const string Privileged = "Privileged";

    /// <summary>A unique identifier for the particular product release. (Required.)</summary>
    public const string ProductCode = "ProductCode";

    /// <summary>ProductIcon</summary>
    public const string ProductIcon = "ProductIcon";

    /// <summary>Full Product ID after a successful validation.</summary>
    public const string ProductID = "ProductID";

    /// <summary>Numeric language identifier (LANGID) for the database. (REQUIRED)</summary>
    public const string ProductLanguage = "ProductLanguage";

    /// <summary>Human-readable name of the application. (Required.)</summary>
    public const string ProductName = "ProductName";

    /// <summary>Set to the installed state of a product.  (-1 unknown, 1 advertised, 2 absent, 5 default)</summary>
    public const string ProductState = "ProductState";

    /// <summary>String format of the product version as a numeric value. (Required.)</summary>
    public const string ProductVersion = "ProductVersion";

    /// <summary>Full path of the predefined 64-bit Program Files folder.</summary>
    public const string ProgramFiles64Folder = "ProgramFiles64Folder";

    /// <summary>Full path of the predefined 32-bit Program Files folder.</summary>
    public const string ProgramFilesFolder = "ProgramFilesFolder";

    /// <summary>Full path to the Program Menu folder.</summary>
    public const string ProgramMenuFolder = "ProgramMenuFolder";

    /// <summary>Action if there is insufficient disk space for the installation.  (P - prompt, D - disable, F - fail)</summary>
    public const string PROMPTROLLBACKCOST = "PROMPTROLLBACKCOST";

    /// <summary>Publisher</summary>
    public const string Publisher = "Publisher";

    /// <summary>Forces or suppresses restarting.  (Force, Suppress, ReallySuppress)</summary>
    public const string REBOOT = "REBOOT";

    /// <summary>Suppresses the display of prompts for restarts to the user. Any restarts that are needed happen automatically.  (S or Suppress)</summary>
    public const string REBOOTPROMPT = "REBOOTPROMPT";

    /// <summary>The installer sets the RedirectedDLLSupport property if the system performing the installation supports Isolated Components.</summary>
    public const string RedirectedDLLSupport = "RedirectedDLLSupport";

    /// <summary>List of features (delimited by commas) to be reinstalled.</summary>
    public const string REINSTALL = "REINSTALL";

    /// <summary>A string containing letters that specify the type of reinstall to perform.</summary>
    public const string REINSTALLMODE = "REINSTALLMODE";

    /// <summary>The installer sets the RemoteAdminTS property when the system is a remote administration server using Terminal Services.</summary>
    public const string RemoteAdminTS = "RemoteAdminTS";

    /// <summary>List of features (delimited by commas) to be removed.</summary>
    public const string REMOVE = "REMOVE";

    /// <summary>Set if the installer installs over a file that is being held in use.</summary>
    public const string ReplacedInUseFiles = "ReplacedInUseFiles";

    /// <summary>Resumed installation.</summary>
    public const string RESUME = "RESUME";

    /// <summary>The installer sets this property whenever rollback is disabled.</summary>
    public const string RollbackDisabled = "RollbackDisabled";

    /// <summary>Default drive for the installation.  (Must end in '\')</summary>
    public const string ROOTDRIVE = "ROOTDRIVE";

    /// <summary>Width, in pixels, of the screen.</summary>
    public const string ScreenX = "ScreenX";

    /// <summary>Height, in pixels, of the screen.</summary>
    public const string ScreenY = "ScreenY";

    /// <summary>Full path to the SendTo folder for the current user.</summary>
    public const string SendToFolder = "SendToFolder";

    /// <summary>A table having the sequence table schema.</summary>
    public const string SEQUENCE = "SEQUENCE";

    /// <summary>The version number of the operating system service pack.</summary>
    public const string ServicePackLevel = "ServicePackLevel";

    /// <summary>The minor version number of the operating system service pack.</summary>
    public const string ServicePackLevelMinor = "ServicePackLevelMinor";

    /// <summary>Set when the system is operating as Shared Windows.</summary>
    public const string SharedWindows = "SharedWindows";

    /// <summary>Set if the shell supports feature advertising.</summary>
    public const string ShellAdvtSupport = "ShellAdvtSupport";

    /// <summary>Causes short file names to be used.</summary>
    public const string SHORTFILENAMES = "SHORTFILENAMES";

    /// <summary>Root directory containing the source files.</summary>
    public const string SourceDir = "SourceDir";

    /// <summary>Full path to the Start Menu folder.</summary>
    public const string StartMenuFolder = "StartMenuFolder";

    /// <summary>Full path to the Startup folder.</summary>
    public const string StartupFolder = "StartupFolder";

    /// <summary>Full path to folder for 16-bit system DLLs.</summary>
    public const string System16Folder = "System16Folder";

    /// <summary>Full path to folder for 64-bit system DLLs.</summary>
    public const string System64Folder = "System64Folder";

    /// <summary>Full path to folder for 32-bit system DLLs.</summary>
    public const string SystemFolder = "SystemFolder";

    /// <summary>Default language identifier for the system.</summary>
    public const string SystemLanguageID = "SystemLanguageID";

    /// <summary>Specifies the root destination directory for the installation. During an administrative installation this property is the location to copy the installation package.</summary>
    public const string TARGETDIR = "TARGETDIR";

    #endregion	Component Location

    #region	Configuration Properties

    /// <summary>Full path to the Temp folder.</summary>
    public const string TempFolder = "TempFolder";

    /// <summary>Full path to the Template folder for the current user.</summary>
    public const string TemplateFolder = "TemplateFolder";

    /// <summary>Set when the system is a server with Windows Terminal Server.</summary>
    public const string TerminalServer = "TerminalServer";

    /// <summary>The height of characters in logical units.</summary>
    public const string TextHeight = "TextHeight";

    /// <summary>The current time.</summary>
    public const string Time = "Time";

    /// <summary>List of transforms to be applied to the database.</summary>
    public const string Transforms = "Transforms";

    /// <summary>Informs the installer that the transforms for the product reside at the source.</summary>
    public const string TRANSFORMSATSOURCE = "TRANSFORMSATSOURCE";

    /// <summary>Setting the TRANSFORMSECURE property to 1 informs the installer that transforms are to be cached locally on the user's computer in a location where the user does not have write access.</summary>
    public const string TRANSFORMSSECURE = "TRANSFORMSSECURE";

    #endregion	Configuration Properties

    #region	Hardware

    /// <summary>Indicates if the operating system supports using .TTC (true type font collections) files.</summary>
    public const string TTCSupport = "TTCSupport";

    /// <summary>Indicates the user interface level.</summary>
    public const string UILevel = "UILevel";

    /// <summary>Set when changes to the system have begun for this installation.</summary>
    public const string UpdateStarted = "UpdateStarted";

    /// <summary>A GUID representing a related set of products.</summary>
    public const string UpgradeCode = "UpgradeCode";

    /// <summary>Set by the installer when an upgrade removes an application. Available with Windows Installer version 1.1 or later.</summary>
    public const string UPGRADINGPRODUCTCODE = "UPGRADINGPRODUCTCODE";

    /// <summary>URLInfoAbout</summary>
    public const string URLInfoAbout = "URLInfoAbout";

    /// <summary>URLUpdateInfo</summary>
    public const string URLUpdateInfo = "URLUpdateInfo";

    /// <summary>Default language identifier of the current user.</summary>
    public const string UserLanguageID = "UserLanguageID";

    /// <summary>User performing the installation.</summary>
    public const string USERNAME = "USERNAME";

    /// <summary>Set by the installer to the user's security identifier (SID).</summary>
    public const string UserSID = "UserSID";

    /// <summary>Version</summary>
    public const string Version = "Version";

    /// <summary>Version number for the Windows operating system.</summary>
    public const string Version9X = "Version9X";

    /// <summary>Numeric database version of the current installation.</summary>
    public const string VersionDatabase = "VersionDatabase";

    /// <summary>VersionMajor</summary>
    public const string VersionMajor = "VersionMajor";

    /// <summary>VersionMinor</summary>
    public const string VersionMinor = "VersionMinor";

    /// <summary>The installer sets this property to the version of Windows Installer run during the installation.</summary>
    public const string VersionMsi = "VersionMsi";

    /// <summary>Version number for the Windows NT/Windows 2000 operating system.</summary>
    public const string VersionNT = "VersionNT";

    /// <summary>Version number for the Windows NT/Windows 2000 operating system if the system is running on a 64-bit computer.</summary>
    public const string VersionNT64 = "VersionNT64";

    /// <summary>VersionString</summary>
    public const string VersionString = "VersionString";

    /// <summary>Amount of available page file space in megabytes.</summary>
    public const string VirtualMemory = "VirtualMemory";

    #endregion	Hardware

    #region	Operating System

    /// <summary>Build number of the operating system.</summary>
    public const string WindowsBuild = "WindowsBuild";

    #endregion	Operating System

    #region	System Folders

    /// <summary>Full path to the Windows folder.</summary>
    public const string WindowsFolder = "WindowsFolder";

    /// <summary>The volume of the Windows folder.</summary>
    public const string WindowsVolume = "WindowsVolume";

    #endregion	System Folders

    #endregion	Constants (Static Fields)

    #region	Construction / Destruction

    private MsiInstallerProperty()
    { }

    #endregion	Construction / Destruction
}
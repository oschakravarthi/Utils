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
/// Internal class containing constants for an MSI database.
/// This class cannot be inherited.
/// This class cannot be instantiated directly.
/// </summary>
internal sealed class MsiDatabaseTable
{
    #region	Constants

    /// <summary>The _Columns table is a read-only system table that contains the column catalog. It lists the columns for all the tables. You can query this table to find out if a given column exists.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string _Columns = "_Columns";

    /// <summary>The _Storages table lists embedded OLE data storages. This is a temporary table, created only when referenced by a SQL statement.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string _Storages = "_Storages";

    /// <summary>The _Streams table lists embedded OLE data streams. This is a temporary table, created only when referenced by a SQL statement.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string _Streams = "_Streams";

    /// <summary>The _Tables table is a read-only system table that lists all the tables in the database. Query this table to find out if a table exists.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string _Tables = "_Tables";

    /// <summary>This is a read-only temporary table used to view transforms with the transform view mode. This table is never persisted by the installer.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string _TransformView = "_TransformView";

    /// <summary>The _Validation table is a system table that contains the column names and the column values for all of the tables in the database. It is used during the database validation process to ensure that all columns are accounted for and have the correct values. This table is not shipped with the installer database.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string _Validation = "_Validation";

    /// <summary>The ActionText table contains text to be displayed in a progress dialog box and written to the log for actions that take a long time to execute. The text displayed consists of the action description and optionally formatted data from the action.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ActionText = "ActionText";

    /// <summary>The AdminExecuteSequence table lists actions that the installer calls in sequence when the top-level ADMIN action is executed.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string AdminExecuteSequence = "AdminExecuteSequence";

    /// <summary>The AdminUISequence table lists actions that the installer calls in sequence when the top-level ADMIN action is executed and the internal user interface level is set to full UI or reduced UI. The installer skips the actions in this table if the user interface level is set to basic UI or no UI.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string AdminUISequence = "AdminUISequence";

    /// <summary>The AdvtExecuteSequence table lists actions the installer calls when the top-level ADVERTISE action is executed.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string AdvtExecuteSequence = "AdvtExecuteSequence";

    /// <summary>The installer does not use this table. The AdvtUISequence table should not exist in the installation database or it should be left empty.</summary>
    public const string AdvtUISequence = "AdvtUISequence";

    /// <summary>The AppId table or the <see cref="Registry"/> table specifies that the installer configure and register DCOM servers to do one of the following during an installation.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string AppId = "AppId";

    /// <summary>The AppSearch table contains properties needed to search for a file having a particular file signature. The AppSearch table can also be used to set a property to the existing value of a registry or .ini file entry.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string AppSearch = "AppSearch";

    /// <summary>The BBControl table lists the controls to be displayed on each billboard.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string BBControl = "BBControl";

    /// <summary>The Billboard table lists the Billboard controls displayed in the full user interface.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Billboard = "Billboard";

    /// <summary>The Binary table holds the binary data for items such as bitmaps, animations, and icons. The binary table is also used to store data for custom actions.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Binary = "Binary";

    /// <summary>The BindImage table contains information about each executable or DLL that needs to be bound to the DLLs imported by it.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string BindImage = "BindImage";

    /// <summary>The CCPSearch table contains the list of file signatures used for the Compliance Checking Program (CCP). At least one of these files needs to be present on a user's computer for the user to be in compliance with the program.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string CCPSearch = "CCPSearch";

    /// <summary>The CheckBox table lists the values for the check boxes.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string CheckBox = "CheckBox";

    /// <summary>The Class table contains COM server-related information that must be generated as a part of the product advertisement. Each row may generate a set of registry keys and values. The associated ProgId information is included in this table.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Class = "Class";

    /// <summary>The lines of a combo box are not treated as individual controls; they are part of a single combo box that functions as a control. This table lists the values for each combo box.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ComboBox = "ComboBox";

    /// <summary>The CompLocator table holds the information needed to find a file or a directory using the installer configuration data.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string CompLocator = "CompLocator";

    /// <summary>The Complus table contains information needed to install COM+ applications.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Complus = "Complus";

    /// <summary>The Component table lists components.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Component = "Component";

    /// <summary>The Condition table can be used to modify the selection state of any entry in the <see cref="Feature"/> table based on a conditional expression.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Condition = "Condition";

    /// <summary>The Control table defines the controls that appear on each dialog box.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Control = "Control";

    /// <summary>The ControlCondition table enables an author to specify special actions to be applied to controls based on the result of a conditional statement. For example, using this table the author could choose to hide a control based on the VersionNT property.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ControlCondition = "ControlCondition";

    /// <summary>The ControlEvent table allows the author to specify the Control Events started when a user interacts with a PushButton Control, CheckBox Control, or SelectionTree Control. These are the only controls users can use to initiate control events. Each control can publish multiple control events.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ControlEvent = "ControlEvent";

    /// <summary>The CreateFolder table contains references to folders that need to be created explicitly for a particular component.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string CreateFolder = "CreateFolder";

    /// <summary>The CustomAction table provides the means of integrating custom code and data into the installation. The source of the code that is executed can be a stream contained within the database, a recently installed file, or an existing executable file.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string CustomAction = "CustomAction";

    /// <summary>The Dialog table contains all the dialogs that appear in the user interface in both the full and reduced modes.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Dialog = "Dialog";

    /// <summary>The Directory table specifies the directory layout for the product. Each row of the table indicates a directory both at the source and the target.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Directory = "Directory";

    /// <summary>The DrLocator table holds the information needed to find a file or directory by searching the directory tree.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string DrLocator = "DrLocator";

    /// <summary>The DuplicateFile table contains a list of files that are to be duplicated, either to a different directory than the original file or to the same directory but with a different name. The original file must be a file installed by the InstallFiles action.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string DuplicateFile = "DuplicateFile";

    /// <summary>The Environment table is used to set the values of environment variables.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Environment = "Environment";

    /// <summary>The Error table is used to look up error message formatting templates when processing errors with an error code set but without a formatting template set (this is the normal situation).</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Error = "Error";

    /// <summary>The EventMapping table lists the controls that subscribe to some control event and lists the attribute to be changed when the event is published by another control or the installer.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string EventMapping = "EventMapping";

    /// <summary>The Extension table contains information about file name extension servers that must be generated as a part of product advertisement. Each row generates a set of registry keys and values.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Extension = "Extension";

    /// <summary>The Feature table defines the logical tree structure of features.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Feature = "Feature";

    /// <summary>The FeatureComponents table defines the relationship between features and components. For each feature, this table lists all the components that make up that feature.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string FeatureComponents = "FeatureComponents";

    /// <summary>The File table contains a complete list of source files with their various attributes, ordered by a unique, non-localized, identifier. Files can be stored on the source media as individual files or compressed within a cabinet file.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string File = "File";

    /// <summary>The FileSFPCatalog table associates specified files with the catalog files used by Windows Millennium Edition for Windows File Protection.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string FileSFPCatalog = "FileSFPCatalog";

    /// <summary>The Font table contains the information for registering font files with the system.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Font = "Font";

    /// <summary>This table contains the icon files. Each icon from the table is copied to a file as a part of product advertisement to be used for advertised shortcuts and OLE servers.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Icon = "Icon";

    /// <summary>The IniFile table contains the .ini information that the application needs to set in an .ini file.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string IniFile = "IniFile";

    /// <summary>The IniLocator table holds the information needed to search for a file or directory using an .ini file or to search for a particular .ini entry itself. The .ini file must be present in the default Microsoft Windows directory.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string IniLocator = "IniLocator";

    /// <summary>The InstallExecuteSequence table lists actions that are executed when the top-level INSTALL action is executed.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string InstallExecuteSequence = "InstallExecuteSequence";

    /// <summary>The InstallUISequence table lists actions that are executed when the top-level INSTALL action is executed and the internal user interface level is set to full UI or reduced UI. The installer skips the actions in this table if the user interface level is set to basic UI or no UI.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string InstallUISequence = "InstallUISequence";

    /// <summary>Each record of the IsolatedComponent table associates the component specified in the Component_Application column (commonly an .exe) with the component specified in the Component_Shared column (commonly a shared DLL). The IsolateComponents action installs a copy of Component_Shared into a private location for use by Component_Application. This isolates the Component_Application from other copies of Component_Shared that may be installed to a shared location on the computer.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string IsolatedComponent = "IsolatedComponent";

    /// <summary>The LaunchCondition table is used by the LaunchConditions action. It contains a list of conditions that all must be satisfied for the installation to begin.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string LaunchCondition = "LaunchCondition";

    /// <summary>The lines of a list box are not treated as individual controls, but they are part of a list box that functions as a control. The ListBox table defines the values for all list boxes.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ListBox = "ListBox";

    /// <summary>The lines of a listview are not treated as individual controls, but they are part of a listview that functions as a control. The ListView table defines the values for all listviews.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ListView = "ListView";

    /// <summary>The LockPermissions table is used to secure individual portions of your application in a locked-down environment. It can be used with the installation of files, registry keys, and created folders.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string LockPermissions = "LockPermissions";

    /// <summary>The Media table describes the set of disks that make up the source media for the installation.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Media = "Media";

    /// <summary>The MIME table associates a MIME content type with a file extension or a CLSID to generate the extension or COM server information required for advertisement of the MIME (Multipurpose Internet Mail Extensions) content.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string MIME = "MIME";

    /// <summary>For merge modules, if a table in the merge module is listed in the ModuleIgnoreTable table, it is not merged into the .msi file. If the table already exists in the .msi file, it is not modified by the merge. The tables in the ModuleIgnoreTable can therefore contain data that is unneeded after the merge.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ModeleIgnore = "ModeleIgnore";

    /// <summary>For merge modules, a merge tool evaluates the ModuleAdminExecuteSequence table and then inserts the calculated actions into the <see cref="AdminExecuteSequence"/> table with a correct sequence number.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ModuleAdminExecuteSequence = "ModuleAdminExecuteSequence";

    /// <summary>For merge modules, a merge tool evaluates the ModuleAdminUISequence table and then inserts the calculated actions into the <see cref="AdminUISequence"/> table with a correct sequence number.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ModuleAdminUISequence = "ModuleAdminUISequence";

    /// <summary>For merge modules, a merge tool evaluates the ModuleAdvtExecuteSequence table and then inserts the calculated actions into the <see cref="AdvtExecuteSequence"/> table with a correct sequence number.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ModuleAdvtExecuteSequence = "ModuleAdvtExecuteSequence";

    /// <summary>For merge modules, the ModuleComponents table contains a list of the components found in the merge module.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ModuleComponents = "ModuleComponents";

    /// <summary>For merge modules, the ModuleConfiguration table identifies the configurable attributes of the module. This table is not merged into the database.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ModuleConfiguration = "ModuleConfiguration";

    /// <summary>For merge modules, the ModuleDependency table keeps a list of other merge modules that are required for this merge module to operate properly. This table enables a merge or verification tool to ensure that the necessary merge modules are in fact included in the user's installer database. The tool checks by cross referencing this table with the <see cref="ModuleSignature"/> table in the installer database.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ModuleDependency = "ModuleDependency";

    /// <summary>For merge modules, the ModuleExclusion table keeps a list of other merge modules that are incompatible in the same installer database. This table enables a merge or verification tool to check that conflicting merge modules are not merged in the user's installer database. The tool checks by cross-referencing this table with the <see cref="ModuleSignature"/> table in the installer database.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ModuleExclusion = "ModuleExclusion";

    /// <summary>For merge modules, a merge tool evaluates the ModuleInstallExecuteSequence table and then inserts the calculated actions into the <see cref="InstallExecuteSequence"/> table with a correct sequence number.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ModuleInstallExecuteSequence = "ModuleInstallExecuteSequence";

    /// <summary>For merge modules, a merge tool evaluates the ModuleInstallUISequence table and then inserts the calculated actions into the <see cref="InstallUISequence"/> table with a correct sequence number.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ModuleInstallUISequence = "ModuleInstallUISequence";

    /// <summary>For merge modules, the ModuleSignature Table is a required table. It contains all the information necessary to identify a merge module. The merge tool adds this table to the .msi file if one does not already exist. The ModuleSignature table in a merge module has only one row containing the ModuleID, Language, and Version. However, the ModuleSignature table in an .msi file has a row containing this information for each .msm file that has been merged into it.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ModuleSignature = "ModuleSignature";

    /// <summary>For merge modules, the ModuleSubstitution table specifies the configurable fields of a module database and provides a template for the configuration of each field. The user or merge tool may query this table to determine what configuration operations are to take place. This table is not merged into the target database.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ModuleSubstitution = "ModuleSubstitution";

    /// <summary>This table contains a list of files to be moved or copied from a specified source directory to a specified destination directory.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string MoveFile = "MoveFile";

    /// <summary>The MsiAssembly table specifies Windows Installer settings for Microsoft .NET Framework assemblies and Win32 assemblies.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string MsiAssembly = "MsiAssembly";

    /// <summary>The MsiAssembly table and MsiAssemblyName table specify Windows Installer settings for common language runtime assemblies and Win32 assemblies.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string MsiAssemblyName = "MsiAssemblyName";

    /// <summary>The MsiDigitalCertificate table stores certificates in binary stream format and associates each certificate with a primary key. The primary key is used to share certificates among multiple digitally signed objects. A digital certificate is a credential that provides a means to verify identity.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string MsiDigitalCertificate = "MsiDigitalCertificate";

    /// <summary>The MsiDigitalSignature table contains the signature information for every digitally signed object in the installation database.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string MsiDigitalSignature = "MsiDigitalSignature";

    /// <summary>The MsiFileHash table is used to store a 128-bit hash of a source file provided by the Windows Installer package. The hash is split into four 32-bit values and stored in separate columns of the table.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string MsiFileHash = "MsiFileHash";

    /// <summary>The MsiPatchHeaders table holds the binary patch header streams used for patch validation. A patch containing a populated MsiPatchHeaders table can only be applied using Windows Installer version 2.0 or later.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string MsiPatchHeaders = "MsiPatchHeaders";

    /// <summary>The ODBCAttribute table contains information about the attributes of ODBC drivers and translators.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ODBCAttribute = "ODBCAttribute";

    /// <summary>The ODBCDataSource table lists the data sources belonging to the installation.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ODBCDataSource = "ODBCDataSource";

    /// <summary>The ODBCDriver table lists the ODBC drivers belonging to the installation.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ODBCDriver = "ODBCDriver";

    /// <summary>The ODBCSourceAttribute table contains information about the attributes of data sources.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ODBCSourceAttribute = "ODBCSourceAttribute";

    /// <summary>The ODBCTranslator table lists the ODBC translators belonging to the installation.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ODBCTranslator = "ODBCTranslator";

    /// <summary>The Patch table specifies the file that is to receive a particular patch and the physical location of the patch files on the media images.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Patch = "Patch";

    /// <summary>The PatchPackage table describes all patch packages that have been applied to this product. For each patch package, the unique identifier for the patch is provided along with information about the media image the on which the patch is located.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string PatchPackage = "PatchPackage";

    /// <summary>The ProgId table contains information for program IDs and version independent program IDs that must be generated as a part of the product advertisement.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ProgId = "ProgId";

    /// <summary>The Property table contains the property names and values for all defined properties in the installation. Properties with Null values are not present in the table.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Property = "Property";

    /// <summary>The PublishComponent table associates components listed in the <see cref="Component"/> table with a qualifier text-string and a category ID GUID. Components with parallel functionality that have been grouped together in this way are referred to as qualified components.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string PublishComponent = "PublishComponent";

    /// <summary>Radio buttons are not treated as individual controls, but they are part of a radio button group that functions as a RadioButtonGroup control. The RadioButton table lists the buttons for all the groups.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string RadioButton = "RadioButton";

    /// <summary>The Registry table holds the registry information that the application needs to set in the system registry.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Registry = "Registry";

    /// <summary>The RegLocator table holds the information needed to search for a file or directory using the registry, or to search for a particular registry entry itself.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string RegLocator = "RegLocator";

    /// <summary>The RemoveFile table contains a list of files to be removed by the RemoveFiles action. Setting the FileName column of this table to Null supports the removal of empty folders.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string RemoveFile = "RemoveFile";

    /// <summary>The RemoveIniFile table contains the information an application needs to delete from a .ini file.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string RemoveIniFile = "RemoveIniFile";

    /// <summary>The RemoveRegistry table contains the registry information the application needs to delete from the system registry.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string RemoveRegistry = "RemoveRegistry";

    /// <summary>The ReserveCost table is an optional table that allows the author to reserve an amount of disk space in any directory that depends on the installation state of a component.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ReserveCost = "ReserveCost";

    /// <summary>The SelfReg table contains information about modules that need to be self registered. The installer calls the DllRegisterServer function during installation of the module; it calls DllUnregisterServer during uninstallation of the module. The installer does not self register EXE files.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string SelfReg = "SelfReg";

    /// <summary>The ServiceControl table is used to control installed or uninstalled services.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ServiceControl = "ServiceControl";

    /// <summary>The ServiceInstall table is used to install a service.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string ServiceInstall = "ServiceInstall";

    /// <summary>The SFPCatalog table contains the catalogs used by Windows Millennium Edition for Windows File Protection on Windows Millennium Edition.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string SFPCatalog = "SFPCatalog";

    /// <summary>The Shortcut table holds the information the application needs to create shortcuts on the user's computer.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Shortcut = "Shortcut";

    /// <summary>The Signature table holds the information that uniquely identifies a file signature.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Signature = "Signature";

    /// <summary>The TextStyle table lists different font styles used in controls having text.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string TextStyle = "TextStyle";

    /// <summary>The TypeLib table contains the information that needs to be placed in the registry registration of type libraries.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string TypeLib = "TypeLib";

    /// <summary>The UIText table contains the localized versions of some of the strings used in the user interface. These strings are not part of any other table. The UIText table is for strings that have no logical place in any other table.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string UIText = "UIText";

    /// <summary>The Upgrade table contains information required during major upgrades. To fully enable the installer's upgrade capabilities, every package should have an UpgradeCode property and an Upgrade table. Each record in the Upgrade table gives a characteristic combination of upgrade code, product version, and language information used to identify a set of products affected by the upgrade.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Upgrade = "Upgrade";

    /// <summary>The Verb table contains command-verb information associated with file extensions that must be generated as a part of product advertisement. Each row generates a set of registry keys and values.</summary>
    /// <remarks>Please refer to the MSDN documentation for more information.</remarks>
    public const string Verb = "Verb";

    #endregion	Constants

    #region	Construction / Destruction

    private MsiDatabaseTable()
    { }

    #endregion	Construction / Destruction
}
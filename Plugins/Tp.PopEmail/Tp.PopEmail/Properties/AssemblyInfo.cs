//
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System.Reflection;
using System.Runtime.InteropServices;
using Tp.Integration.Plugin.Common;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("Tp.PopEmailIntegration")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("Tp.PopEmailIntegration")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid("3e52b4ec-7caf-4b8d-b40f-9d544f67269c")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("3.8.9.26184")]

[assembly: AssemblyVersion("3.8.9.26184")]
[assembly: AssemblyFileVersion("3.8.9.26184")]
[assembly:
	PluginAssembly("Project Email Integration",
		"Retrieves emails from your mail account into internal Inbox and creates requests from emails.", "Email Integration",
		"email-integration-icon.png")]

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Tp.Integration.Plugin.Common;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Tp.Integration.Plugin.Bugzilla")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("Tp.Integration.Plugin.Bugzilla")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("bba8bd9f-b602-49c6-866c-799522ba9a63")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("3.7.0.16545")]
[assembly: AssemblyVersion("3.7.0.16545")]
[assembly: AssemblyFileVersion("3.7.0.16545")]
[assembly: PluginAssembly("Bugzilla", "Imports bugs from Bugzilla to TargetProcess in real time. Sends the updated states, comments, assignments back to Bugzilla.", "Bug Tracking", "bugzilla-icon.png")]

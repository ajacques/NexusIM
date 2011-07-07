using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("NexusIM")]
[assembly: AssemblyDescription("Connect to all your favorite IM and social networks all in one place.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Adren Software")]
[assembly: AssemblyProduct("NexusIM")]
[assembly: AssemblyCopyright("Copyright © Adam Jacques 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: InternalsVisibleTo("NexusIMTests, PublicKey=00240000048000009400000006020000002400005253413100040000010001000fee9dfcce51ef77b1aed6e564965eb071c252ce6874737b38729e0a78afb48644827a1d737f2c08a8160dae9d4f628f914084e95c865cbd6379af8996a1d0a17c8e4e87387ae535a1bdc5aa2dcd5a933b070a28cc6ab1a91d90eefdec7c2e499666fbc38a050ecf5bcc0ccd73eb8a9a79a4718399d9c4d58aa4755949f24995")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
	ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
	//(used if a resource is not found in the page, 
	// or application resource dictionaries)
	ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
	//(used if a resource is not found in the page, 
	// app, or any theme specific resource dictionaries)
)]


// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: CLSCompliant(true)]
[assembly: NeutralResourcesLanguageAttribute("en-US")]
[assembly: SecurityRules(SecurityRuleSet.Level2)]
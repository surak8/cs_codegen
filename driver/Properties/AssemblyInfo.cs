using System.Reflection;
using System.Runtime.InteropServices;

[assembly:AssemblyTitle("driver")]
[assembly:AssemblyProduct("driver")]
[assembly:AssemblyDescription("description of driver.")]
[assembly:AssemblyCompany("Colt Manufacturing Company LLC")]
[assembly:AssemblyCopyright("Copyright © 2018, Colt Manufacturing Company LLC")]
#if DEBUG
[assembly:AssemblyConfiguration("Debug version")]
#else
[assembly:AssemblyConfiguration("Release version")]
#endif
[assembly:ComVisible(false)]

[assembly:AssemblyVersion("1.0.0.0")]


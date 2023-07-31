using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyTitle("Marine")]
[assembly: AssemblyFileVersion("1.0.0")]
[assembly: AssemblyCompany(".grey#9120")]
[assembly: AssemblyProduct("Marine")]
[assembly: AssemblyTrademark(".grey#9120")]
[assembly: InternalsVisibleTo("PluginAPI")]
[assembly: Guid("4542d853-c48e-45eb-b4d9-76cdd36b25d1")]
[assembly: AssemblyDescription("Plugin for SCP:SL.")]
[assembly: AssemblyCopyright("Copyright © .grey#9120 2023")]
[assembly: AssemblyMetadata("RepositoryURL", "https://github.com/NotAloneAgain/Marine")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#elif TRACE
[assembly: AssemblyConfiguration("Release")]
#endif

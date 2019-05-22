using System.Collections.Generic;
using Tp.Integration.Plugin.Common;

namespace Tp.Perforce.StructureMap
{
    public class P4PluginExcludedAssemblies : List<string>, IExcludedAssemblyNamesSource
    {
        public P4PluginExcludedAssemblies()
        {
            AddRange(new[] { "p4api.net.dll", "p4bridge.dll" });
        }
    }
}

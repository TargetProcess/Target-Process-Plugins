using System.Collections.Generic;
using Tp.Integration.Plugin.Common;

namespace Tp.PopEmailIntegration.StructureMap
{
    public class EmailPluginExcludedAssemblies : List<string>, IExcludedAssemblyNamesSource
    {
        public EmailPluginExcludedAssemblies()
        {
            AddRange(new[] { "DotNetOpenAuth.dll" });
        }
    }
}

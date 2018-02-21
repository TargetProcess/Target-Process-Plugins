using System.Collections.Generic;
using Tp.Integration.Plugin.Common;

namespace Tp.Git.StructureMap
{
    public class GitPluginExcludedAssemblies : List<string>, IExcludedAssemblyNamesSource
    {
        public GitPluginExcludedAssemblies()
        {
            AddRange(new[] { "git2-ssh-baa87df.dll", "libssh2.dll", "zlib.dll" });
        }
    }
}
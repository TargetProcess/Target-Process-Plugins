using System.Collections.Generic;
using Tp.Integration.Plugin.Common;

namespace Tp.Git.StructureMap
{
    public class GitPluginExcludedAssemblies : List<string>, IExcludedAssemblyNamesSource
    {
        public GitPluginExcludedAssemblies()
        {
            AddRange(new[]
            {
                // libgit-ssh
                "git2-ssh-baa87df.dll",
                // ssh-keygen
                "ssh-keygen.exe", "msys-z.dll", "msys-ssp-0.dll", "msys-gcc_s-seh-1.dll", "msys-crypto-1.0.0.dll", "msys-2.0.dll"
            });
        }
    }
}

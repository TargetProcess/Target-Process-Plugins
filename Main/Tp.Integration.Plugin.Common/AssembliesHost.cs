using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using StructureMap;

namespace Tp.Integration.Plugin.Common
{
    class AssembliesHost : IAssembliesHost
    {
        private readonly IEnumerable<string> _excludedAssemblyNamesSource;

        public AssembliesHost()
        {
            IEnumerable<string> maybe = ObjectFactory.TryGetInstance<IExcludedAssemblyNamesSource>();
            _excludedAssemblyNamesSource = maybe ?? new string[] { };
        }

        public IEnumerable<Assembly> GetAssemblies()
        {
            return Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll").Where(CanBeLoaded).Select(Assembly.LoadFrom);
        }

        private bool CanBeLoaded(string assemblyName)
        {
            return !_excludedAssemblyNamesSource.Any(x => assemblyName.EndsWith(x, true, CultureInfo.InvariantCulture));
        }
    }
}

using System.Collections.Generic;
using System.Reflection;
using Tp.Integration.Plugin.Common;

namespace Tp.Integration.Testing.Common
{
	public class PredefinedAssembliesHost : IAssembliesHost
	{
		private readonly IEnumerable<Assembly> _assemblies;

		public PredefinedAssembliesHost(IEnumerable<Assembly> assemblies)
		{
			_assemblies = assemblies;
		}

		public IEnumerable<Assembly> GetAssemblies()
		{
			return _assemblies;
		}
	}
}
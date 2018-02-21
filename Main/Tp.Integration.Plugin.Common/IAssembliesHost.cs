using System.Collections.Generic;
using System.Reflection;

namespace Tp.Integration.Plugin.Common
{
    public interface IAssembliesHost
    {
        IEnumerable<Assembly> GetAssemblies();
    }
}

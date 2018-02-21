using System.Collections.Generic;

namespace Tp.Core.Diagnostics
{
    public interface IDiagnosticDataProvider
    {
        void Load(IDictionary<string, object> data);
    }
}

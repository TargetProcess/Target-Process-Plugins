
namespace Tp.Core.Diagnostics.Counters
{
    public interface IDiagnosticCounter
    {
        void Increment(ulong val);
        void Decrement(ulong val);
        DiagnosticCounterData GetData();
    }
}

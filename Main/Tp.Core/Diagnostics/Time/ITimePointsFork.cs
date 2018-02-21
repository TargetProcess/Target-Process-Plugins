using Tp.Core.Annotations;

namespace Tp.Core.Diagnostics.Time
{
    public interface ITimePointsFork
    {
        [Pure]
        [NotNull]
        ITimePoints Fork();
    }
}

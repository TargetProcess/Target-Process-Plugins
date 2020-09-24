using Tp.Core.Annotations;

namespace Tp.Core.Diagnostics.Time
{
    public interface ITimePointsForkable
    {
        [Pure]
        [NotNull]
        ITimePoints Fork();
    }
}

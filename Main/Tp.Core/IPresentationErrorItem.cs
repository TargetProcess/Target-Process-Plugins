using Tp.Core.Annotations;
using Tp.I18n;

namespace Tp.Core
{
    public interface IPresentationErrorItem
    {
        [UsedImplicitly]
        string Type { get; }
        IFormattedMessage Message { get; }
    }
}

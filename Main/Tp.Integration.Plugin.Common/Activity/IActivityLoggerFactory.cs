using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.Activity
{
    public interface IActivityLoggerFactory
    {
        IActivityLogger Create(IPluginContextSnapshot contextSnapshot);
    }
}

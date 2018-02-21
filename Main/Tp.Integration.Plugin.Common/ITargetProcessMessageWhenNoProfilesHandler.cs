using Tp.Integration.Messages.EntityLifecycle;

namespace Tp.Integration.Plugin.Common
{
    public interface ITargetProcessMessageWhenNoProfilesHandler
    {
        void Handle(ITargetProcessMessage message);
    }
}

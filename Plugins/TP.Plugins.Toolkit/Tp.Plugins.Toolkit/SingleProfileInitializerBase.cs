using StructureMap;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;

namespace Tp.Plugins.Toolkit
{
    public abstract class SingleProfileInitializerBase : ITargetProcessMessageWhenNoProfilesHandler
    {
        private readonly string _profileName;

        protected SingleProfileInitializerBase(string profileName)
        {
            _profileName = profileName;
        }

        public void Handle(ITargetProcessMessage message)
        {
            if (message.IsMessageOfCrudType())
            {
                var command = ObjectFactory.GetInstance<AddOrUpdateProfileCommand>();
                command.Execute(new PluginProfileDto { Name = _profileName }.Serialize());
            }
        }
    }
}

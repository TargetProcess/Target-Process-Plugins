using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;

namespace Tp.Search.Bus
{
    public class AutomaticOnDemandProfileCreator : ITargetProcessMessageWhenNoProfilesHandler, ITargetProcessConditionalMessageRouter
    {
        private readonly IPluginContext _pluginContext;
        private readonly ITpBus _bus;

        public AutomaticOnDemandProfileCreator(IPluginContext pluginContext, ITpBus bus)
        {
            _pluginContext = pluginContext;
            _bus = bus;
        }

        public void Handle(ITargetProcessMessage message)
        {
            CreateProfile(message);
        }

        public bool Handle(MessageEx message)
        {
            return message.AccountTag != AccountName.Empty;
        }

        private void CreateProfile(ITargetProcessMessage message)
        {
            if (message.IsMessageOfCrudType() && _pluginContext.AccountName != AccountName.Empty)
            {
                var command = ObjectFactory.GetInstance<AddOrUpdateProfileCommand>();
                command.Execute(new PluginProfileDto { Name = SearcherProfile.Name }.Serialize());
            }
        }
    }
}

// 
//   Copyright (c) 2005-2011 TargetProcess. All rights reserved.
//   TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 


using Tp.Integration.Messages;
using Tp.Integration.Messages.ServiceBus.Transport.Router;

namespace Tp.Integration.Plugin.Common.Handlers
{
    using System;
    using NServiceBus;
    using System.Linq;
    using Messages;
    using Messages.AccountLifecycle;
    using Messages.PluginLifecycle;
    using Domain;
    using Tp.Core.Annotations;
    using Tp.Integration.Messages.ServiceBus.Transport;
    using Tp.Integration.Messages.Commands;
    using Tp.Integration.Plugin.Common.Logging;
    using Tp.Integration.Plugin.Common.PluginLifecycle;
    using log4net;

    public sealed class AccountHandler
        : IHandleMessages<AccountAddedMessage>,
          IHandleMessages<AccountRemovedMessage>,
          IHandleMessages<AccountRemovedLastStepMessage>
    {
        private readonly IAccountCollection _collection;
        private readonly ITpBus _bus;
        private readonly IPluginContext _context;
        private readonly IMsmqTransport _msmqTransport;
        private readonly ILog _log;

        public AccountHandler([NotNull] IAccountCollection collection, [NotNull] ITpBus bus, [NotNull] IPluginContext context,
            [NotNull] ILogManager logManager, [NotNull] IMsmqTransport msmqTransport)
        {
            if (collection == null)
            {
                throw new NullReferenceException("collection");
            }

            if (bus == null)
            {
                throw new ArgumentNullException("bus");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            _collection = collection;
            _bus = bus;
            _context = context;
            _msmqTransport = msmqTransport;
            _log = logManager.GetLogger(this.GetType());
        }

        public void Handle(AccountAddedMessage message)
        {
            _log.InfoFormat("Adding account {0}", message.AccountName);
            var accountInfo = _collection.GetOrCreate(new AccountName(message.AccountName));
            var account = PluginInfoSender.CreatePluginAccount(
                _context.PluginName,
                accountInfo.Name,
                accountInfo.Profiles.Select(profile => new PluginProfile(profile.Name)).ToArray());

            var pluginAccountMessage = new PluginAccountMessageSerialized
            {
                SerializedMessage = new[] { account }.Serialize()
            };

            _bus.Send(pluginAccountMessage);
        }

        public void Handle(AccountRemovedMessage message)
        {
            _log.InfoFormat("Removing account {0} profiles", message.AccountName);
            var account = _collection.GetOrCreate(message.AccountName);
            foreach (var profile in account.Profiles)
            {
                _log.InfoFormat("Removing account {0} profile {1}", message.AccountName, profile.Name);
                var deleteProfileCommand = new ExecutePluginCommandCommand
                {
                    CommandName = EmbeddedPluginCommands.DeleteProfile,
                    Arguments = string.Empty
                };
                _bus.SendLocalWithContext(profile.Name, account.Name, deleteProfileCommand);
            }

            _bus.SendLocalWithContext(string.Empty, string.Empty, new AccountRemovedLastStepMessage(message.AccountName));
        }

        public void Handle(AccountRemovedLastStepMessage message)
        {
            _log.InfoFormat("Removing account {0}", message.AccountName);
            _collection.Remove(message.AccountName);
            if (_msmqTransport.RoutableTransportMode == RoutableTransportMode.OnDemand)
            {
                _msmqTransport.TryDeleteQueue(message.AccountName);
            }
        }
    }
}

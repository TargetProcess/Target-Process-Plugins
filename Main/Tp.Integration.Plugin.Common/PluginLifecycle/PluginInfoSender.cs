// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.ServiceBus.Transport;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.Mashup;

namespace Tp.Integration.Plugin.Common.PluginLifecycle
{
	using System;

	public abstract class PluginInfoSender
	{
		protected PluginInfoSender()
		{
			_context = ObjectFactory.GetInstance<IPluginContext>();
			_bus = ObjectFactory.GetInstance<ITpBus>();
			PluginMetadata = ObjectFactory.GetInstance<IPluginMetadata>();
			_accountCollection = ObjectFactory.GetInstance<IAccountCollection>();
			_pluginQueueFactory = ObjectFactory.GetInstance<IPluginQueueFactory>();
			_pluginSettings = ObjectFactory.GetInstance<IPluginSettings>();
			_pluginIcon = ObjectFactory.GetInstance<PluginIcon>();
		}

		private readonly IPluginContext _context;
		private readonly ITpBus _bus;
		protected readonly IPluginMetadata PluginMetadata;
		private readonly IAccountCollection _accountCollection;
		private readonly IPluginQueueFactory _pluginQueueFactory;
		private readonly IPluginSettings _pluginSettings;
		private readonly PluginIcon _pluginIcon;

		protected ITpBus Bus
		{
			get { return _bus; }
		}

		protected IAccountCollection AccountCollection
		{
			get { return _accountCollection; }
		}

		protected IPluginContext Context
		{
			get { return _context; }
		}

		public virtual void SendInfoMessages()
		{
			SendPluginInfoMessage();
			SendPluginAccountMessages();
		}

		private void SendPluginInfoMessage()
		{
			var attribute = PluginMetadata.PluginData;

			Bus.Send(new PluginInfoMessage
			          	{
			          		Info = new PluginInfo(Context.PluginName)
			          		       	{
			          		       		Category = attribute.Category,
			          		       		Description = attribute.Description,
			          		       		PluginInputQueue = _pluginQueueFactory.Create(_pluginSettings.PluginInputQueue).IndependentAddressForQueue,
			          		       		PluginIconContent = _pluginIcon.GetIconContent(),
										IsHidden = _pluginSettings.IsHidden
			          		       	}
			          	});
		}

		private void SendPluginAccountMessages()
		{
			var pluginAccounts = AccountCollection.Select(a => new PluginAccount
			                                                   	{
			                                                   		PluginName = Context.PluginName,
			                                                   		Name = a.Name,
			                                                   		PluginProfiles =
			                                                   			a.Profiles.Select(profile => new PluginProfile(profile.Name)).
			                                                   			ToArray()
			                                                   	});

			Bus.Send(new PluginAccountMessageSerialized {SerializedMessage = pluginAccounts.Serialize()});
		}

		internal static PluginAccount CreatePluginAccount(PluginName pluginName, AccountName accountName,
		                                                PluginProfile[] pluginProfiles)
		{
			return new PluginAccount
			       	{
			       		PluginName = pluginName,
			       		Name = accountName,
			       		PluginProfiles = pluginProfiles
			       	};
		}

		protected void SendPluginScriptMessages()
		{
			var log = ObjectFactory.GetInstance<ILogManager>().GetLogger(GetType());

			log.Info("Processing plugin mashups");

			var pluginMashupRepository = ObjectFactory.TryGetInstance<IPluginMashupRepository>() ??
			                             new DefaultPluginMashupRepository();

			var scriptMessages = new List<PluginMashupMessage>();
			foreach (var pluginMashup in pluginMashupRepository.PluginMashups)
			{
				if (pluginMashup.IsValid)
				{
					scriptMessages.Add(new PluginMashupMessage
					                   	{
					                   		PluginMashupScripts = pluginMashup.GetScripts(),
					                   		PluginName = Context.PluginName,
                                            Placeholders = pluginMashup.Placeholders,
					                   		MashupName = pluginMashup.MashupName,
											AccountName = AccountName.Empty
					                   	});
				}
				else
				{
					log.Error(string.Format("Some mashup '{0}' scripts paths are invalid", pluginMashup.MashupName));
				}
			}

			if (scriptMessages.Any())
			{
				try
				{
					var messages = scriptMessages.ToArray();
					log.InfoFormat("Mashup script count is {0}", messages.Length);
					Bus.Send(messages);
				}
				catch (Exception e)
				{
					log.Fatal("Error sending plugin mashups", e);
				}
			}
			else
			{
				log.Info("No mashup scripts found");
			}
		}
	}
}
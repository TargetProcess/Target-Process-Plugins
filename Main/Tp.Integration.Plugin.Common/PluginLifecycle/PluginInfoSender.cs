// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.Mashup;

namespace Tp.Integration.Plugin.Common.PluginLifecycle
{
	public abstract class PluginInfoSender
	{
		protected PluginInfoSender()
		{
			_context = ObjectFactory.GetInstance<IPluginContext>();
			_bus = ObjectFactory.GetInstance<ITpBus>();
			PluginMetadata = ObjectFactory.GetInstance<IPluginMetadata>();
			_accountCollection = ObjectFactory.GetInstance<IAccountCollection>();
		}

		private readonly IPluginContext _context;
		private readonly ITpBus _bus;
		protected readonly IPluginMetadata PluginMetadata;
		private readonly IAccountCollection _accountCollection;

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

		protected void SendInfoMessages()
		{
			SendPluginInfoMessage();
			SendPluginAccountMessages();
			SendPluginScriptMessages();
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
			          		       		PluginInputQueue = ObjectFactory.GetInstance<IPluginSettings>().PluginInputQueue,
			          		       		PluginIconContent = ObjectFactory.GetInstance<PluginIcon>().GetIconContent()
			          		       	}
			          	});
		}

		private void SendPluginAccountMessages()
		{
			var pluginAccounts = AccountCollection.Select(a => new PluginAccount
				             	{
									PluginName = Context.PluginName,
				             		Name = a.Name,
				             		PluginProfiles = a.Profiles.Select(profile => new PluginProfile(profile.Name)).ToArray()
				             	});

			foreach (var pluginAccount in pluginAccounts)
			{
				Bus.Send(new PluginAccountMessage {PluginAccount = pluginAccount});
			}
		}

		public static PluginAccount CreatePluginAccount(PluginName pluginName, AccountName accountName,
		                                                PluginProfile[] pluginProfiles)
		{
			return new PluginAccount
			       	{
			       		PluginName = pluginName,
			       		Name = accountName,
			       		PluginProfiles = pluginProfiles
			       	};
		}

		private void SendPluginScriptMessages()
		{
			var pluginMashupRepository = ObjectFactory.TryGetInstance<IPluginMashupRepository>() ??
			                             new DefaultPluginMashupRepository();

			var scriptMessages = new List<PluginMashupMessage>();
			var log = ObjectFactory.GetInstance<ILogManager>().GetLogger(GetType());
			foreach (var pluginMashup in pluginMashupRepository.PluginMashups)
			{
				if (pluginMashup.IsValid)
				{
					scriptMessages.Add(new PluginMashupMessage
					                   	{
					                   		PluginMashupScripts = pluginMashup.GetScripts(),
					                   		PluginName = Context.PluginName,
                                            Placeholders = pluginMashup.Placeholders,
					                   		MashupName = pluginMashup.MashupName
					                   	});
				}
				else
				{
					log.Error(string.Format("Some mashup '{0}' scripts paths are invalid", pluginMashup.MashupName));
				}
			}

			if (scriptMessages.Any())
			{
				Bus.Send(scriptMessages.ToArray());
			}
		}
	}
}
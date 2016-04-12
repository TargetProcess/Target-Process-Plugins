// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using Tp.Bugzilla.CustomCommand.Dtos;
using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Bugzilla.CustomCommand
{
	public class CheckConnectionCommand : IPluginCommand
	{
		private readonly IActivityLogger _logger;

		public CheckConnectionCommand(IActivityLogger logger)
		{
			_logger = logger;
		}

		public PluginCommandResponseMessage Execute(string args, UserDTO user)
		{
			var errors = new PluginProfileErrorCollection();
			var profile = args.DeserializeProfile();
			var settings = (BugzillaProfile) profile.Settings;

			var properties = GetBugzillaProperties(settings, errors);

			return errors.Any()
			       	? new PluginCommandResponseMessage
			       	  	{PluginCommandStatus = PluginCommandStatus.Fail, ResponseData = errors.Serialize()}
			       	: new PluginCommandResponseMessage
			       	  	{PluginCommandStatus = PluginCommandStatus.Succeed, ResponseData = properties.Serialize()};
		}

		public BugzillaProperties GetBugzillaProperties(BugzillaProfile profile, PluginProfileErrorCollection errors)
		{
			profile.ValidateCredentials(errors);

			if (!errors.Any())
			{
				try
				{
					_logger.Info("Checking connection");
					var bugzillaProperties = new BugzillaService().CheckConnection(profile);
					_logger.Info("Connection success");
					return new BugzillaProperties(bugzillaProperties);
				}
				catch (BugzillaPluginProfileException e)
				{
					e.ErrorCollection.ForEach(errors.Add);
					_logger.WarnFormat("Connection failed: {0}", e);
				}
			}

			return null;
		}

		public string Name
		{
			get { return "CheckConnection"; }
		}
	}
}
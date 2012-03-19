// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.Commands
{
	public abstract class VcsCheckConnectionCommand<TVcsPluginProfile> : IPluginCommand
		where TVcsPluginProfile : ISourceControlConnectionSettingsSource
	{
		public PluginCommandResponseMessage Execute(string args)
		{
			return new PluginCommandResponseMessage
			       	{ResponseData = OnExecute(args), PluginCommandStatus = PluginCommandStatus.Succeed};
		}

		private string OnExecute(string args)
		{
			var profile = args.DeserializeProfile();
			var errors = new PluginProfileErrorCollection();
			try
			{
				OnCheckConnection(errors, (TVcsPluginProfile) profile.Settings);
			}
			catch (StructureMapException e)
			{
				ObjectFactory.GetInstance<ICheckConnectionErrorResolver>().HandleConnectionError(e.InnerException, errors);
			}
			catch (Exception e)
			{
				ObjectFactory.GetInstance<ICheckConnectionErrorResolver>().HandleConnectionError(e, errors);
			}

			OnExecuted((TVcsPluginProfile) profile.Settings);
			return errors.Serialize();
		}

		protected virtual void OnExecuted(TVcsPluginProfile profile)
		{
		}

		protected virtual void OnCheckConnection(PluginProfileErrorCollection errors, TVcsPluginProfile settings)
		{
			var vcs = CreateVcs(settings);
			CheckStartRevision(settings, vcs, errors);
		}

		protected virtual IVersionControlSystem CreateVcs(TVcsPluginProfile settings)
		{
			return ObjectFactory.GetInstance<IVersionControlSystemFactory>().Get(settings);
		}

		protected abstract void CheckStartRevision(TVcsPluginProfile settings, IVersionControlSystem versionControlSystem,
		                                           PluginProfileErrorCollection errors);

		public string Name
		{
			get { return "CheckConnection"; }
		}
	}
}
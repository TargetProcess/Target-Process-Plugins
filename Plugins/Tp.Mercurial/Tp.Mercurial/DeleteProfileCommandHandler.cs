// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.

using System.Linq;
using NServiceBus;
using StructureMap;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Mercurial.VersionControlSystem;
using Tp.SourceControl.Settings;

namespace Tp.Mercurial
{
	public class DeleteProfileCommandHandler : IHandleMessages<ExecutePluginCommandCommand>
	{
		private readonly IStorageRepository _storage;

		public DeleteProfileCommandHandler(IStorageRepository storage)
		{
			_storage = storage;
		}

		public void Handle(ExecutePluginCommandCommand message)
		{
			if (message.CommandName == EmbeddedPluginCommands.DeleteProfile)
			{
                var folder = _storage.Get<MercurialRepositoryFolder>().FirstOrDefault();
                if (folder != null)
                {
                    folder.Delete();
                }
			}
		}
	}
}
// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus.Saga;
using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.Integration.Plugin.Common
{
	/// <summary>
	/// Base saga class for profile initialization. Inherit from this class if you need to perform custom profile initialization logic after profile was added.
	/// When this saga is started profile is marked as not initialized. All messages passed to this profile will be postponed.
	/// After you call MarkAsComplete() method, profile is marked as initialized. All postponed will be processed then.
	/// </summary>
	/// <typeparam name="TSagaData">The type of saga data.</typeparam>
	public abstract class NewProfileInitializationSaga<TSagaData> : InitializationSaga<TSagaData>,
	                                                                IAmStartedByMessages<ProfileAddedMessage>
		where TSagaData : ISagaEntity
	{
		public void Handle(ProfileAddedMessage message)
		{
			StartInitialization();
		}
	}
}
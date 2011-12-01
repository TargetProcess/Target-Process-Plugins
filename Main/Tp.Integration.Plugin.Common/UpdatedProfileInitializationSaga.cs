// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus.Saga;
using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.Integration.Plugin.Common
{
	public abstract class UpdatedProfileInitializationSaga<TSagaData> : InitializationSaga<TSagaData>,
	                                                                    IAmStartedByMessages<ProfileUpdatedMessage>
		where TSagaData : ISagaEntity
	{
		public void Handle(ProfileUpdatedMessage message)
		{
			StartInitialization();
		}
	}
}
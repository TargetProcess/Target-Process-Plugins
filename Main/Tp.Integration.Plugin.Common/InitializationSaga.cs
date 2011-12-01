// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus.Saga;
using StructureMap;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common
{
	public abstract class InitializationSaga<TSagaData> :
		TpSaga<TSagaData> where TSagaData : ISagaEntity
	{
		private Func<IProfile> Profile;

		protected InitializationSaga()
		{
			Profile = () =>
			          	{
			          		//var context = ObjectFactory.GetInstance<IPluginContext>();
							var profile = ObjectFactory.GetInstance<IProfile>();

			          		Profile = () => profile;
			          		return profile;
			          	};
		}

		protected void StartInitialization()
		{
			OnStartInitialization();
		}

		protected abstract void OnStartInitialization();

		protected new void MarkAsComplete()
		{
			Profile().MarkAsInitialized();
			Profile().Save();
			base.MarkAsComplete();
		}
	}
}
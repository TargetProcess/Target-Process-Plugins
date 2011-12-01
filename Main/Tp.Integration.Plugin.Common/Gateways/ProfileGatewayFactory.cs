// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.Gateways
{
	internal class ProfileGatewayFactory : IProfileGatewayFactory
	{
		private readonly ITpBus _tpBus;

		public ProfileGatewayFactory(ITpBus tpBus)
		{
			_tpBus = tpBus;
		}

		public IProfileGateway Create(AccountName accountName, IProfileReadonly profile)
		{
			return new ProfileGateway(profile, accountName, _tpBus);
		}
	}
}
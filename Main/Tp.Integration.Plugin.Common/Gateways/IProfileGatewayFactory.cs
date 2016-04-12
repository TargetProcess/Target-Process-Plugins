// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.Gateways
{
	public interface IProfileGatewayFactory
	{
		IProfileGateway Create(AccountName accountName, IProfileReadonly profile);
	}
}
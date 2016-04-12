// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Messages.EntityLifecycle;

namespace Tp.Integration.Plugin.Common.Gateways
{
	public interface IProfileGateway : IDisposable
	{
		void Send(ITargetProcessMessage message);
	}
}
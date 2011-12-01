// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using NServiceBus;
using Tp.Integration.Plugin.Common.Gateways;

namespace Tp.Integration.Plugin.Common
{
	/// <summary>
	/// Put this interface implementation in StructureMap in order to specify handlers order.
	/// Example : For&lt;ICustomPluginSpecifyMessageHandlerOrdering&gt;.HybridHttpOrThreadLocalScoped().Use&lt;CustomPluginSpecifyMessageHandlerOrdering&gt;();
	/// </summary>
	public interface ICustomPluginSpecifyMessageHandlerOrdering
	{
		/// <summary>
		/// Specifies message handlers order.
		/// </summary>
		/// <param name="ordering">Use this object to specify handlers order</param>
		void SpecifyHandlersOrder(First<PluginGateway> ordering);
	}
}
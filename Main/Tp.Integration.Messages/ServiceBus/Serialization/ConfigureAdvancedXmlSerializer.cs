// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using NServiceBus;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.ObjectBuilder;

namespace Tp.Integration.Messages.ServiceBus.Serialization
{
	public static class ConfigureAdvancedXmlSerializer
	{
		/// <summary>
		/// Use XML serialization that supports interface-based messages.
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public static Configure AdvancedXmlSerializer(this Configure config)
		{
			config.Configurer.ConfigureComponent<MessageMapper>(ComponentCallModelEnum.Singleton);
			config.Configurer.ConfigureComponent<AdvancedXmlSerializer>(ComponentCallModelEnum.Singleton);

			return config;
		}
	}
}
// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap.Configuration.DSL;
using Tp.Plugin.Core.Attachments;
using Tp.PopEmailIntegration.EmailReader;
using Tp.PopEmailIntegration.EmailReader.Client;

namespace Tp.PopEmailIntegration.StructureMap
{
	public class EmailPluginRegistry : Registry
	{
		public EmailPluginRegistry()
		{
			For<IBufferSize>().Singleton().Use(new BufferSize(1000000));
			For<IMessagePackSize>().Singleton().Use(new MessagePackSize(20));
			For<IEmailClient>().Use<MailBeePop3EmailClient>();
		}
	}
}
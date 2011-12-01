// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.TestRunImport;
using Tp.Integration.Testing.Common;

namespace Tp.TestRunImport.Tests.Context
{
	public class TestRunImportPluginContext
	{
		public TransportMock Transport { get; private set; }
		public IProfileReadonly CurrentProfile { get; private set; }

		public TestRunImportPluginContext(TransportMock transportMock)
		{
			Transport = transportMock;
		}

		public void AddProfile(string profileName, TestRunImportPluginProfile settings)
		{
			CurrentProfile = Transport.AddProfile(profileName, settings);
		}
	}
}

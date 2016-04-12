// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using StructureMap;
using Tp.Testing.Common.NBehave;

namespace Tp.Integration.Plugin.Common.Tests.Activity
{
	[TestFixture]
    [Category("PartPlugins1")]
    public class ShouldLogUnhandledExceptions : ActivityTestBase
	{
		protected override void OnInit()
		{
			ObjectFactory.GetInstance<ActivityLoggingContext>().InitializeActivityLoggingMock();
		}

		[Test]
		public void ShouldLogUnhandledException()
		{
			@"
				Given profile 'Profile' for account 'Account' created
				When unhandled exception is thrown during message handling for account 'Account' and profile 'Profile'
				Then error log for profile 'Profile' for account 'Account' should be written
			"
				.Execute(In.Context<ActivityLoggingActionSteps>());
		}
	}
}
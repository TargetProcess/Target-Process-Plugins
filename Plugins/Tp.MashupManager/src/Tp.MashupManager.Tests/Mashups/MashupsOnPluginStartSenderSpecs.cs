// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NBehave.Narrator.Framework;
using NUnit.Framework;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Testing.Common.NBehave;

namespace Tp.MashupManager.Tests.Mashups
{
	[ActionSteps, TestFixture]
	public class MashupsOnPluginStartSenderSpecs : MashupManagerTestBase
	{
		[Test]
		public void ShouldSendMashupsToTpOnPluginStart()
		{
			@"
				Given profile created
					And profile mashups are: mashup1, mashup2
				When TargetProcess starts
				Then 2 mashups should be sent to TP
					And default mashup 'mashup1' should be sent to TP
					And default mashup 'mashup2' should be sent to TP
			"
				.Execute();
		}

		[Test]
		public void ShouldNotSendMashupsWhenNoProfiles()
		{
			@"
				Given no profiles created
				When TargetProcess starts
				Then 0 mashups should be sent to TP
			"
				.Execute();
		}

		[When("TargetProcess starts")]
		public void ProfileStart()
		{
			new MashupsOnPluginStartSender().Handle(new TargetProcessStartedMessage());
		}
	}
}
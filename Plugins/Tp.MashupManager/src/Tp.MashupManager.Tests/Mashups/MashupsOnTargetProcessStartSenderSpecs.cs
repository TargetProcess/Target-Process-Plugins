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
    [Category("PartPlugins0")]
	public class MashupsOnTargetProcessStartSenderSpecs : MashupManagerTestBase
	{
		[Test]
		public void ShouldSendMashupsToTpOnTpStart()
		{
			@"
				Given profile 'profile1' created for account 'Account1'
					And profile mashups are: first, second
					And profile 'profile2' created for account 'Account2'
					And profile mashups are: third, fourth
				When TargetProcess starts
				Then 4 mashups should be sent to TP
					And default mashup 'Account1 first' with accounts 'Account1' should be sent to TP
					And default mashup 'Account1 second' with accounts 'Account1' should be sent to TP
					And default mashup 'Account2 third' with accounts 'Account2' should be sent to TP
					And default mashup 'Account2 fourth' with accounts 'Account2' should be sent to TP
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
			new MashupsOnTargetProcessStartSender().Handle(new TargetProcessStartedMessage());
		}
	}
}
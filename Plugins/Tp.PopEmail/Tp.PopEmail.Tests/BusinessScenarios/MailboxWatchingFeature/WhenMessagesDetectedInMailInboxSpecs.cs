// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.Testing.Common.NBehave;

namespace Tp.PopEmailIntegration.BusinessScenarios.MailboxWatchingFeature
{
	[TestFixture]
	public class WhenMessagesDetectedInMailInboxSpecs
	{
		[Test]
		public void ShouldDownloadNewMessagesOnly()
		{
			@"
				Given profile has downloaded message 'Uid1'
					And mail server has uids: Uid1,Uid2
					And message with uid 'Uid1' has sender address '1@1.com'
					And message with uid 'Uid2' has sender address '2@2.com'
				When tick occurs
				Then message 'Uid2' should be passed to process
				"
				.Execute(In.Context<MessageDownloadActionSteps>());
		}

		[Test]
		public void ShouldNotMarlMessageAsReadIfExceptionOccursOnMailServer()
		{
			@"Given project 1
					And profile has a rule: then attach to project 1
					And there are messages in mail inbox:
					|uid	|from|
					|uid1	|1@1.com|
					|uid2	|2@2.com|
					And email server is down
				When tick occurs
				Then downloaded messages should be empty
					And no messages should be passed to process"
				.Execute(In.Context<MessageDownloadActionSteps>());
		}

		[Test]
		public void ShouldSkipMessagesWithEmptyFromAddress()
		{
			@"Given project 1
					And profile has a rule: then attach to project 1
					And there are messages in mail inbox:
					|uid	|from|
					|uid1	|''|
					|uid2	|1@1.com|
				When tick occurs
				Then downloaded messages should be: uid1, uid2
					And message 'uid2' should be passed to process"
				.Execute(In.Context<MessageDownloadActionSteps>());
		}
	}
}
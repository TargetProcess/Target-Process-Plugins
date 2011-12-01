// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tp.PopEmailIntegration.BusinessScenarios.HandleEmailsFromUserFeature;
using Tp.Testing.Common.NBehave;

namespace Tp.PopEmailIntegration.BusinessScenarios.HandleEmailsFromRequestersFeature
{
	[TestFixture]
	public class WhenMailFromDeletedRequesterReceivedSpecs
	{
		[Test]
		public void ShouldAliveDeletedRequesterWhenTargetProcessReceivesMailFromHim()
		{
			@"Given requester with email 'sender@company.com' is deleted
					And project 7
					And sender email is 'sender@company.com'
					And email subject is 'Jira'
				And profile has a rule: when subject contains 'Jira' then attach to project 7
				When the email arrived
				Then requester with email 'sender@company.com' should be made alive
					And message from requester with email 'sender@company.com' should be created"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldAttachToAliveRequesterIfThereIsOneWithDesiredMail()
		{
			@"Given project 1
					And deleted requester 'Joe Black' with email 'sender@company.com'
					And requester 'Sara White' with email 'sender@company.com'
					And profile has a rule: then attach to project 1 and create request in project 1
					And sender email is 'sender@company.com'
				When the email arrived
				Then the message from requester 'Sara White' should be created
					And request in project 1 should be created from the message".Execute(In.Context<EmailProcessingSagaActionSteps>());

		}
	}
}
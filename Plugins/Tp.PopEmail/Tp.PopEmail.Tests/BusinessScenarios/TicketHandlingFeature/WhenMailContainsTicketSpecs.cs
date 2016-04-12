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

namespace Tp.PopEmailIntegration.BusinessScenarios.TicketHandlingFeature
{
	[TestFixture]
    [Category("PartPlugins0")]
	public class WhenMailContainsTicketSpecs
	{
		[Test]
		public void ShouldCreateCommentWhenTicketExistsAndIgnoreRules()
		{
			@"Given requester with email 'sender@company.com'
					And project 7
					And sender email is 'sender@company.com'
					And email body is 'mail with Ticket#30'
					And profile has a rule: then attach to project 7 and create request in project 7
				When the email arrived
				Then comment for general 30 should be created
					And the message should not be attached to project
					And no request should be created from the message"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldNotCreateCommentIfEmailSentByTargetProcess()
		{
			@"Given TargetProcess with email 'sender@company.com'
					And sender email is 'sender@company.com'
					And email body is 'mail with Ticket#30'
				When the email arrived
				Then no comments should be created"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldNotCreateCommentIfEmailSentByProject()
		{
			@"Given project 7 with email 'sender@company.com'
					And sender email is 'sender@company.com'
					And email body is 'mail with Ticket#30'
				When the email arrived
				Then no comments should be created"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldNotCreateRequestIfEmailSentByTargetProcess()
		{
			@"Given TargetProcess with email 'sender@company.com'
					And project 7
					And profile has a rule: then create request in project 7
					And sender email is 'sender@company.com'
				When the email arrived
				Then no request should be created from the message"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldNotCreateRequestIfEmailSentByProject()
		{
			@"Given project 7 with email 'sender@company.com'
					And sender email is 'sender@company.com'
					And profile has a rule: then create request in project 7
				When the email arrived
				Then no request should be created from the message"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldProcessAllRulesIfEntityWithTicketDoesNotExist()
		{
			@"Given requester with email 'sender@company.com'
					And project 7
					And sender email is 'sender@company.com'
					And email body is 'mail with Ticket#30'
					And profile has a rule: then attach to project 7 and create request in project 7
					And target process is not able to create comment
				When the email arrived
				Then the message should be attached to project 7
					And request in project 7 should be created from the message"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldPassAttachmentsFromMailToEntityWithTicket()
		{
			@"Given requester with email 'sender@company.com'
				And project 7
				And sender email is 'sender@company.com'
				And email body is 'Description Ticket#30'
				And message has attachment 'file1'
				And message has attachment 'file2'
				And saga is in message body updating state
			When MessageBodyUpdatedMessageInternal message arrived
			Then comment for general 30 should be created
				And the comment should have owner 'sender@company.com'
				And the comment should have description 'Description Ticket#30'
				And general 30 should have attachment 'file1'
				And general 30 should have attachment 'file2'
				And the message should not be attached to project
				And no request should be created from the message"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void OnlyActiveRequestersCanPostComments()
		{
			@"Given project 1
					And deleted requester 'Joe Black' with email 'sender@company.com'
					And requester 'Sara White' with email 'sender@company.com'
					And profile has a rule: then attach to project 1 and create request in project 1
					And sender email is 'sender@company.com'
					And email body is 'mail with Ticket#30'
				When the email arrived
				Then comment for general 30 should be created
					And the comment owner should be user 'Sara White'
					And the message should not be attached to project
					And no request should be created from the message".Execute(In.Context<EmailProcessingSagaActionSteps>());
		}
	}
}

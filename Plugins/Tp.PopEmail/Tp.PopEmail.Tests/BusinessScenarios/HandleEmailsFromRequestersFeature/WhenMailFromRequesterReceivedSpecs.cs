// 
// Copyright (c) 2005-2015 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.PopEmailIntegration.BusinessScenarios.HandleEmailsFromUserFeature;
using Tp.Testing.Common.NBehave;

namespace Tp.PopEmailIntegration.BusinessScenarios.HandleEmailsFromRequestersFeature
{
	[TestFixture]
	[Category("PartPlugins0")]
	public class WhenMailFromRequesterReceivedSpecs
	{
		[Test]
		public void ShouldSkipMessageIfRequesterCannotBeCreated()
		{
			@"Given sender email is 'sender@company.com'
					And project 7
					And sender email display name is 'John Smith'
					And email subject is 'Jira extra bug'
					And TargetProcess cannot to create requester
					And profile has a rule: when subject contains 'Jira,' then attach to project 7
				When the email arrived
				Then email should not be processed
					And there should be no messages to process"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldForwardEmailFromNewRequesterIfSubjectMatched()
		{
			@"Given sender email is 'sender@company.com'
					And sender email display name is 'John Smith'
					And email subject is 'Jira extra bug'
					And profile has a rule: when subject contains 'Jira,' then attach to project 7
					And project 7
				When the email arrived
				Then requester with email 'sender@company.com' and first name 'John' and last name 'Smith' should be created
					And message from requester with email 'sender@company.com' should be created
					And the message should have subject 'Jira extra bug'
					And the message should be attached to project 7"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldCreateOneRequesterIfSeveralMessagesArrived()
		{
			@"Given sender email is 'sender@company.com'
					And sender email display name is 'John Smith'
					And email subject is 'Jira extra bug'
					And project 7
					And profile has a rule: when subject contains 'Jira,' then attach to project 7
				When 2 emails arrived
				Then requester with email 'sender@company.com' and first name 'John' and last name 'Smith' should be created
					And 2 messages should be created"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldCreateRequesterWhenMailFromDeletedUserArrived()
		{
			@"Given user with email 'sender@company.com' is deleted
				And sender email display name is 'John Smith'
				And sender email is 'sender@company.com'
				And email subject is 'Jira'
					And project 7
				And profile has a rule: when subject contains 'Jira' then attach to project 7
				When the email arrived
			Then requester with email 'sender@company.com' and first name 'John' and last name 'Smith' should be created
				And message from requester with email 'sender@company.com' should be created"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldCreateRequestFromMessage()
		{
			@"Given requester with email 'sender@company.com'
					And sender email is 'sender@company.com'
					And project 7
					And profile has a rule: then create request in project 7
				When the email arrived
				Then message from requester with email 'sender@company.com' should be created
					And request in project 7 should be created from the message"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldCreateRequesterWithEmptyName()
		{
			@"Given sender email is 'sender@company.com'
					And sender email display name is empty
					And email subject is 'Jira extra bug'
					And project 7
					And profile has a rule: when subject contains 'Jira,' then attach to project 7
				When the email arrived
				Then requester with email 'sender@company.com' and empty first name and emtpy last name should be created
					And message from requester with email 'sender@company.com' should be created"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldForwardEmailFromExistingRequesterIfSubjectMatched()
		{
			@"Given requester with email 'sender@company.com'
					And sender email is 'sender@company.com'
					And email subject is 'Jira extra bug'
					And profile has a rule: then attach to project 7
					And project 7
				When the email arrived
				Then message from requester with email 'sender@company.com' should be created
					And the message should have subject 'Jira extra bug'
					And the message should be attached to project 7"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldCreateRequestAndAttachToProjectFromMessage()
		{
			@"Given sender 'sender@company.com' is from company 1
					And email subject is 'Jira extra bug'
					And project 7 is from company 1
					And project 8 is from company 1
					And profile has a rule: when company matched to project 7 and subject contains 'Jira' then create request in project 7 and attach to project 8
				When the email arrived
				Then the message should be attached to project 8
					And request in project 7 should be created from the message"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldConsiderRequesterMoreImportantThanUser()
		{
			@"Given requester with email 'sender@company.com'
				And projects: 6, 7
				And user with email 'sender@company.com'
					And sender email is 'sender@company.com'
					And email subject is 'Jira extra bug'
					And profile has a rule: when subject contains 'SuperJira' then attach to project 6
					And profile has a rule: when subject contains 'Jira' then attach to project 7
				When the email arrived
				Then message from requester with email 'sender@company.com' should be created
					And the message should be attached to project 7"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void RequestShouldBeAttachedToActiveRequester()
		{
			@"Given project 1
					And deleted requester 'Joe Black' with email 'sender@company.com'
					And requester 'Sara White' with email 'sender@company.com'
					And profile has a rule: then attach to project 1 and create request in project 1
				And sender email is 'sender@company.com'
			When the email arrived
				Then the message from requester 'Sara White' should be created
					And request in project 1 should be created from the message"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void RequestShouldBeAttachedToTeam()
		{
			@"Given project 1
					And deleted requester 'Joe Black' with email 'sender@company.com'
					And requester 'Sara White' with email 'sender@company.com'
					And profile has a rule: then attach to project 1 and create public request in project 1 and attach request to team 100
				And sender email is 'sender@company.com'
			When the email arrived
				Then the message from requester 'Sara White' should be created
					And public request with team 100 in project 1 should be created from the message"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}
	}
}

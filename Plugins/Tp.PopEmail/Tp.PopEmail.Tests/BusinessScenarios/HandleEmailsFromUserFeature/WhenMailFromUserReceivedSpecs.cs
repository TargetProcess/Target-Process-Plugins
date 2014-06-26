// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.Testing.Common.NBehave;

namespace Tp.PopEmailIntegration.BusinessScenarios.HandleEmailsFromUserFeature
{
	[TestFixture]
    [Category("PartPlugins0")]
	public class WhenMailFromUserReceivedSpecs
	{
		[Test]
		public void ShouldSkipAttachmentForDeletedProject()
		{
			@"Given project 1
					And deleted project 2
					And requester with email 'sender@company.com'
					And sender email is 'sender@company.com'
					And profile has rules:
					|Rule										 |
					|then attach to project 2|
					|then attach to project 1|
				When the email arrived
				Then the message should be attached to project 1"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void AllProjectIdsAreTheSameShouldBeThreatedAsCorrect()
		{
			@"Given profile has a rule: when company matched to project 1 then attach to project 1
				  And project 1 is from company 2
					And sender email is 'sender@company.com'
					And sender 'sender@company.com' is from company 2
					And requester with email 'sender@company.com'
					And requester with email 'sender@company.com' works for company 2
				When the email arrived
				Then the message should be attached to project 1"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void RulesWithDifferentProjectIdsShouldWork()
		{
			@"Given profile has a rule: when company matched to project 2 then attach to project 1
				  And project 2 is from company 2
					And project 1
					And sender email is 'sender@company.com'
					And sender 'sender@company.com' is from company 2
				When the email arrived
				Then the message should be attached to project 1"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldSkipARuleWhenMatchCompanyIsWithoutProjectId()
		{
			@"Given project 1
				 And project 2
				 And sender email is 'sender@company.com'
				 And profile has rules:
				|Rule|
				|when company matched to project then attach to project 1|
				|then attach to project 2|
			When the email arrived
			Then the message should be attached to project 2"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldSkipRulesWhichWereNotParsed()
		{
			@"Given project 1
					And project 2
					And requester with email 'sender@company.com'
					And sender email is 'sender@company.com'
					And profile has rules:
					|Rule										 |
					|then attach to project 2 and bla-bla-project|
					|then attach to project 1|
				When the email arrived
				Then the message should be attached to project 1"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldForwardEmailIfSenderIsFromTheSameCompanyAsProject()
		{
			@"Given sender 'sender@company.com' is from company 1
					And project 7 is from company 1
					And profile has a rule: when company matched to project 7 then attach to project 7
				When the email arrived
				Then the message should be attached to project 7"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldNotProcessEmailWhenSubjectNotMatched()
		{
			@"Given requester with email 'sender@company.com'
					And project 7
					And sender email is 'sender@company.com'
					And email subject is 'Jira extra bug'
					And profile has a rule: when subject contains 'SuperJira' then attach to project 7
				When the email arrived
				Then email should not be processed"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldProcessFirstMatchedRule()
		{
			@"Given requester with email 'sender@company.com'
					And sender email is 'sender@company.com'
					And email subject is 'Jira extra bug'
					And project 7
					And profile has a rule: when subject contains 'SuperJira' then attach to project 6
					And profile has a rule: when subject contains 'Jira' then attach to project 7
				When the email arrived
				Then message from requester with email 'sender@company.com' should be created
					And the message should be attached to project 7"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

		[Test]
		public void ShouldAttachRequestToActiveUser()
		{
			@"Given project 1
					And deleted user 'Joe Black' with email 'sender@company.com'
					And user 'Sara White' with email 'sender@company.com'
					And profile has a rule: then attach to project 1 and create request in project 1
					And sender email is 'sender@company.com'
				When the email arrived
					Then the message from user 'Sara White' should be created
						And request in project 1 should be created from the message
						And user 'Sara White' should be added as requester and owner to the request
			".Execute(In.Context<EmailProcessingSagaActionSteps>());
		}

	}
}
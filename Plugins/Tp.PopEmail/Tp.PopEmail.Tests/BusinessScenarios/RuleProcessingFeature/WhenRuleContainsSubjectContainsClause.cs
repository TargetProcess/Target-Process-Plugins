// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.PopEmailIntegration.BusinessScenarios.HandleEmailsFromUserFeature;
using Tp.Testing.Common.NBehave;

namespace Tp.PopEmailIntegration.BusinessScenarios.RuleProcessingFeature
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class WhenRuleContainsSubjectContainsClause
    {
        [Test]
        public void ShouldSupportCommasSeparatedSubjectMatching()
        {
            @"Given requester with email 'sender@company.com'
					And sender email is 'sender@company.com'
					And email subject is 'Jira extra bug'
					And project 7
					And profile has a rule: when subject contains 'jira, AnotherBug' then attach to project 7
				When the email arrived
				Then message from requester with email 'sender@company.com' should be created
					And the message should have subject 'Jira extra bug'
					And the message should be attached to project 7"
                .Execute(In.Context<EmailProcessingSagaActionSteps>());
        }

        [Test]
        public void ShouldSupportKeywordWithSeveralSpaces()
        {
            @"Given requester with email 'sender@company.com'
					And sender email is 'sender@company.com'
					And email subject is 'Jira extra bug'
					And project 7
					And profile has a rule: when subject contains 'jira' then attach  to  project 7
				When the email arrived
				Then message from requester with email 'sender@company.com' should be created
					And the message should have subject 'Jira extra bug'
					And the message should be attached to project 7"
                .Execute(In.Context<EmailProcessingSagaActionSteps>());
        }
    }
}

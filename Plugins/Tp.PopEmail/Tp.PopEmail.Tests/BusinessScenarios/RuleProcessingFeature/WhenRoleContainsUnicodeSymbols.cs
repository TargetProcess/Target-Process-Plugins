// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Text;
using System;
using NUnit.Framework;
using Tp.PopEmailIntegration.BusinessScenarios.HandleEmailsFromUserFeature;
using Tp.Testing.Common.NBehave;

namespace Tp.PopEmailIntegration.BusinessScenarios.RuleProcessingFeature
{
	[TestFixture]
	public class WhenRoleContainsUnicodeSymbols
	{
		[Test]
		public void ShouldProcessRuleWithUnicodeSymbol()
		{
			@"Given requester with email 'sender@company.com'
					And sender email is 'sender@company.com'
					And email subject is 'Ä extra bug'
					And project 7
					And profile has a rule: when subject contains 'Ä' then attach to project 7
				When the email arrived
				Then message from requester with email 'sender@company.com' should be created
					And the message should have subject 'Ä extra bug'
					And the message should be attached to project 7"
				.Execute(In.Context<EmailProcessingSagaActionSteps>());
		}
	}
}
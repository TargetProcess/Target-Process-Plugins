// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using Tp.Integration.Common;
using Tp.Testing.Common.NBehave;

namespace Tp.Bugzilla.Tests.Synchronization
{
	[ActionSteps]
    [Category("PartPlugins0")]
	public class BugzillaProfileInitializationSagaSpecs : BugzillaTestBase
	{
		[Test]
		public void CheckInitialization()
		{
			@"
				Given TargetProcess contains following severities : 1-blocking,2-critical
					And TargetProcess contains bug entity states for project 1 : 1-Open,2-Done
					And TargetProcess contains user story entity states for project 1 : 1-Open,2-Done
					And TargetProcess contains following priorities : 1-great,2-good

					And TargetProcess contains following projects : 1-Project1, 2-Project2

					And bugzilla profile for project 1 created
				
				Then severities storage should contain 2 items
					And severities storage should contain item with id 1 and name 'blocking'
					And severities storage should contain item with id 2 and name 'critical'

					And projects storage should contain 1 items
					And projects storage should contain item with id 1 and name 'Project1'

					And entity states storage should contain 2 items
					And entity states storage should contain item 'Open' with id 1
					And entity states storage should contain item 'Done' with id 2

					And priorities storage should contain 2 items
					And priorities storage should contain item with id 1 and name 'great'
					And priorities storage should contain item with id 2 and name 'good'
					"
				.Execute(In.Context<BugSyncActionSteps>().And<BugzillaProfileInitializationSagaSpecs>());
		}

		[Given(@"TargetProcess contains following projects : (?<projects>([^,]+,?\s*)+)")]
		public void CreatePrjectsInTargetProcess(string[] projects)
		{
			Context.Projects.AddRange(projects.Select(name =>
			{
				var pair = name.Split('-');

				return new ProjectDTO
				{
					ID = int.Parse(pair[0]),
					ProjectID = int.Parse(pair[0]),
					Name = pair[1]
				};
			}));
		}
	}
}
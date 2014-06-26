//  
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Testing.Common;
using Tp.SourceControl.Comments;
using Tp.Subversion.StructureMap;
using Tp.Testing.Common.NBehave;

namespace Tp.Subversion.EditProfileFeature.AutomappingFeature
{
	[TestFixture]
    [Category("PartPlugins1")]
	public class WhenAutomappingRequestedSpecs
	{
		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<VcsMockEnvironmentRegistry>());
			ObjectFactory.Configure(x => x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof(SubversionPluginProfile).Assembly, new List<Assembly> { typeof(Command).Assembly })));
		}

		[Test]
		public void ShouldMapUserUsingEmailFirst()
		{
			@"
				Given vcs history is:
					|commit|
					|{Author:""John@tp.com""}|
					|{Author:""Mark@fakecompany.com""}|
					|{Author:""Ilai@tp.com""}|
					And unsaved plugin profile
					And target process users: 
					|name|mail|
					|John Smith|John@tp.com|
					|Ilai Zemiakis|Ilai@tp.com|
				When automapping requested
				Then users should be mapped this way:
					|svnuser|tpuser|
					|John@tp.com|John Smith|
					|Ilai@tp.com|Ilai Zemiakis|
"
				.Execute(In.Context<VcsPluginActionSteps>().And<CommandActionSteps>());
		}

		[Test]
		public void ShouldNotRemoveAlreadyMappedUsers()
		{
			@"
				Given vcs history is:
					|commit|
					|{Author:""John@tp.com""}|
					|{Author:""Mark@company.com""}|
					|{Author:""Ilai@tp.com""}|
					And unsaved plugin profile
					And target process users: 
					|name|mail|
					|John Smith|John@tp.com|
					|Ilai Zemiakis|Ilai@tp.com|
					|Mark Spencer|Mark@company.com|
					And user mapping is:
					|subversion|targetprocess|
					|John@tp.com|Ilai Zemiakis|
				When automapping requested
				Then users should be mapped this way:
					|svnuser|tpuser|
					|John@tp.com|Ilai Zemiakis|
					|Ilai@tp.com|Ilai Zemiakis|
					|Mark@company.com|Mark Spencer|
					And 3 users should be mapped at whole
				And mapping result info should be: All the 2 Subversion user(s) were mapped
"
				.Execute(In.Context<VcsPluginActionSteps>().And<CommandActionSteps>());
		}


		[Test]
		public void ShouldMapUserUsingFirstNameIfEmailMatchesWereNotFound()
		{
			@"
				Given vcs history is:
					|commit|
					|{Author:""John Smith""}|
					|{Author:""Mark@fakecompany.com""}|
					|{Author:""Ilai Zemiakis""}|
					And unsaved plugin profile
					And target process users: 
					|name|mail|
					|John Smith|John@tp.com|
					|Ilai Zemiakis|Ilai@tp.com|
				When automapping requested
				Then users should be mapped this way:
					|svnuser|tpuser|
					|John Smith|John Smith|
					|Ilai Zemiakis|Ilai Zemiakis|
"
				.Execute(In.Context<VcsPluginActionSteps>().And<CommandActionSteps>());
		}

		[Test]
		public void ShouldMapUserUsingLoginIfEmailAndNameWereNotFound()
		{
			@"
				Given vcs history is:
					|commit|
					|{Author:""ValentinePalazkov""}|
					And unsaved plugin profile
					And target process users with logins, names and mails:
						|login|name|mail|
						|valentine@tp.com|Valentpa|vp@tp.com|
						|ValentinePalazkov|ValentineMalkovich|Valentine@tp.com|				
				When automapping requested
				Then users should be mapped this way:
					|svnuser|tpuser|
					|ValentinePalazkov|ValentineMalkovich|
"
				.Execute(In.Context<VcsPluginActionSteps>().And<CommandActionSteps>());
		}

		[Test]
		public void ShouldUseCommitsWithin1MonthForMapping()
		{
			@"Given vcs history is:
				|commit|
				|{Time:""1 Jan 2010"", Author:""John Smith""}|
				|{Time:""1 Feb 2010"", Author:""Ilai Zemiakis""}|
				|{Time:""1 March 2010"", Author:""Lara Croft""}|
				|{Time:""1 Apr 2010"", Author:""Lion""}|
				And unsaved plugin profile
				And target process users:
				|name|mail|
				|John Smith|John@tp.com|
				|Ilai Zemiakis|Ilai@tp.com|
				|Lara Croft|Croft@tp.com|
				|Lion|Lion@tp.com|
				And current date is 1 May 2010
				When automapping requested
				Then users should be mapped this way:
					|svnuser|tpuser|
					|Lion|Lion|
					And 1 users should be mapped at whole
"
				.Execute(In.Context<VcsPluginActionSteps>().And<CommandActionSteps>());
		}

		[Test]
		public void ShouldProvideMappingResultInfo()
		{
			@"Given vcs history contains 20 svn users
					And 5 svn users mapped to TP users
					And 40 unmapped TP users
					And unsaved plugin profile
				When automapping requested
				Then mapping result info should be: 5 Subversion user(s) were mapped, and no matches were found for 15 Subversion user(s)"
				.Execute(In.Context<VcsPluginActionSteps>().And<CommandActionSteps>());
		}

		[Test]
		public void ShouldProvideMappingResultInfoWhenAllSubversionUsersWereMapped()
		{
			@"Given vcs history contains 5 svn users
					And 5 svn users mapped to TP users
					And 40 unmapped TP users
					And unsaved plugin profile
				When automapping requested
				Then mapping result info should be: All the 5 Subversion user(s) were mapped"
				.Execute(In.Context<VcsPluginActionSteps>().And<CommandActionSteps>());
		}

		[Test]
		public void ShouldProvideMappingResultInfoWhenNotAllSubversionUsersWereMapped()
		{
			@"Given vcs history contains 15 svn users
					And 5 svn users mapped to TP users
					And unsaved plugin profile
				When automapping requested
				Then mapping result info should be: 5 Subversion user(s) were mapped, and no matches were found for 10 Subversion user(s)"
				.Execute(In.Context<VcsPluginActionSteps>().And<CommandActionSteps>());
		}

		[Test]
		public void ShouldProvideMappingResultInfoWhenNoneSvnUsersWereMapped()
		{
			@"Given vcs history contains 15 svn users
					And 0 svn users mapped to TP users
					And unsaved plugin profile
				When automapping requested
				Then mapping result info should be: No matches for Subversion user(s) found"
				.Execute(In.Context<VcsPluginActionSteps>().And<CommandActionSteps>());
		}
	}
}
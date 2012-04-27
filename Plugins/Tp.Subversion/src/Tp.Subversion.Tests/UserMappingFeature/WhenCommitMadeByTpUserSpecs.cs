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

namespace Tp.Subversion.UserMappingFeature
{
	[TestFixture]
	public class WhenCommitMadeByTpUserSpecs
	{
		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<VcsMockEnvironmentRegistry>());
			ObjectFactory.Configure(
				x =>
				x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof (SubversionPluginProfile).Assembly,
				                                                                        new List<Assembly>
				                                                                        	{typeof (Command).Assembly})));
		}

		[Test]
		public void ShouldMapUsers()
		{
			@"Given vcs history is:
					|commit|
					|{Id:1, Author:""mich"", Comment:""#123""}|
				And tp user 'Michael Jackson' with id 1
				And vcs users mapped to tp users as:
					|vcsuser	|tpuser	|
					|mich		|Michael Jackson	|
			When plugin started up
			Then revision 1 in TP should have author 'Michael Jackson'"
				.Execute(In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>().And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldAutomapUsersByLogin()
		{
			@"Given vcs history is:
					|commit|
					|{Id:1, Author:""john"", Comment:""#123""}|
				And tp user with name 'John Smith', login 'john' and id 1
				When plugin started up
				Then revision 1 in TP should have author 'John Smith'"
				.Execute(In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>().And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldPutMessageToLogIfNoMapping()
		{
			@"Given vcs history is:
					|commit|
					|{Id:1, Author:""Bob"", Comment:""#123""}|
				And tp user 'John' with id 1
				And vcs users mapped to tp users as:
					|vcsuser	|tpuser	|
					|John		|John   |
			When plugin started up
			Then log should contain message: Revision author doesn't match any TP User name. There is no valid mapping for user Bob"
				.Execute(In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>().And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void SvnUserShouldBeCaseInsensivite()
		{
			@"Given vcs history is:
					|commit|
					|{Id:1, Author:""John"", Comment:""#123""}|
				And tp user 'John' with id 1
				And vcs users mapped to tp users as:
					|vcsuser	|tpuser	|
					|jOhN		|John	|
			When plugin started up
			Then revision 1 in TP should have author 'John'"
				.Execute(In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>().And<UserMappingFeatureActionSteps>());
		}
	}
}
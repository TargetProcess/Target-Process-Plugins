// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Reflection;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Testing.Common;
using Tp.SourceControl.Comments;
using Tp.Subversion.StructureMap;
using Tp.Subversion.UserMappingFeature;
using Tp.Testing.Common.NBehave;

namespace Tp.Subversion.TargetProcessControlByCommentsPostingFeature
{
	[TestFixture, ActionSteps]
    [Category("PartPlugins1")]
	public class ShouldSupportMultipleEntityControlSpecs
	{
		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<VcsMockEnvironmentRegistry>());
			ObjectFactory.Configure(x => x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof(SubversionPluginProfile).Assembly, new List<Assembly> { typeof(Command).Assembly })));
		}

		[Test]
		public void ShouldProcessAllActions()
		{
			@"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {Id:1, Comment:""#123 comment:bla-bla my comment time:7 status:fixed"", Author:""svnuser""}
				When plugin started up
				Then comment 'bla-bla my comment' should be posted via ChangeEntityStateCommand on entity 123 by the 'tpuser'
					And time 7 should be posted on entity 123 by the 'tpuser'
					And entity 123 status should be changed to 'fixed' by the 'tpuser'
					And no additional comments should be posted"
				.Execute(In.Context<VcsPluginActionSteps>()
							.And<WhenCommitMadeByTpUserSpecs>()
							.And<ShouldChangeStatusSpecs>()
							.And<ShouldPostCommentSpecs>()
							.And<ShouldPostTimeSpecs>()
							.And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldProcessAllActionsToMultipleEntities()
		{
			@"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {Id:1, Comment:""#123 comment:bla-bla my comment time:7 #10 status:fixed"", Author:""svnuser""}
				When plugin started up
				Then comment 'bla-bla my comment' should be posted on entity 123 by the 'tpuser'
					And time 7 should be posted on entity 123 by the 'tpuser'
					And entity 10 status should be changed to 'fixed' by the 'tpuser'"
				.Execute(In.Context<VcsPluginActionSteps>()
							.And<WhenCommitMadeByTpUserSpecs>()
							.And<ShouldChangeStatusSpecs>()
							.And<ShouldPostCommentSpecs>()
							.And<ShouldPostTimeSpecs>()
							.And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldSupportEntityIdInTheMiddleOfComment()
		{
			@"Given tp user 'tpuser' with id 5
				And vcs user 'svnuser' mapped as 'tpuser'
				And vcs commit is: {Id:1, Comment:""#123 state:fixed,  #222  comment:hello world"", Author: ""svnuser""}
			When plugin started up
			Then revision 1 should be created in TP
				And revision 1 should be attached to TP entity 123
				And revision 1 should be attached to TP entity 222
				And entity 123 status should be changed to 'fixed' by the 'tpuser'
				And comment 'hello world' should be posted on entity 222 by the 'tpuser'"
				.Execute(In.Context<VcsPluginActionSteps>()
							.And<WhenVcsRevisionContainsEntityId>()
							.And<ShouldProcessEntityIdSpecs>()
							.And<WhenCommitMadeByTpUserSpecs>()
							.And<ShouldChangeStatusSpecs>()
							.And<ShouldPostCommentSpecs>()
							.And<ShouldPostTimeSpecs>()
							.And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldProcessSeveralEntitiesAtOneTime()
		{
			@"Given tp user 'tpuser' with id 5
				And vcs user 'svnuser' mapped as 'tpuser'
				And vcs commit is: {Id:1, Comment:""#123, id:456 time:1 state:fixed, #222 comment:hello world state:fixed"", Author: ""svnuser""}
			When plugin started up
			Then revision 1 should be created in TP
				And revision 1 should be attached to TP entity 123
				And revision 1 should be attached to TP entity 456
				And revision 1 should be attached to TP entity 222
				And time 1 should be posted on entity 123 by the 'tpuser'
				And time 1 should be posted on entity 456 by the 'tpuser'
				And entity 123 status should be changed to 'fixed' by the 'tpuser'
				And entity 456 status should be changed to 'fixed' by the 'tpuser'
				And entity 222 status should be changed to 'fixed' by the 'tpuser'
				And comment 'hello world' should be posted via ChangeEntityStateCommand on entity 222 by the 'tpuser'"
				.Execute(In.Context<VcsPluginActionSteps>()
							.And<WhenVcsRevisionContainsEntityId>()
							.And<ShouldProcessEntityIdSpecs>()
							.And<WhenCommitMadeByTpUserSpecs>()
							.And<ShouldChangeStatusSpecs>()
							.And<ShouldPostCommentSpecs>()
							.And<ShouldPostTimeSpecs>()
							.And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldChangeStateToBothEntities()
		{
			@"Given tp user 'tpuser' with id 5
				And vcs user 'svnuser' mapped as 'tpuser'
				And vcs commit is: {Id:1, Comment:""#123, id:456 state:fixed comment:hello world"", Author: ""svnuser""}
			When plugin started up
			Then revision 1 should be created in TP
				And revision 1 should be attached to TP entity 123
				And revision 1 should be attached to TP entity 456
				And entity 123 status should be changed to 'fixed' by the 'tpuser'
				And entity 456 status should be changed to 'fixed' by the 'tpuser'
				And comment 'hello world' should be posted via ChangeEntityStateCommand on entity 123 by the 'tpuser'
				And comment 'hello world' should be posted via ChangeEntityStateCommand on entity 456 by the 'tpuser'"
				.Execute(In.Context<VcsPluginActionSteps>()
							.And<WhenVcsRevisionContainsEntityId>()
							.And<ShouldProcessEntityIdSpecs>()
							.And<WhenCommitMadeByTpUserSpecs>()
							.And<ShouldChangeStatusSpecs>()
							.And<ShouldPostCommentSpecs>()
							.And<ShouldPostTimeSpecs>()
							.And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldSupportMultilineComments()
		{
			string.Format(
				@"Given tp user 'tpuser' with id 5
				And vcs user 'svnuser' mapped as 'tpuser'
				And vcs commit is: {{Id:1, Comment:""id:123, state:open cmt:test comment{0} id:456 time:1:2 comment:test2"", Author: ""svnuser""}}
			When plugin started up
			Then revision 1 should be created in TP
				And revision 1 should be attached to TP entity 123
				And revision 1 should be attached to TP entity 456
				And entity 123 status should be changed to 'open' by the 'tpuser'
				And time spent 1 and time left 2 should be posted on entity 456 by the 'tpuser'
				And comment 'test comment' should be posted via ChangeEntityStateCommand on entity 123 by the 'tpuser'
				And comment 'test2' should be posted on entity 456 by the 'tpuser'",
				Environment.NewLine)
				.Execute(In.Context<VcsPluginActionSteps>()
							.And<WhenVcsRevisionContainsEntityId>()
							.And<ShouldProcessEntityIdSpecs>()
							.And<WhenCommitMadeByTpUserSpecs>()
							.And<ShouldChangeStatusSpecs>()
							.And<ShouldPostCommentSpecs>()
							.And<ShouldPostTimeSpecs>()
							.And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldPostCommentOnlyToOneEntity()
		{
			@"Given tp user 'tpuser' with id 5
				And vcs user 'svnuser' mapped as 'tpuser'
				And vcs commit is: {Id:1, Comment:""#123 cmt:test comment #456"", Author: ""svnuser""}
			When plugin started up
			Then revision 1 should be created in TP
				And revision 1 should be attached to TP entity 123
				And revision 1 should be attached to TP entity 456
				And comment 'test comment' should be posted on entity 123 by the 'tpuser'"
				.Execute(In.Context<VcsPluginActionSteps>()
							.And<WhenVcsRevisionContainsEntityId>()
							.And<ShouldProcessEntityIdSpecs>()
							.And<WhenCommitMadeByTpUserSpecs>()
							.And<ShouldChangeStatusSpecs>()
							.And<ShouldPostCommentSpecs>()
							.And<ShouldPostTimeSpecs>()
							.And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldNotAssignRevisionToTheEntityMultipleTimes()
		{
			string.Format(
				@"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {{Id:1, Comment:""id:255 comment:test{0}id:255, id:256 state:Open{0}id:255 comments:close"", Author: ""svnuser""}}
				When plugin started up
				Then revision 1 should be created in TP
					And revision 1 should be attached to TP entity 255
					And revision 1 should be attached to TP entity 256
				", Environment.NewLine)
				.Execute(In.Context<VcsPluginActionSteps>()
							.And<WhenVcsRevisionContainsEntityId>()
							.And<ShouldProcessEntityIdSpecs>()
							.And<WhenCommitMadeByTpUserSpecs>()
							.And<ShouldChangeStatusSpecs>()
							.And<ShouldPostCommentSpecs>()
							.And<ShouldPostTimeSpecs>()
							.And<UserMappingFeatureActionSteps>());
		}
	}
}

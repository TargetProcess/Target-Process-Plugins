//  
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Testing.Common;
using Tp.SourceControl.Comments;
using Tp.Subversion.StructureMap;
using Tp.Subversion.UserMappingFeature;
using Tp.Testing.Common.NBehave;

namespace Tp.Subversion.TargetProcessControlByCommentsPostingFeature
{
	[TestFixture]
    [Category("PartPlugins1")]
	public class ShouldSupportMultilineCommentSpecs
	{
		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<VcsMockEnvironmentRegistry>());
			ObjectFactory.Configure(x => x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof(SubversionPluginProfile).Assembly, new List<Assembly> { typeof(Command).Assembly })));
		}

		[Test]
		public void ShouldProcessMultilineInCommitWithSeveralActions()
		{
			string.Format(
				@"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {{Id:1, Comment:""#123{0} comment:bla-bla my comment {0}time:7 {0}status:fixed"", Author:""svnuser""}}
				When plugin started up
				Then comment 'bla-bla my comment' should be posted via ChangeEntityStateCommand on entity 123 by the 'tpuser'
					And time 7 should be posted on entity 123 by the 'tpuser'
					And entity 123 status should be changed to 'fixed' by the 'tpuser'",
				Environment.NewLine)
				.Execute(In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>()
				         	.And<ShouldChangeStatusSpecs>()
				         	.And<ShouldPostCommentSpecs>()
									.And<ShouldPostTimeSpecs>().And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldProcessMultilineInCommitWithNoActions()
		{
			string.Format(
				@"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {{Id:1, Comment:""added headerRenderer to WindowShade control{0}added fix to allow CanvasButton to work within a Repeater"", Author:""svnuser""}}
				When plugin started up
				Then 0 revisions should be created in TP",
				Environment.NewLine)
				.Execute(In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>()
									.And<ShouldChangeStatusSpecs>()
									.And<ShouldPostCommentSpecs>()
									.And<ShouldPostTimeSpecs>().And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldProcessMultilineInComment()
		{
			string.Format(
				@"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {{Id:1, Comment:""#123 comment:bla-{0}bla {0}my comment time:7 status:fixed"", Author:""svnuser""}}
				When plugin started up
				Then comment 'bla-{0}bla{0}my comment' should be posted via ChangeEntityStateCommand on entity 123 by the 'tpuser'
					And time 7 should be posted on entity 123 by the 'tpuser'
					And entity 123 status should be changed to 'fixed' by the 'tpuser'",
				Environment.NewLine)
				.Execute(In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>()
				         	.And<ShouldChangeStatusSpecs>()
				         	.And<ShouldPostCommentSpecs>()
									.And<ShouldPostTimeSpecs>().And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldSupportMultilineInCommitWhenClausesSeparatedByNewLineWithNoSpace()
		{
			string.Format(
				@"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {{Id:1, Comment:""#279 state:open{0}comment:I open this bug"", Author:""svnuser""}}
				When plugin started up
				Then comment 'I open this bug' should be posted via ChangeEntityStateCommand on entity 279 by the 'tpuser'
					And entity 279 status should be changed to 'open' by the 'tpuser'",
				Environment.NewLine)
				.Execute(In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>()
				         	.And<ShouldChangeStatusSpecs>()
				         	.And<ShouldPostCommentSpecs>()
									.And<ShouldPostTimeSpecs>().And<UserMappingFeatureActionSteps>());
		}
	}
}
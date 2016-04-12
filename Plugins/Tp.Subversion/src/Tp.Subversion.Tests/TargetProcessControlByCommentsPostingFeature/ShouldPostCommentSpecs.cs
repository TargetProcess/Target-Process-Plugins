//  
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Testing.Common;
using Tp.SourceControl.Comments;
using Tp.Subversion.Context;
using Tp.Subversion.StructureMap;
using Tp.Subversion.UserMappingFeature;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Subversion.TargetProcessControlByCommentsPostingFeature
{
	[TestFixture, ActionSteps]
    [Category("PartPlugins1")]
	public class ShouldPostCommentSpecs
	{
		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<VcsMockEnvironmentRegistry>());
			ObjectFactory.Configure(x => x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof(SubversionPluginProfile).Assembly, new List<Assembly> { typeof(Command).Assembly })));
		}

		[Test]
		public void ShouldPostComment()
		{
			@"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {Id:1, Comment:""#123 comment:bla-bla my comment"", Author:""svnuser""}
				When plugin started up
				Then comment 'bla-bla my comment' should be posted on entity 123 by the 'tpuser'"
				.Execute(In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>().And<ShouldPostCommentSpecs>().And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldPostCommentEvenIfItsEmpty()
		{
			@"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {Id:1, Comment:""#123 comment:"", Author:""svnuser""}
				When plugin started up
				Then empty comment should be posted on entity 123 by the 'tpuser'"
				.Execute(In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>().And<ShouldPostCommentSpecs>().And<UserMappingFeatureActionSteps>());
		}


		[Test]
		public void ShouldNotPostCommentWhenParametersMissed()
		{
			@"Given vcs commit is: {Id:1, Comment:""#123 comment:bla-bla my comment"", Author:""svnuser""}
				When plugin started up
				Then no comments should be posted"
				.Execute(In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>().And<ShouldPostCommentSpecs>().And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldSupportShortenedKeywordAsComments()
		{
			@"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {Id:1, Comment:""#123 comments:bla-bla my comment"", Author:""svnuser""}
				When plugin started up
				Then comment 'bla-bla my comment' should be posted on entity 123 by the 'tpuser'"
				.Execute(In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>().And<ShouldPostCommentSpecs>().And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldSupportShortenedKeywordAsComm()
		{
			@"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {Id:1, Comment:""#123 comm:bla-bla my comment"", Author:""svnuser""}
				When plugin started up
				Then comment 'bla-bla my comment' should be posted on entity 123 by the 'tpuser'"
				.Execute(In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>().And<ShouldPostCommentSpecs>().And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldSupportShortenedKeywordAsCmt()
		{
			@"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {Id:1, Comment:""#123 cmt:bla-bla my comment"", Author:""svnuser""}
				When plugin started up
				Then comment 'bla-bla my comment' should be posted on entity 123 by the 'tpuser'"
				.Execute(In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>().And<ShouldPostCommentSpecs>().And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldPostCommentToMultipleEntities()
		{
			@"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {Id:1, Comment:""#123 comment:bla-bla my comment #55 cmt: another comment"", Author:""svnuser""}
				When plugin started up
				Then comment 'bla-bla my comment' should be posted on entity 123 by the 'tpuser'
					And comment 'another comment' should be posted on entity 55 by the 'tpuser'"
				.Execute(In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>().And<ShouldPostCommentSpecs>().And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldNotPostCommentIfCommentHasTypo()
		{
			@"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {Id:1, Comment:""#123 commentss:bla-bla my comment"", Author:""svnuser""}
				When plugin started up
				Then no comments should be posted"
				.Execute(In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>().And<ShouldPostCommentSpecs>().And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldEncodeComment()
		{
			@"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {Id:1, Comment:""#123 comment:bla-bla-bla <b>бла-бла</b> déjà vu"", Author:""svnuser""}
				When plugin started up
				Then comment 'bla-bla-bla &lt;b&gt;бла-бла&lt;/b&gt; déjà vu' should be posted on entity 123 by the 'tpuser'"
				.Execute(In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>().And<ShouldPostCommentSpecs>().And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void ShouldIgnoreInvalidCommand()
		{
			@"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {Id:1, Comment:""#123 commentss:bla-bla-bla <b>бла-бла</b> déjà vu"", Author:""svnuser""}
				When plugin started up
				Then no comments should be posted"
				.Execute(In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>().And<ShouldPostCommentSpecs>().And<UserMappingFeatureActionSteps>());
		}

		[Then("no comments should be posted")]
		public void EntityShouldNotHaveAnyComments()
		{
			Context.Transport.TpQueue.GetCreatedDtos<CommentDTO>().ToArray().Should(Be.Empty, "Context.Transport.TpQueue.GetCreatedDtos<CommentDTO>().ToArray().Should(Be.Empty)");
		}

		[Then("comment '$comment' should be posted on entity $entityId by the '$tpUserName'")]
		public void CommentShouldBePosted(string comment, int entityId, string userName)
		{
			var commentDto =
				ObjectFactory.GetInstance<TransportMock>().TpQueue.GetCreatedDtos<CommentDTO>().Where(x => x.GeneralID == entityId).
					Single();

			string postedComment = commentDto.Description;
			var user = Context.GetTpUserByName(userName);

			commentDto.OwnerID.Should(Be.EqualTo(user.Id), "commentDto.OwnerID.Should(Be.EqualTo(user.Id))");
			commentDto.GeneralID.Should(Be.EqualTo(entityId), "commentDto.GeneralID.Should(Be.EqualTo(entityId))");

			postedComment.Should(Be.EqualTo(comment), "postedComment.Should(Be.EqualTo(comment))");
		}

		[Then("empty comment should be posted on entity $entityId by the '$tpUserName'")]
		public void EmptyCommentShouldBePosted(int entityId, string userName)
		{
			var commentDto =
				ObjectFactory.GetInstance<TransportMock>().TpQueue.GetCreatedDtos<CommentDTO>().Where(x => x.GeneralID == entityId).
					Single();

			var user = Context.GetTpUserByName(userName);

			commentDto.OwnerID.Should(Be.EqualTo(user.Id), "commentDto.OwnerID.Should(Be.EqualTo(user.Id))");
			commentDto.GeneralID.Should(Be.EqualTo(entityId), "commentDto.GeneralID.Should(Be.EqualTo(entityId))");

			commentDto.Description.Should(Be.Null, "commentDto.Description.Should(Be.Null)");
		}

		[Then("comment '$comment' should be posted via ChangeEntityStateCommand on entity $entityId by the '$tpUserName'")]
		public void CommentShouldBePostedViaChangeEntityStateCmd(string comment, int entityId, string userName)
		{
			var changeStatusCmd =
				ObjectFactory.GetInstance<TransportMock>().TpQueue.GetMessages<ChangeEntityStateCommand>().Where(
					x => x.EntityId == entityId).Single();

			var user = Context.GetTpUserByName(userName);

			changeStatusCmd.UserID.Should(Be.EqualTo(user.Id), "changeStatusCmd.UserID.Should(Be.EqualTo(user.Id))");
			changeStatusCmd.EntityId.Should(Be.EqualTo(entityId), "changeStatusCmd.EntityId.Should(Be.EqualTo(entityId))");

			changeStatusCmd.Comment.Should(Be.EqualTo(comment), "changeStatusCmd.Comment.Should(Be.EqualTo(comment))");
		}

		[Then("no additional comments should be posted")]
		public void NoAdditionalCommentsShouldBePosted()
		{
			ObjectFactory.GetInstance<TransportMock>().TpQueue.GetCreatedDtos<CommentDTO>().Should(Be.Empty, "ObjectFactory.GetInstance<TransportMock>().TpQueue.GetCreatedDtos<CommentDTO>().Should(Be.Empty)");
		}

		private static VcsPluginContext Context
		{
			get { return ObjectFactory.GetInstance<VcsPluginContext>(); }
		}
	}
}
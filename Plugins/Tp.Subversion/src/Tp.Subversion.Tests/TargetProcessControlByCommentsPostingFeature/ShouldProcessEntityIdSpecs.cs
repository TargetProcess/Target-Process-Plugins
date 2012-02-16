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
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Testing.Common;
using Tp.SourceControl.Comments;
using Tp.Subversion.StructureMap;
using Tp.Subversion.UserMappingFeature;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Subversion.TargetProcessControlByCommentsPostingFeature
{
	[TestFixture, ActionSteps]
	public class ShouldProcessEntityIdSpecs
	{
		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<VcsMockEnvironmentRegistry>());
			ObjectFactory.Configure(x => x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof(SubversionPluginProfile).Assembly, new List<Assembly> { typeof(Command).Assembly })));
		}

		[Test]
		public void WhenNoEntityIdProvidedShouldNotCreateRevision()
		{
			@"Given vcs history is:
					|commit|
					|{Id:1, Comment:""time:1 state:fixed comment:hello world"", Author:""John""}|
				When plugin started up
				Then 0 revisions should be created in TP"
				.Execute(In.Context<VcsPluginActionSteps>().And<ShouldProcessEntityIdSpecs>().And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void WhenSvnCommentDoesNotHaveAuthorThenAllInfoButEntityIdShouldBeConsideredAsTpRevisionDescription()
		{
			@"Given vcs history is:
					|commit|
					|{Id:1, Comment:""#123 time:1 state:fixed comment:hello world"", Author:""unknown""}|
				When plugin started up
				Then revision 1 should be created in TP
					And revision 1 should be attached to TP entity 123
					And no other actions should be taken"
				.Execute(In.Context<VcsPluginActionSteps>().And<ShouldProcessEntityIdSpecs>().And<WhenVcsRevisionContainsEntityId>().And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void WhenSvnCommentHasIdWrappedInSquareBrackets()
		{
			@"Given vcs history is:
					|commit|
					|{Id:1, Comment:""[id:123] time:1 state:fixed comment:hello world"", Author:""unknown""}|
				When plugin started up
				Then revision 1 should be created in TP
					And revision 1 should be attached to TP entity 123
					And no other actions should be taken"
				.Execute(In.Context<VcsPluginActionSteps>().And<ShouldProcessEntityIdSpecs>().And<WhenVcsRevisionContainsEntityId>().And<UserMappingFeatureActionSteps>());
		}

		[Test]
		public void WhenSvnCommentHasSeveralIdsWrappedInSquareBrackets()
		{
			@"Given vcs history is:
					|commit|
					|{Id:1, Comment:""[id:123], [#24] time:1 state:fixed comment:hello world"", Author:""unknown""}|
				When plugin started up
				Then revision 1 should be created in TP
					And revision 1 should be attached to TP entity 123
					And revision 1 should be attached to TP entity 24
					And no other actions should be taken"
				.Execute(In.Context<VcsPluginActionSteps>().And<ShouldProcessEntityIdSpecs>().And<WhenVcsRevisionContainsEntityId>().And<UserMappingFeatureActionSteps>());
		}

		[Then("revision (?<sourceControlId>\\d+) should be created in TP")]
		public void RevisionShouldBeCreatedInTp(string sourceControlId)
		{
			var revisionIds =
				ObjectFactory.GetInstance<TransportMock>().TpQueue.GetCreatedDtos<RevisionDTO>().Select(x => x.SourceControlID).
					ToArray();
			revisionIds.Should(Be.EquivalentTo(new[] {sourceControlId}));
		}

		[Then("revision (?<sourceControlId>\\d+) with description '$description' should be created in TP")]
		public void RevisionWithDescriptionShouldBeCreatedInTp(string sourceControlId, string description)
		{
			var revision = ObjectFactory.GetInstance<TransportMock>().TpQueue.GetCreatedDtos<RevisionDTO>().Single();
			revision.SourceControlID.Should(Be.EqualTo(sourceControlId));
			revision.Description.Should(Be.EqualTo(description));
		}

		[Then("no other actions should be taken")]
		public void NoOtherActionsWereTaken()
		{
			var commands = ObjectFactory.GetInstance<TransportMock>().TpQueue.GetMessages<ITargetProcessCommand>();

			commands.Where(IsNotCreateCommand<RevisionDTO>).Where(IsNotCreateCommand<RevisionAssignableDTO>).Where(x => !(x is RetrieveAllUsersQuery)).ToArray().Should(
				Be.Empty);
		}

		private static bool IsNotCreateCommand<TEntityDto>(ITargetProcessCommand command)
		{
			var createCommand = command as CreateCommand;
			if (createCommand != null)
			{
				if (createCommand.Dto is TEntityDto)
				{
					return false;
				}
			}

			return true;
		}
	}
}
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
using Tp.Integration.Testing.Common;
using Tp.SourceControl.Comments;
using Tp.SourceControl.VersionControlSystem;
using Tp.Subversion.Context;
using Tp.Subversion.StructureMap;
using Tp.Testing.Common.NBehave;

namespace Tp.Subversion.Subversion
{
	[TestFixture, ActionSteps]
	public class ProcessNewRevisionOnlyFeature
	{
		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<VcsMockEnvironmentRegistry>());
			ObjectFactory.Configure(x => x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof(SubversionPluginProfile).Assembly, new List<Assembly> { typeof(Command).Assembly })));
		}

		[Test]
		public void ShouldProcessWhenAllRevisionsAreNew()
		{
			@"Given repository with 250 revisions
					And profile Start Revision is 110
				When plugin started up
				Then 141 revisions should be created in TP".
				Execute(
					In.GlobalContext());
		}

		[Test]
		public void ShouldProcessOnlyNewRevisionsWhenSyncronized()
		{
			@"Given repository with 250 revisions
					And profile Start Revision is 110
				And plugin started up
				When another 75 revisions committed to vcs
					And plugin synchronized
				Then 75 revisions should be created in TP"
				.Execute(In.GlobalContext());
		}

		[Test]
		public void ShouldImportPreviousRevisionsWhenStartRevisionChanged()
		{
			@"Given vcs history is:
					|commit|
					|{Id:1, Comment:""#1"", Author:""John"",Time:""10.01.2000"", Entries:[]}|
					|{Id:2, Comment:""#2"", Author:""John"",Time:""10.02.2000"", Entries:[]}|
					And profile Start Revision is 2
					And plugin started up
					And profile Start Revision changed to 1
				When new revisions committed to vcs:
					|commit|
					|{Id:3, Comment:""#3"", Author:""Tomara"",Time:""10.03.2000"", Entries:[]}|
					And plugin synchronized
				Then 2 revisions should be created in TP"
				.Execute(In.Context<VcsPluginActionSteps>());
		}

		[Given("repository with $revisionAmount revisions")]
		public void CreateRevisions(int revisionAmount)
		{
			for (int revisionId = 1; revisionId <= revisionAmount; revisionId++)
			{
				Context.Revisions.Add(CreateRevision(revisionId));
			}
		}

		[When("another $revisionAmount revisions committed to vcs")]
		public void CreateMoreRevisions(int revisionAmount)
		{
			var revisionIdInitial = long.Parse(Context.Revisions.Last().Id.Value);
			for (long revisionCount = revisionIdInitial + 1; revisionCount <= revisionIdInitial + revisionAmount; revisionCount++)
			{
				Context.Revisions.Add(CreateRevision(revisionCount));
			}
		}

		private static RevisionInfo CreateRevision(long revisionId)
		{
			return new RevisionInfo { Id = revisionId.ToString(), Comment = string.Format("#{0}", revisionId) };
		}

		private static VcsPluginContext Context
		{
			get { return ObjectFactory.GetInstance<VcsPluginContext>(); }
		}
	}
}
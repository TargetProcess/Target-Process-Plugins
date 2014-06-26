// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Reflection;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Testing.Common;
using Tp.SourceControl.Comments;
using Tp.Subversion.Context;
using Tp.Subversion.StructureMap;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Subversion.Subversion
{
	[TestFixture, ActionSteps]
    [Category("PartPlugins1")]
	public class VcsPluginIntegrationSpecs
	{
		private VcsPluginContext Context
		{
			get { return ObjectFactory.GetInstance<VcsPluginContext>(); }
		}

		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<VcsEnvironmentRegistry>());
			ObjectFactory.Configure(x => x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof(SubversionPluginProfile).Assembly, new List<Assembly> { typeof(Command).Assembly })));
		}

		[Test]
		public void ShouldScanForRevisions()
		{
			@"Given plugin profile
					And local repository is 'TestRepository' 
					And Start Revision is 2
				When plugin started up
				Then revisions should be created in TP"
				.Execute(In.Context<VcsPluginIntegrationSpecs>().And<VcsPluginActionSteps>());
		}

		[Given("plugin profile")]
		public void CreatePluginProfile()
		{
		}

		[Given("local repository is '$repoPath'")]
		public void SetLocalRepository(string repoPath)
		{
			var settings = LocalRepositorySettings.Create(repoPath);
			Context.Profile.Uri = settings.Uri;
			Context.Profile.Login = settings.Login;
			Context.Profile.Password = settings.Password;
		}

		[Given("Start Revision is $startRevision")]
		public void SetStartRevision(string startRevision)
		{
			var profile = Context.Profile;
			profile.StartRevision = startRevision;
		}

//		[When("plugin started up")]
//		public void PluginStartedUp()
//		{
//			Context.StartPlugin();
//		}

		[Then("revisions should be created in TP")]
		public void RevisionsShoulBeCreated()
		{
			Context.Transport.TpQueue.GetMessages<CreateCommand>().Should(Be.Not.Empty);
		}
	}
}
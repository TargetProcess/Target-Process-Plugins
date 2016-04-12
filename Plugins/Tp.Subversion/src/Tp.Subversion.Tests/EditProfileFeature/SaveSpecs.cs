// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.

using NUnit.Framework;
using StructureMap;
using Tp.Integration.Testing.Common;
using Tp.Subversion.StructureMap;
using Tp.Testing.Common.NBehave;

namespace Tp.Subversion.EditProfileFeature
{
	[TestFixture]
    [Category("PartPlugins1")]
	public class SaveSpecs
	{
		[SetUp]
		public void SetUp()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<VcsEnvironmentRegistry>());
			ObjectFactory.Configure(
				x =>
				x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof (SubversionPluginProfile).Assembly)));
		}

		[Test]
		public void ValidationShouldFailOnNegativeRevision()
		{
			@"Given unsaved plugin profile
				And profile start revision is -1
			When saved
			Then error should occur for StartRevision: ""Start Revision cannot be less than zero.""
			"
				.Execute(
					In.Context<CommandActionSteps>());
		}

		[Test]
		public void ValidationShouldFailOnNonNumericRevision()
		{
			@"Given unsaved plugin profile
				And profile start revision is startrevision
			When saved
			Then error should occur for StartRevision: ""Start Revision should be a number.""
			"
				.Execute(
					In.Context<CommandActionSteps>());
		}

		[Test]
		public void ValidationShouldFailOnUriWithSsh()
		{
			@"Given unsaved plugin profile
				And profile repository path is 'svn+ssh://user@ssh.yourdomain.com/path'
			When saved
			Then error should occur for Uri: ""Connection via SSH is not supported.""
			"
				.Execute(
					In.Context<CommandActionSteps>());
		}

		[Test]
		public void ValidateUriConsistingOfWhitespaces()
		{
			@"Given unsaved plugin profile
				And profile repository path is '   '
			When saved
			Then error should occur for Uri: ""Uri should not be empty.""
			"
				.Execute(
					In.Context<CommandActionSteps>());
		}
}
}
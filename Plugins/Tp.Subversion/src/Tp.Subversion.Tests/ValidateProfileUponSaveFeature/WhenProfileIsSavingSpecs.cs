// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Text;
using System;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Testing.Common;
using Tp.Subversion.EditProfileFeature;
using Tp.Subversion.StructureMap;
using Tp.Testing.Common.NBehave;

namespace Tp.Subversion.ValidateProfileUponSaveFeature
{
	[TestFixture]
	public class WhenProfileIsSavingSpecs
	{
		[SetUp]
		public void SetUp()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<VcsEnvironmentRegistry>());
			ObjectFactory.Configure(x => x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof (SubversionPluginProfile).Assembly)));
		}

		[Test]
		public void ValidationShouldFailOnNegativeRevision()
		{
			@"Given unsaved plugin profile
				And profile start revision is -1
			When saved
			Then error should occur for StartRevision: ""Start Revision cannot be less than zero.""
			".Execute(
				In.Context<CommandActionSteps>());
		}

		[Test]
		public void ValidationShouldFailOnNonNumericRevision()
		{
			@"Given unsaved plugin profile
				And profile start revision is startrevision
			When saved
			Then error should occur for StartRevision: ""Start Revision should be a number.""
			".Execute(
				In.Context<CommandActionSteps>());
		}

		[Test]
		public void OnlyUniqueSvnUsersShouldBeAllowed()
		{
			@"Given unsaved plugin profile
			And user mapping is:
			|subversion|targetprocess|
			|svnuser1|tpuser1|
			|svnuser1|tpuser2|
				When saved
				Then error should occur for UserMapping: ""Can't map an svn user to TargetProcess user twice.""
				"
				.Execute(In.Context<CommandActionSteps>());
		}
	}
}
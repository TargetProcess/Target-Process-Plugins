// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Testing.Common;
using Tp.SourceControl.Commands;
using Tp.SourceControl.Comments;
using Tp.SourceControl.Diff;
using Tp.SourceControl.Settings;
using Tp.Subversion.StructureMap;
using Tp.Testing.Common.NBehave;

namespace Tp.Subversion.EditProfileFeature
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class CheckConnectionSpecs
    {
        [SetUp]
        public void SetUp()
        {
            ObjectFactory.Initialize(x => x.AddRegistry<VcsEnvironmentRegistry>());
            ObjectFactory.Configure(
                x =>
                    x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof(SubversionPluginProfile).Assembly,
                        new List<Assembly>
                            { typeof(Command).Assembly })));
        }

        [Test]
        public void SubversionOnLocalhostShouldBeUpAndRunning()
        {
            ConnectionSettings settings = new SubversionPluginProfile();

            settings.Uri = "https://localhost:443/svn/IntegrationTest";
            settings.Login = "subversion";
            settings.Password = "123456";

            new Subversion.Subversion(settings, ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(),
                ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>());
        }

        [Test]
        [Category("PartUnstable")]
        public void ShouldBeSuccessOnValidSettings()
        {
            @"Given unsaved plugin profile
				And profile repository path is 'https://localhost:443/svn/IntegrationTest'
				And profile login is 'subversion'
				And profile password is '123456'
			When checked connection
			Then no errors should occur
			"
                .Execute(In.Context<CommandActionSteps>());
        }

        [Test]
        public void ShouldFailOnUnexistingRevision()
        {
            @"Given unsaved plugin profile
				And profile local repository path is '.\TestRepository'
				And profile start revision is 2147483647
			When checked connection
			Then error should occur for StartRevision
			"
                .Execute(In.Context<CommandActionSteps>());
        }

        [Test]
        public void ShouldFailOnNegativeRevision()
        {
            @"Given unsaved plugin profile
				And profile local repository path is '.\TestRepository'
				And profile start revision is -1
			When checked connection
			Then error should occur for StartRevision: ""Start Revision cannot be less than zero.""
			"
                .Execute(In.Context<CommandActionSteps>());
        }

        [Test]
        public void ShouldFailOnNonnumericRevision()
        {
            @"Given unsaved plugin profile
				And profile local repository path is '.\TestRepository'
				And profile start revision is startrevision
			When checked connection
			Then error should occur for StartRevision: ""Start Revision should be a number.""
			"
                .Execute(In.Context<CommandActionSteps>());
        }

        [Test]
        public void ShouldFailOnBadUriFormat()
        {
            @"Given unsaved plugin profile
				And profile repository path is 'invalidauri'
			When checked connection
			Then error should occur	for Uri: ""Bad url format.""
			"
                .Execute(In.Context<CommandActionSteps>());
        }

        [Test]
        public void ShouldIgnoreRevisionIfBadUriFormat()
        {
            @"Given unsaved plugin profile
				And profile repository path is 'invalidauri'
				And profile start revision is startrevision
			When checked connection
			Then error should occur	for Uri: ""Bad url format.""
				And no errors should occur for StartRevision
			"
                .Execute(In.Context<CommandActionSteps>());
        }

        [Test]
        public void ShouldFailOnInvalidHost()
        {
            @"Given unsaved plugin profile
				And profile repository path is 'http://unknownhost'
			When checked connection
			Then error should occur for Uri: ""Could not connect to server. See plugin log for details.|Unable to connect to a repository at URL 'http://unknownhost'. See plugin log for details.""
			"
                .Execute(In.Context<CommandActionSteps>());
        }

        [Test]
        public void ShouldLogErrorMessages()
        {
            @"Given unsaved plugin profile
				And profile repository path is 'http://unknownhost'
			When checked connection
			Then log should contain message: Connection failed.
			"
                .Execute(In.Context<CommandActionSteps>().And<VcsPluginActionSteps>());
        }

        [Test]
        public void ShouldFailOnUriWithSsh()
        {
            @"Given unsaved plugin profile
				And profile local repository path is 'svn+ssh://user@ssh.yourdomain.com/path'
				And profile start revision is 125
			When checked connection
			Then error should occur for Uri: ""Connection via SSH is not supported.""
			"
                .Execute(In.Context<CommandActionSteps>());
        }

        [Test]
        public void ShouldFailOnInvalidRepository()
        {
            @"Given unsaved plugin profile
				And profile repository path is 'https://localhost:443/nosuchrepo'
			When checked connection
			Then error should occur for Uri: ""Could not connect to server. See plugin log for details.""
			"
                .Execute(In.Context<CommandActionSteps>());
        }

        [Test]
        public void ShouldFailOnUnknownLogin()
        {
            @"Given unsaved plugin profile
				And profile repository path is 'https://localhost:443/svn/IntegrationTest'
				And profile login is 'nosuchlogin'
			When checked connection
			Then error should occur for Login: ""Authentication failed. See plugin log for details.""
				And error should occur for Password
				And no errors should occur for Uri
				And no errors should occur for StartRevision
			"
                .Execute(In.Context<CommandActionSteps>());
        }

        [Test]
        public void ShouldFailOnIncorrectPassword()
        {
            @"Given unsaved plugin profile
				And profile repository path is 'https://localhost:443/svn/IntegrationTest'
				And profile login is 'office\testuser'
				And profile password is '123456'
			When checked connection
			Then error should occur for Login: ""Authentication failed. See plugin log for details.""
				And error should occur for Password
				And no errors should occur for Uri
				And no errors should occur for StartRevision
			"
                .Execute(In.Context<CommandActionSteps>());
        }
    }
}

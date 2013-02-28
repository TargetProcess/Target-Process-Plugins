// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Testing.Common;
using Tp.Mercurial.Tests.Context;
using Tp.Mercurial.Tests.StructureMap;
using Tp.SourceControl.Comments;
using Tp.SourceControl.VersionControlSystem;
using Tp.SourceControl.Workflow.Workflow;
using Tp.Testing.Common.NUnit;

namespace Tp.Mercurial.Tests
{
	[TestFixture]
	public class UserMapperSpecs
	{
		private const string _tpName = "someone great";
		private const string _tpLogin = "someone";
		private const string _tpEmail = "someone@somewhere.com";
		private const int _tpId = 1;

		private static VcsPluginContext Context
		{
			get { return ObjectFactory.GetInstance<VcsPluginContext>(); }
		}

		private static UserMapper Mapper
		{
			get { return ObjectFactory.GetInstance<UserMapper>(); }
		}

		public static UserDTO User
		{
			get { return Context.UserDtos.Single(x => x.Login == _tpLogin); }
		}

		[SetUp]
		public void Setup()
		{
		    try
		    {
                ObjectFactory.Initialize(x => x.AddRegistry<VcsMockEnvironmentRegistry>());
                ObjectFactory.Configure(
                    x =>
                    x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof(MercurialPluginProfile).Assembly,
                                                                                            new List<Assembly> { typeof(Command).Assembly })));
		    }
		    catch (Exception e)
		    {
		        
		        throw;
		    }
			

			Context.CreateTpUser(_tpName, _tpLogin, _tpEmail, _tpId);
			InitializeProfile();
		}

        private MercurialPluginProfile InitializeProfile()
		{
			return Context.Profile;
		}

		[Test]
		public void ShouldMapUsersByEmail()
		{
			var vcsEmail = "someone@somewhere.com";

			Context.MapUser(vcsEmail, _tpName);

			var lookup = Mapper.GetAuthorBy(new RevisionInfo {Email = vcsEmail});

			lookup.Name.Should(Be.EqualTo(_tpName));
		}

		[Test]
		public void ShouldMapUserByName()
		{
			var vcsName = "someone great";

			Context.MapUser(vcsName, _tpName);

			var lookup = Mapper.GetAuthorBy(new RevisionInfo {Author = vcsName});

			lookup.Name.Should(Be.EqualTo(_tpName));
		}

		[Test]
		public void ShouldGuessUserByEmail()
		{
			var vcsEmail = "someone@somewhere.com";

			var lookup = Mapper.GetAuthorBy(new RevisionInfo {Email = vcsEmail});

			lookup.Name.Should(Be.EqualTo(_tpName));
		}

		[Test]
		public void ShouldGuessUserByName()
		{
			var vcsName = _tpName;

			var lookup = Mapper.GetAuthorBy(new RevisionInfo {Email = "other@mail.com", Author = vcsName});

			lookup.Name.Should(Be.EqualTo(_tpName));
		}

		[Test]
		public void ShouldGuessUserByLogin()
		{
			var vcsName = _tpLogin;

			var lookup = Mapper.GetAuthorBy(new RevisionInfo { Email = "other@mail.com", Author = vcsName });

			lookup.Name.Should(Be.EqualTo(_tpName));
		}
	}
}
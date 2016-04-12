// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Web;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.Bugzilla.BugzillaQueries;
using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.UnitTests
{
	[TestFixture]
    [Category("PartPlugins0")]
	public class BugzillaActionFactoryTests : BugzillaTestBase
	{
		public override void Init()
		{
			base.Init();

			var bugzillaStorageStub = MockRepository.GenerateStub<IBugzillaInfoStorageRepository>();
			bugzillaStorageStub.Expect(b => b.GetBugzillaBug(1)).IgnoreArguments().Return(new BugzillaBugInfo {Id = "1"});
			ObjectFactory.Configure(x => x.For<IBugzillaInfoStorageRepository>().Use(bugzillaStorageStub));

			Context.Users.AddRange(new[]
			                       	{
			                       		new UserDTO {ID = 1, Email = "ann@mail.com", IsActive = true},
			                       		new UserDTO {ID = 2, Email = "jane@mail.com", IsActive = true},
			                       		new UserDTO {ID = 3, Email = "kate@mail.com", IsActive = true}
			                       	});

			Context.TpBugs.Add(new BugDTO {ID = 1});
			Context.AddProfile(1);

			Profile.GetProfile<BugzillaProfile>().UserMapping = new MappingContainer
			                                                    	{
			                                                    		new MappingElement
			                                                    			{
			                                                    				Key = "bzAnn@mail.com",
			                                                    				Value = new MappingLookup {Id = 1, Name = "Ann"}
			                                                    			}
			                                                    	};
		}

		[Test]
		public void ShouldSetOwnerAsBugzillaUserIfMappedForCommentAction()
		{
			CheckAction(1, "bzAnn@mail.com");
		}

		[Test]
		public void ShouldSetOwnerAsTargetProcessUserIfCannotMapUser()
		{
			CheckAction(3, "kate@mail.com");
		}

		private static void CheckAction(int? ownerId, string ownerEmail)
		{
			var action = ObjectFactory.GetInstance<IBugzillaActionFactory>().GetCommentAction(
				new CommentDTO
					{
						OwnerID = ownerId,
						GeneralID = 1,
						CreateDate = CurrentDate.Value,
						Description = "text"
					}, new TimeSpan(1));

			var queryString = HttpUtility.ParseQueryString(action.Value());

			queryString["owner"].Should(Be.EqualTo(ownerEmail), "queryString[\"owner\"].Should(Be.EqualTo(ownerEmail))");
		}
	}
}
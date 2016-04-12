// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NBehave.Narrator.Framework;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Testing.Common;
using Tp.Subversion.Context;
using Tp.Testing.Common.NUnit;

namespace Tp.Subversion.UserMappingFeature
{
	[ActionSteps]
	public class UserMappingFeatureActionSteps
	{
		private static VcsPluginContext Context
		{
			get { return ObjectFactory.GetInstance<VcsPluginContext>(); }
		}

		[Given("tp user '$tpUserName' with id $id")]
		public void CreateTpUser(string tpUserName, int tpUserId)
		{
			Context.CreateTpUser(tpUserName, tpUserId);
		}

		[Given("vcs users mapped to tp users as:")]
		[Given("vcs user '$vcsuser' mapped as '$tpuser'")]
		public void MapVcsUserToTpUser(string vcsuser, string tpuser)
		{
			Context.MapUser(vcsuser.Trim(), tpuser.Trim());
		}

		[Then("revision $revisionId in TP should have author '$tpUserName'")]
		public void RevisionAuthorShouldBe(string revisionId, string tpUserName)
		{
			var user = Context.GetTpUserByName(tpUserName);
			var revision = Context.Transport.TpQueue.GetCreatedDtos<RevisionDTO>().Single(x => x.SourceControlID == revisionId);
			revision.AuthorID.Should(Be.EqualTo(user.Id), "revision.AuthorID.Should(Be.EqualTo(user.Id))");
		}

		[When("tp user with name '$userName', login '$userLogin' and id $userId")]
		public void CreateTpUser(string userName, string userLogin, int userId)
		{
			Context.CreateTpUser(userName, userLogin, userId);
		}
	}
}
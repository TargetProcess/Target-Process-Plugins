// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using Tp.Bugzilla.Tests.Synchronization;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.Handlers
{
	[ActionSteps]
    [Category("PartPlugins0")]
	public class UserChangedHandlerSpecs : BugzillaTestBase
	{
		[Test]
		public void ShouldCheckCreate()
		{
			@"
				Given bugzilla profile created
					And set of Users set to storage:
					|id|login|email|
					|1|user1|user1@mail.com|
					|2|user2|user2@mail.com|
				When handled UserCreatedMessage message with user id 3, login 'user3' and email 'user3@mail.com'
				Then users storage should contain 6 items
					And users storage should contain following items:
					|id|login|email|
					|1|user1|user1@mail.com|
					|2|user2|user2@mail.com|
					|3|user3|user3@mail.com|
			"
				.Execute(In.Context<BugSyncActionSteps>().And<UserChangedHandlerSpecs>());
		}

		[Test]
		public void ShouldCheckUpdate()
		{
			@"
				Given bugzilla profile created
					And set of Users set to storage:
					|id|login|email|
					|1|user1|user1@mail.com|
					|2|user2|user2@mail.com|
				When handled UserUpdatedMessage message with user id 2, login 'user2 updated' and email 'user2@mail.com'
				Then users storage should contain 4 items
					And users storage should contain following items:
					|id|login|email|
					|1|user1|user1@mail.com|
					|2|user2 updated|user2@mail.com|
			"
				.Execute(In.Context<BugSyncActionSteps>().And<UserChangedHandlerSpecs>());
		}

		[Test]
		public void ShouldCheckUpdateWhenEmailChanged()
		{
			@"
				Given bugzilla profile created
					And set of Users set to storage:
					|id|login|email|
					|1|user1|user1@mail.com|
					|2|user2|user2@mail.com|
				When handled UserUpdatedMessage message with user id 2, login 'user2' and changed email 'user2Upd@mail.com'
				Then users storage should contain 4 items
					And users storage should contain following items:
					|id|login|email|
					|1|user1|user1@mail.com|
					|2|user2|user2Upd@mail.com|
			"
				.Execute(In.Context<BugSyncActionSteps>().And<UserChangedHandlerSpecs>());
		}

		[Test]
		public void ShouldCheckDelete()
		{
			@"
				Given bugzilla profile created
					And set of Users set to storage:
					|id|login|email|
					|1|user1|user1@mail.com|
					|2|user2|user2@mail.com|
				When handled UserDeletedMessage message with user id 2 and email 'user2@mail.com'
				Then users storage should contain 2 items
					And users storage should contain following items:
					|id|login|email|
					|1|user1|user1@mail.com|
			"
				.Execute(In.Context<BugSyncActionSteps>().And<UserChangedHandlerSpecs>());
		}

		[Test]
		public void ShouldCheckCreateInactive()
		{
			@"
				Given bugzilla profile created
					And set of Users set to storage:
					|id|login|email|
					|1|user1|user1@mail.com|
					|2|user2|user2@mail.com|
				When handled UserCreatedMessage message with inactive user id 3, login 'user3' and email 'user3@mail.com'
				Then users storage should contain 6 items
					And users storage should contain following items:
					|id|login|email|
					|1|user1|user1@mail.com|
					|2|user2|user2@mail.com|
					|3|user3|user3@mail.com|
			"
				.Execute(In.Context<BugSyncActionSteps>().And<UserChangedHandlerSpecs>());
		}

		[Given("set of Users set to storage:")]
		public void SetUserToStorage(int id, string login, string email)
		{
			var user = new UserDTO {ID = id, Login = login, Email = email};

			Profile.Get<UserDTO>(user.ID.ToString()).Add(user);
			Profile.Get<UserDTO>(user.Email).Add(user);
		}

		[When("handled UserCreatedMessage message with user id $id, login '$login' and email '$email'")]
		public void HandleCreate(int id, string login, string email)
		{
			var user = new UserDTO { ID = id, Login = login, Email = email, IsActive = true };
			TransportMock.HandleMessageFromTp(Profile, new UserCreatedMessage {Dto = user});
		}

		[When("handled UserUpdatedMessage message with user id $id, login '$login' and email '$email'")]
		public void HandleUpdate(int id, string login, string email)
		{
			var user = new UserDTO { ID = id, Login = login, Email = email, IsActive = true };
			TransportMock.HandleMessageFromTp(Profile, new UserUpdatedMessage {Dto = user});
		}

		[When("handled UserCreatedMessage message with inactive user id $id, login '$login' and email '$email'")]
		public void HandlCreateInactiveWithLogin(int id, string login, string email)
		{
			var user = new UserDTO { ID = id, Login = login, Email = email, IsActive = false };
			TransportMock.HandleMessageFromTp(Profile, new UserCreatedMessage { Dto = user });
		}

		[When("handled UserUpdatedMessage message with user id $id, login '$login' and changed email '$email'")]
		public void HandleUpdateWithEmailChanged(int id, string login, string email)
		{
			var user = new UserDTO {ID = id, Login = login, Email = email, IsActive = true};
			TransportMock.HandleMessageFromTp(Profile,
			                                  new UserUpdatedMessage {Dto = user, ChangedFields = new[] {UserField.Email}});
		}

		[When("handled UserDeletedMessage message with user id $id and email '$email'")]
		public void HandleDelete(int id, string email)
		{
			var user = new UserDTO {ID = id, Email = email};
			TransportMock.HandleMessageFromTp(Profile, new UserDeletedMessage {Dto = user});
		}

		[Then("users storage should contain $count items")]
		public void ChechStorageCount(int count)
		{
			Profile.Get<UserDTO>().Count().Should(Be.EqualTo(count), "Profile.Get<UserDTO>().Count().Should(Be.EqualTo(count))");
		}

		[Then("users storage should contain following items:")]
		public void CheckStorageItem(int id, string login, string email)
		{
			Profile.Get<UserDTO>(id.ToString())
				.Where(u => u.Login == login)
				.Where(u => u.Email == email)
				.SingleOrDefault().Should(Be.Not.Null, "Profile.Get<UserDTO>(id.ToString()).Where(u => u.Login == login).Where(u => u.Email == email).SingleOrDefault().Should(Be.Not.Null)");

			Profile.Get<UserDTO>(email)
				.Where(u => u.Login == login)
				.Where(u => u.ID == id)
				.SingleOrDefault().Should(Be.Not.Null, "Profile.Get<UserDTO>(email).Where(u => u.Login == login).Where(u => u.ID == id).SingleOrDefault().Should(Be.Not.Null)");
		}
	}
}
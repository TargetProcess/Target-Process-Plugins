// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.PopEmailIntegration.Data;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.PopEmailIntegration
{
	[TestFixture, ActionSteps]
    [Category("PartPlugins0")]
	public class TpGeneralUserChangedMessageHandlerSpecs : PopEmailIntegrationContext
	{
		private const string FIRST_NAME = "FirstName";
		private const string LAST_NAME = "LastName";
		private const string EMAIL = "Email@Email.com";
		private const string LOGIN = "Login";
		private const bool IS_ACTIVE = true;
		private readonly DateTime _deleteDate = new DateTime(2007, 1, 1);

		[Test]
		public void ShouldAddUserLiteIfUserIsAddedInTp()
		{
			@"
				Given no users stored for profile
				When user with id 1 is added in TP
				Then user with id 1 and type 'User' and empty company should be stored for profile"
				.Execute();
		}

		[Test]
		public void ShouldAddUserLiteIfRequesterIsAddedInTp()
		{
			@"
				Given no users stored for profile
				When requester with id 2 and company 10 is added in TP
				Then user with id 2 and type 'Requester' and company 10 should be stored for profile"
				.Execute();
		}

		[Test]
		public void ShouldUpdateUserLiteIfUpdateInTP()
		{
			@"
				Given user with id 1 and first name 'FirstName' stored for profile
				When user with id 1 first name is set to 'FirstNameUpdated' in TP
				Then user with id 1 should have first name 'FirstNameUpdated' in profile storage"
				.Execute();
		}

		[Test]
		public void ShouldDeleteUserLiteIfUpdateInTP()
		{
			@"
				Given user with id 1 and first name 'FirstName' stored for profile
				When user with id 1 is deleted in TP
				Then no users should be stored for profile"
				.Execute();
		}

		[Given("no users stored for profile")]
		public void ClearUserStorage()
		{
			Storage.Get<UserLite>().Clear();
		}

		[Given("user with id $userId and first name '$firstName' stored for profile")]
		public void StoreUser(int userId, string firstName)
		{
			ObjectFactory.GetInstance<UserRepository>().Add(new UserLite { Id = userId, FirstName = firstName });
			//Storage.Get<UserLite>().Add(new UserLite {Id = userId, FirstName = firstName});
		}

		[When("user with id $userId is added in TP")]
		public void CreateUserInTP(int userId)
		{
			Transport.HandleMessageFromTp(new UserCreatedMessage
			                               	{
			                               		Dto =
			                               			new UserDTO
			                               				{
			                               					ID = userId,
			                               					FirstName = FIRST_NAME,
			                               					LastName = LAST_NAME,
			                               					Email = EMAIL,
			                               					Login = LOGIN,
			                               					IsActive = IS_ACTIVE,
			                               					DeleteDate = _deleteDate
			                               				}
			                               	});
		}

		[When("requester with id $requesterId and company $companyId is added in TP")]
		public void CreateRequesterInTP(int requesterId, int companyId)
		{
			Transport.HandleMessageFromTp(new RequesterCreatedMessage
			                               	{
			                               		Dto =
			                               			new RequesterDTO
			                               				{
			                               					ID = requesterId,
			                               					FirstName = FIRST_NAME,
			                               					LastName = LAST_NAME,
			                               					Email = EMAIL,
			                               					Login = LOGIN,
			                               					IsActive = IS_ACTIVE,
			                               					DeleteDate = _deleteDate,
			                               					CompanyID = companyId
			                               				}
			                               	});
		}

		[When("user with id $userId first name is set to '$firstNameUpdated' in TP")]
		public void UpdateUserFirstNameInTP(int userId, string firstNameUpdated)
		{
			Transport.HandleMessageFromTp(new UserUpdatedMessage
			                               	{
			                               		ChangedFields = new[] {UserField.FirstName},
			                               		Dto = new UserDTO {ID = userId, FirstName = firstNameUpdated}
			                               	});
		}

		[When("user with id $userId is deleted in TP")]
		public void DeleteUserInTP(int userId)
		{
			Transport.HandleMessageFromTp(new UserDeletedMessage {Dto = new UserDTO {ID = userId}});
		}

		[Then("user with id $userId and type '$userType' and empty company should be stored for profile")]
		public void StoreUserForProfile(int userId, string userType)
		{
			var user = Storage.Get<UserLite>().Where(x => x.Id == userId).First();
			user.FirstName.Should(Be.EqualTo(FIRST_NAME));
			user.LastName.Should(Be.EqualTo(LAST_NAME));
			user.Email.Should(Be.EqualTo(EMAIL));
			user.Login.Should(Be.EqualTo(LOGIN));
			user.IsActive.Should(Be.EqualTo(IS_ACTIVE));
			user.DeleteDate.Should(Be.EqualTo(_deleteDate));
			user.CompanyId.Should(Be.Null);
			user.UserType.Should(Be.EqualTo((UserType) Enum.Parse(typeof (UserType), userType)));
		}

		[Then("user with id $userId and type '$userType' and company $companyId should be stored for profile")]
		public void StoreRequesterForProfile(int userId, string userType, int companyId)
		{
			var user = Storage.Get<UserLite>().Where(x => x.Id == userId).First();
			user.FirstName.Should(Be.EqualTo(FIRST_NAME));
			user.LastName.Should(Be.EqualTo(LAST_NAME));
			user.Email.Should(Be.EqualTo(EMAIL));
			user.Login.Should(Be.EqualTo(LOGIN));
			user.IsActive.Should(Be.EqualTo(IS_ACTIVE));
			user.DeleteDate.Should(Be.EqualTo(_deleteDate));
			user.CompanyId.Should(Be.EqualTo(companyId));
			user.UserType.Should(Be.EqualTo((UserType) Enum.Parse(typeof (UserType), userType)));
		}

		[Then("user with id $userId should have first name '$firstNameUpdated' in profile storage")]
		public void StorageShouldContainUpdatedUser(int userId, string firstNameUpdated)
		{
			var user = Storage.Get<UserLite>().Where(x => x.Id == userId).First();
			user.FirstName.Should(Be.EqualTo(firstNameUpdated));
		}

		[Then("no users should be stored for profile")]
		public void NoUsersShouldBeStoredForProfile()
		{
			Storage.Get<UserLite>().Should(Be.Empty);
		}
	}
}
// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.Linq;
using NBehave.Narrator.Framework;
using NServiceBus.Unicast.Transport;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.BugTracking.ImportToTp;
using Tp.Bugzilla.LegacyProfileConversion;
using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Testing.Common;
using Tp.LegacyProfileConversion.Common.Testing;
using Tp.LegacyProfileConvertsion.Common;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.LegacyProfileConversion
{
	[ActionSteps]
	public class LegacyProfileConverterActionSteps :
		LegacyProfileConverterActionStepsBase<LegacyProfileConvertor, BugzillaLegacyProfileConversionUnitTestRegistry, PluginProfile>
	{
		private int _externalId = 1;

		protected override string SettingsXmlNode
		{
			get { return "Settings"; }
		}

		protected override string PluginName
		{
			get { return "Bugzilla Integration"; }
		}

		#region given

		[Given("account is empty")]
		public void SetEmptyAccount()
		{
			CreateAccount(AccountName.Empty.Value);
		}

		[Given("create account '$accountName'")]
		public void CreateAccount(string accountName)
		{
			AccountNameIs(accountName);
			ObjectFactory.GetInstance<IPluginContext>().Stub(x => x.AccountName).Return(accountName);
		}

		[Given("user '$userLogin' created")]
		public void CreateUser(string userLogin)
		{
			CreateUser(userLogin, true, null);
		}

		[Given("inactive user '$userLogin' created")]
		public void CreateInactiveUser(string userLogin)
		{
			CreateUser(userLogin, false, null);
		}

		[Given("deleted user '$userLogin' created")]
		public void CreateDeletedUser(string userLogin)
		{
			CreateUser(userLogin, true, DateTime.Now);
		}

		private void CreateUser(string login, bool isActive, DateTime? deletedDate)
		{
			var user = new TpUser
			           	{
			           		Login = login,
			           		Email = string.Format("{0}@targetprocess.com", login),
			           		FirstName = "FirstName",
			           		LastName = "LastName",
			           		SecretWord = "abc",
			           		Type = 1,
			           		IsActive = isActive,
			           		DeleteDate = deletedDate
			           	};
			Context.TpUsers.InsertOnSubmit(user);
			Context.SubmitChanges();

			Context.ProjectMembers.InsertOnSubmit(new ProjectMember
			                                      	{
			                                      		ProjectID = Context.Projects.First().ProjectID,
														UserID = user.UserID
			                                      	});
			Context.SubmitChanges();
		}

		[Given("state '$state' created")]
		public void CreateState(string state)
		{
			Context.EntityStates.InsertOnSubmit(new EntityState
			                                    	{
			                                    		Name = state,
			                                    		ProcessID = DefaultProcessId,
			                                    		EntityTypeID = BugzillaConstants.BugEntityTypeId
			                                    	});
			Context.SubmitChanges();
		}

		[Given("priority '$priority' created")]
		public void CreatePriority(string priority)
		{
			Context.Priorities.InsertOnSubmit(new Priority
			                                  	{
			                                  		EntityTypeID = BugzillaConstants.BugEntityTypeId,
			                                  		Name = priority
			                                  	});
		}

		[Given("severity '$severity' created")]
		public void CreateSeverity(string severity)
		{
			Context.Severities.InsertOnSubmit(new Severity {Name = severity});
		}

		[Given("role '$role' created")]
		public void CreateRole(string role)
		{
			Context.Roles.InsertOnSubmit(new Role {Name = role});
		}

		[Given("bugzilla url is '$url'")]
		public void SetBugzillaUrl(string url)
		{
			SetLegacyProfileValue(LegacyBugzillaProfileFields.Url, url);
		}

		[Given("sync interval is $interval")]
		public void SetBugzillaSyncInterval(int interval)
		{
			SetLegacyProfileValue(LegacyBugzillaProfileFields.SyncInterval, interval.ToString(CultureInfo.InvariantCulture));
		}

		[Given("bugzilla login is '$login'")]
		public void SetBugzillaLogin(string login)
		{
			SetLegacyProfileValue(LegacyBugzillaProfileFields.Login, login);
		}

		[Given("bugzilla password is '$password'")]
		public void SetBugzillaPassvord(string password)
		{
			SetLegacyProfileValue(LegacyBugzillaProfileFields.Password, password);
		}

		[Given("bugzilla queries are '$query'")]
		public void SetBugzillaQuery(string query)
		{
			SetLegacyProfileValue(LegacyBugzillaProfileFields.Queries, query);
		}

		[Given("bugzilla project is '$projectAbbreviation'")]
		public void SetBugzillaProject(string projectAbbreviation)
		{
			SetLegacyProfileValue(LegacyBugzillaProfileFields.Project,
			                      Context.Projects.Single(p => p.Abbreviation == projectAbbreviation).ProjectID.ToString(
			                      	CultureInfo.InvariantCulture));
		}

		[Given("bugzilla assignee role is '$assignee'")]
		public void SetBugzillaAssignee(string assignee)
		{
			SetLegacyProfileValue(LegacyBugzillaProfileFields.AssigneeRole, assignee);
		}

		[Given("bugzilla reporter role is '$reporter'")]
		public void SetBugzillaReporter(string reporter)
		{
			SetLegacyProfileValue(LegacyBugzillaProfileFields.ReporterRole, reporter);
		}

		[Given("user mapping is:")]
		public void UpdateUserMapping(string bugzilla, string targetprocess)
		{
			SetValueToMap(settings => settings.Users[bugzilla] = targetprocess);
		}

		[Given("state mapping is:")]
		public void UpdateStateMapping(string bugzilla, string targetprocess)
		{
			SetValueToMap(settings => settings.EntityStates[bugzilla] = targetprocess);
		}

		[Given("priority mapping is:")]
		public void UpdatePriorityMapping(string bugzilla, string targetprocess)
		{
			SetValueToMap(settings => settings.Priorities[bugzilla] = targetprocess);
		}

		[Given("severity mapping is:")]
		public void UpdateSeverityMapping(string bugzilla, string targetprocess)
		{
			SetValueToMap(settings => settings.Severities[bugzilla] = targetprocess);
		}

		[Given("bugzilla bug '$name' with external id $id for project '$projectName' stored in legacy profile")]
		public void SaveBugzillaBugs(string name, string id, string projectName)
		{
			var project = Context.Projects.Single(p => p.Abbreviation == projectName);

			Context.Generals.InsertOnSubmit(new General
			{
				Name = name,
				EntityTypeID = BugzillaConstants.BugEntityTypeId,
				ParentProjectID = project.ProjectID
			});

			Context.SubmitChanges();
			var generalId = GetLastBugIdByName(name);
			// wtf? project process doesn't contain workflow for bugs
			// so just pull anyone that satisfies FK_Assignable_EntityState_EntityStateID constraint
			var entityState = Context.EntityStates.First(es => es.EntityTypeID == BugzillaConstants.BugEntityTypeId);

			Context.ExecuteCommand(
				"INSERT INTO Assignable (AssignableID, Effort, EffortCompleted, EffortToDo, EntityStateID) VALUES ({0}, 0, 0, 0, {1})"
					.Fmt(generalId, entityState.EntityStateID));

			Context.Bugs.InsertOnSubmit(new Bug {BugID = generalId});

			InsertExternalReference(BugzillaConstants.BugEntityTypeId, id, generalId);

			Context.SubmitChanges();
		}

		[Given(@"bugzilla bug '$bugName' have comments: (?<comments>([^,]+,?\s*)+)")]
		public void SaveBugzillaComments(string bugName, string[] comments)
		{
			var bugId = GetLastBugIdByName(bugName);

			foreach (var comment in comments)
			{
				Context.Comments.InsertOnSubmit(new Comment { GeneralID = bugId, Description = comment });
				InsertExternalReference(BugzillaConstants.CommentEntityTypeId, _externalId++.ToString(CultureInfo.InvariantCulture), bugId);
				Context.SubmitChanges();
			}
		}

		[Given(@"bugzilla bug '$bugName' have attachments: (?<attachments>([^,]+,?\s*)+)")]
		public void SaveBugzillaAttachments(string bugName, string[] attachments)
		{
			var bugId = GetLastBugIdByName(bugName);

			foreach (var attachment in attachments)
			{
				Context.Attachments.InsertOnSubmit(new Attachment {GeneralID = bugId, Description = attachment});
				InsertExternalReference(BugzillaConstants.AttachmentEntityTypeId,
				                        _externalId++.ToString(CultureInfo.InvariantCulture), bugId);

				Context.SubmitChanges();
			}
		}

		[Given("delete bug '$bugName'")]
		public void DeleteBug(string bugName)
		{
			Context.Bugs.DeleteOnSubmit(Context.Bugs.Single(b => b.BugID == GetLastBugIdByName(bugName)));
			Context.SubmitChanges();
		}

		[Given("user '$userLogin' removed from project team")]
		public void RemoveUserFromTeam(string userLogin)
		{
			Context.ProjectMembers.DeleteAllOnSubmit(Context.ProjectMembers.Where(m => m.TpUser.Login == userLogin));
			Context.SubmitChanges();
		}

		private void InsertExternalReference(int entityType, string externalId, int tpId)
		{
			Context.ExternalReferences.InsertOnSubmit(new ExternalReference
			{
				EntityTypeID = entityType,
				ExternalID = externalId,
				TpID = tpId,
				PluginProfileID = LegacyProfile.PluginProfileID,
				CreateDate = DateTime.Now
			});
		}

		private int GetLastBugIdByName(string name)
		{
			return Context.Generals.ToList().Last(b => b.Name == name && b.EntityTypeID == BugzillaConstants.BugEntityTypeId).GeneralID;
		}

		private int GetFirstBugIdByName(string name)
		{
			return Context.Generals.ToList().First(b => b.Name == name && b.EntityTypeID == BugzillaConstants.BugEntityTypeId).GeneralID;
		}

		private void SetValueToMap(Action<LegacyMappingParser> setValueAction)
		{
			var maps = GetLegacyProfileValue(LegacyBugzillaProfileFields.Maps);
			var settings = new LegacyMappingParser();
			if (!string.IsNullOrEmpty(maps))
			{
				settings.Maps = maps.TrimEnd(Environment.NewLine.ToArray());
			}

			setValueAction(settings);

			SetLegacyProfileValue(LegacyBugzillaProfileFields.Maps, settings.Maps);
		}

		#endregion

		#region when

		[When("convert bugzilla legacy profile '$profileName'")]
		public void ConvertBugzillaLegacyProfile(string profileName)
		{
			Context.SubmitChanges();

			var transportMock = ObjectFactory.GetInstance<TransportMock>();

			transportMock.TpQueue.Clear();
//			transportMock.HandleMessageFromTp(new List<HeaderInfo>
//			                                  	{
//			                                  		new HeaderInfo
//			                                  			{
//			                                  				Key = BusExtensions.ACCOUNTNAME_KEY,
//			                                  				Value = ObjectFactory.GetInstance<IConvertorArgs>().AccountName
//			                                  			}
//			                                  	}, new ExecutePluginCommandCommand
//			                                  	   	{
//			                                  	   		Arguments = profileName,
//			                                  	   		CommandName = new ConvertProfileCommand().Name
//			                                  	   	});

			transportMock.HandleLocalMessage(new List<HeaderInfo>
			                                 	{
			                                 		new HeaderInfo
			                                 			{
			                                 				Key = BusExtensions.ACCOUNTNAME_KEY,
			                                 				Value = ObjectFactory.GetInstance<IConvertorArgs>().AccountName
			                                 			}
			                                 	}, new ConvertLegacyProfileLocalMessage
			                                 	   	{
			                                 	   		LegacyProfileName = profileName
			                                 	   	});
		}

		#endregion

		#region then

		private BugzillaProfile Profile
		{
			get { return Storage.GetProfile<BugzillaProfile>(); }
		}

		private static IProfileReadonly Storage
		{
			get { return Account.Profiles.Single(); }
		}

		[Then("legacy profile '$name' should be disabled")]
		public void CheckLegasyProfileDisabled(string name)
		{
			Context.Refresh(RefreshMode.OverwriteCurrentValues, Context.PluginProfiles);

			var profile = Context.PluginProfiles.Single(p => p.ProfileName == name);
			profile.Active.Should(Be.False, "profile.Active.Should(Be.False)");
		}

		[Then("bugzilla url should be '$url'")]
		public void CheckUrl(string url)
		{
			Profile.Url.Should(Be.EqualTo(url), "Profile.Url.Should(Be.EqualTo(url))");
		}

		[Then("bugzilla login should be '$login'")]
		public void CheckLogin(string login)
		{
			Profile.Login.Should(Be.EqualTo(login), "Profile.Login.Should(Be.EqualTo(login))");
		}

		[Then("bugzilla password should be '$password'")]
		public void CheckPassword(string password)
		{
			Profile.Password.Should(Be.EqualTo(password), "Profile.Password.Should(Be.EqualTo(password))");
		}

		[Then("bugzilla project should be '$projectAbbr'")]
		public void CheckProject(string projectAbbr)
		{
			int projectId = Context.Projects.Single(p => p.Abbreviation == projectAbbr).ProjectID;
			Profile.Project.Should(Be.EqualTo(projectId), "Profile.Project.Should(Be.EqualTo(projectId))");
		}

		[Then("bugzilla queries should be '$query'")]
		public void CheckQuery(string query)
		{
			Profile.SavedSearches.Should(Be.EqualTo(query), "Profile.SavedSearches.Should(Be.EqualTo(query))");
		}

		[Then("bugzilla assignee role should be '$role'")]
		public void CheckAssignee(string role)
		{
			Profile.RolesMapping[DefaultRoles.Assignee].Name.Should(Be.EqualTo(role), "Profile.RolesMapping[DefaultRoles.Assignee].Name.Should(Be.EqualTo(role))");
		}

		[Then("bugzilla reporter role should be '$role'")]
		public void CheckReporter(string role)
		{
			Profile.RolesMapping[DefaultRoles.Reporter].Name.Should(Be.EqualTo(role), "Profile.RolesMapping[DefaultRoles.Reporter].Name.Should(Be.EqualTo(role))");
		}

		[Then("user mapping should be:")]
		public void CheckUserMapping(string bugzilla, string targetprocess)
		{
			Profile.UserMapping[bugzilla.Trim()].Name.Should(Be.EqualTo(targetprocess.Trim()), "Profile.UserMapping[bugzilla.Trim()].Name.Should(Be.EqualTo(targetprocess.Trim()))");
		}

		[Then("state mapping should be:")]
		public void CheckStateMapping(string bugzilla, string targetprocess)
		{
			Profile.StatesMapping[bugzilla.Trim()].Name.Should(Be.EqualTo(targetprocess.Trim()), "Profile.StatesMapping[bugzilla.Trim()].Name.Should(Be.EqualTo(targetprocess.Trim()))");
		}

		[Then("priority mapping should be:")]
		public void CheckPriorityMapping(string bugzilla, string targetprocess)
		{
			Profile.PrioritiesMapping[bugzilla.Trim()].Name.Should(Be.EqualTo(targetprocess.Trim()), "Profile.PrioritiesMapping[bugzilla.Trim()].Name.Should(Be.EqualTo(targetprocess.Trim()))");
		}

		[Then("severity mapping should be:")]
		public void CheckSeverityMapping(string bugzilla, string targetprocess)
		{
			Profile.SeveritiesMapping[bugzilla.Trim()].Name.Should(Be.EqualTo(targetprocess.Trim()), "Profile.SeveritiesMapping[bugzilla.Trim()].Name.Should(Be.EqualTo(targetprocess.Trim()))");
		}

		[Then(@"profile should have tp users: (?<users>([^,]+,?\s*)+)")]
		public void CheckUsers(string[] users)
		{
			Storage.Get<UserDTO>().Select(x => x.Login).Distinct().ToArray().Should(Be.EquivalentTo(users), "Storage.Get<UserDTO>().Select(x => x.Login).Distinct().ToArray().Should(Be.EquivalentTo(users))");
		}

		[Then(@"profile should have tp states: (?<states>([^,]+,?\s*)+)")]
		public void CheckStates(string[] states)
		{
			foreach (var state in states)
			{
				Storage.Get<EntityStateDTO>().Select(x => x.Name).ToArray().Should(Contains.Item(state), "Storage.Get<EntityStateDTO>().Select(x => x.Name).ToArray().Should(Contains.Item(state))");
			}
		}

		[Then(@"profile should have tp priorities: (?<priorities>([^,]+,?\s*)+)")]
		public void CheckPriorities(string[] priorities)
		{
			foreach (var priority in priorities)
			{
				Storage.Get<PriorityDTO>().Select(x => x.Name).ToArray().Should(Contains.Item(priority), "Storage.Get<PriorityDTO>().Select(x => x.Name).ToArray().Should(Contains.Item(priority))");
			}
		}

		[Then(@"profile should have tp severities: (?<severities>([^,]+,?\s*)+)")]
		public void CheckSeverities(string[] severities)
		{
			foreach (var severity in severities)
			{
				Storage.Get<SeverityDTO>().Select(x => x.Name).ToArray().Should(Contains.Item(severity), "Storage.Get<SeverityDTO>().Select(x => x.Name).ToArray().Should(Contains.Item(severity))");
			}
		}

		[Then(@"profile should have tp roles: (?<roles>([^,]+,?\s*)+)")]
		public void CheckRoles(string[] roles)
		{
			foreach (var role in roles)
			{
				Storage.Get<RoleDTO>().Select(x => x.Name).ToArray().Should(Contains.Item(role), "Storage.Get<RoleDTO>().Select(x => x.Name).ToArray().Should(Contains.Item(role))");
			}
		}

		[Then(@"profile should have tp projects: (?<projects>([^,]+,?\s*)+)")]
		public void CheckProjects(string[] projects)
		{
			Storage.Get<ProjectDTO>().Select(p => p.Abbreviation).ToArray().Should(Be.EquivalentTo(projects), "Storage.Get<ProjectDTO>().Select(p => p.Abbreviation).ToArray().Should(Be.EquivalentTo(projects))");
		}

		[Then(@"profile should be initialized")]
		public void ProfileShouldBeInitialized()
		{
			Storage.Initialized.Should(Be.True, "Storage.Initialized.Should(Be.True)");
		}

		[Then("mapped users count shoud be $count")]
		public void CheckMappedUsersCount(int count)
		{
			Profile.UserMapping.Count.Should(Be.EqualTo(count), "Profile.UserMapping.Count.Should(Be.EqualTo(count))");
		}

		[Then("mapped states count shoud be $count")]
		public void CheckMappedStatesCount(int count)
		{
			Profile.StatesMapping.Count.Should(Be.EqualTo(count), "Profile.StatesMapping.Count.Should(Be.EqualTo(count))");
		}

		[Then("mapped priorities count shoud be $count")]
		public void CheckMappedPrioritiesCount(int count)
		{
			Profile.PrioritiesMapping.Count.Should(Be.EqualTo(count), "Profile.PrioritiesMapping.Count.Should(Be.EqualTo(count))");
		}

		[Then("mapped severities count shoud be $count")]
		public void CheckMappedSeveritiesCount(int count)
		{
			Profile.SeveritiesMapping.Count.Should(Be.EqualTo(count), "Profile.SeveritiesMapping.Count.Should(Be.EqualTo(count))");
		}

		[Then("plugin '$pluginName' should have empty account")]
		public void CheckAccount(string pluginName)
		{
			PluginShouldHaveAccount(pluginName, AccountName.Empty.Value);
		}

		[Then(@"bugzilla bug '$bugName' with external id $externalId for project '$projectName' should be stored in new profile")]
		public void CheckBug(string bugName, string externalId, string projectName)
		{
			var bugId = GetFirstBugIdByName(bugName);

			Storage.Get<TargetProcessBugId>(externalId).Single().Value.Should(Be.EqualTo(bugId), "Storage.Get<TargetProcessBugId>(externalId).Single().Value.Should(Be.EqualTo(bugId))");
			Storage.Get<BugzillaBugInfo>(bugId.ToString(CultureInfo.InvariantCulture)).Single().Id.Should(Be.EqualTo(externalId), "Storage.Get<BugzillaBugInfo>(bugId.ToString(CultureInfo.InvariantCulture)).Single().Id.Should(Be.EqualTo(externalId))");
		}

		[Then(@"bugzilla bug '$bugName' should have comments stored in profile: (?<comments>([^,]+,?\s*)+)")]
		public void CheckComments(string bugName, string[] comments)
		{
			var bugId = GetLastBugIdByName(bugName);
			
			Storage.Get<CommentDTO>(bugId.ToString(CultureInfo.InvariantCulture))
				.Select(c => c.Description)
				.ToArray()
				.Should(Be.EquivalentTo(comments), "Storage.Get<CommentDTO>(bugId.ToString(CultureInfo.InvariantCulture)).Select(c => c.Description).ToArray().Should(Be.EquivalentTo(comments))");
		}

		[Then(@"bugzilla bug '$bugName' should have attachments stored in profile: (?<attachments>([^,]+,?\s*)+)")]
		public void CheckAttachments(string bugName, string[] attachments)
		{
			var bugId = GetLastBugIdByName(bugName);

			Storage.Get<AttachmentDTO>().Where(c => c.GeneralID == bugId)
				.Select(c => c.Description)
				.ToArray()
				.Should(Be.EquivalentTo(attachments), "Storage.Get<AttachmentDTO>().Where(c => c.GeneralID == bugId).Select(c => c.Description).ToArray().Should(Be.EquivalentTo(attachments))");
		}

		[Then("profile should have $count saved bugs")]
		public void CheckBugsCount(int count)
		{
			Storage.Get<TargetProcessBugId>().Count().Should(Be.EqualTo(count), "Storage.Get<TargetProcessBugId>().Count().Should(Be.EqualTo(count))");
			Storage.Get<BugzillaBugInfo>().Count().Should(Be.EqualTo(count), "Storage.Get<BugzillaBugInfo>().Count().Should(Be.EqualTo(count))");
		}

		#endregion
	}
}
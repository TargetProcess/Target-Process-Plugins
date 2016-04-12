// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using AutoMapper;
using Tp.BugTracking;
using Tp.BugTracking.Settings;
using Tp.Integration.Common;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Plugin.Common.PluginLifecycle;
using Tp.LegacyProfileConvertsion.Common;
using log4net;
using PluginProfile = Tp.LegacyProfileConvertsion.Common.PluginProfile;

namespace Tp.Bugzilla.LegacyProfileConversion
{
	public class LegacyProfileConvertor : LegacyProfileConvertorBase<PluginProfile>
	{
		
		private readonly PluginInfoSender _initializer;
		private readonly LegacyExternalDataConvertor _externalDataConvertor;
		private readonly ILog _log;
		private const string CONVERTING = " _converting...";

		public LegacyProfileConvertor(IConvertorArgs args, IAccountCollection accountCollection, ILogManager logManager, PluginInitializer initializer)
			: base(args, accountCollection)
		{
			_log = logManager.GetLogger(GetType());
			_initializer = initializer;
			_externalDataConvertor = new LegacyExternalDataConvertor(_context);
		}

		public void Execute(string legacyProfileName)
		{
			var legacyProfile = GetLegacyProfile(legacyProfileName);
			
			_log.Info("Bugzilla legacy profile converter disabling legacy profile");
			DisableLegacyProfile(legacyProfile);
			_log.Info("Bugzilla legacy profile converter disabled legacy profile");

			var profile = legacyProfile;
			profile.ProfileName = legacyProfileName;

			_log.InfoFormat("Bugzilla legacy profile converter processing conversion for profile '{0}'", legacyProfileName);
			ExecuteForProfile(profile, GetAccount());
			_log.InfoFormat("Bugzilla legacy profile converter processed conversion for profile '{0}'", legacyProfileName);

			_log.Info("Bugzilla legacy profile converter updating legacy profile name");
			UpdateLegacyProfileName(legacyProfile);
			_log.Info("Bugzilla legacy profile converter updated legacy profile name");
		}

		private void UpdateLegacyProfileName(PluginProfile legacyProfile)
		{
			legacyProfile.ProfileName = legacyProfile.ProfileName.Replace(CONVERTING, string.Empty);

			_context.SubmitChanges();
		}

		private void DisableLegacyProfile(PluginProfile legacyProfile)
		{
			legacyProfile.Active = false;
			legacyProfile.ProfileName += CONVERTING;
			
			_context.SubmitChanges();
		}

		#region OnProfileMigrated

		protected override void OnProfileMigrated(IStorageRepository storageRepository, PluginProfile legacyProfile)
		{
			_initializer.SendInfoMessages();

			var project =
				_context.Projects.SingleOrDefault(p => p.ProjectID == storageRepository.GetProfile<BugzillaProfile>().Project);

			var process = project != null ? _context.Processes.SingleOrDefault(p => p.ProcessID == project.ProcessID) : null;

			MigrateUsers(storageRepository);
			MigrateProjects(storageRepository, project);
			MigrateStates(storageRepository, process);
			MigratePriorities(storageRepository);
			MigrateSeverities(storageRepository);
			MigrateRoles(storageRepository);

			_externalDataConvertor.MigrateBugzillaEntities(storageRepository, legacyProfile);
		}
		
		private void MigrateProjects(IStorageRepository storageRepository, Project project)
		{
			var general = _context.Generals.Single(g => g.GeneralID == project.ProjectID);

			storageRepository.Get<ProjectDTO>().Add(new ProjectDTO
			                                        	{
															Name = general.Name,
															Description = general.Description,
															CreateDate = general.CreateDate,
															StartDate = general.StartDate,
															EndDate = general.EndDate,
															LastCommentDate = general.LastCommentDate,
															LastCommentUserID = general.LastCommentUserID,
															ModifyDate = general.ModifyDate,
															NumericPriority = general.NumericPriority,
															OwnerID = general.OwnerID,
															ParentProjectID = general.ParentProjectID,
															LastEditorID = general.LastEditorID,
															CustomField1 = general.CustomField1,
															CustomField2 = general.CustomField2,
															CustomField3 = general.CustomField3,
															CustomField4 = general.CustomField4,
															CustomField5 = general.CustomField5,
															CustomField6 = general.CustomField6,
															CustomField7 = general.CustomField7,
															CustomField8 = general.CustomField8,
															CustomField9 = general.CustomField9,
															CustomField10 = general.CustomField10,
															CustomField11 = general.CustomField11,
															CustomField12 = general.CustomField12,
															CustomField13 = general.CustomField13,
															CustomField14 = general.CustomField14,
															CustomField15 = general.CustomField15,

			                                        		Abbreviation = project.Abbreviation,
															IsActive = project.IsActive,
															ProcessID = project.ProcessID,
															DeleteDate = project.DeleteDate,
															CompanyID = project.CompanyID,
															ID = project.ProjectID,
															ProjectID = project.ProjectID,
															IsProduct = project.IsProduct,
															InboundMailAutoCheck = project.InboundMailAutoCheck,
															InboundMailAutomaticalEmailCheckTime = project.InboundMailAutomaticalEmailCheckTime,
															InboundMailCreateRequests = project.InboundMailCreateRequests,
															InboundMailLogin = project.InboundMailLogin,
															InboundMailPassword = project.InboundMailPassword,
															InboundMailPort = project.InboundMailPort,
															InboundMailProtocol = project.InboundMailProtocol,
															InboundMailReplyAddress = project.InboundMailReplyAddress,
															InboundMailServer = project.InboundMailServer,
															InboundMailUseSSL = project.InboundMailUseSSL,
															IsInboundMailEnabled = project.IsInboundMailEnabled,
															ProgramOfProjectID = project.ProgramOfProjectID,
															SCConnectionString = project.SCConnectionString,
															SCPassword = project.SCPassword,
															SCStartingRevision = project.SCStartingRevision,
															SCUser = project.SCUser
			                                        	});
		}

		private void MigrateRoles(IStorageRepository storageRepository)
		{
			MigrateEntities<Role, RoleDTO>(storageRepository, _context.Roles);
		}

		private void MigrateSeverities(IStorageRepository storageRepository)
		{
			MigrateEntities<Severity, SeverityDTO>(storageRepository, _context.Severities);
		}

		private void MigratePriorities(IStorageRepository storageRepository)
		{
			MigrateEntities<Priority, PriorityDTO>(storageRepository,
														 _context.Priorities
															.Where(s => s.EntityTypeID == BugzillaConstants.BugEntityTypeId));
		}

		private void MigrateStates(IStorageRepository storageRepository, Process process)
		{
			MigrateEntities<EntityState, EntityStateDTO>(storageRepository,
			                                             _context.EntityStates
			                                             	.Where(s => process != null && s.ProcessID == process.ProcessID)
			                                             	.Where(s => s.EntityTypeID == BugzillaConstants.BugEntityTypeId));
		}

		private void MigrateEntities<TTpType, TDtoType>(IStorageRepository storageRepository, IEnumerable<TTpType> source)
		{
			Mapper.CreateMap<TTpType, TDtoType>();
			foreach (var entity in source)
			{
				var mapped = Mapper.Map<TTpType, TDtoType>(entity);
				storageRepository.Get<TDtoType>().Add(mapped);
			}
		}

		private void MigrateUsers(IStorageRepository storageRepository)
		{
			Mapper.CreateMap<TpUser, UserDTO>();
			foreach (var tpUser in _context.TpUsers.Where(x => x.Type == 1 || x.Type == 4))
			{
				var mapped = Mapper.Map<TpUser, UserDTO>(tpUser);
				if (mapped.IsNotDeletedUser())
				{
					storageRepository.Get<UserDTO>(mapped.UserID.ToString()).Add(mapped);
					storageRepository.Get<UserDTO>(mapped.Email).Add(mapped);
				}
			}
		}

		#endregion

		protected override IEnumerable<PluginProfile> GetLegacyProfiles()
		{
			return _context.PluginProfiles.Where(x => x.Active == true && x.PluginName == BugzillaConstants.LegacyPluginName);
		}

		private PluginProfile GetLegacyProfile(string name)
		{
			return
				_context.PluginProfiles.SingleOrDefault(
					x => x.PluginName == BugzillaConstants.LegacyPluginName && x.ProfileName == name);
		}

		#region ConvertToPluginProfile

		protected override PluginProfileDto ConvertToPluginProfile(PluginProfile legacyProfile)
		{
			var pluginProfileDto = new PluginProfileDto {Name = legacyProfile.ProfileName};

			var document = ConvertToXmlDocument(legacyProfile);
			var converted = Parse(document);
			pluginProfileDto.Settings = converted;

			return pluginProfileDto;
		}

		private static XmlDocument ConvertToXmlDocument(PluginProfile profile)
		{
			var document = new XmlDocument();
			document.LoadXml(profile.Settings);
			return document;
		}

		private BugzillaProfile Parse(XmlDocument document)
		{
			var result = new BugzillaProfile();
			var root = document.SelectSingleNode("./{0}".Fmt(LegacyBugzillaProfileFields.XmlRoot));

			result.Url = GetValueByName(root, LegacyBugzillaProfileFields.Url);
			result.Login = GetValueByName(root, LegacyBugzillaProfileFields.Login);
			result.Password = GetValueByName(root, LegacyBugzillaProfileFields.Password);
			result.Project = Int32.Parse(GetValueByName(root, LegacyBugzillaProfileFields.Project), CultureInfo.InvariantCulture);
			result.SavedSearches = GetValueByName(root, LegacyBugzillaProfileFields.Queries);

			var mapping = GetValueByName(root, LegacyBugzillaProfileFields.Maps);
			var parser = new LegacyMappingParser();

			if (!string.IsNullOrEmpty(mapping))
			{
				parser.Maps = mapping;

				MapUsers(result, parser);
				MapPriorities(result, parser);
				MapSeverities(result, parser);
				MapEntityState(result, parser);
				MapRoles(result, root);
			}

			return result;
		}

		private void MapRoles(BugzillaProfile result, XmlNode root)
		{
			var assigneeRole = CreateRole(DefaultRoles.Assignee, LegacyBugzillaProfileFields.AssigneeRole, root);
			AddRole(result, assigneeRole);

			var reporterRole = CreateRole(DefaultRoles.Reporter, LegacyBugzillaProfileFields.ReporterRole, root);
			AddRole(result, reporterRole);
		}

		private void AddRole(BugzillaProfile result, MappingElement role)
		{
			if(role != null)
			{
				result.RolesMapping.Add(role);
			}
		}

		private MappingElement CreateRole(string key, string xmlElementName, XmlNode root)
		{
			var mappingValue = GetValueByName(root, xmlElementName);

			return string.IsNullOrEmpty(mappingValue) 
				? null 
				: new MappingElement
			       	{
						Key = key,
						Value = Create(GetRoleBy(mappingValue, role => role.Name))
			       	};
		}

		private void MapEntityState(BugzillaProfile result, LegacyMappingParser parser)
		{
			var project = _context.Projects.Single(p => p.ProjectID == result.Project);
			var process = _context.Processes.SingleOrDefault(p => p.ProcessID == project.ProcessID);

			foreach (string state in parser.EntityStates.Keys)
			{
				Func<EntityState, string> comparator = x => x.Name;
				var tpState = GetEntityStateBy(process, BugzillaConstants.BugEntityTypeId, parser.EntityStates[state], comparator);

				if (tpState != null)
				{
					result.StatesMapping.Add(new MappingElement { Key = state, Value = Create(tpState) });
				}
			}
		}

		private void MapSeverities(IBugTrackingMappingSource result, LegacyMappingParser parser)
		{
			foreach (string severity in parser.Severities.Keys)
			{
				Func<Severity, string> comparator = x => x.Name;
				var tSeverity = GetSeverityBy(parser.Severities[severity], comparator);

				if (tSeverity != null)
				{
					result.SeveritiesMapping.Add(new MappingElement {Key = severity, Value = Create(tSeverity)});
				}
			}
		}

		private void MapPriorities(IBugTrackingMappingSource result, LegacyMappingParser parser)
		{
			foreach (string priority in parser.Priorities.Keys)
			{
				Func<Priority, string> comparator = x => x.Name;
				var tpPriority = GetPriorityBy(parser.Priorities[priority], comparator);

				if (tpPriority != null)
				{
					result.PrioritiesMapping.Add(new MappingElement {Key = priority, Value = Create(tpPriority)});
				}
			}
		}

		private void MapUsers(BugzillaProfile result, LegacyMappingParser parser)
		{
			foreach (string user in parser.Users.Keys)
			{
				var tpUser = GetUserForProjectBy(result.Project, parser.Users[user], x => x.Login);
				if (tpUser != null)
				{
					result.UserMapping.Add(new MappingElement {Key = user, Value = Create(tpUser)});
					continue;
				}

				tpUser = GetUserForProjectBy(result.Project, parser.Users[user], x => x.Email);
				if (tpUser != null)
				{
					result.UserMapping.Add(new MappingElement {Key = user, Value = Create(tpUser)});
				}
			}
		}

		private static MappingLookup Create(TpUser tpUser)
		{
			return new MappingLookup {Id = tpUser.UserID, Name = tpUser.Login};
		}

		private static MappingLookup Create(Priority priority)
		{
			return new MappingLookup {Id = priority.PriorityID, Name = priority.Name};
		}

		private static MappingLookup Create(Severity priority)
		{
			return new MappingLookup {Id = priority.SeverityID, Name = priority.Name};
		}

		private static MappingLookup Create(EntityState entityState)
		{
			return new MappingLookup { Id = entityState.EntityStateID, Name = entityState.Name };
		}

		private static MappingLookup Create(Role role)
		{
			return new MappingLookup { Id = role.RoleID, Name = role.Name };
		}
		
		protected TpUser GetUserByEmail(string userMail)
		{
			return _context.TpUsers.FirstOrDefault(x => x.Email.ToLower() == userMail.ToLower() && x.DeleteDate == null);
		}

		private static string GetValueByName(XmlNode root, string pathtoproject)
		{
			var node = root.SelectSingleNode(pathtoproject);
			return node != null ? node.InnerText : string.Empty;
		}

		#endregion
	}
}
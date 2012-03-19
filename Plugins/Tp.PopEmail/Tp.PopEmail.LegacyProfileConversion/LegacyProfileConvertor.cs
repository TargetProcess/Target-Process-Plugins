// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Xml;
using Tp.Integration.Common;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.LegacyProfileConvertsion.Common;
using Tp.PopEmailIntegration.Data;

namespace Tp.PopEmailIntegration.LegacyProfileConversion
{
	public class LegacyProfileConvertor : LegacyProfileConvertorBase<Project[]>
	{
		public LegacyProfileConvertor(IConvertorArgs args,IAccountCollection accountCollection): base(args, accountCollection)
		{
		}

		protected override void OnProfileMigrated(IStorageRepository storageRepository, Project[] legacyProfile)
		{
			MigrateUsers(storageRepository);
			MigrateUids(storageRepository);
			MigrateProjects(storageRepository);
			MigrateGlobalSetting(storageRepository);
		}

		private void MigrateGlobalSetting(IStorageRepository profile)
		{
			var globalSettingRepository = profile.Get<GlobalSettingDTO>();
			foreach (var globalSetting in _context.GlobalSettings)
			{
				globalSettingRepository.Add(new GlobalSettingDTO
				                            	{
				                            		CompanyName = globalSetting.CompanyName,
				                            		SMTPServer = globalSetting.SMTPServer,
				                            		SMTPPort = globalSetting.SMTPPort,
				                            		SMTPLogin = globalSetting.SMTPLogin,
				                            		SMTPPassword = globalSetting.SMTPPassword,
				                            		SMTPAuthentication = globalSetting.SMTPAuthentication,
				                            		SMTPSender = globalSetting.SMTPSender,
				                            		IsEmailNotificationsEnabled = globalSetting.IsEmailNotificationsEnabled,
				                            		HelpDeskPortalPath = globalSetting.HelpDeskPortalPath,
				                            		AppHostAndPath = globalSetting.AppHostAndPath,
				                            		NotifyRequester = globalSetting.NotifyRequester,
				                            		NotifyAutoCreatedRequester = globalSetting.NotifyAutoCreatedRequester,
				                            		DisableHttpAccess = globalSetting.DisableHttpAccess,
				                            		CsvExportDelimiter = globalSetting.CsvExportDelimiter,
				                            	});
			}
		}

		protected override IEnumerable<Project[]> GetLegacyProfiles()
		{
			var profiles = _context.Projects.Where(x => x.DeleteDate == null && x.IsActive == true).Where(
				x => x.IsInboundMailEnabled == true).GroupBy(
					x => x.InboundMailServer + x.InboundMailLogin);

			return profiles.Select(profile => profile.ToArray());
		}

		private void MigrateProjects(IStorageRepository profile)
		{
			var projectDtos = profile.Get<ProjectDTO>();
			foreach (var project in _context.Projects)
			{
				projectDtos.Add(new ProjectDTO
				                	{
				                		ID = project.ProjectID,
				                		CompanyID = project.CompanyID,
				                		Abbreviation = project.Abbreviation
				                	});
			}
		}

		private void MigrateUids(IStorageRepository profile)
		{
			var projectEmailProfile = profile.GetProfile<ProjectEmailProfile>();
			var uids = new MessageUidCollection();
			uids.AddRange(
				_context.MessageUids.Where(
					x => x.MailServer == projectEmailProfile.MailServer && projectEmailProfile.Login == x.MailLogin).Select(
						messageUid => messageUid.UID));
			profile.Get<MessageUidCollection>().Add(uids);
		}

		private void MigrateUsers(IStorageRepository pluginProfile)
		{
			if (pluginProfile == null)
			{
				throw new ArgumentNullException("pluginProfile");
			}
			var userRepository = new UserRepository(pluginProfile);
			foreach (var tpUser in _context.TpUsers.Where(x => x.Type == 1 || x.Type == 4))
			{
				userRepository.Add(CreateUserLite(tpUser));
			}
		}

		private static UserLite CreateUserLite(TpUser tpUser)
		{
			var userType = tpUser.Type == 1 ? UserType.User : UserType.Requester;

			return new UserLite
			       	{
			       		Id = tpUser.UserID,
			       		Email = tpUser.Email,
			       		UserType = userType,
			       		IsActive = tpUser.IsActive,
			       		DeleteDate = tpUser.DeleteDate,
			       		FirstName = tpUser.FirstName,
			       		LastName = tpUser.LastName,
			       		Login = tpUser.Login,
			       		CompanyId = tpUser.CompanyID
			       	};
		}

		protected override PluginProfileDto ConvertToPluginProfile(Project[] legacyProfile)
		{
			var rules = new List<ProfileRule>();
			rules.AddRange(GetRulesFromBinderProfiles(legacyProfile.First()));

			rules.AddRange(
				legacyProfile.Select(
					project => new ProfileRule(project.ProjectID.ToString(), project.InboundMailCreateRequests.GetValueOrDefault())));

			var pluginProfile = new ProjectEmailProfile
			                    	{
			                    		Login = legacyProfile.First().InboundMailLogin,
			                    		Password = legacyProfile.First().InboundMailPassword,
			                    		Port = legacyProfile.First().InboundMailPort.GetValueOrDefault(),
			                    		Protocol = legacyProfile.First().InboundMailProtocol,
			                    		MailServer = legacyProfile.First().InboundMailServer,
			                    		UseSSL = legacyProfile.First().InboundMailUseSSL.GetValueOrDefault(),
			                    		Rules = rules.Select(x => x.ToString()).ToString(Environment.NewLine)
			                    	};

			var profileName = legacyProfile.Aggregate("Email integration for ",
			                                          (current, project) =>
			                                          current + string.Format("Project {0} ", project.ProjectID));
			profileName = profileName.Substring(0, profileName.Length - 1);

			return new PluginProfileDto
			       	{
			       		Name = profileName,
			       		Settings = pluginProfile
			       	};
		}

		private IEnumerable<ProfileRule> GetRulesFromBinderProfiles(Project project)
		{
			var binderRules = new List<ProfileRule>();

			var bindEmailProfiles =
				_context.PluginProfiles.Where(x => x.PluginName == "Bind Email/Request To Project" && x.Active.GetValueOrDefault());
			foreach (var bindEmailProfile in bindEmailProfiles)
			{
				var xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(bindEmailProfile.Settings);
				var mapNode = xmlDocument.SelectSingleNode("/Settings/Map");
				if (mapNode == null)
				{
					continue;
				}

				var reader = new StringReader(mapNode.InnerText);
				var map = ReadMap(reader);

				foreach (DictionaryEntry keyValuePair in map)
				{
					var binderRule = binderRules.FirstOrDefault(x => x.ProjectId == keyValuePair.Key.ToString());
					if (binderRule == null)
					{
						binderRule = new ProfileRule(keyValuePair.Key.ToString(), project.InboundMailCreateRequests.GetValueOrDefault());
						binderRules.Add(binderRule);
					}

					binderRule.AddSubjectContainsCondition(keyValuePair.Value.ToString());
				}
			}

			return binderRules;
		}

		private static StringDictionary ReadMap(TextReader reader)
		{
			var map = new StringDictionary();

			var mappings = reader.ReadToEnd().Split(new[] {"_$_"}, StringSplitOptions.RemoveEmptyEntries);
			foreach (var mapping in mappings)
			{
				var parts = mapping.Split(new[] {"_&_"}, StringSplitOptions.RemoveEmptyEntries);
				map.Add(parts[0], parts[1]);
			}
			return map;
		}
	}
}

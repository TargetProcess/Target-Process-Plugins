// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AutoMapper;
using Tp.Integration.Common;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Plugin.Common.PluginLifecycle;
using Tp.LegacyProfileConvertsion.Common;
using Tp.SourceControl.RevisionStorage;
using log4net;
using PluginProfile = Tp.LegacyProfileConvertsion.Common.PluginProfile;

namespace Tp.Tfs.LegacyProfileConversion
{
	public class LegacyProfileConvertor : LegacyProfileConvertorBase<PluginProfile>
	{
		public const string PluginName = "Team Foundation Server Integration";
		private const string Converting = " _converting...";

		private readonly PluginInfoSender _initializer;
		private readonly ILog _log;

		public LegacyProfileConvertor(
				IConvertorArgs args,
				IAccountCollection accountCollection,
				ILogManager logManager,
				PluginInitializer initializer)
			: base(args, accountCollection)
		{
			_log = logManager.GetLogger(GetType());
			_initializer = initializer;
		}

		public void Execute(string legacyProfileName)
		{
			var legacyProfile = GetLegacyProfile(legacyProfileName);

			_log.Info("TFS legacy profile converter disabling legacy profile");
			DisableLegacyProfile(legacyProfile);
			_log.Info("TFS legacy profile converter disabled legacy profile");

			var profile = legacyProfile;
			profile.ProfileName = legacyProfileName;

			_log.InfoFormat("TFS legacy profile converter processing conversion for profile '{0}'", legacyProfileName);
			ExecuteForProfile(profile, GetAccount());
			_log.InfoFormat("TFS legacy profile converter processed conversion for profile '{0}'", legacyProfileName);

			_log.Info("TFS legacy profile converter updating legacy profile name");
			UpdateLegacyProfileName(legacyProfile);
			_log.Info("TFS legacy profile converter updated legacy profile name");
		}

		private PluginProfile GetLegacyProfile(string name)
		{
			return _context.PluginProfiles.SingleOrDefault(x => x.PluginName == PluginName && x.ProfileName == name);
		}

		private void DisableLegacyProfile(PluginProfile legacyProfile)
		{
			legacyProfile.Active = false;

			_context.SubmitChanges();
		}

		private void UpdateLegacyProfileName(PluginProfile legacyProfile)
		{
			legacyProfile.ProfileName = legacyProfile.ProfileName.Replace(Converting, string.Empty);

			_context.SubmitChanges();
		}

		protected override void CreateNewProfileName(PluginProfileDto pluginProfile, IAccount account)
		{
			do
			{
				pluginProfile.Name = String.Concat(pluginProfile.Name, "_converted");
			} while (account.Profiles.Any(x => x.Name == pluginProfile.Name));
		}

		protected override void OnProfileMigrated(IStorageRepository storageRepository, PluginProfile legacyProfile)
		{
			_initializer.SendInfoMessages();

			MigrateUsers(storageRepository);
			MigrateRevisions(legacyProfile, storageRepository);
		}

		public void MigrateRevisions(PluginProfile legacyProfile, IStorageRepository storageRepository)
		{
			var alreadyImportedRevisions = _context.Revisions.Where(x => x.PluginProfileID == legacyProfile.PluginProfileID);

			foreach (var revision in alreadyImportedRevisions)
			{
				storageRepository.Get<RevisionIdRelation>(revision.RevisionID.ToString())
					.ReplaceWith(new RevisionIdRelation { RevisionId = revision.SourceControlID.ToString(), TpId = revision.RevisionID });
			}
		}

		private void MigrateUsers(IStorageRepository storageRepository)
		{
			if (storageRepository == null)
			{
				throw new ArgumentNullException("storageRepository");
			}
			Mapper.CreateMap<TpUser, UserDTO>();
			foreach (var tpUser in _context.TpUsers.Where(x => x.Type == 1 || x.Type == 4))
			{
				storageRepository.Get<UserDTO>().Add(Mapper.Map<TpUser, UserDTO>(tpUser));
			}
		}

		protected override IEnumerable<PluginProfile> GetLegacyProfiles()
		{
			return _context.PluginProfiles.Where(x => x.Active == true && x.PluginName == "Team Foundation Server Integration");
		}

		private static XmlDocument ConvertToXmlDocument(PluginProfile profile)
		{
			var document = new XmlDocument();
			document.LoadXml(profile.Settings);
			return document;
		}

		protected override PluginProfileDto ConvertToPluginProfile(PluginProfile legacyProfile)
		{
			var pluginProfileDto = new PluginProfileDto { Name = legacyProfile.ProfileName };

			var document = ConvertToXmlDocument(legacyProfile);
			var converted = Parse(legacyProfile, document);
			pluginProfileDto.Settings = converted;

			return pluginProfileDto;
		}

		private TfsPluginProfile Parse(PluginProfile legacyProfile, XmlDocument document)
		{
			var result = new TfsPluginProfile();
			var root = document.SelectSingleNode("./Settings");

			string server = GetValueByName(root, "ServerName").TrimEnd('/');
			string projectName = GetValueByName(root, "TeamProjectName");
			result.Uri = string.Concat(server, "/", projectName);

			var syncIntervalValue = GetValueByName(root, "SyncInterval");
			if (!string.IsNullOrEmpty(syncIntervalValue))
			{
				result.SynchronizationInterval = Int32.Parse(syncIntervalValue);
			}

			result.Login = GetValueByName(root, "Login");
			result.Password = GetValueByName(root, "Password");

			result.StartRevision = GetStartRevision(legacyProfile, root);

			var userMapping = GetValueByName(root, "Maps");
			var parser = new LegacyMappingParser();
			if (!string.IsNullOrEmpty(userMapping))
			{
				parser.Maps = userMapping;
			}
			foreach (string tfsUser in parser.Users.Keys)
			{
				var tpUser = GetUserBy(parser.Users[tfsUser], x => x.Login);
				if (tpUser != null)
				{
					result.UserMapping.Add(new MappingElement { Key = tfsUser, Value = Create(tpUser) });
					continue;
				}

				tpUser = GetUserBy(parser.Users[tfsUser], x => x.Email);
				if (tpUser != null)
				{
					result.UserMapping.Add(new MappingElement { Key = tfsUser, Value = Create(tpUser) });
				}
			}

			return result;
		}

		protected override bool IsUserTypeCorrect(TpUser user)
		{
			return user.Type == 1;
		}

		private static MappingLookup Create(TpUser tpUser)
		{
			return new MappingLookup { Id = tpUser.UserID, Name = tpUser.Login };
		}

		protected TpUser GetUserByEmail(string userMail)
		{
			return _context.TpUsers.FirstOrDefault(x => x.Email.ToLower() == userMail.ToLower() && x.DeleteDate == null);
		}

		private string GetStartRevision(PluginProfile legacyProfile, XmlNode root)
		{
			var pluginProfileID = legacyProfile.PluginProfileID;
			var alreadyImportedRevisions = _context.Revisions.Where(x => x.PluginProfileID == pluginProfileID);
			var revisionFromLegacyProfile = RevisionFromLegacyProfile(root);

			if (!alreadyImportedRevisions.Empty())
			{
				var lastImportedRevision = alreadyImportedRevisions.Max(x => x.SourceControlID);

				return (lastImportedRevision >= revisionFromLegacyProfile
									? (lastImportedRevision + 1).ToString()
									: revisionFromLegacyProfile.ToString());
			}

			return revisionFromLegacyProfile.ToString();
		}

		private static long RevisionFromLegacyProfile(XmlNode root)
		{
			string revisionFromLegacyProfile = GetValueByName(root, "StartRevision");
			if (string.IsNullOrEmpty(revisionFromLegacyProfile) || revisionFromLegacyProfile == "0")
			{
				return 1;
			}

			return Convert.ToInt64(revisionFromLegacyProfile);
		}

		private static string GetValueByName(XmlNode root, string pathtoproject)
		{
			var node = root.SelectSingleNode(pathtoproject);
			return node != null ? node.InnerText : string.Empty;
		}
	}
}
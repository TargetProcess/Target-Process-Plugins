// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
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
using Tp.Integration.Plugin.Common.Mapping;
using Tp.LegacyProfileConvertsion.Common;
using Tp.SourceControl.RevisionStorage;
using PluginProfile = Tp.LegacyProfileConvertsion.Common.PluginProfile;

namespace Tp.Subversion.LegacyProfileConversion
{
	public class LegacyProfileConvertor : LegacyProfileConvertorBase<PluginProfile>
	{
		public LegacyProfileConvertor(IConvertorArgs args, IAccountCollection accountCollection)
			: base(args, accountCollection)
		{
		}

		protected override void OnProfileMigrated(IStorageRepository storageRepository, PluginProfile legacyProfile)
		{
			MigrateUsers(storageRepository);
			MigrateRevisions(legacyProfile, storageRepository);
		}

		public void MigrateRevisions(PluginProfile legacyProfile, IStorageRepository storageRepository)
		{
			var alreadyImportedRevisions = _context.Revisions.Where(x => x.PluginProfileID == legacyProfile.PluginProfileID);

			foreach (var revision in alreadyImportedRevisions)
			{
				storageRepository.Get<RevisionIdRelation>(revision.RevisionID.ToString())
					.ReplaceWith(new RevisionIdRelation {RevisionId = revision.SourceControlID.ToString(), TpId = revision.RevisionID});
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
			return _context.PluginProfiles.Where(x => x.Active == true && x.PluginName == "Subversion Integration");
		}

		private static XmlDocument ConvertToXmlDocument(PluginProfile profile)
		{
			var document = new XmlDocument();
			document.LoadXml(profile.Settings);
			return document;
		}

		protected override PluginProfileDto ConvertToPluginProfile(PluginProfile legacyProfile)
		{
			var pluginProfileDto = new PluginProfileDto {Name = legacyProfile.ProfileName};

			var document = ConvertToXmlDocument(legacyProfile);
			var converted = Parse(legacyProfile, document);
			pluginProfileDto.Settings = converted;

			return pluginProfileDto;
		}

		private SubversionPluginProfile Parse(PluginProfile legacyProfile, XmlDocument document)
		{
			var result = new SubversionPluginProfile();
			var root = document.SelectSingleNode("./Settings");

			result.Uri = GetValueByName(root, "PathToProject");

			var syncIntervalValue = GetValueByName(root, "SyncInterval");
			if (!string.IsNullOrEmpty(syncIntervalValue))
			{
				result.SynchronizationInterval = Int32.Parse(syncIntervalValue);
			}

			result.Login = GetValueByName(root, "SubversionLogin");
			result.Password = GetValueByName(root, "SubversionPassword");

			result.StartRevision = GetStartRevision(legacyProfile, root);

			var userMapping = GetValueByName(root, "Maps");
			var parser = new LegacyMappingParser();
			if (!string.IsNullOrEmpty(userMapping))
			{
				parser.Maps = userMapping;
			}
			foreach (string svnUser in parser.Users.Keys)
			{
				var tpUser = GetUserBy(parser.Users[svnUser], x => x.Login);
				if (tpUser != null)
				{
					result.UserMapping.Add(new MappingElement {Key = svnUser, Value = Create(tpUser)});
					continue;
				}

				tpUser = GetUserBy(parser.Users[svnUser], x => x.Email);
				if (tpUser != null)
				{
					result.UserMapping.Add(new MappingElement {Key = svnUser, Value = Create(tpUser)});
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
			return new MappingLookup {Id = tpUser.UserID, Name = tpUser.Login};
		}

		protected TpUser GetUserByEmail(string userMail)
		{
			return _context.TpUsers.FirstOrDefault(x => x.Email.ToLower() == userMail.ToLower() && x.DeleteDate == null);
		}

		private string GetStartRevision(PluginProfile legacyProfile, XmlNode root)
		{
			var pluginProfileID = legacyProfile.PluginProfileID;
			var alreadyImportedRevisions = _context.Revisions.Where(x => x.PluginProfileID == pluginProfileID);
			if (!alreadyImportedRevisions.Empty())
			{
				var lastImportedRevision = alreadyImportedRevisions.Max(x => x.SourceControlID);

				var revisionFromLegacyProfile = RevisionFromLegacyProfile(root);

				return (lastImportedRevision > revisionFromLegacyProfile
				        	? (lastImportedRevision + 1).ToString()
				        	: revisionFromLegacyProfile.ToString());
			}

			return RevisionFromLegacyProfile(root).ToString();
		}

		private static long RevisionFromLegacyProfile(XmlNode root)
		{
			string revisionFromLegacyProfile = GetValueByName(root, "StartRevision");
			if (string.IsNullOrEmpty(revisionFromLegacyProfile))
			{
				return 0;
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
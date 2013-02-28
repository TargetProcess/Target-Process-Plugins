// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using Tp.BugTracking.ImportToTp;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.LegacyProfileConvertsion.Common;

namespace Tp.Bugzilla.LegacyProfileConversion
{
	public class LegacyExternalDataConvertor
	{
		private readonly TpDatabaseDataContext _context;

		public LegacyExternalDataConvertor(TpDatabaseDataContext context)
		{
			_context = context;
			Mapper.CreateMap<Comment, CommentDTO>();
			Mapper.CreateMap<Attachment, AttachmentDTO>();
		}

		public void MigrateBugzillaEntities(IStorageRepository storageRepository, PluginProfile legacyProfile)
		{
			MigrateBugs(storageRepository, legacyProfile);
		}
		
		private void MigrateBugs(IStorageRepository storageRepository, PluginProfile legacyProfile)
		{
			var bugs = _context.ExternalReferences
				.Where(r => r.EntityTypeID == BugzillaConstants.BugEntityTypeId)
				.Where(r => r.PluginProfileID == legacyProfile.PluginProfileID)
				.ToList()
				.Distinct(new ExternalIdComparer())
				.Join(_context.Bugs, e => e.TpID, b => b.BugID, (e, b) => new {TpId = b.BugID, ExternalId = e.ExternalID})
				.ToList();

			var tpBugs = new List<int>();
			foreach (var externalReference in bugs)
			{
				var tpId = externalReference.TpId;
				var externalId = externalReference.ExternalId;

				storageRepository.Get<TargetProcessBugId>(externalId).Add(new TargetProcessBugId {Value = tpId});
				storageRepository.Get<BugzillaBugInfo>(tpId.ToString(CultureInfo.InvariantCulture))
					.Add(new BugzillaBugInfo
					     	{
					     		Id = externalId,
					     		TpId = tpId
					     	});

				foreach (var team in _context.Teams.Where(t => t.AssignableID == tpId))
				{
					storageRepository.Get<TeamDTO>(team.TeamID.ToString(CultureInfo.InvariantCulture)).Clear();
					storageRepository.Get<TeamDTO>(team.TeamID.ToString(CultureInfo.InvariantCulture)).Add(
						new TeamDTO
						{
							AssignableID = team.AssignableID,
							ID = team.TeamID,
							RoleID = team.RoleID,
							TeamID = team.TeamID,
							UserID = team.UserID
						});
				}

				tpBugs.Add(tpId);
			}

			MigrateBugsEntities(tpBugs, storageRepository);
		}

		private void MigrateBugsEntities(List<int> tpBugs, IStorageRepository storageRepository)
		{
			const int take = 500;
			var skip = 0;
			var chain = tpBugs.Skip(skip).Take(take).ToList();

			while (chain.Any())
			{
				MigrateAttachments(chain, storageRepository);
				MigrateComments(chain, storageRepository);

				skip += take;
				chain = tpBugs.Skip(skip).Take(take).ToList();
			}
		}

		private void MigrateAttachments(IEnumerable<int> bugIds, IStorageRepository storageRepository)
		{
			var attachs = _context.Attachments.Where(c => c.GeneralID.HasValue && bugIds.Contains(c.GeneralID.Value)).ToList();

			storageRepository.Get<AttachmentDTO>()
				.AddRange(attachs.Select(Mapper.Map<Attachment, AttachmentDTO>));
		}

		private void MigrateComments(IEnumerable<int> bugIds, IStorageRepository storageRepository)
		{
			var comments =
				_context.Comments.Where(c => c.GeneralID.HasValue && bugIds.Contains(c.GeneralID.Value)).GroupBy(c => c.GeneralID).
					ToList();

			foreach (var comment in comments)
			{
				storageRepository.Get<CommentDTO>(comment.Key.Value.ToString(CultureInfo.InvariantCulture))
					.AddRange(comment.Select(Mapper.Map<Comment, CommentDTO>));
			}
		}
	}

	public class ExternalIdComparer : IEqualityComparer<ExternalReference>
	{
		public bool Equals(ExternalReference x, ExternalReference y)
		{
			return x.ExternalID == y.ExternalID;
		}

		public int GetHashCode(ExternalReference obj)
		{
			return obj.ExternalID.GetHashCode();
		}
	}
}
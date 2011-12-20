// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Plugin.Core.Attachments;

namespace Tp.Bugzilla.ImportToTp
{
	public class AttachmentImporter : IHandleMessages<NewBugImportedToTargetProcessMessage>,
	                                  IHandleMessages<ExistingBugImportedToTargetProcessMessage>
	{
		private readonly IStorageRepository _storageRepository;
		private readonly ILocalBus _localBus;
		private readonly IActivityLogger _logger;

		public AttachmentImporter(IStorageRepository storageRepository, ILocalBus localBus, IActivityLogger logger)
		{
			_storageRepository = storageRepository;
			_localBus = localBus;
			_logger = logger;
		}

		public void Handle(NewBugImportedToTargetProcessMessage message)
		{
			PushAttachmentsToTp(message.TpBugId, message.BugzillaBug, message.BugzillaBug.attachmentCollection);
		}

		public void Handle(ExistingBugImportedToTargetProcessMessage message)
		{
			var newAttachments =
				message.BugzillaBug.attachmentCollection.Where(attachment => !AttachmentExists(message, attachment)).ToList();

			AttachmentFolder.Delete(message.BugzillaBug.attachmentCollection.Except(newAttachments).Select(x => x.FileId));

			PushAttachmentsToTp(message.TpBugId, message.BugzillaBug, newAttachments);
		}

		private void PushAttachmentsToTp(int? tpBugId, BugzillaBug bug, List<LocalStoredAttachment> attachments)
		{
			_logger.InfoFormat("Processing attachments. Bug: {0}", bug.ToString());
			_localBus.SendLocal(new PushAttachmentsToTpCommandInternal
			                    	{
			                    		LocalStoredAttachments = attachments.ToArray(),
			                    		GeneralId = tpBugId
			                    	});
		}

		private bool AttachmentExists(ExistingBugImportedToTargetProcessMessage message, LocalStoredAttachment attachment)
		{
			return _storageRepository.Get<AttachmentDTO>()
				.Where(c => c.GeneralID == message.TpBugId)
				.Where(c => c.CreateDate == attachment.CreateDate)
				.Any();
		}
	}
}
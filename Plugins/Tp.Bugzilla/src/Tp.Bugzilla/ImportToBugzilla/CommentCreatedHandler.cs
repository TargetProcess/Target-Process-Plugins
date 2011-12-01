// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus;
using Tp.Bugzilla.BugFieldConverters;
using Tp.Bugzilla.BugzillaQueries;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Bugzilla.ImportToBugzilla
{
	public class CommentCreatedHandler : IHandleMessages<CommentCreatedMessage>
	{
		private readonly IStorageRepository _storage;
		private readonly IBugzillaInfoStorageRepository _bugzillaInfoStorageRepository;
		private readonly IBugzillaService _bugzillaService;
		private readonly IBugzillaActionFactory _actionFactory;

		public CommentCreatedHandler(IStorageRepository storage, IBugzillaInfoStorageRepository bugzillaInfoStorageRepository,
		                             IBugzillaService bugzillaService, IBugzillaActionFactory actionFactory)
		{
			_storage = storage;
			_bugzillaInfoStorageRepository = bugzillaInfoStorageRepository;
			_bugzillaService = bugzillaService;
			_actionFactory = actionFactory;
		}

		public void Handle(CommentCreatedMessage message)
		{
			if (!NeedToProcess(message.Dto)) return;

			var offset = _bugzillaService.GetTimeOffset();

			_bugzillaService.Execute(_actionFactory.GetCommentAction(message.Dto, offset));

			_storage.Get<CommentDTO>(message.Dto.GeneralID.ToString()).Add(message.Dto);
		}

		private bool NeedToProcess(CommentDTO dto)
		{
			return !dto.Description.Equals(CommentConverter.StateIsChangedComment, StringComparison.InvariantCultureIgnoreCase)
				&& _bugzillaInfoStorageRepository.GetBugzillaBug(dto.GeneralID) != null;
		}
	}
}
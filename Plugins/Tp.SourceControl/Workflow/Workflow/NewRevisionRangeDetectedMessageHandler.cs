// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.SourceControl.Comments;
using Tp.SourceControl.Messages;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.Workflow.Workflow
{
	public class NewRevisionRangeDetectedMessageHandler : IHandleMessages<NewRevisionRangeDetectedLocalMessage>
	{
		private readonly IVersionControlSystem _versionControlSystem;
		private readonly ILocalBus _bus;
		private readonly IActivityLogger _logger;
		private readonly CommentParser _parser;

		public NewRevisionRangeDetectedMessageHandler(IVersionControlSystem versionControlSystem, ILocalBus bus, IActivityLogger logger)
		{
			_versionControlSystem = versionControlSystem;
			_bus = bus;
			_logger = logger;
			_parser = new CommentParser();
		}

		public void Handle(NewRevisionRangeDetectedLocalMessage message)
		{
			_logger.Info("Retrieving new revisions");
			var revisions = _versionControlSystem.GetRevisions(message.Range);
			_logger.Info("Filtering out non-assignable revisions");
			revisions = revisions.Where(ContainsEntityId).ToArray();

			_logger.InfoFormat("Revisions retrieved. Revision IDs: {0}", string.Join(", ", revisions.Select(x => x.Id.Value).ToArray()));

			SendLocal(revisions);
		}

		private bool ContainsEntityId(RevisionInfo revisionInfo)
		{
			return !_parser.ParseAssignToEntityAction(revisionInfo).Empty();
		}

		private void SendLocal(IEnumerable<RevisionInfo> revisions)
		{
			var messages = revisions.Select(x => new NewRevisionDetectedLocalMessage {Revision = x});
			messages.ForEach(x => _bus.SendLocal(x));
		}
	}

	[Serializable]
	public class NewRevisionRangeDetectedLocalMessage : IPluginLocalMessage
	{
		public RevisionRange Range { get; set; }
	}
}
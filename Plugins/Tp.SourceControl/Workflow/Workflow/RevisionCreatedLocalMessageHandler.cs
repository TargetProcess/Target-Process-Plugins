// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using Tp.Integration.Plugin.Common;
using Tp.SourceControl.Comments;
using Tp.SourceControl.Messages;

namespace Tp.SourceControl.Workflow.Workflow
{
	public class RevisionCreatedLocalMessageHandler
		: IHandleMessages<RevisionCreatedLocalMessage>
	{
		private readonly ILocalBus _bus;

		public RevisionCreatedLocalMessageHandler(ILocalBus bus)
		{
			_bus = bus;
		}

		public void Handle(RevisionCreatedLocalMessage message)
		{
			var commentParser = new CommentParser();

			var actions = commentParser.ParseAssignToEntityAction(message.Dto);

			var actionParamFiller = new ActionParameterFillerVisitor(message.Dto);

			foreach (var action in actions)
			{
				action.Execute(actionParamFiller, x => { _bus.SendLocal(action); });
			}
		}
	}
}
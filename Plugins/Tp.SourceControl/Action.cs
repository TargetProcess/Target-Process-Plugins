//  
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Plugin.Common.Activity;
using Tp.SourceControl.Comments;

namespace Tp.SourceControl
{
	public abstract class Action : IAction
	{
		protected Action()
		{
			Children = new List<IAction>();
		}

		protected abstract void Visit(IActionVisitor visitor);

		public void Execute(IActionVisitor visitor, Action<ITargetProcessCommand> executor, IActivityLogger logger)
		{
			Visit(visitor);

			if (CanBeExecuted)
			{
				Log(logger);

				executor(CreateCommand());
			}
		}

		public List<IAction> Children { get; private set; }

		protected abstract bool CanBeExecuted { get; }

		protected abstract ITargetProcessCommand CreateCommand();

		protected abstract void Log(IActivityLogger logger);
	}
}
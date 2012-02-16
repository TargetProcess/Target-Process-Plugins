// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Activity;

namespace Tp.SourceControl.Comments
{
	public interface IAction : IPluginLocalMessage
	{
		void Execute(IActionVisitor visitor, Action<ITargetProcessCommand> executor, IActivityLogger logger);

		List<IAction> Children { get; }
	}
}
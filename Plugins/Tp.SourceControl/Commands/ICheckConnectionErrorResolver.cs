// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.

using System;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.SourceControl.Commands
{
	public interface ICheckConnectionErrorResolver
	{
		void HandleConnectionError(Exception exception, PluginProfileErrorCollection errors);
	}
}
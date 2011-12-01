// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using NServiceBus;
using Tp.Integration.Messages.TargetProcessLifecycle;

namespace Tp.Integration.Plugin.Common.Handlers
{
	public class ExceptionHandler : IHandleMessages<TargetProcessExceptionThrownMessage>
	{
		public void Handle(TargetProcessExceptionThrownMessage message)
		{
		}
	}
}
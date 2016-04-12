// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using NServiceBus;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common.Activity;

namespace Tp.Integration.Plugin.Common.Handlers
{
	public class ExceptionHandler : IHandleMessages<TargetProcessExceptionThrownMessage>
	{
		private readonly IActivityLogger _logger;

		public ExceptionHandler(IActivityLogger logger)
		{
			_logger = logger;
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			//_logger.Error(string.Format("TargetProcess has thrown an exception : {0}",message.ExceptionString));
		}
	}
}
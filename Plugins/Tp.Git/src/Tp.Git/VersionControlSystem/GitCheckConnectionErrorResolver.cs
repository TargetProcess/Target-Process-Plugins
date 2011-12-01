// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.

using System;
using NGit.Api.Errors;
using NGit.Errors;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;

namespace Tp.Git.VersionControlSystem
{
	public class GitCheckConnectionErrorResolver : ICheckConnectionErrorResolver
	{
		public void HandleConnectionError(Exception exception, PluginProfileErrorCollection errors)
		{
			if (exception.InnerException is TransportException)
			{
				if (exception.InnerException.Message.ToLower().Contains("not authorized"))
				{
					errors.Add(new PluginProfileError { FieldName = "Login", Message = exception.InnerException.Message });
					errors.Add(new PluginProfileError { FieldName = "Password", Message = exception.InnerException.Message });
				}
				return;
			}

			var fieldName = string.Empty;
			if (exception is JGitInternalException)
			{
				if (exception.Message.ToLower().Contains("invalid remote") || exception.Message.ToLower().Contains("uri"))
				{
					fieldName = "Uri";
				}
			}

			if (exception is InvalidRevisionException)
			{
				fieldName = "Revision";
			}

			errors.Add(new PluginProfileError {FieldName = fieldName, Message = exception.Message});
		}
	}
}
// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using Microsoft.TeamFoundation.Framework.Client;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;

namespace Tp.Tfs.VersionControlSystem
{
	public class TfsCheckConnectionErrorResolver : ICheckConnectionErrorResolver
	{
		private readonly ILogManager _log;
		public const string INVALID_URI_OR_INSUFFICIENT_ACCESS_RIGHTS_ERROR_MESSAGE = "invalid uri or insufficient access rights";

		public TfsCheckConnectionErrorResolver(ILogManager log)
		{
			_log = log;
		}

		public void HandleConnectionError(Exception exception, PluginProfileErrorCollection errors)
		{
			_log.GetLogger("Tfs").Warn("Check connection failed", exception);
			exception = exception.InnerException ?? exception;
			const string uriFieldName = "Uri";
			if (exception is TeamFoundationServiceException)
			{
				errors.Add(new PluginProfileError { FieldName = "Login", Message = exception.Message });
				errors.Add(new PluginProfileError { FieldName = "Password", Message = exception.Message });
				errors.Add(new PluginProfileError { FieldName = uriFieldName, Message = exception.Message });
				return;
			}

			if (exception is ArgumentNullException || exception is DirectoryNotFoundException)
			{
				errors.Add(new PluginProfileError { FieldName = uriFieldName, Message = INVALID_URI_OR_INSUFFICIENT_ACCESS_RIGHTS_ERROR_MESSAGE });
			}

			var fieldName = string.Empty;
			if (exception is InvalidRevisionException)
			{
				fieldName = "Revision";
			}

			errors.Add(new PluginProfileError { FieldName = fieldName, Message = exception.Message });
		}
	}
}
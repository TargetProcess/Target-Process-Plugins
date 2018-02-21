﻿// 
// Copyright (c) 2005-2017 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NGit.Api.Errors;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;

namespace Tp.Git.VersionControlSystem
{
    public class NGitCheckConnectionErrorResolver : ICheckConnectionErrorResolver
    {
        private readonly ILogManager _log;
        public const string INVALID_URI_OR_INSUFFICIENT_ACCESS_RIGHTS_ERROR_MESSAGE = "invalid uri or insufficient access rights";

        public NGitCheckConnectionErrorResolver(ILogManager log)
        {
            _log = log;
        }

        public void HandleConnectionError(Exception exception, PluginProfileErrorCollection errors)
        {
            _log.GetLogger("Git").Warn("Check connection failed", exception);
            exception = exception.InnerException ?? exception;
            const string uriFieldName = "Uri";
            if (exception is NGit.Errors.TransportException)
            {
                errors.Add(new PluginProfileError { FieldName = "Login", Message = exception.Message });
                errors.Add(new PluginProfileError { FieldName = "Password", Message = exception.Message });
                errors.Add(new PluginProfileError { FieldName = uriFieldName, Message = exception.Message });
                return;
            }

            if (exception is ArgumentNullException)
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = uriFieldName,
                    Message = INVALID_URI_OR_INSUFFICIENT_ACCESS_RIGHTS_ERROR_MESSAGE
                });
            }

            var fieldName = string.Empty;
            if (exception is JGitInternalException && (exception.Message.ToLower().Contains("invalid remote") || exception.Message.ToLower().Contains("uri")))
            {
                fieldName = uriFieldName;
            }            

            errors.Add(new PluginProfileError { FieldName = fieldName, Message = exception.Message });
        }
    }
}

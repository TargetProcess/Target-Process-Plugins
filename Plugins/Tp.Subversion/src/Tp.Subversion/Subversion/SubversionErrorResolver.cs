// 
// Copyright (c) 2005-2015 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.

using System;
using SharpSvn;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Subversion.Subversion
{
    public class SubversionErrorResolver : ICheckConnectionErrorResolver
    {
        public void HandleConnectionError(Exception exception, PluginProfileErrorCollection errors)
        {
            if (exception is VersionControlException)
            {
                HandleConnectionError((SvnException) exception.InnerException, errors);
            }
            else if (exception is UriFormatException)
            {
                HandleConnectionError(errors);
            }
            else
            {
                throw exception;
            }
        }

        private static void HandleConnectionError(SvnException e, PluginProfileErrorCollection errors)
        {
            if (e is SvnAuthorizationException)
            {
                errors.Add(ErrorFor(ConnectionSettings.LoginField, "Authorization failed"));
                errors.Add(ErrorFor(ConnectionSettings.PasswordField));
            }
            else if (e is SvnAuthenticationException || e.RootCause is SvnAuthenticationException)
            {
                errors.Add(ErrorFor(ConnectionSettings.LoginField, "Authentication failed"));
                errors.Add(ErrorFor(ConnectionSettings.PasswordField));
            }
            else if (e.Message.Contains("200 OK"))
            {
                errors.Add(ErrorFor(ConnectionSettings.UriField, "Invalid path to repository"));
            }
            else if (e is SvnRepositoryIOException)
            {
                errors.Add(ErrorFor(ConnectionSettings.UriField, "Could not connect to server"));
            }
            else if (e is SvnFileSystemException || e is SvnClientUnrelatedResourcesException)
            {
                errors.Add(ErrorFor(SubversionPluginProfile.StartRevisionField));
            }
            else
            {
                errors.Add(ErrorFor(ConnectionSettings.UriField, e.Message.Fmt("Connection failed. {0}")));
            }
        }

        private static PluginProfileError ErrorFor(string field)
        {
            return new PluginProfileError { FieldName = field, Message = string.Empty };
        }

        private static PluginProfileError ErrorFor(string field, string message)
        {
            return new PluginProfileError
                { FieldName = field, Message = $"{message}. See plugin log for details." };
        }

        private static void HandleConnectionError(PluginProfileErrorCollection errors)
        {
            errors.Add(new PluginProfileError { FieldName = ConnectionSettings.UriField, Message = "Bad url format." });
        }
    }
}

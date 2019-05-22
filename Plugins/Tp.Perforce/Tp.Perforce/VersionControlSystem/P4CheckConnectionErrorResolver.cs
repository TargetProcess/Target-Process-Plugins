using System;
using Perforce.P4;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Perforce.VersionControlSystem
{
    public class P4CheckConnectionErrorResolver : ICheckConnectionErrorResolver
    {
        public void HandleConnectionError(Exception exception, PluginProfileErrorCollection errors)
        {
            if (exception is VersionControlException)
            {
                HandleConnectionError((P4Exception)exception.InnerException, errors);
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

        private static void HandleConnectionError(P4Exception e, PluginProfileErrorCollection errors)
        {
            if (e is P4LostConnectionException)
            {
                errors.Add(ErrorFor(ConnectionSettings.UriField, "Could not connect to server"));
            }
            else if (e.Message.Contains("check $P4PORT", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(ErrorFor(ConnectionSettings.UriField, "Invalid path to repository"));
            }
            else if (e is P4RevisionNotFoundException)
            {
                errors.Add(ErrorFor(P4PluginProfile.StartRevisionField));
            }
            else
            {
                errors.Add(ErrorFor(ConnectionSettings.LoginField, "Authentication failed"));
                errors.Add(ErrorFor(ConnectionSettings.PasswordField));
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

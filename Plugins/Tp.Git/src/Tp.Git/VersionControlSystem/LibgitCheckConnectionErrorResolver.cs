using System;
using LibGit2Sharp;
using Tp.Core;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;

namespace Tp.Git.VersionControlSystem
{
    public class LibgitCheckConnectionErrorResolver : ICheckConnectionErrorResolver
    {
        private readonly ILogManager _log;

        public LibgitCheckConnectionErrorResolver(ILogManager log)
        {
            _log = log;
        }

        public void HandleConnectionError(Exception exception, PluginProfileErrorCollection errors)
        {
            _log.GetLogger("Git").Warn("Check connection failed", exception);

            var libgitException = exception as LibGit2SharpException;
            if (libgitException != null)
            {
                if (exception.Message.Contains("401"))
                {
                    errors.Add(new PluginProfileError
                    {
                        FieldName = Reflect<GitPluginProfile>.GetPropertyName(p => p.Uri),
                        Message = libgitException.Message
                    });

                    errors.Add(new PluginProfileError
                    {
                        FieldName = Reflect<GitPluginProfile>.GetPropertyName(p => p.Login),
                        Message = libgitException.Message
                    });

                    errors.Add(new PluginProfileError
                    {
                        FieldName = Reflect<GitPluginProfile>.GetPropertyName(p => p.Password),
                        Message = libgitException.Message
                    });

                    return;
                }

                if (libgitException.Message.Contains("address could not be resolved"))
                {
                    errors.Add(new PluginProfileError
                    {
                        FieldName = Reflect<GitPluginProfile>.GetPropertyName(p => p.Uri),
                        Message = libgitException.Message
                    });

                    return;
                }
            }
            
            errors.Add(new PluginProfileError { Message = exception.Message, Status = PluginProfileErrorStatus.Error });
        }
    }
}

// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.SourceControl.Diff;
using Tp.SourceControl.RevisionStorage;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.Commands
{
    using System;
    using Tp.Integration.Plugin.Common.Activity;


    public class ViewDiffCommand : IPluginCommand
    {
        private readonly IRevisionStorageRepository _repository;
        private readonly IVersionControlSystemFactory _vcsFactory;

        private readonly IActivityLogger _logger;

        public ViewDiffCommand(IRevisionStorageRepository repository, IVersionControlSystemFactory vcsFactory, IActivityLogger logger)
        {
            _repository = repository;
            _vcsFactory = vcsFactory;
            _logger = logger;
        }

        public PluginCommandResponseMessage Execute(string args, UserDTO user)
        {
            var fileArgs = args.Deserialize<FileViewDiffArgs>();

            try
            {
                var revision = _repository.GetRevisionId(fileArgs.TpRevisionId);
                var diff = new DiffResult();

                if (revision != null)
                {
                    var vcs = _vcsFactory.Get(revision.Profile);
                    diff = vcs.GetDiff(revision.RevisionId.RevisionId, fileArgs.Path);
                }

                return new PluginCommandResponseMessage
                {
                    PluginCommandStatus = PluginCommandStatus.Succeed,
                    ResponseData = diff.Serialize()
                };
            }
            catch (Exception e)
            {
                _logger.Error("ViewDiff error", e);
                return new PluginCommandResponseMessage
                {
                    PluginCommandStatus = PluginCommandStatus.Error,
                    ResponseData = "Unable to connect to a remote repository: {0}.".Fmt(e.Message)
                };
            }
        }

        public string Name
        {
            get { return "ViewDiff"; }
        }
    }
}

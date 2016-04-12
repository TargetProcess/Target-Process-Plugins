// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Threading;
using Mercurial;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Mercurial.VersionControlSystem;
using Tp.SourceControl.Commands;
using Tp.SourceControl.VersionControlSystem;
using System.Linq;
using MercurialSDK = Mercurial;

namespace Tp.Mercurial
{
	public class MercurialCheckConnectionCommand : VcsCheckConnectionCommand<MercurialPluginProfile>
	{
        private MercurialRepositoryFolder _folder;

		protected override void CheckStartRevision(
            MercurialPluginProfile settings, 
            IVersionControlSystem versionControlSystem, 
            PluginProfileErrorCollection errors)
		{
			settings.ValidateStartRevision(errors);
		}

		protected override void OnCheckConnection(PluginProfileErrorCollection errors, MercurialPluginProfile settings)
		{
			settings.ValidateUri(errors);

			if (!errors.Any())
			{
				var identifyCommand = new IdentifyCommand().WithPath(settings.Uri).WithTimeout(20);
				NonPersistentClient.Execute(identifyCommand);
				if(identifyCommand.RawExitCode != 0)
				{
					throw new Exception("Can't establish connection");
				}
			}
		}

        protected override void OnExecuted(MercurialPluginProfile profile)
        {
            base.OnExecuted(profile);

            if (_folder != null && _folder.Exists())
            {
                _folder.Delete();
            }
        }
	}
}

// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl.Settings;

namespace Tp.SourceControl.VersionControlSystem
{
    public interface IVersionControlSystemFactory
    {
        IVersionControlSystem Get(IStorageRepository profile);
        IVersionControlSystem Get(ISourceControlConnectionSettingsSource settings);
    }
}

// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Validation;
using Tp.MashupManager.CustomCommands.Args;

namespace Tp.MashupManager
{
    public interface IMashupInfoRepository
    {
        PluginProfileErrorCollection Add(Mashup dto, bool generateUniqueName);
        PluginProfileErrorCollection Update(UpdateMashupCommandArg commandArg);
        PluginProfileErrorCollection Delete(string mashupName);
    }
}

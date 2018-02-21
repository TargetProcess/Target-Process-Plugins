// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Validation;

namespace Tp.MashupManager.CustomCommands
{
    public class DeleteMashupCommand : CrudMashupCommand<Mashup>
    {
        protected override PluginProfileErrorCollection ExecuteOperation(Mashup mashup)
        {
            return MashupRepository.Delete(mashup.Name);
        }

        public override string Name
        {
            get { return "DeleteMashup"; }
        }
    }
}

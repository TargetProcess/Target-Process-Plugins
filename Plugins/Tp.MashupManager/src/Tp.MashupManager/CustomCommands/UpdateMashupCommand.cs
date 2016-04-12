// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Validation;
using Tp.MashupManager.CustomCommands.Args;

namespace Tp.MashupManager.CustomCommands
{
	public class UpdateMashupCommand : CrudMashupCommand<UpdateMashupCommandArg>
	{
		protected override PluginProfileErrorCollection ExecuteOperation(UpdateMashupCommandArg mashup)
		{
			return MashupRepository.Update(mashup);
		}

		public override string Name
		{
			get { return "UpdateMashup"; }
		}
	}
}
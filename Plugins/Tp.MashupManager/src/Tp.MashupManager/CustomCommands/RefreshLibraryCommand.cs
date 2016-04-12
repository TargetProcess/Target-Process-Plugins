// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages.Commands;
using Tp.MashupManager.CustomCommands.Args;

namespace Tp.MashupManager.CustomCommands
{
	public class RefreshLibraryCommand : LibraryCommand<LibraryCommandArg>
	{
		protected override PluginCommandResponseMessage ExecuteOperation(LibraryCommandArg commandArg)
		{
			Library.Refresh();
			return new PluginCommandResponseMessage {PluginCommandStatus = PluginCommandStatus.Succeed};
		}

		public override string Name
		{
			get { return "RefreshLibrary"; }
		}
	}
}
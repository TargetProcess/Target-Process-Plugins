// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap;
using Tp.Integration.Plugin.Common;

namespace Tp.MashupManager.MashupStorage
{
	public class MashupLocalFolder : IMashupLocalFolder
	{
		public string Path
		{
			get { return ObjectFactory.GetInstance<PluginDataFolder>().Path; }
		}
	}
}
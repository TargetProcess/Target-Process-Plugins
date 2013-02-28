// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl;

namespace Tp.Mercurial
{
	public class MercurialCurrentProfileToConnectionSettingsAdapter : CurrentProfileToConnectionSettingsAdapter<MercurialPluginProfile>
	{
		public MercurialCurrentProfileToConnectionSettingsAdapter(IStorageRepository repository)
			: base(repository)
		{
		}
	}
}
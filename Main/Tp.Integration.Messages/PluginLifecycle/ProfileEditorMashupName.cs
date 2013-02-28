// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
namespace Tp.Integration.Messages.PluginLifecycle
{
	public class ProfileEditorMashupName
	{
		private readonly string _pluginName;
		public const string ProfileEditorMashupPrefix = "ProfileEditor";

		public ProfileEditorMashupName(string pluginName)
		{
			this._pluginName = pluginName;
		}

		public string Value
		{
			get { return string.Format("{0}{1}", ProfileEditorMashupPrefix, this._pluginName).Replace(" ", string.Empty).ToLower(); }
		}
	}
}
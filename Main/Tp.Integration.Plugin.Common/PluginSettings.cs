// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Configuration;
using Tp.Core;
using Tp.Integration.Plugin.Common.Properties;

namespace Tp.Integration.Plugin.Common
{
	public class PluginSettings : IPluginSettings
	{
		private const string PluginSettingsSectionName = "applicationSettings/Tp.Integration.Plugin.Common.Properties.Settings";

		public string PluginInputQueue
		{
			get { return Settings.Default.PluginInputQueue; }
		}

		public bool IsHidden
		{
			get { return Settings.Default.IsHidden; }
		}

		public static int LoadInt(string sectionName, int defaultValue)
		{
			return Load(sectionName, defaultValue, s => Maybe.FromTryOut<string, int>(int.TryParse, s));
		}

		public static int? LoadInt(string sectionName)
		{
			return Load(sectionName, null, s => Maybe.FromTryOut<string, int>(int.TryParse, s).Select(x => (int?)x));
		}

		public static T LoadEnum<T>(string sectionName, T defaultValue)
			where T : struct
		{
			return Load(sectionName, defaultValue, s => s.TryParseEnum<T>());
		}

		private static T Load<T>(string sectionName, T defaultValue, Func<string, Maybe<T>> converter)
		{
			var section = GetPluginSettingsSection();
			if (section == null)
			{
				return defaultValue;
			}
			var setting = section.Settings.Get(sectionName);
			if (setting == null)
			{
				return defaultValue;
			}
			return converter(setting.Value.ValueXml.InnerXml).GetOrElse(() => defaultValue);
		}

		private static ClientSettingsSection GetPluginSettingsSection()
		{
			return (ClientSettingsSection)ConfigurationManager.GetSection(PluginSettingsSectionName);
		}
	}

	public interface IPluginSettings
	{
		string PluginInputQueue { get; }

		bool IsHidden { get; }
	}
}
using System;
using System.Configuration;
using Tp.Core;
using Tp.Integration.Plugin.Common;
using Tp.Search.Model.Document;
using Tp.Search.Model.Optimization;

namespace Tp.Search.Config
{
	class DocumentIndexSetupConfig
	{
		private const string IndexAliveTimeoutInMinutesName = "IndexAliveTimeoutInMinutes";
		private const string DeferredOptimizeCallsOnChangeName = "DeferredOptimizeCallsOnChange";
		private const string DeferredOptimizeTypeName = "DeferredOptimizeType";
		private const string SearchCheckIntervalInMinutesName = "SearchCheckIntervalInMinutes";
		private const string ManagedMemoryThresholdInMbName = "ManagedMemoryThresholdInMb";
		private const string PluginSettingsSectionName = "applicationSettings/Tp.Integration.Plugin.Common.Properties.Settings";

		public DocumentIndexSetup Load()
		{
			int aliveTimeoutInMinutes = LoadInt(IndexAliveTimeoutInMinutesName, 10);
			int deferredOptimizeCallsOnChange = LoadInt(DeferredOptimizeCallsOnChangeName, 1);
			DeferredOptimizeType optimizeType = LoadEnum(DeferredOptimizeTypeName, DeferredOptimizeType.None);
			int checkIntervalInMinutes = LoadInt(SearchCheckIntervalInMinutesName, aliveTimeoutInMinutes);
			int? managedMemoryThresholdInMb = LoadInt(ManagedMemoryThresholdInMbName);
			var folder = new PluginDataFolder();
			return new DocumentIndexSetup(indexPath: folder.Path, minStringLengthToSearch: 2, maxStringLengthIgnore: 60, aliveTimeoutInMinutes: aliveTimeoutInMinutes, deferredOptimizeCounter: deferredOptimizeCallsOnChange, deferredOptimizeType: optimizeType, checkIntervalInMinutes: checkIntervalInMinutes, managedMemoryThresholdInMb: managedMemoryThresholdInMb);
		}

		private int LoadInt(string sectionName, int defaultValue)
		{
			return Load(sectionName, defaultValue, MaybeInt);
		}

		private int? LoadInt(string sectionName)
		{
			return Load(sectionName, null, s => MaybeInt(s).Bind(x => (int?)x));
		}

		private T LoadEnum<T>(string sectionName, T defaultValue)
			where T : struct
		{
			return Load(sectionName, defaultValue, s =>
			{
				T value;
				if(Enum.TryParse(s, out value))
				{
					return value;
				}
				return Maybe.Nothing;
			});
		}

		private T Load<T>(string sectionName, T defaultValue, Func<string, Maybe<T>> converter)
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

		private ClientSettingsSection GetPluginSettingsSection()
		{
			return (ClientSettingsSection) ConfigurationManager.GetSection(PluginSettingsSectionName);
		}

		private Maybe<int> MaybeInt(string s)
		{
			int value;
			if (int.TryParse(s, out value))
			{
				return value;
			}
			return Maybe.Nothing;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Tp.Core;

namespace Tp.MashupManager.MashupLibrary.Repository.Config
{
	public class LibraryRepositoryConfigStorage : ILibraryRepositoryConfigStorage
	{
		private const string LibraryConfigSettingName = "LibraryConfig";
		private const string PluginSettingsSectionName = "applicationSettings/Tp.Integration.Plugin.Common.Properties.Settings";
		private readonly ISingleProfile _singleProfile;
		private IEnumerable<ILibraryRepositoryConfig> _defaultConfigs;

		public IEnumerable<ILibraryRepositoryConfig> DefaultConfigs
		{
			get
			{
				if (_defaultConfigs == null)
				{
					var configsSection = (ClientSettingsSection)ConfigurationManager.GetSection(PluginSettingsSectionName);
					var configsString = configsSection.Settings.Get(LibraryConfigSettingName).Value.ValueXml.InnerXml;
					var configsXmlSerializer = new XmlSerializer(typeof(List<LibraryRepositoryConfig>));
					_defaultConfigs = ((List<LibraryRepositoryConfig>)configsXmlSerializer.Deserialize(new StringReader(configsString))).Cast<ILibraryRepositoryConfig>();
				}

				return _defaultConfigs;
			}
			set { _defaultConfigs = value; }
		}

		public LibraryRepositoryConfigStorage(ISingleProfile singleProfile)
		{
			_singleProfile = singleProfile;
		}

		public IEnumerable<ILibraryRepositoryConfig> GetConfigs()
		{	
			return _singleProfile.Profile.NothingIfNull()
				.Bind(x => x.GetProfile<MashupManagerProfile>().LibraryRepositoryConfigs.NothingIfNull()
					.Where(y => y.Any())
					.Select(y => y.Cast<ILibraryRepositoryConfig>()))
				.GetOrElse(()=>DefaultConfigs);
		}

		
	}
}

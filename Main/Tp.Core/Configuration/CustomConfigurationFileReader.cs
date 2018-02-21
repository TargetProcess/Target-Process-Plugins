using System;
using System.Configuration;
using System.IO;
using Tp.Core.Annotations;

namespace Tp.Core.Configuration
{
    /// <summary>
    /// Reads customer configuration file
    /// </summary>
    public class CustomConfigurationFileReader : IConfigurationReader
    {
        // The configuration file name
        private readonly string _configFileName;

        /// <summary>
        /// Raises when the configuraiton file is modified
        /// </summary>
        public event System.EventHandler FileChanged;

        /// <summary>
        /// Initialize a new instance of the CustomConfigurationFileReader class
        /// </summary>
        /// <param name="configFileName">The full path to the custom configuration file</param>
        /// <param name="notifyOnFileChange">Indicate if to raise the FileChange event when the configuraiton file changes </param>
        public CustomConfigurationFileReader([NotNull] string configFileName, bool notifyOnFileChange = false)
        {
            // Set the configuration file name
            _configFileName = configFileName;

            // Read the configuration File
            ReadConfiguration();

            // Start watch the configuration file (if notifyOnFileChanged is true)
            if (notifyOnFileChange)
                WatchConfigFile();
        }

        /// <summary>
        /// Get the configuration that represents the content of the configuration file
        /// </summary>
        public IConfiguration Config { get; private set; }

        /// <summary>
        /// Watch the configuraiton file for changes
        /// </summary>
        private void WatchConfigFile()
        {
            var watcher = new FileSystemWatcher(_configFileName);
            watcher.Changed += ConfigFileChangedEvent;
        }

        /// <summary>
        /// Read the configuration file
        /// </summary>
        public void ReadConfiguration()
        {
            // Create config file map to point to the configuration file
            var configFileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = _configFileName
            };

            // Create configuration object that contains the content of the custom configuration file
            Config = new ConfigurationWrapper(ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None));
        }

        public class ConfigurationWrapper : IConfiguration
        {
            private readonly System.Configuration.Configuration _configuration;

            public ConfigurationWrapper(System.Configuration.Configuration configuration)
            {
                _configuration = configuration;
            }

            public T GetSection<T>(string sectionName) where T : ConfigurationSection
            {
                return (T) _configuration.GetSection(sectionName);
            }
        }

        /// <summary>
        /// Called when the configuration file changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigFileChangedEvent(object sender, FileSystemEventArgs e)
        {
            // Check if the file changed event has listeners
            if (FileChanged != null)
                // Raise the event
                FileChanged(this, new EventArgs());
        }
    }
}

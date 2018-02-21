using System;
using System.Configuration;
using log4net;
using Tp.Core.Annotations;

namespace Tp.Core.Configuration
{
    public static class AppSettingsReader
    {
        private static readonly ILog Log = TpLogManager.Instance.GetLog(typeof(AppSettingsReader));

        public static bool ReadBoolean(
            [NotNull] string configurationKey, bool defaultValue)
        {
            return Read(configurationKey, defaultValue, bool.TryParse);
        }

        public static int ReadInt32(
            [NotNull] string configurationKey, int defaultValue)
        {
            return Read(configurationKey, defaultValue, int.TryParse);
        }

        public static ushort ReadUInt16(
            [NotNull] string configurationKey, ushort defaultValue)
        {
            return Read(configurationKey, defaultValue, ushort.TryParse);
        }

        public static TimeSpan ReadTimeSpan(
            [NotNull] string configurationKey, TimeSpan defaultValue)
        {
            return Read(configurationKey, defaultValue, TimeSpan.TryParse);
        }

        public static string ReadString(
            [NotNull] string configurationKey, string defaultValue)
        {
            Maybe.TryDelegate<string, string> dummyStringParser = (string s1, out string s2) => (s2 = s1) == s1;

            return Read(configurationKey, defaultValue, dummyStringParser);
        }

        private static T Read<T>(
            [NotNull] string configurationKey, T defaultValue,
            [NotNull] [InstantHandle] Maybe.TryDelegate<string, T> parser)
        {
            Log.Debug($"Attempting to read {typeof(T)} '{configurationKey}' from app settings");

            var settings = ConfigurationManager.AppSettings;
            var storedValue = settings[configurationKey];

            if (storedValue == null)
            {
                Log.Debug($"There is no configuration entry for '{configurationKey}', using default value '{defaultValue}' instead");
                return defaultValue;
            }

            T result;
            if (!parser(storedValue, out result))
            {
                Log.Error(
                    $"Unable to parse {typeof(T)} from '{configurationKey}' value '{storedValue}'. Using default value '{defaultValue}' instead");
                return defaultValue;
            }

            Log.Debug($"Parsed '{configurationKey}': '{result}'");

            return result;
        }
    }
}

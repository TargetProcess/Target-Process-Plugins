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

        public static TEnum ReadEnum<TEnum>(
            [NotNull] string configurationKey, TEnum defaultValue) where TEnum: struct, IComparable, IFormattable, IConvertible
        {
            return Read(configurationKey, defaultValue, Enum.TryParse);
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
            Log.DebugFormat("Attempting to read {0} '{1}' from app settings", typeof(T), configurationKey);

            var settings = ConfigurationManager.AppSettings;
            var storedValue = settings[configurationKey];

            if (storedValue == null)
            {
                Log.DebugFormat("There is no configuration entry for '{0}', using default value '{1}' instead",
                    configurationKey, defaultValue);
                return defaultValue;
            }

            if (!parser(storedValue, out var result))
            {
                Log.Error(
                    $"Unable to parse {typeof(T)} from '{configurationKey}' value '{storedValue}'. Using default value '{defaultValue}' instead");
                return defaultValue;
            }

            Log.DebugFormat("Parsed '{0}': '{1}'", configurationKey, result);

            return result;
        }
    }
}

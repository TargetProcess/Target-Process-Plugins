using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tp.Core;

namespace Tp.Integration.Messages.PluginLifecycle
{
    public class MashupConfig
    {
        public static readonly StringProperty AccountsProperty = new StringProperty("Accounts");
        public static readonly StringProperty PlaceholdersProperty = new StringProperty("Placeholders");
        public static readonly BoolProperty IsEnabledProperty = new BoolProperty("IsEnabled");
        public static readonly StringProperty PackageNameProperty = new StringProperty("PackageName");
        public static readonly UlongProperty CreationDateProperty = new UlongProperty("CreationDate");
        public static readonly MashupUserInfoProperty CreatedByProperty = new MashupUserInfoProperty("CreatedBy");
        public static readonly UlongProperty LastModificationDateProperty = new UlongProperty("LastModificationDate");
        public static readonly MashupUserInfoProperty LastModifiedByProperty = new MashupUserInfoProperty("LastModifiedBy");

        public List<AccountName> Accounts { get; }
        public List<string> Placeholders { get; }
        public MashupMetaInfo MashupMetaInfo { get; }

        public MashupConfig(IEnumerable<string> configLines)
        {
            MashupMetaInfo = new MashupMetaInfo
            {
                IsEnabled = true
            };
            var accounts = new List<string>();
            var placeholders = new List<string>();

            foreach (var line in configLines)
            {
                string stringValue;
                bool boolValue;
                ulong intValue;
                MashupUserInfo userInfo;

                if (AccountsProperty.TryParse(line, out stringValue))
                {
                    accounts.AddRange(stringValue.Split(','));
                }
                else if (PlaceholdersProperty.TryParse(line, out stringValue))
                {
                    placeholders.AddRange(stringValue.Split(','));
                }
                else if (IsEnabledProperty.TryParse(line, out boolValue))
                {
                    MashupMetaInfo.IsEnabled = boolValue;
                }
                else if (PackageNameProperty.TryParse(line, out stringValue))
                {
                    MashupMetaInfo.PackageName = stringValue;
                }
                else if (CreationDateProperty.TryParse(line, out intValue))
                {
                    MashupMetaInfo.CreationDate = intValue;
                }
                else if (CreatedByProperty.TryParse(line, out userInfo))
                {
                    MashupMetaInfo.CreatedBy = userInfo;
                }
                else if (LastModificationDateProperty.TryParse(line, out intValue))
                {
                    MashupMetaInfo.LastModificationDate = intValue;
                }
                else if (LastModifiedByProperty.TryParse(line, out userInfo))
                {
                    MashupMetaInfo.LastModifiedBy = userInfo;
                }
            }

            Accounts = accounts.Select(x => x.Trim().ToLower()).Distinct().Select(x => new AccountName(x)).ToList();
            Placeholders = placeholders.Select(x => x.Trim().ToLower()).Distinct().ToList();
        }

        public static IEnumerable<string> GetConfigLines(MashupMetaInfo mashupMetaInfo, string placeholders = "", string accountNames = "")
        {
            return new List<string>
            {
                AccountsProperty.Write(accountNames),
                PlaceholdersProperty.Write(placeholders),
                IsEnabledProperty.Write(mashupMetaInfo.IsEnabled),
                PackageNameProperty.Write(mashupMetaInfo.PackageName),
                CreationDateProperty.Write(mashupMetaInfo.CreationDate),
                CreatedByProperty.Write(mashupMetaInfo.CreatedBy),
                LastModificationDateProperty.Write(mashupMetaInfo.LastModificationDate),
                LastModifiedByProperty.Write(mashupMetaInfo.LastModifiedBy)
            }.Where(s => s.IsNotBlank());
        }

        public static IEnumerable<string> GetConfigLines(MashupMetaInfo mashupMetaInfo, IReadOnlyCollection<string> placeholders,
            IReadOnlyCollection<AccountName> accountNames = null)
        {
            var accountNamesString = accountNames.IsNullOrEmpty() ? string.Empty : accountNames.ToString(",");
            var placeholdersString = placeholders.IsNullOrEmpty() ? string.Empty : placeholders.ToString(",");
            return GetConfigLines(mashupMetaInfo, placeholdersString, accountNamesString);
        }

        public static string AccountsConfigLine(AccountName accountName)
        {
            return AccountsProperty.Write(accountName.ToString());
        }

        public static string AccountsConfigLine(IReadOnlyCollection<AccountName> accountNames)
        {
            return accountNames.IsNullOrEmpty() ? string.Empty : AccountsProperty.Write(accountNames.ToString(","));
        }

        public bool Matches(MashupPlaceholder mashupPlaceHolderValue, AccountName accountName)
        {
            return PlaceholderMatches(mashupPlaceHolderValue) && AccountMatches(accountName);
        }

        public bool AccountMatches(AccountName accountName)
        {
            return Accounts.Contains(accountName.Value.ToLower()) || Accounts.Empty();
        }

        public bool PlaceholderMatches(MashupPlaceholder mashupPlaceHolderValue)
        {
            return Placeholders.Any(x => x.EqualsIgnoreCase(mashupPlaceHolderValue.Value));
        }
    }

    public abstract class ConfigProperty<T>
    {
        protected string Prefix { get; }
        private const string Delimiter = ":";

        protected ConfigProperty(string prefix)
        {
            Prefix = prefix;
        }

        protected abstract bool TryParseInternal(string text, out T value);

        public bool TryParse(string line, out T value)
        {
            value = default;
            line = line.Trim();
            if (!line.StartsWith(Prefix + Delimiter))
            {
                return false;
            }

            var strValue = line.Substring(Prefix.Length + Delimiter.Length);
            return !string.IsNullOrEmpty(strValue) && TryParseInternal(strValue, out value);
        }

        protected string WriteInternal(string value)
        {
            return $"{Prefix}{Delimiter}{value}";
        }

        public abstract string Write(T value);
    }

    public class BoolProperty : ConfigProperty<bool>
    {
        public BoolProperty(string prefix) : base(prefix)
        {
        }

        protected override bool TryParseInternal(string text, out bool value)
        {
            value = text.EqualsIgnoreCase("true");
            return true;
        }

        public override string Write(bool value)
        {
            return WriteInternal(value.ToString().ToLowerInvariant());
        }
    }

    public class UlongProperty : ConfigProperty<ulong>
    {
        public UlongProperty(string prefix) : base(prefix)
        {
        }

        protected override bool TryParseInternal(string text, out ulong value)
        {
            return ulong.TryParse(text, out value);
        }

        public override string Write(ulong value)
        {
            return value > 0 ? WriteInternal(value.ToString()) : string.Empty;
        }
    }

    public class StringProperty : ConfigProperty<string>
    {
        public StringProperty(string prefix) : base(prefix)
        {
        }

        protected override bool TryParseInternal(string text, out string value)
        {
            value = text;
            return true;
        }

        public override string Write(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : WriteInternal(value);
        }

        public string Write(IReadOnlyCollection<string> values)
        {
            return values.IsNullOrEmpty() ? string.Empty : Write(values.ToString(","));
        }
    }

    public class MashupUserInfoProperty : ConfigProperty<MashupUserInfo>
    {
        public MashupUserInfoProperty(string prefix) : base(prefix)
        {
        }

        protected override bool TryParseInternal(string text, out MashupUserInfo value)
        {
            value = null;
            var regEx = new Regex(@"#(\d+)\s+(.+)");
            var match = regEx.Match(text);

            if (!match.Success)
            {
                return false;
            }

            var idText = match.Groups[1].Value;
            int id;
            if (!int.TryParse(idText, out id))
            {
                return false;
            }

            var nameText = match.Groups[2].Value;
            value = new MashupUserInfo
            {
                Id = id,
                Name = nameText.IsNullOrEmpty() ? string.Empty : nameText
            };
            return true;
        }

        public override string Write(MashupUserInfo value)
        {
            return (value == null || value.Id == 0) ? string.Empty : WriteInternal($"#{value.Id} {value.Name}");
        }
    }
}

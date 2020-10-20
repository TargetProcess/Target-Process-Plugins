using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Tp.Core;

namespace System
{
    public static class EnumHelper
    {
        private static EnumCache<TEnum> CreateEnumCache<TEnum, TAttribute>(Func<FieldInfo, Attribute, string> valueProvider)
            where TEnum : struct
            where TAttribute : Attribute
        {
            return new EnumCache<TEnum>(e =>
            {
                string s = EnumStringCache<TEnum>.Cache.GetValue(e);
                FieldInfo f = typeof(TEnum).GetField(s, BindingFlags.Public | BindingFlags.Static);
                if (f == null)
                {
                    throw new ArgumentException("Cannot find {0} enum member in {1} enum".Fmt(s, typeof(TEnum).Name));
                }
                var attr = Attribute.GetCustomAttribute(f, typeof(TAttribute));
                return valueProvider(f, attr);
            });
        }

        private static class EnumStringCache<TEnum>
            where TEnum : struct
        {
            public static readonly EnumCache<TEnum> Cache = new EnumCache<TEnum>(e => e.ToString());
        }

        public static class EnumDescriptionCache<TEnum>
            where TEnum : struct
        {
            public static readonly EnumCache<TEnum> Instance =
                CreateEnumCache<TEnum, DescriptionAttribute>(
                    (f, attr) => { return attr.MaybeAs<DescriptionAttribute>().Select(x => x.Description).GetOrDefault(f.Name); });
        }

        private static class EnumMetadataCache<TEnum, TAttribute>
            where TEnum : struct
            where TAttribute : Attribute, ITextProvider
        {
            public static readonly EnumCache<TEnum> Instance =
                CreateEnumCache<TEnum, TAttribute>(
                    (f, attr) => { return attr.MaybeAs<ITextProvider>().Select(x => x.GetText()).GetOrDefault(f.Name); });
        }

        public class EnumCache<TEnum> : IEnumerable<KeyValuePair<TEnum, string>>
            where TEnum : struct
        {
            private readonly IReadOnlyDictionary<TEnum, string> _values;

            private readonly ConcurrentBag<Maybe.TryDelegate<TEnum, string>> _fallbackFunctions =
                new ConcurrentBag<Maybe.TryDelegate<TEnum, string>>();

            public EnumCache(Func<TEnum, string> valueProvider)
            {
                if (!typeof(TEnum).IsEnum)
                {
                    throw new ArgumentException($"Type {typeof(TEnum)} is not an enumeration.");
                }
                _values = ((TEnum[]) Enum.GetValues(typeof(TEnum))).ToDictionary(e => e, valueProvider).ToReadOnly();
            }

            public void AddFallbackFunction(Maybe.TryDelegate<TEnum, string> function)
            {
                _fallbackFunctions.Add(function);
            }

            public string GetValue(TEnum @enum)
            {
                if (_values.TryGetValue(@enum, out var result))
                {
                    return result;
                }

                foreach (var fallback in _fallbackFunctions)
                {
                    if (fallback(@enum, out result))
                    {
                        return result;
                    }
                }

                throw new KeyNotFoundException($"Unable to find cache entry for enum value {typeof(TEnum).Name}.{@enum}");
            }

            public IEnumerable<TEnum> GetKeys()
            {
                return _values.Keys;
            }

            public IEnumerable<string> GetValues()
            {
                return _values.Values;
            }

            public IEnumerator<KeyValuePair<TEnum, string>> GetEnumerator()
            {
                return _values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static string GetDescription<TEnum>(this TEnum @enum) where TEnum : struct
        {
            return EnumDescriptionCache<TEnum>.Instance.GetValue(@enum);
        }

        public static IEnumerable<string> GetDescriptions<TEnum>() where TEnum : struct
        {
            return EnumDescriptionCache<TEnum>.Instance.GetValues();
        }

        public static string GetMetadata<TEnum, TAttribute>(this TEnum @enum)
            where TEnum : struct
            where TAttribute : Attribute, ITextProvider
        {
            return EnumMetadataCache<TEnum, TAttribute>.Instance.GetValue(@enum);
        }

        public static IEnumerable<string> GetMetadatas<TEnum, TAttribute>()
            where TEnum : struct
            where TAttribute : Attribute, ITextProvider
        {
            return EnumMetadataCache<TEnum, TAttribute>.Instance.GetValues();
        }

        public static string AsString<TEnum>(this TEnum @enum) where TEnum : struct
        {
            return EnumStringCache<TEnum>.Cache.GetValue(@enum);
        }

        public static IEnumerable<string> AsStrings<TEnum>() where TEnum : struct
        {
            return EnumStringCache<TEnum>.Cache.GetValues();
        }

        public static Maybe<TEnum> TryParseEnum<TEnum>(this string val) where TEnum : struct
        {
            RaiseErrorIfNotEnum<TEnum>();
            return Enum.TryParse(val, true, out TEnum parsed) ? Maybe.Return(parsed) : Maybe.Nothing;
        }

        public static TEnum? TryParseEnumCached<TEnum>(this string val) where TEnum : struct
        {
            RaiseErrorIfNotEnum<TEnum>();
            return EnumStringCache<TEnum>.Cache
                // It may look like it's ineffective to iterate over all items, and a map must be used instead, however:
                //   - most of the time there is a small number of enum values
                //   - with this approach we don't have to think about cache exhaustion when clients pass too many different strings
                .Where(x => string.Equals(val, x.Value, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Key)
                .FirstOrNull();
        }

        public static IReadOnlyDictionary<TEnum, string> TryParseEnumByDescription<TEnum>(this string description, bool ignoreCase)
            where TEnum : struct
        {
            RaiseErrorIfNotEnum<TEnum>();

            var comparisonType = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            Func<KeyValuePair<TEnum, string>, bool> condition = e => string.Equals(description, e.Value, comparisonType);

            return EnumDescriptionCache<TEnum>.Instance.Where(d => condition(d)).ToDictionary(k => k.Key, pair => pair.Value);
        }

        public static TEnum ParseEnum<TEnum>(this string val) where TEnum : struct
        {
            RaiseErrorIfNotEnum<TEnum>();
            return TryParseEnum<TEnum>(val)
                .GetOrThrow(() => new InvalidCastException("Could not parse value '{0}' for enum '{1}'".Fmt(val, typeof(TEnum))));
        }

        public static bool TryParse(this string s, Type enumType, bool ignoreCase, out object parsed)
        {
            parsed = null;
            RaiseErrorIfNotEnum(enumType);
            if (ignoreCase)
            {
                string candidate = s.ToLower();
                if (Enum.GetNames(enumType).Select(n => n.ToLower()).All(n => n != candidate))
                {
                    return false;
                }
            }
            else if (!Enum.IsDefined(enumType, s))
            {
                return false;
            }
            parsed = Enum.Parse(enumType, s, ignoreCase);
            return true;
        }

        public static IEnumerable<TEnum> GetValues<TEnum>() where TEnum : struct
        {
            return EnumStringCache<TEnum>.Cache.GetKeys();
        }

        private static void RaiseErrorIfNotEnum<TEnum>()
        {
            RaiseErrorIfNotEnum(typeof(TEnum));
        }

        private static void RaiseErrorIfNotEnum(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type argument must be enum instead of {0}.".Fmt(enumType.Name));
            }
        }

        public static Maybe<TEnum> TryGetByDescription<TEnum>(string description) where TEnum : struct
        {
            RaiseErrorIfNotEnum<TEnum>();
            return EnumDescriptionCache<TEnum>.Instance.FirstOrNothing(x => x.Value.EqualsIgnoreCase(description)).Select(x => x.Key);
        }

        public static Exception CreateUnexpectedEnumError<TEnum>(TEnum val)
        {
            return new InvalidOperationException("Unexpected enum value {0} of type {1}".Fmt(val, typeof(TEnum)));
        }
    }
}

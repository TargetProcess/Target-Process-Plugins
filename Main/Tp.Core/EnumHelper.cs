using System.Collections;
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

		private static class EnumStringCache<TEnum> where TEnum : struct
		{
			public static readonly EnumCache<TEnum> Cache = new EnumCache<TEnum>(e => e.ToString());
		}

		private static class EnumDescriptionCache<TEnum> where TEnum : struct
		{
			public static readonly EnumCache<TEnum> Instance =
				CreateEnumCache<TEnum, DescriptionAttribute>(
					(f, attr) => { return attr.MaybeAs<DescriptionAttribute>().Select(x => x.Description).GetOrDefault(f.Name); });
		}

		private static class EnumMetadataCache<TEnum, TAttribute> where TEnum : struct where TAttribute : Attribute, ITextProvider
		{
			public static readonly EnumCache<TEnum> Instance =
				CreateEnumCache<TEnum, TAttribute>((f, attr) => { return attr.MaybeAs<ITextProvider>().Select(x => x.GetText()).GetOrDefault(f.Name); });
		}

		private class EnumCache<TEnum> : IEnumerable<KeyValuePair<TEnum, string>> where TEnum : struct
		{
			private readonly IDictionary<TEnum, string> _values;

			public EnumCache(Func<TEnum, string> valueProvider)
			{
				if (!typeof(TEnum).IsEnum)
				{
					throw new ArgumentException(string.Format("Type {0} is not an enumeration.", typeof(TEnum)));
				}
				_values = ((TEnum[]) Enum.GetValues(typeof(TEnum))).ToDictionary(e => e, valueProvider);
			}

			public string GetValue(TEnum @enum)
			{
				return _values[@enum];
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
			TEnum parsed;
			return Enum.TryParse(val, true, out parsed) ? Maybe.Return(parsed) : Maybe.Nothing;
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

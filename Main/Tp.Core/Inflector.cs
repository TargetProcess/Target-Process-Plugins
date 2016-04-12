using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tp.Core
{
	/// <summary>
	/// A class inspired by Ruby on Rails <a href="http://api.rubyonrails.org/classes/Inflector.html">Inflector</a>
	/// and Commons-Lang <a href="http://commons.apache.org/lang/api/org/apache/commons/lang/WordUtils.html">WordUtils</a>.
	/// </summary>
	public static class Inflector
	{
		private static readonly List<InflectorRule> _plurals = new List<InflectorRule>();
		private static readonly List<InflectorRule> _singulars = new List<InflectorRule>();
		private static readonly List<string> _uncountables = new List<string>();

		/// <summary>
		/// Initializes the <see cref="Inflector"/> class.
		/// </summary>
		static Inflector()
		{
			AddPluralRule("$", "s");
			AddPluralRule("s$", "s");
			AddPluralRule("(ax|test)is$", "$1es");
			AddPluralRule("(octop|vir)us$", "$1i");
			AddPluralRule("(alias|status)$", "$1es");
			AddPluralRule("(bu)s$", "$1ses");
			AddPluralRule("(buffal|tomat)o$", "$1oes");
			AddPluralRule("([ti])um$", "$1a");
			AddPluralRule("sis$", "ses");
			AddPluralRule("(?:([^f])fe|([lr])f)$", "$1$2ves");
			AddPluralRule("(hive)$", "$1s");
			AddPluralRule("([^aeiouy]|qu)y$", "$1ies");
			AddPluralRule("(x|ch|ss|sh)$", "$1es");
			AddPluralRule("(matr|vert|ind)ix|ex$", "$1ices");
			AddPluralRule("([m|l])ouse$", "$1ice");
			AddPluralRule("^(ox)$", "$1en");
			AddPluralRule("(quiz)$", "$1zes");

			AddSingularRule("s$", "");
			AddSingularRule("ss$", "ss");
			AddSingularRule("(n)ews$", "$1ews");
			AddSingularRule("([ti])a$", "$1um");
			AddSingularRule("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis");
			AddSingularRule("(^analy)ses$", "$1sis");
			AddSingularRule("([^f])ves$", "$1fe");
			AddSingularRule("(hive)s$", "$1");
			AddSingularRule("(tive)s$", "$1");
			AddSingularRule("([lr])ves$", "$1f");
			AddSingularRule("([^aeiouy]|qu)ies$", "$1y");
			AddSingularRule("(s)eries$", "$1eries");
			AddSingularRule("(m)ovies$", "$1ovie");
			AddSingularRule("(x|ch|ss|sh)es$", "$1");
			AddSingularRule("([m|l])ice$", "$1ouse");
			AddSingularRule("(bus)es$", "$1");
			AddSingularRule("(o)es$", "$1");
			AddSingularRule("(shoe)s$", "$1");
			AddSingularRule("(cris|ax|test)es$", "$1is");
			AddSingularRule("(octop|vir)i$", "$1us");
			AddSingularRule("(alias|status)$", "$1");
			AddSingularRule("(alias|status)es$", "$1");
			AddSingularRule("^(ox)en", "$1");
			AddSingularRule("(vert|ind)ices$", "$1ex");
			AddSingularRule("(matr)ices$", "$1ix");
			AddSingularRule("(quiz)zes$", "$1");

			AddIrregularRule("person", "people");
			AddIrregularRule("man", "men");
			AddIrregularRule("child", "children");
			AddIrregularRule("sex", "sexes");
			AddIrregularRule("tax", "taxes");
			AddIrregularRule("move", "moves");

			AddUnknownCountRule("equipment");
			AddUnknownCountRule("information");
			AddUnknownCountRule("rice");
			AddUnknownCountRule("money");
			AddUnknownCountRule("species");
			AddUnknownCountRule("series");
			AddUnknownCountRule("fish");
			AddUnknownCountRule("sheep");
		}

		private static void AddSingularRule(string rule, string replacement)
		{
			_singulars.Add(new InflectorRule(rule, replacement));
		}

		private static void AddIrregularRule(string singular, string plural)
		{
			AddPluralRule("(" + singular[0] + ")" + singular.Substring(1) + "$", "$1" + plural.Substring(1));
			AddSingularRule("(" + plural[0] + ")" + plural.Substring(1) + "$", "$1" + singular.Substring(1));
		}

		/// <summary>
		/// Adds the unknown count rule.
		/// </summary>
		/// <param name="word">The word.</param>
		private static void AddUnknownCountRule(string word)
		{
			_uncountables.Add(word.ToLower());
		}

		/// <summary>
		/// Adds the plural rule.
		/// </summary>
		/// <param name="rule">The rule.</param>
		/// <param name="replacement">The replacement.</param>
		private static void AddPluralRule(string rule, string replacement)
		{
			_plurals.Add(new InflectorRule(rule, replacement));
		}


		/// <summary>
		/// Abbreviates a string using ellipses. This will turn "Now is the time for all good men" into "Now is the time for..."
		/// </summary>
		/// <remarks>
		/// Specifically:
		/// <ul>
		/// If <paramref name="s"/> is less than maxWidth characters long, return it.
		/// Else abbreviate it to (substring(<paramref name="s"/>, 0, max-3) + "...").
		/// If <paramref name="maxWidth"/> is less than 4, throw an <see cref="ArgumentException"/>.
		/// In no case will it return a string of length greater than <paramref name="maxWidth"/>.
		/// </ul>
		/// A <c>null</c> input string returns <c>null</c>.
		/// <example>
		/// <code>
		/// Abbreviate(null, 4)				= null
		/// Abbreviate("", 4)				= ""
		/// Abbreviate("abcdefg", 6)		= "abc..."
		/// Abbreviate("abcdefg", 7)		= "abcdefg"
		/// Abbreviate("abcdefg", 8)		= "abcdefg"
		/// Abbreviate("abcdefg", 4)		= "a..."
		/// Abbreviate("abcdefg", 3)		= IllegalArgumentException
		/// </code>
		/// </example>
		/// </remarks>
		/// <param name="s"></param>
		/// <param name="maxWidth"></param>
		/// <returns></returns>
		public static string Abbreviate(this string s, int maxWidth)
		{
			if (maxWidth < 4)
			{
				throw new ArgumentException();
			}

			if (s == null)
			{
				return null;
			}

			if (s.Length <= maxWidth)
			{
				return s;
			}

			return s.Substring(0, maxWidth - 3) + "...";
		}

		/// <summary>
		/// Capitalizes all the whitespace separated words in a string.
		/// </summary>
		/// <remarks>
		/// A <c>null</c> input string returns <c>null</c>.
		/// <example>
		/// <code>
		/// </code>
		/// </example>
		/// </remarks>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string Capitalize(this string s)
		{
			if (s == null)
			{
				return null;
			}

			return s.Split(' ').Select(x => x.Substring(0, 1).ToUpper() + x.Substring(1).ToLower()).ToString(" ");
		}

		/// <summary>
		/// Capitalizes the first word and turns underscores into spaces.
		/// </summary>
		/// <remarks>
		/// A <c>null</c> input string returns <c>null</c>.
		/// <example>
		/// <code>
		/// </code>
		/// </example>
		/// </remarks>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string Humanize(this string s)
		{
			if (s == null)
			{
				return null;
			}

			var builder = new StringBuilder();
			bool lastWasLower = false;
			foreach (char c in s)
			{
				if (c == '_' || c == ' ')
				{
					builder.Append(' ');
					lastWasLower = false;
				}
				else
				{
					if (Char.IsLetterOrDigit(c))
					{
						if (Char.IsUpper(c))
						{
							if (lastWasLower)
								builder.Append(' ');
							lastWasLower = false;
						}
						else
						{
							lastWasLower = true;
						}
					}
					builder.Append(c);
				}
			}

			if (builder.Length > 0)
			{
				builder[0] = Char.ToUpper(builder[0]);
			}

			return builder.ToString();
		}

		/// <summary>
		/// Anti-<see cref="Humanize"/>.
		/// </summary>
		/// <remarks>
		/// If you are curious where the name comes from, 
		/// see <a href="http://au.encarta.msn.com/thesaurus_1861857776/humanize.html">Article</a> for details.
		/// </remarks>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string Brutalize(this string s)
		{
			if (s == null)
			{
				return null;
			}

			var result = new char[s.Length];

			int n = 0;
			foreach (var c in s)
			{
				if (c != ' ')
				{
					result[n++] = c;
				}
			}

			return new string(result, 0, n);
		}

		/// <summary>
		/// Returns the plural form of the word in the string. 
		/// </summary>
		/// <remarks>
		/// A <c>null</c> input string returns <c>null</c>.
		/// <example>
		/// <code>
		/// "post".Pluralize()					=> "posts"
		/// "octopus".Pluralize()				=> "octopi"
		/// "sheep".Pluralize()					=> "sheep"
		/// "words".Pluralize()					=> "words"
		/// "the blue mailman".Pluralize()		=> "the blue mailmen"
		/// "CamelOctopus".Pluralize()			=> "CamelOctopi"
		/// </code>
		/// </example>
		/// </remarks>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string Pluralize(this string s)
		{
			if (s == null)
			{
				return null;
			}

			var plural1 = new Regex("(?<keep>[^aeiou])y$");
			var plural2 = new Regex("(?<keep>[aeiou]y)$");
			var plural3 = new Regex("(?<keep>[sxzh])$");
			var plural4 = new Regex("(?<keep>[^sxzhy])$");

			if (plural1.IsMatch(s))
			{
				return plural1.Replace(s, "${keep}ies");
			}
			if (plural2.IsMatch(s))
			{
				return plural2.Replace(s, "${keep}s");
			}
			if (plural3.IsMatch(s))
			{
				return plural3.Replace(s, "${keep}es");
			}
			if (plural4.IsMatch(s))
			{
				return plural4.Replace(s, "${keep}s");
			}

			return s;
		}

		public static string Pluralize(this string s, int count)
		{
			return count == 1 ? s : s.Pluralize();
		}

		/// <summary>
		/// The reverse of <see cref="Pluralize(string)"/>, returns the singular form of a word in a string.
		/// </summary>
		/// <remarks>
		/// A <c>null</c> input string returns <c>null</c>.
		/// <example>
		/// <code>
		/// "posts".Singularize()               => "post"
		/// "octopi".Singularize()              => "octopus"
		/// "sheep".Singluarize()               => "sheep"
		/// "word".Singluarize()                => "word"
		/// "the blue mailmen".Singularize()    => "the blue mailman"
		/// "CamelOctopi".Singularize()         => "CamelOctopus"
		/// </code>
		/// </example>
		/// </remarks>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string Singularize(this string s)
		{
			if (s == null)
			{
				return null;
			}
			return ApplyRules(_singulars, s);
		}


		/// <summary>
		/// Applies the rules.
		/// </summary>
		/// <param name="rules">The rules.</param>
		/// <param name="word">The word.</param>
		/// <returns></returns>
		private static string ApplyRules(IList<InflectorRule> rules, string word)
		{
			string result = word;
			if (!_uncountables.Contains(word.ToLower()))
			{
				for (int i = rules.Count - 1; i >= 0; i--)
				{
					string currentPass = rules[i].Apply(word);
					if (currentPass != null)
					{
						result = currentPass;
						break;
					}
				}
			}
			return result;
		}

		private class InflectorRule
		{
			/// <summary>
			/// 
			/// </summary>
			private readonly Regex _regex;

			/// <summary>
			/// 
			/// </summary>
			private readonly string _replacement;

			/// <summary>
			/// Initializes a new instance of the <see cref="InflectorRule"/> class.
			/// </summary>
			/// <param name="regexPattern">The regex pattern.</param>
			/// <param name="replacementText">The replacement text.</param>
			public InflectorRule(string regexPattern, string replacementText)
			{
				_regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
				_replacement = replacementText;
			}

			/// <summary>
			/// Applies the specified word.
			/// </summary>
			/// <param name="word">The word.</param>
			/// <returns></returns>
			public string Apply(string word)
			{
				if (!_regex.IsMatch(word))
					return null;

				string replace = _regex.Replace(word, _replacement);
				if (word == word.ToUpper())
					replace = replace.ToUpper();

				return replace;
			}
		}
	}
}

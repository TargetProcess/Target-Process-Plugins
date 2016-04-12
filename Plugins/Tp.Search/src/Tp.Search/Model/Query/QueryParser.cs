using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Tp.Search.Model.Document;
using hOOt;
using Tp.Utils.Html;

namespace Tp.Search.Model.Query
{
	class QueryParser
	{
		private readonly DocumentIndexSetup _setup;
		private readonly Regex _doubleQuotes;
		private readonly IEnumerable<Func<ParserContext, ParserContext>> _steps;
		private readonly SpecialSymbolRemover _symbolRemover;
		private readonly DigitsTokensParser _digitsTokensParser;
		public QueryParser(DocumentIndexSetup setup)
		{
			_setup = setup;
			_doubleQuotes = new Regex("\"[^\"]+\"");
			_symbolRemover = new SpecialSymbolRemover();
			_steps = new List<Func<ParserContext, ParserContext>>
				{
					ReplaceDigitsWithSpecSymbol,
					RemoveNewLines,
					MakeToLowerCase,
					TransformDoubleQuotesPhraseToMandatoryWords,
					SplitBySpecialSymbolsThenRemoveThem,
					RemoveEmptyWords,
					RemoveNotIndexedString,
					TransformNonMandatoryWordsIntoCandidatesForContains,
					AddMandatoryWords,
					RemoveDuplicateWords,
				};
			_digitsTokensParser = new DigitsTokensParser();
		}

		private ParserContext ReplaceDigitsWithSpecSymbol(ParserContext c)
		{
			var q = c.CurrentQuery;
			var buf = new StringBuilder();
			for (int i = 0; i < q.Length; ++i )
			{
				var symbol = q[i];
				if (!Char.IsDigit(symbol))
				{
					buf.Append(symbol);
				}
				else
				{
					buf.Append(' ');
				}
			}
			c.CurrentQuery = buf.ToString();
			return c;
		}

		public ParsedQuery Parse(string query)
		{
			var context = _steps.Aggregate(new ParserContext
				{
					CurrentQuery = query,
					OriginQuery = query
				}, (c, step) => step(c));
			return new ParsedQuery(words: context.CurrentQuery, numbers: ParseNumbers(query));
		}
		private string ParseNumbers(string query)
		{
			Dictionary<string, int> parsedDigits = _digitsTokensParser.Parse(query);
			return string.Join(" ", parsedDigits.Keys.Select(x => "+" + x).ToArray());
		}

		public IEnumerable<string> ParseIntoWords(string query)
		{
			var parsed = Parse(query);
			var result = new List<string>(Split(parsed.Words));
			result.AddRange(Split(parsed.Numbers));
			return result;
		}

		private ParserContext RemoveDuplicateWords(ParserContext c)
		{
			c.CurrentQuery = Join(Split(c.CurrentQuery).Distinct());
			return c;
		}

		private ParserContext AddMandatoryWords(ParserContext c)
		{
			if (c.MandatoryWords == null)
			{
				return c;
			}
			var mandatoryString = Join(c.MandatoryWords.Select(w => "+" + w));
			c.CurrentQuery = (c.CurrentQuery + " " + mandatoryString).Trim();
			return c;
		}

		private ParserContext RemoveNewLines(ParserContext context)
		{
			context.CurrentQuery = context.CurrentQuery.Replace("\r", " ").Replace("\n", " ");
			return context;
		}
	
		private ParserContext RemoveNotIndexedString(ParserContext c)
		{
			c.CurrentQuery = Join(Split(c.CurrentQuery).Where(IsIndexedString));
			return c;
		}

		private bool IsIndexedString(string w)
		{
			w = w.Trim('+', '-', '*', '?');
			return w.Length >= _setup.MinStringLengthToSearch && w.Length <= _setup.MaxStringLengthIgnore;
		}

		private ParserContext RemoveEmptyWords(ParserContext c)
		{
			c.CurrentQuery = Join(Split(c.CurrentQuery).Where(ss => !string.IsNullOrEmpty(ss)));
			return c;
		}

		private ParserContext TransformNonMandatoryWordsIntoCandidatesForContains(ParserContext c)
		{
			if (string.IsNullOrEmpty(c.CurrentQuery))
			{
				return c;
			}
			var newWords = Split(c.CurrentQuery).Aggregate(new List<string>(), (acc, w) =>
				{
					var word = w.All(l => l != '+' && l != '-' && l != '*' && l != '?')
						? "*" + w + "*"
						: w;
					acc.Add(word);
					return acc;
				});
			c.CurrentQuery = Join(newWords);
			return c;
		}

		private ParserContext TransformDoubleQuotesPhraseToMandatoryWords(ParserContext c)
		{
			var mandatoryWords = new List<string>();
			c.CurrentQuery = _doubleQuotes.Matches(c.CurrentQuery)
			                              .Cast<Match>()
			                              .Aggregate(c.CurrentQuery,
			                                         (current, m) =>
				                                         {
					                                         var words = m.Value.Trim('"')
					                                                          .Split(' ')
					                                                          .Select(w => w.Trim())
					                                                          .ToArray();
					                                         mandatoryWords.AddRange(words);
					                                         return current.Replace(m.Value, string.Empty);
				                                         });
			var mandatoryString = Join(mandatoryWords);
			var buf = new StringBuilder();
			foreach (var symbol in mandatoryString)
			{
				buf.Append(IsSupported(symbol) ? symbol : ' ');
			}
			c.MandatoryWords = Split(buf.ToString()).Where(w => !string.IsNullOrEmpty(w) && IsIndexedString(w)).ToList();
			return c;
		}

		private ParserContext SplitBySpecialSymbolsThenRemoveThem(ParserContext c)
		{
			c.CurrentQuery = Join(Split(c.CurrentQuery).SelectMany(word => _symbolRemover.Run(word)));
			return c;
		}

		private ParserContext MakeToLowerCase(ParserContext c)
		{
			c.CurrentQuery = c.CurrentQuery.ToLower();
			return c;
		}

		private class SpecialSymbolRemover
		{
			struct Word
			{
				public string Str { get; set; }
				public bool AutoDecorated { get; set; }
			}

			public IEnumerable<string> Run(string word)
			{
				var words = new List<Word>();
				var runningWord = new StringBuilder();
				char? lastSpecialSymbol = null;
				char? breakingSpecialSymbol = null;
				char? firstWildcard = null;
				for (int i = 0; i < word.Length; ++i)
				{
					breakingSpecialSymbol = null;
					char symbol = word[i];
					if (i == 0)
					{
						switch (symbol)
						{
							case '+':
							case '-':
							case '*':
							case '?':
								firstWildcard = symbol;
								continue;
						}
					}
					if (IsSupported(symbol))
					{
						runningWord.Append(symbol);
					}
					else
					{
						breakingSpecialSymbol = symbol;
						CreateWord(runningWord.ToString(), words, firstWildcard, lastSpecialSymbol);
						lastSpecialSymbol = symbol;
						runningWord.Clear();
					}
				}
				string lastWord = runningWord.ToString();
				runningWord.Clear();
				CreateWord(lastWord, words, firstWildcard, lastSpecialSymbol);
				if (breakingSpecialSymbol != null && words.Count != 0)
				{
					Word w = words.Last();
					if(!w.Str.StartsWith("+") && !w.Str.StartsWith("-"))
					{
						words[words.Count - 1] = new Word
							{
								Str = w.Str + (breakingSpecialSymbol == '*' || breakingSpecialSymbol == '?' ? breakingSpecialSymbol : '*'),
								AutoDecorated = true
							};
					}
				}
				if ((words.Count == 0 && lastSpecialSymbol == null))
				{
					return new List<string>
						{
							word
						};
				}
				if (words.Count == 1)
				{
					var head = words[0];
					if (!head.AutoDecorated)
					{
						return new List<string>{head.Str};
					}
					if (!head.Str.StartsWith("*") && head.Str.StartsWith("?"))
					{
						head.Str = "*" + head.Str;
					}
					if (!head.Str.EndsWith("*") && head.Str.StartsWith("?"))
					{
						head.Str += "*";
					}
					return new List<string> { head.Str };
				}
				return words.Select(w => w.Str).ToArray();
			}

			private char? ResolveWildcard(char? wildcard, char? lastSpecialSymbol, bool isFirstWord)
			{
				if (isFirstWord)
				{
					if (lastSpecialSymbol != null)
					{
						return '*';
					}
				}
				else
				{
					if (lastSpecialSymbol != null)
					{
						return '+';
					}
				}
				return wildcard;
			}

			private void CreateWord(string currentWord, List<Word> words, char? wildcard, char? lastSpecialSymbol)
			{
				if (currentWord.Length == 0)
				{
					return;
				}
				var isFirstWord = words.Count == 0;
				var resolveWildcard = ResolveWildcard(wildcard, lastSpecialSymbol, isFirstWord);
				words.Add(new Word
					{
						Str = resolveWildcard + currentWord,
						AutoDecorated = resolveWildcard != null && !isFirstWord
					});
			}
		}
		
		private static bool IsSupported(char c)
		{
			return Char.IsLetter(c);
		}
		
		private IEnumerable<string> Split(string s)
		{
			return s.Split(' ');
		}

		private string Join(IEnumerable<string> words)
		{
			return string.Join(" ", words);
		}

		private struct ParserContext
		{
			public string CurrentQuery { get; set; }
			public string OriginQuery { get; set; }
			public List<string> MandatoryWords { get; set; }
		}
	}
}

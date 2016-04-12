using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Tp.Utils.Html
{
	/// <summary>
	/// Clean up HTML code, remove dangerous fragments, such as styles, scripts, event attributes, forms, etc...
	/// </summary>
	/// <remarks>
	/// IDEA: Use <see cref="System.Web.UI.HtmlTextWriter">HtmlTextWriter from the .NET framework</see> to write resulting HTML code.
	/// </remarks>
	public class Sanitizer
	{
		/// <summary>
		/// Tags whose outer html to be suppressed. In other words, entire tag with its content will be suppressed.
		/// </summary>
		protected readonly HashSet<string> ExcludeTags = new HashSet<string>
		{
			"head",
			"base",
			"basefont",
			"meta",
			"link",
			"title",
			"style",
			"script",
			"input",
			"isindex",
			"textarea",
			"button",
			"option",
			"select",
			"frameset",
			"frame",
			"iframe",
			"object",
			"embed",
			"applet",
			"bgsound",
		};

		/// <summary>
		/// Overrides suppression caused by the tags above, enables outer html.
		/// </summary>
		protected readonly HashSet<string> IncludeTags = new HashSet<string>
		{
			"body",
		};

		/// <summary>
		/// Write inner instead of outer html for these tags. In other words, write tag content without tag.
		/// </summary>
		protected readonly HashSet<string> IgnoreTags = new HashSet<string>
		{
			"html",
			"body",
			"form",
			"blink",
			"plaintext",
		};

		/// <summary>
		/// These tags need not be closed explicitly.
		/// </summary>
		protected readonly HashSet<string> EmptyTags = new HashSet<string>
		{
			"base",
			"basefont",
			"meta",
			"link",
			"br",
			"hr",
			"input",
			"isindex",
			"img",
			"col",
			"frame",
			"param",
		};

		/// <summary>
		/// These tags may not be explicitly closed, but presense of a new open tag automaticaly closes the previously open but not closed tag.
		/// </summary>
		protected readonly HashSet<string> AutoClosedTags = new HashSet<string>
		{
			"p",
		};

		/// <summary>
		/// Rewrite some (obsolete, deprecated) tags to another tags.
		/// </summary>
		protected IDictionary<string, string> RewriteTags = new Dictionary<string, string> { { "plaintext", "pre" }, };

		/// <summary>
		/// Attributes to be suppressed.
		/// </summary>
		protected readonly HashSet<string> EventAttributes = new HashSet<string>
		{
			"onabort",
			"onblur",
			"onchange",
			"onclick",
			"ondblclick",
			"ondragdrop",
			"onerror",
			"onfocus",
			"onkeydown",
			"onkeypress",
			"onkeyup",
			"onload",
			"onmousedown",
			"onmousemove",
			"onmouseout",
			"onmouseover",
			"onmouseup",
			"onmove",
			"onreset",
			"onresize",
			"onselect",
			"onsubmit",
			"onunload"
		};

		/// <summary>
		/// Untouchable attributes.
		/// </summary>
		protected readonly HashSet<string> UntouchableAttributes = new HashSet<string>
		{
			"style",
			"src",
			"data-mention"
		};

		public Sanitizer()
		{
			RemoveStyles = true;
			RemoveForms = false;
			RemoveIds = true;
			OuterTag = null;
		}

		/// <summary>
		/// Remove <em>&lt;style...&gt;</em> elements from html.
		/// </summary>
		public bool RemoveStyles { get; set; }

		/// <summary>
		/// Remove <em>&lt;form...&gt;</em> and <em>&lt;input...&gt;</em> elements from html.
		/// </summary>
		public bool RemoveForms { get; set; }

		/// <summary>
		/// Remove <em>&lt;id=&quot;...&quot;&gt;</em> attributes from html to avoid duplicate ids.
		/// </summary>
		public bool RemoveIds { get; set; }

		/// <summary>
		/// Wrap result with outer tag, e.g. <em>&lt;div&gt;sanitized markup&lt;/div&gt;</em>.
		/// </summary>
		public string OuterTag { get; set; }

		/// <summary>
		/// Stack with tags.
		/// </summary>
		protected readonly List<string> _tags = new List<string>();

		/// <summary>
		/// Whether to write elements.
		/// </summary>
		protected bool _enabled = true;

		/// <summary>
		/// Sanitize input HTML using default settings.
		/// </summary>
		/// <param name="input">Input HTML. May be <c>null</c>.</param>
		/// <returns>Sanitized HTML.</returns>
		public static string Sanitize(string input)
		{
			return new Sanitizer().Process(input);
		}

		public string Process(string input)
		{
			if (string.IsNullOrEmpty(input) || IsMarkdown(input))
			{
				return input;
			}

			var inputReader = new StringReader(input);
			var resultWriter = new StringWriter();
			resultWriter.GetStringBuilder().EnsureCapacity(input.Length + 32);
			resultWriter.NewLine = "\n";
			Sanitize(inputReader, resultWriter);
			return resultWriter.ToString();
		}

		private const string _markdownTag = "<!--markdown-->";

		public static bool IsMarkdown(string input)
		{
			return input.StartsWith(_markdownTag);
		}

		public static string ClearMarkdownTag(string input)
		{
			return IsMarkdown(input) ? input.Substring(_markdownTag.Length) : input;
		}

		public void Sanitize(TextReader input, TextWriter result)
		{
			if (input == null)
			{
				throw new ArgumentNullException(nameof(input));
			}
			if (result == null)
			{
				throw new ArgumentNullException(nameof(result));
			}
			using (var htmlReader = new HtmlReader(input))
			{
				Sanitize(htmlReader, result);
			}
			Reset();
		}

		/// <summary>
		/// Reset internal state left from previous run.
		/// </summary>
		protected virtual void Reset()
		{
			_tags.Clear();
			_enabled = true;
		}

		protected virtual void Sanitize(HtmlReader htmlReader, TextWriter result)
		{
			BeforeDocument(result);
			var entity = string.Empty;

			while (htmlReader.Read())
			{
				entity = htmlReader.Entity;

				switch (htmlReader.NodeType)
				{
					case HtmlNodeType.Element:
						CaseElement(htmlReader, result);
						break;

					case HtmlNodeType.EndElement:
						CaseEndElement(htmlReader, result);
						break;

					case HtmlNodeType.Text:
						CaseText(htmlReader, result);
						break;

					case HtmlNodeType.CDATA:
						CaseCData(htmlReader, result);
						break;
				}
			}
			// close unclosed tags
			while (_tags.Count > 0)
			{
				string tagName = PopTag();
				CheckStack();
				if (_enabled && !IgnoreTags.Contains(tagName))
				{
					WriteEndElement(result, tagName);
				}
			}

			result.Write(entity);

			if (!string.IsNullOrEmpty(htmlReader.Name))
				result.Write("&lt;{0}", htmlReader.Name);

			AfterDocument(result);
			result.Flush();
		}

		protected virtual void BeforeDocument(TextWriter result)
		{
			if (!string.IsNullOrEmpty(OuterTag))
			{
				WriteElement(result, OuterTag, new Dictionary<string, string>(), false);
			}
		}

		protected virtual void AfterDocument(TextWriter result)
		{
			if (!string.IsNullOrEmpty(OuterTag))
			{
				WriteEndElement(result, OuterTag);
			}
		}

		protected virtual void CaseElement(HtmlReader htmlReader, TextWriter result)
		{
			string tagName = htmlReader.Name.ToLowerInvariant();

			// automatically close unclosed tag, if any
			if (AutoClosedTags.Contains(tagName))
			{
				if (FuzzyPopTag(tagName))
				{
					WriteEndElement(result, RewriteTag(tagName));
				}
			}
			// reevaluate tags stack
			PushTag(tagName);
			CheckStack();
			if (_enabled && !IgnoreTags.Contains(tagName))
			{
				WriteElement(result, RewriteTag(tagName), htmlReader.Attributes, htmlReader.IsEmptyElement);
			}
			// pop empty tags immediately
			if (htmlReader.IsEmptyElement || EmptyTags.Contains(tagName))
			{
				if (FuzzyPopTag(tagName))
				{
					CheckStack();
				}
			}
		}

		protected virtual void CaseEndElement(HtmlReader htmlReader, TextWriter result)
		{
			string tagName = htmlReader.Name.ToLowerInvariant();
			if (FuzzyPopTag(tagName))
			{
				// only close tag if it was previously open
				if (_enabled && !IgnoreTags.Contains(tagName))
				{
					WriteEndElement(result, RewriteTag(tagName));
				}
				CheckStack();
			}
		}

		protected virtual void CaseText(HtmlReader htmlReader, TextWriter result)
		{
			if (_enabled)
			{
				WriteText(result, htmlReader.Value);
			}
		}

		protected virtual void CaseCData(HtmlReader htmlReader, TextWriter result)
		{
			if (_enabled)
			{
				WriteCData(result, htmlReader.Value);
			}
		}

		protected virtual void CheckStack()
		{
			_enabled = true;
			int count = _tags.Count;
			for (int i = count; i > 0; i--)
			{
				string tag = _tags[i - 1];
				if (IncludeTags.Contains(tag))
				{
					_enabled = true;
					return;
				}
				if (ExcludeTags.Contains(tag))
				{
					_enabled = false;
					return;
				}
			}
		}

		protected virtual void WriteElement(TextWriter result, string name, Dictionary<string, string> attributes, bool empty)
		{
			result.Write("<");
			result.Write(name);
			foreach (var entry in attributes)
			{
				var key = entry.Key.ToLowerInvariant();
				var value = entry.Value;
				if (IsValidAttribute(name, key, value))
				{
					result.Write(' ');
					result.Write(key);
					if (value != null)
					{
						result.Write('=');
						result.Write('"');
						//HttpUtility.HtmlAttributeEncode(value, result);
						if (UntouchableAttributes.Contains(key))
							result.Write(value);
						else
							HtmlAttributeEncode(value, result);
						result.Write('"');
					}
				}
			}
			if (empty /* && !EmptyTags.Contains(name)*/)
			{
				result.Write(" /");
			}
			result.Write(">");
		}

		protected virtual void HtmlAttributeEncode(string value, TextWriter writer)
		{
			Encoder.HtmlAttributeEncode(value, writer);
		}

		protected virtual void HtmlEncode(string value, TextWriter writer)
		{
			Encoder.HtmlEncode(value, writer);
		}

		/// <summary>
		/// Checks for rewrite and rewrites tag if necessary
		/// </summary>
		protected virtual string RewriteTag(string tagName)
		{
			if (RewriteTags.Keys.Contains(tagName))
			{
				return RewriteTags[tagName];
			}
			return tagName;
		}

		protected virtual bool IsValidAttribute(string name, string key, string value)
		{
			// Suppress event attributes.
			if (EventAttributes.Contains(key))
			{
				return false;
			}

			// Suppress "href='javascript:...'" attributes.
			if (key == "href" && value.IndexOf("javascript", StringComparison.InvariantCultureIgnoreCase) != -1)
			{
				return false;
			}
			if (key == "id" && RemoveIds)
			{
				return false;
			}
			return true;
		}

		protected virtual void WriteEndElement(TextWriter result, string name)
		{
			result.Write("</");
			result.Write(name);
			result.Write(">");
		}

		protected virtual void WriteText(TextWriter result, string value)
		{
			//HttpUtility.HtmlEncode(value, result); we think that the standard encoder is weak
			HtmlEncode(value, result);
		}

		protected virtual void WriteCData(TextWriter result, string value)
		{
			result.Write("<![CDATA[");
			result.Write(value);
			result.Write("]]>");
		}

		private void PushTag(string item)
		{
			_tags.Add(item);
		}

		protected string PopTag()
		{
			int index = _tags.Count - 1;
			string tag = _tags[index];
			_tags.RemoveAt(index);
			return tag;
		}

		/// <summary>
		/// Finds the specified tag somewhere in the stack and removes it from there.
		/// </summary>
		/// <param name="item">Item to remove.</param>
		/// <returns><c>true</c> if found and removed; <c>false</c> otherwise.</returns>
		protected bool FuzzyPopTag(string item)
		{
			int i = _tags.LastIndexOf(item);
			if (i != -1)
			{
				_tags.RemoveAt(i);
				return true;
			}
			return false;
		}

		/// <summary>
		/// The last open tag, or <code>null</code>.
		/// </summary>
		protected string TopTag
		{
			get
			{
				if (_tags.Count > 0)
				{
					return _tags[_tags.Count - 1];
				}
				return null;
			}
		}

		#region Obsolete

		/// <summary>
		/// Htmlize text, that is replace new line with &lt;br/&gt;, etc.
		/// </summary>
		/// <remarks>
		/// This method will try to sniff whether the original text is already a html document.
		/// </remarks>
		/// <param name="text">Original text, may be <code>null</code>.</param>
		/// <returns>Htmlized text.</returns>
		public static string TextToHtml(string text)
		{
			if (text == null)
			{
				return null;
			}

			// thext is already HTML
			if (Regex.Match(text, "<(html|body|br)[^>]*>", RegexOptions.IgnoreCase).Success)
			{
				return text;
			}

			string newText = text.Replace(Environment.NewLine, "<br />");
			newText = Regex.Replace(newText, "(>\\s*<br[^>]*>\\s*<)+", "><", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			return newText;
		}

		#endregion

		public static string FormatComment(string text)
		{
			if (text == null)
			{
				return null;
			}

			if (Sanitizer.IsMarkdown(text))
			{
				return text;
			}

			const string regExpOutlook2007 =
				@"<div>\s*<div\s+style=[""']border[^>]+>\s*<p[^>]*>\s*<b[^>]*>\s*<span[^>]*>\s*From.*\z";
			const string regExpOutlook2003 =
				@"<div>\s*<div\s+style=[""'][^>]+>\s*<font[^>]*>\s*<span[^>]*>\s*<hr[^>]*>\s*</span>\s*</font>\s*</div>\s*<p[^>]*>\s*<b[^>]*>\s*<font[^>]*>\s*<span[^>]*>\s*From.*\z";
			const string regExpOutlookExpress =
				@"<blockquote[^>]*>\s*<div[^>]*>\s*-----\s*original\s*message\s*-----\s*</div>\s*<div[^>]*><b[^>]*>\s*From.*\z";
			const string regExpGmail = @"<br[^>]*>\s*<br[^>]*>\s*<div\s+class=['""]gmail_quote['""][^>]*>.*\z";
			const string regExpMac = @"<div[^>]*>\s*<div[^>]*>\s*On[^<]*<a[^>]*>[^<]*</a>\s*wrote[^<]*</div>\s*<br[^>]*>.*\z";
			const string regExpMacPlain = @"<div[^>]*>\s*<div[^>]*>\s*On[^<]*</div>\s*<br[^>]*>\s*<blockquote[^>]*>.*\z";

			const string regUnknown1 =
				@"<div[^>]*?class=[""']OutlookMessageHeader[""'][^>]*>\s*<hr[^>]*>\s*<font[^>]*>\s*<b[^>]*>From.*\z";
			const string regUnknown2 =
				@"<br[^>]*>(([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9}))\s*wrote(:|&#58;)\s*<br[^>]*>.*\z";

			//Outlook 2007 Remove Replies
			string newText = Regex.Replace(text, regExpOutlook2007, "",
				RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

			//Outlook 2003 Remove Replies
			newText = Regex.Replace(newText, regExpOutlook2003, "",
				RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

			//Outlook Express Remove Replies
			newText = Regex.Replace(newText, regExpOutlookExpress, "",
				RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

			//Gmail Remove Replies
			newText = Regex.Replace(newText, regExpGmail, "",
				RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

			//Mac Remove Replies
			newText = Regex.Replace(newText, regExpMac, "",
				RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

			//Mac Plain Text Remove Replies
			newText = Regex.Replace(newText, regExpMacPlain, "",
				RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

			//Unknown1 Remove Replies
			newText = Regex.Replace(newText, regUnknown1, "",
				RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

			//Unknown2 Remove Replies
			newText = Regex.Replace(newText, regUnknown2, "",
				RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

			return newText;
		}
	}
}

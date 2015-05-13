// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections.Generic;
using System.IO;

namespace Tp.Utils.Html
{
	/// <summary>
	/// A specialized sanitizer which outputs formatted plain text instead of HTML.
	/// </summary>
	/// <remarks>
	/// The renderer removes extra whitespace, and converts block level HTML elements to lines of plain text.
	/// </remarks>
	public class PlainTextRenderer : Sanitizer
	{
		/// <summary>
		/// These tags start new line when HTML is converted to text.
		/// </summary>
		private readonly HashSet<string> BlockElementTags = new HashSet<string>
		{
			// Headings
			"h1", "h2", "h3", "h4", "h5", "h6", "h7", "h8", "h9",
			// Text containers
			"p", "pre", "blockquote", "address",
			// Lists
			"ul", "ol", "li", "dl", "dt", "dd", "dir", "menu",
			// Other
			"div", "center", "form", "table",
			// Tables
			"caption", "tr", //"td", "th",
			// Explicit line break
			"br", "hr",
		};

		/// <summary>
		/// If new line has already been written.
		/// </summary>
		protected bool _newLineStarted = true;

		/// <summary>
		/// Whether currently outputing whitespace.
		/// </summary>
		protected bool _whiteSpaceStarted;

		/// <summary>
		/// Converts HTML to plain text.
		/// </summary>
		/// <remarks>
		/// Warning, the returned text may contain HTML fragments!!!
		/// For example, the original HTML code is:
		/// <code>
		/// &lt;p&gt;&amp;lt;hello&amp;lt;&gt;/p&gt;
		/// </code>
		/// Then, the converted text will be:
		/// <code>
		/// &lt;hello&gt;
		/// </code>
		/// So if you intend to output the stripped text in HTML, you need encode it first!
		/// </remarks>
		/// <param name="input">Input HTML. May be <c>null</c>.</param>
		/// <param name="newLine">The line terminator string</param>
		/// <returns>Plain text, may contain angle braces!</returns>
		public static string RenderToPlainText(string input, string newLine = "\n")
		{
			if (input == null)
			{
				return null;
			}

			var inputReader = new StringReader(input);
			var resultWriter = new StringWriter();
			resultWriter.GetStringBuilder().EnsureCapacity(input.Length);
			resultWriter.NewLine = newLine;
			var textRenderer = new PlainTextRenderer();
			textRenderer.Sanitize(inputReader, resultWriter);
			return resultWriter.ToString();
		}

		protected override void BeforeDocument(TextWriter result)
		{
			//
		}

		protected override void AfterDocument(TextWriter result)
		{
			//
		}

		protected override void WriteElement(TextWriter result, string name, Dictionary<string, string> attributes, bool empty)
		{
			if (BlockElementTags.Contains(name))
			{
				StartNewLine(result);
			}
		}

		protected override void WriteEndElement(TextWriter result, string name)
		{
			if (BlockElementTags.Contains(name))
			{
				StartNewLine(result);
			}
		}

		protected override void WriteText(TextWriter result, string value)
		{
			if (TopTag == "pre")
			{
				result.Write(value);
			}
			else
			{
				RenderText(result, value);
			}
		}

		protected override void WriteCData(TextWriter result, string value)
		{
			WriteText(result, value);
		}

		protected virtual void StartNewLine(TextWriter result)
		{
			if (!_newLineStarted)
			{
				_newLineStarted = true;
				result.WriteLine();
			}
			_whiteSpaceStarted = false;
		}

		protected virtual void RenderText(TextWriter result, string value)
		{
			foreach (char ch in value)
			{
				if (Char.IsWhiteSpace(ch))
				{
					if (_newLineStarted)
					{
						continue;
					}
					_whiteSpaceStarted = true;
				}
				else
				{
					if (_whiteSpaceStarted)
					{
						result.Write(' ');
						_whiteSpaceStarted = false;
					}
					result.Write(ch);
					_newLineStarted = false;
				}
			}
		}
	}
}
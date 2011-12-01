// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;

namespace Tp.Integration.Plugin.TestRunImport.Utils
{
	public enum HtmlNodeType
	{
		/// <summary>
		/// This is returned by the <see cref="HtmlReader"/> if the Read <see cref="HtmlReader.Read"/> method has not been called.
		/// </summary>
		None,
		/// <summary>
		/// The XML declaration (for example, <code>&lt;?xml version='1.0'?&gt;</code>).
		/// </summary>
		XmlDeclaration,
		/// <summary>
		/// The document type declaration, indicated by the following tag (for example, <code>&lt;!DOCTYPE...&gt;</code>).
		/// </summary>
		DocumentType,
		/// <summary>
		/// An element (for example, <code>&lt;item name=&quot;root&quot;&gt;</code>).
		/// </summary>
		Element,
		/// <summary>
		/// An end element tag (for example, <code>&lt;/item&gt;</code>).
		/// </summary>
		EndElement,
		/// <summary>
		/// The text content of a node.
		/// </summary>
		Text,
		/// <summary>
		/// A CDATA section (for example, <code>&lt;![CDATA[my escaped text]]&gt;</code>).
		/// </summary>
		CDATA,
		/// <summary>
		/// White space between markup.
		/// </summary>
		Whitespace,
		/// <summary>
		/// A processing instruction (for example, <code>&lt;?pi test?&gt;</code>).
		/// </summary>
		ProcessingInstruction,
		/// <summary>
		/// A comment (for example, <code>&lt;!-- my comment --&gt;</code>).
		/// </summary>
		Comment,
	}

	/// <summary>
	/// A very loose HTML parser.
	/// </summary>
	/// <remarks>
	/// See also <a href="http://code.google.com/p/twintsam/">twintsam project</a> 
	/// for an alternative and more capable implementation of HtmlReader compatible 
	/// with .NET framework XML readers.
	/// </remarks>
	public class HtmlReader : IDisposable
	{
		private enum State
		{
			XmlDeclaration = 1,
			DocType = 2,
			Pi = 3,
			PiValue = 4,
			Comment = 5,
			Text = 6,
			CData = 7,
			Amp = 8,
			Lt = 9,
			ElemName = 10,
			ElemClose = 11,
			ElemSingle = 12,
			ElemAttributes = 13,
			AttrKey = 14,
			AttrEq = 15,
			AttrQuote = 16,
			AttrValue = 17,
		}

		private readonly TextPeeker _peeker;

		private static HtmlNodeType _nodeType = HtmlNodeType.None;

		private readonly StringBuilder _name = new StringBuilder();

		private readonly StringBuilder _value = new StringBuilder();

		private StringDictionary _attributes = new StringDictionary();

		private bool _isEmptyElement;

		private State _state = State.Text;

		public HtmlReader(TextReader textReader)
		{
			if (textReader == null)
			{
				throw new ArgumentNullException("textReader");
			}
			_peeker = new TextPeeker(textReader);
		}

		public HtmlNodeType NodeType
		{
			get { return _nodeType; }
		}

		public string Name
		{
			get { return _name.ToString(); }
		}

		public string Value
		{
			get { return _value.ToString(); }
		}

		public StringDictionary Attributes
		{
			get { return _attributes; }
		}

		public bool IsEmptyElement
		{
			get { return _isEmptyElement; }
		}

		public bool Read()
		{
			_nodeType = HtmlNodeType.None;
			_name.Length = 0;
			_value.Length = 0;
			_isEmptyElement = false;

			var attrName = new StringBuilder();
			var attrValue = new StringBuilder();

			var quoteStyle = '"';
			var customDoctype = false;
			StringBuilder entity = null;

			while (_peeker.Read())
			{
				char c = _peeker.Current;

				switch (_state)
				{
					case State.Text:
						if (c == '&')
						{
							entity = new StringBuilder();
							_state = State.Amp;
						}
						else if (c == '<')
						{
							_state = State.Lt;
							if (_value.Length > 0)
							{
								_nodeType = HtmlNodeType.Text;
								return true;
							}
						}
						else
						{
							_value.Append(c);
						}
						break;

					case State.Amp:
						if (c == ';')
						{
							_state = State.Text;
							if (entity.Length > 0)
							{
								_value.Append(DecodeEntity("&" + entity + ";"));
							}
							else
							{
								_value.Append("&");
								_value.Append(";");
							}
						}
						else if (c == '#' && entity.Length == 0)
						{
							entity.Append(c);
						}
						else if (Char.IsLetterOrDigit(c))
						{
							entity.Append(c);
						}
						else
						{
							_state = State.Text;
							_peeker.Push(c);
							if (entity.Length > 0)
							{
								_value.Append(DecodeEntity("&" + entity + ";"));
							}
							else
							{
								_value.Append("&");
							}
							entity = null;
						}
						break;

					case State.Lt:
						if (c == '/')
						{
							_state = State.ElemClose;
						}
						else if (c == '?' && _peeker.Match("xml"))
						{
							_state = State.XmlDeclaration;
							_peeker.Read(3);
						}
						else if (c == '?')
						{
							_state = State.Pi;
						}
						else if (c == '!' && _peeker.Match("--"))
						{
							_peeker.Read(2);
							_state = State.Comment;
						}
						else if (c == '!' && _peeker.Match("[CDATA["))
						{
							_peeker.Read(7);
							_state = State.CData;
						}
						else if (c == '!' && _peeker.Match("DOCTYPE"))
						{
							_peeker.Read(7);
							_state = State.DocType;
						}
						else if (!Char.IsLetter(c))
						{
							_state = State.Text;
							_value.Append('<');
							_value.Append(c);
						}
						else
						{
							_attributes = new StringDictionary();
							_state = State.ElemName;
							_name.Append(c);
						}
						break;

					case State.ElemName:
						if (Char.IsWhiteSpace(c))
						{
							_state = State.ElemAttributes;
						}
						else if (c == '/')
						{
							_isEmptyElement = true;
							_state = State.ElemSingle;
						}
						else if (c == '>')
						{
							_state = State.Text;
							_nodeType = HtmlNodeType.Element;
							return true;
						}
						else
						{
							_name.Append(c);
						}
						break;

					case State.ElemClose:
						if (c == '>')
						{
							_state = State.Text;
							_nodeType = HtmlNodeType.EndElement;
							return true;
						}
						_name.Append(c);
						break;

					case State.ElemSingle:
						if (c == '>')
						{
							_nodeType = HtmlNodeType.Element;
							return true;
						}
						_state = State.Text;
						_nodeType = HtmlNodeType.None;
						_name.Length = 0;
						_value.Length = 0;
						_value.Append(c);
						break;

					case State.ElemAttributes:
						if (c == '>')
						{
							_state = State.Text;
							_nodeType = HtmlNodeType.Element;
							return true;
						}
						else if (c == '/')
						{
							_isEmptyElement = true;
							_state = State.ElemSingle;
						}
						else if (Char.IsWhiteSpace(c)) {}
						else
						{
							_state = State.AttrKey;
							attrName.Append(c);
						}
						break;

					case State.Comment:
						if (c == '-' && _peeker.Match("->"))
						{
							_peeker.Read(2);
							_state = State.Text;
							_nodeType = HtmlNodeType.Comment;
							return true;
						}
						_value.Append(c);
						break;

					case State.CData:
						if (c == ']' && _peeker.Match("]>"))
						{
							_peeker.Read(2);
							_state = State.Text;
							_nodeType = HtmlNodeType.CDATA;
							return true;
						}
						_value.Append(c);
						break;

					case State.XmlDeclaration:
						if (c == '?' && _peeker.Match(">"))
						{
							_peeker.Read(1);
							_state = State.Text;
							_nodeType = HtmlNodeType.XmlDeclaration;
							return true;
						}
						_value.Append(c);
						break;

					case State.DocType:
						if (c == '[')
						{
							customDoctype = true;
						}
						else
						{
							if (customDoctype)
							{
								if (c == ']' && _peeker.Match(">"))
								{
									_peeker.Read(1);
									_state = State.Text;
									_nodeType = HtmlNodeType.DocumentType;
									return true;
								}
								_value.Append(c);
							}
							else
							{
								if (c == '>')
								{
									_state = State.Text;
									_nodeType = HtmlNodeType.DocumentType;
									return true;
								}
								_name.Append(c);
							}
						}

						break;

					case State.Pi:
						if (c == '?' && _peeker.Match(">"))
						{
							_peeker.Read(1);
							_state = State.Text;
							_nodeType = HtmlNodeType.ProcessingInstruction;
							return true;
						}
						if (Char.IsWhiteSpace(c))
						{
							_state = State.PiValue;
						}
						else
						{
							_name.Append(c);
						}
						break;

					case State.PiValue:
						if (c == '?' && _peeker.Match(">"))
						{
							_peeker.Read(1);
							_state = State.Text;
							_nodeType = HtmlNodeType.ProcessingInstruction;
							return true;
						}
						_value.Append(c);
						break;

					case State.AttrKey:
						if (Char.IsWhiteSpace(c))
						{
							_state = State.AttrEq;
						}
						else if (c == '=')
						{
							_state = State.AttrValue;
						}
						else if (c == '>')
						{
							_attributes[attrName.ToString()] = null;
							_state = State.ElemAttributes;
							_peeker.Push(c);
							attrName.Length = 0;
							attrValue.Length = 0;
						}
						else
						{
							attrName.Append(c);
						}
						break;

					case State.AttrEq:
						if (Char.IsWhiteSpace(c)) {}
						else if (c == '=')
						{
							_state = State.AttrValue;
						}
						else
						{
							_attributes[attrName.ToString()] = null;
							_state = State.ElemAttributes;
							_peeker.Push(c);
							attrName.Length = 0;
							attrValue.Length = 0;
						}
						break;

					case State.AttrValue:
						if (Char.IsWhiteSpace(c)) {}
						else if (c == '"' || c == '\'')
						{
							quoteStyle = c;
							_state = State.AttrQuote;
						}
						else
						{
							quoteStyle = ' ';
							_state = State.AttrQuote;
							attrValue.Append(c);
						}
						break;

					case State.AttrQuote:
						if (c == quoteStyle || (' ' == quoteStyle && c == '>'))
						{
							_attributes[attrName.ToString()] = HttpUtility.HtmlDecode(attrValue.ToString());
							_state = State.ElemAttributes;
							if (' ' == quoteStyle && c == '>')
							{
								_peeker.Push(c);
							}
							attrName.Length = 0;
							attrValue.Length = 0;
						}
						else
						{
							attrValue.Append(c);
						}
						break;
				}
			}

			switch (_state)
			{
				case State.Text:
					_state = 0;
					if (_value.Length > 0)
					{
						_nodeType = HtmlNodeType.Text;
						return true;
					}
					return false;

				case State.Amp:
					_state = 0;
					_value.Append('&');
					_nodeType = HtmlNodeType.Text;
					return true;

				case State.Lt:
					_state = 0;
					_value.Append('<');
					_nodeType = HtmlNodeType.Text;
					return true;
			}

			return false;
		}

		public string GetInnerTextUpToElement(string elementName, HtmlNodeType nodeType)
		{
			var s = new StringBuilder();
			while (Read())
			{
				if (NodeType == nodeType && Name.ToUpper() == elementName.ToUpper())
				{
					break;
				}
				if (NodeType == HtmlNodeType.Text || NodeType == HtmlNodeType.CDATA)
				{
					var part = Value.Replace("\r\n", "");
					s.Append(Value);
				}
			}
			return s.ToString().Trim();
		}

		public bool ReadToFollowing(string elementName)
		{
			while (Read())
			{
				if (NodeType == HtmlNodeType.Element && Name.ToUpper() == elementName.ToUpper())
				{
					return true;
				}
			}
			return false;
		}

		public string ReadElementString()
		{
			var s = new StringBuilder();
			while (Read())
			{
				if (NodeType == HtmlNodeType.Text || NodeType == HtmlNodeType.CDATA)
				{
					s.Append(Value);
				}
				else
				{
					break;
				}
			}
			return s.ToString();
		}

		private string DecodeEntity(string s)
		{
			return HttpUtility.HtmlDecode(s);
		}

		public void Dispose()
		{
			//
		}
	}

	public sealed class TextPeeker : IDisposable
	{
		private const int Bof = -2;
		private const int Eof = -1;

		private readonly TextReader _reader;

		private readonly StringBuilder _buf = new StringBuilder();

		private int _readPos;

		private int _current = Bof;

		public TextPeeker(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			_reader = reader;
		}

		public bool IsBof
		{
			get { return _current == Bof; }
		}

		public bool IsEof
		{
			get { return _current == Eof; }
		}

		public char Current
		{
			get
			{
				if (_current == Bof)
				{
					throw new InvalidOperationException("Read has not been called yet");
				}
				if (_current == Eof)
				{
					throw new InvalidOperationException("Cannot read past end of stream");
				}
				return (char) _current;
			}
		}

		public bool Read()
		{
			if (_buf.Length > 0)
			{
				if (_readPos < _buf.Length)
				{
					_current = _buf[_readPos];
					_readPos++;
					return true;
				}
				_buf.Length = _readPos = 0;
			}
			_current = _reader.Read();
			return _current != Eof;
		}

		public bool Read(int count)
		{
			while (count > 0)
			{
				if (!Read())
				{
					return false;
				}
				count--;
			}
			return true;
		}

		public void Push(char s)
		{
			_buf.Insert(_readPos, s);
		}

		public void Push(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (s.Length == 0)
			{
				throw new ArgumentException("Cannot push empty string");
			}
			_buf.Insert(_readPos, s);
		}

		public bool Match(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (s.Length == 0)
			{
				throw new ArgumentException("Cannot match empty string");
			}
			for (int i = 0; i < s.Length; i++)
			{
				if (_buf.Length < i + _readPos + 1)
				{
					int n = _reader.Read();
					if (n != Eof)
					{
						_buf.Append((char) n);
					}
					else
					{
						return false;
					}
				}
				if (_buf[i + _readPos] != s[i])
				{
					return false;
				}
			}
			return true;
		}

		public void Dispose()
		{
			_reader.Dispose();
		}
	}
}
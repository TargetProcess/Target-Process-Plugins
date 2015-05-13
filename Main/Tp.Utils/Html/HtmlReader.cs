// 
// Copyright (c) 2005-2009 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Tp.Utils.IO;

namespace Tp.Utils.Html
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
		/// The XML namespace (for example, <code>&lt;?xml:namespace ns ="urn:schemas-microsoft-com:office:office"/&gt;</code>).
		/// </summary>
		XmlNamespace,
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
			XmlNamespace = 18
		}

		private readonly PushBackTextReader _reader;

		private static HtmlNodeType _nodeType = HtmlNodeType.None;

		private readonly StringBuilder _name = new StringBuilder();

		private readonly StringBuilder _value = new StringBuilder();

		private Dictionary<string, string> _attributes = new Dictionary<string, string>();

		private bool _isEmptyElement;

		private State _state = State.Text;
		private StringBuilder _entity;

		public HtmlReader(TextReader textReader)
		{
			if (textReader == null)
			{
				throw new ArgumentNullException("textReader");
			}
			_reader = new PushBackTextReader(textReader);
		}

		public HtmlNodeType NodeType
		{
			get { return _nodeType; }
		}

		public string Entity
		{
			get { return _entity.ToString(); }
		}

		public string Name
		{
			get { return _name.ToString(); }
		}

		public string Value
		{
			get { return _value.ToString(); }
		}

		public Dictionary<string, string> Attributes
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
			_entity = new StringBuilder();

			while (_reader.Read())
			{
				char c = _reader.Current;

				switch (_state)
				{
					case State.Text:
						if (c == '&')
						{
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
							if (_entity.Length > 0)
							{
								_value.Append(DecodeEntity("&" + _entity + ";"));
							}
							else
							{
								_value.Append("&");
								_value.Append(";");
							}
							_entity = new StringBuilder();
						}
						else if (c == '#' && _entity.Length == 0)
						{
							_entity.Append(c);
						}
						else if (Char.IsLetterOrDigit(c))
						{
							_entity.Append(c);
						}
						else if (c == '=')
						{
							_state = State.Text;
							_value.Append(DecodeEntity("&" + _entity + "="));
							_entity = new StringBuilder();
						}
						else
						{
							_state = State.Text;
							_reader.Push(c);
							if (_entity.Length > 0)
							{
								_value.Append(DecodeEntity("&" + _entity));
							}
							else
							{
								_value.Append("&");
							}
							_entity = new StringBuilder();
						}
						break;

					case State.Lt:
						if (c == '/')
						{
							_state = State.ElemClose;
						}
						else if (c == '?' && _reader.Match("xml:namespace"))
						{
							_state = State.XmlNamespace;
							_reader.Read(13);
						}
						else if (c == '?' && _reader.Match("xml"))
						{
							_state = State.XmlDeclaration;
							_reader.Read(3);
						}
						else if (c == '?')
						{
							_state = State.Pi;
						}
						else if (c == '!' && _reader.Match("--"))
						{
							_reader.Read(2);
							_state = State.Comment;
						}
						else if (c == '!' && _reader.Match("[CDATA["))
						{
							_reader.Read(7);
							_state = State.CData;
						}
						else if (c == '!' && _reader.Match("DOCTYPE"))
						{
							_reader.Read(7);
							_state = State.DocType;
						}
						else if (c == '!' && _reader.Match("["))
						{
							//This is a special case for word conditional comments like '<![if !supportLists]>'
							//Treat them as DocType so they will not be included into markup.
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
							_attributes = new Dictionary<string, string>();
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
							_state = State.Text;
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
						else if (Char.IsWhiteSpace(c)) { }
						else
						{
							_state = State.AttrKey;
							attrName.Append(c);
						}
						break;

					case State.Comment:
						if (c == '-' && _reader.Match("->"))
						{
							_reader.Read(2);
							_state = State.Text;
							_nodeType = HtmlNodeType.Comment;
							return true;
						}
						_value.Append(c);
						break;

					case State.CData:
						if (c == ']' && _reader.Match("]>"))
						{
							_reader.Read(2);
							_state = State.Text;
							_nodeType = HtmlNodeType.CDATA;
							return true;
						}
						_value.Append(c);
						break;

					case State.XmlDeclaration:
						if (c == '?' && _reader.Match(">"))
						{
							_reader.Read(1);
							_state = State.Text;
							_nodeType = HtmlNodeType.XmlDeclaration;
							return true;
						}
						_value.Append(c);
						break;

					case State.XmlNamespace:
						if ((c == '/' || c == '?') && _reader.Match(">"))
						{
							_reader.Read(1);
							_state = State.Text;
							_nodeType = HtmlNodeType.XmlNamespace;
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
								if (c == ']' && _reader.Match(">"))
								{
									_reader.Read(1);
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
						if (c == '?' && _reader.Match(">"))
						{
							_reader.Read(1);
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
						if (c == '?' && _reader.Match(">"))
						{
							_reader.Read(1);
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
							_attributes[attrName.ToString().ToLower()] = null;
							_state = State.ElemAttributes;
							_reader.Push(c);
							attrName.Length = 0;
							attrValue.Length = 0;
						}
						else
						{
							attrName.Append(c);
						}
						break;

					case State.AttrEq:
						if (Char.IsWhiteSpace(c)) { }
						else if (c == '=')
						{
							_state = State.AttrValue;
						}
						else
						{
							_attributes[attrName.ToString().ToLower()] = null;
							_state = State.ElemAttributes;
							_reader.Push(c);
							attrName.Length = 0;
							attrValue.Length = 0;
						}
						break;

					case State.AttrValue:
						if (Char.IsWhiteSpace(c)) { }
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
							_attributes[attrName.ToString().ToLower()] = HttpUtility.HtmlDecode(attrValue.ToString());
							_state = State.ElemAttributes;
							if (' ' == quoteStyle && c == '>')
							{
								_reader.Push(c);
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

		public bool ReadToFollowing(string elementName)
		{
			if (elementName == null)
			{
				throw new ArgumentNullException("elementName");
			}
			while (Read())
			{
				if (NodeType == HtmlNodeType.Element && String.Compare(Name, elementName, true) == 0)
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
}
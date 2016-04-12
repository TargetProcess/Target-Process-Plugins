using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tp.Utils.Html
{
	public class AttributeScavenger : Sanitizer
	{
		private readonly string[] _keepElements;

		public AttributeScavenger(params string[] keepElements)
		{
			RewriteTags = new Dictionary<string, string> { { "pre", "p" }, };
			_keepElements = keepElements ?? new string[0];
		}

		public AttributeScavenger(bool requiredHtmlEncode, params string[] keepElements)
			: this(keepElements)
		{
			RequiredHtmlEncode = requiredHtmlEncode;
		}

		protected override void WriteElement(TextWriter result, string name, Dictionary<string, string> attributes, bool empty)
		{
			if (_keepElements.Any(element => String.Compare(name, element, StringComparison.OrdinalIgnoreCase) == 0))
			{
				base.WriteElement(result, name, attributes, empty);
				return;
			}

			base.WriteElement(result, name, new Dictionary<string, string>(), empty);
		}

		protected override void WriteText(TextWriter result, string value)
		{
			if (TopTag != null)
			{
				switch (TopTag.ToLowerInvariant())
				{
					case "pre":
						string text = RewritePreText(RequiredHtmlEncode ? value.HtmlEncode() : value);
						result.Write(text);
						return;
				}
			}
			base.WriteText(result, value);
		}

		protected override void HtmlEncode(string value, TextWriter writer)
		{
			if (RequiredHtmlEncode)
				base.HtmlEncode(value, writer);
			else
				writer.Write(value);
		}

		protected override void HtmlAttributeEncode(string value, TextWriter writer)
		{
			if (RequiredHtmlEncode)
				base.HtmlAttributeEncode(value, writer);
			else
				writer.Write(value);
		}

		private string RewritePreText(string preText)
		{
			return preText.Replace("\r\n", "<br />");
		}

		public bool RequiredHtmlEncode { get; set; }
	}
}

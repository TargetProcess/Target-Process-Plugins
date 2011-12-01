// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Text.RegularExpressions;

namespace Tp.PopEmailIntegration
{
	public static class Utils
	{
		//TODO: This is copy from Sanitizer. Refactor this duplication
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

			var newText = text.Replace(Environment.NewLine, "<br />");
			newText = Regex.Replace(newText, "(>\\s*<br[^>]*>\\s*<)+", "><", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			return newText;
		}

		public static string FormatComment(string text)
		{
			if (text == null)
			{
				return null;
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
				@"<br>[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}\s*?wrote(:|&#58;)\s*?<br>.*\z";

			//Outlook 2007 Remove Replies
			var newText = Regex.Replace(text, regExpOutlook2007, "",
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
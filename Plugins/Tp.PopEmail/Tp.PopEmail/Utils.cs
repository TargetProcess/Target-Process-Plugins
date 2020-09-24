// 
// Copyright (c) 2005-2018 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Text.RegularExpressions;

namespace Tp.PopEmailIntegration
{
    public static class Utils
    {
        const string RegExpOutlook2007 =
            @"<div>\s*<div\s+style=[""']border[^>]+>\s*<p[^>]*>\s*<b[^>]*>\s*<span[^>]*>\s*(.*?):</span>.*\z";
        const string RegExpOutlook2003 =
            @"<div>\s*<div\s+style=[""'][^>]+>\s*<font[^>]*>\s*<span[^>]*>\s*<hr[^>]*>\s*</span>\s*</font>\s*</div>\s*<p[^>]*>\s*<b[^>]*>\s*<font[^>]*>\s*<span[^>]*>\s*(.*?):</span>*\z";
        const string RegExpOutlookExpress =
            @"<blockquote[^>]*>\s*<div[^>]*>\s*-----\s*original\s*message\s*-----\s*</div>\s*<div[^>]*><b[^>]*>\s*(.*?):</b>*\z";
        private const string RegExpOutlookMobile =
            @"(\s*<div[^>]*>\s*<span\s+id=['""]?OutlookSignature['""]?[^>]*>\s*<div[^>]*>\s*Sent from\s*(<a[^>]*>).*</a>\s*</div>\s*</span>.*</div>).*<div\s+id=['""]?divRplyFwdMsg['""]?[^>]*>.*\z";
        const string RegExpGmail = @"<[^>]*>\s*<br[^>]*>\s*<div\s+class=['""]gmail_quote['""][^>]*>.*\z";

        const string RegExpMacBlockQuted =
            @"<div[^>]*>\s*<blockquote[^>]*>\s*<div[^>]*>\s*On[^<]*<a[^>]*>[^<]*</a>\s*wrote[^<]*</div>\s*<br[^>]*>(?<reply>.*Ticket#.*)</blockquote[^>]*></div[^>]*>";
        const string RegExpMac = @"<div[^>]*>(<blockquote[^>]*>)?\s*<div[^>]*>\s*On[^<]*<a[^>]*>[^<]*</a>\s*wrote[^<]*</div>\s*<br[^>]*>.*\z";
        const string RegExpMacPlain = @"<div[^>]*>\s*<div[^>]*>\s*On[^<]*</div>\s*<br[^>]*>\s*<blockquote[^>]*>.*\z";

        /*private const string RegExpMailWinNoAgentMessage =
            @"((<p\s+class=['""]?MsoNormal['""]?[^>]*><o:p>&nbsp;</o:p></p>\s*)?<p\s+class=['""]?MsoNormal['""]?[^>]*>Sent from\s*<a\s+href=['""][^'""]*(?:go.microsoft.com)[^'""]+['""][^>]*>Mail</a>.*(?:Windows 10)\s*</p>.*)?(<p\s+class=['""]?MsoNormal['""]?[^>]*><o:p>&nbsp;</o:p></p>\s*)?<div\s+style=\s*['""][^'""]+['""][^>]*>\s*<p\s+class=['""]?MsoNormal['""]?[^>]*><b>.*(?:From:\s+).*</b>.*\z";*/
        private const string RegExpMailWin =
            @"(\s*<p\s+class=['""]?MsoNormal['""]?[^>]*><o:p>&nbsp;</o:p></p>)\s*(<div>\s*)*<div\s+style=\s*['""][^'""]+['""][^>]*>\s*<p\s+class=['""]?MsoNormal['""]?[^>]*><b>.*(?:From:\s*).*</b>.*\z";

        const string RegUnknown1 =
            @"<div[^>]*?class=[""']OutlookMessageHeader[""'][^>]*>\s*<hr[^>]*>\s*<font[^>]*>\s*<b[^>]*>From.*\z";
        const string RegUnknown2 =
            @"<br>[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}\s*?wrote(:|&#58;)\s*?<br>.*\z";

        //TODO: This is copy from Sanitizer. Refactor this duplication
        public static string TextToHtml(string text)
        {
            if (text == null)
            {
                return null;
            }

            // text is already HTML
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

            //Outlook 2007 Remove Replies
            var newText = Regex.Replace(text, RegExpOutlook2007, "",
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

            //Outlook 2003 Remove Replies
            newText = Regex.Replace(newText, RegExpOutlook2003, "",
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

            //Outlook Express Remove Replies
            newText = Regex.Replace(newText, RegExpOutlookExpress, "",
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

            //Outlook Mobile Remove Replies
            newText = Regex.Replace(newText, RegExpOutlookMobile, "",
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

            //Gmail Remove Replies
            newText = Regex.Replace(newText, RegExpGmail, "",
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

            //Mac Remove Block Quoted Replies
            newText = Regex.Replace(newText, RegExpMacBlockQuted, "",
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

            //Mac Remove Replies
            newText = Regex.Replace(newText, RegExpMac, "",
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

            //Mac Plain Text Remove Replies
            newText = Regex.Replace(newText, RegExpMacPlain, "",
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

            //Windows 10 Mail Remove Replies
            newText = Regex.Replace(newText, RegExpMailWin, "",
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

            //Unknown1 Remove Replies
            newText = Regex.Replace(newText, RegUnknown1, "",
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

            //Unknown2 Remove Replies
            newText = Regex.Replace(newText, RegUnknown2, "",
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

            return newText;
        }
    }
}

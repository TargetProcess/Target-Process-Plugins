using System.Collections.Generic;
using System.IO;

namespace Tp.Utils.Html
{
    public class DescriptionScavenger : Sanitizer
    {
        public int MaxWordLength { get; set; }

        public DescriptionScavenger()
        {
            //Tag <pre> converts into <p>
            RewriteTags = new Dictionary<string, string> { { "pre", "p" }, };
            MaxWordLength = 100;
        }

        protected override void WriteText(TextWriter result, string value)
        {
            if (TopTag != null)
            {
                switch (TopTag.ToLowerInvariant())
                {
                    case "pre":
                        var text = RewritePreText(RequiredHtmlEncode ? value.HtmlEncode() : value);
                        result.Write(text);
                        return;
                }
            }

            base.WriteText(result, value);
        }

        //private string InsertWbrIfNecessary(string value)
        //{
        //    StringBuilder sb = new StringBuilder(value);
        //    int n = MaxWordLength;
        //    while( n < sb.Length )
        //    {								
        //        sb.Insert(n, "<wbr/>");
        //        n += 6;
        //        n += MaxWordLength;
        //    }
        //    return sb.ToString();
        //}

        private static string RewritePreText(string preText)
        {
            return preText.Replace("\r\n", "<br />");
        }

        public bool RequiredHtmlEncode { get; set; }

        public new static string Sanitize(string input)
        {
            return new DescriptionScavenger { RequiredHtmlEncode = false }.Process(input);
        }

        public static string Sanitize(string input, int maxWordLength)
        {
            return new DescriptionScavenger { MaxWordLength = maxWordLength }.Process(input);
        }
    }
}

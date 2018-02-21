// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using System.Text;
using Tp.BugTracking.BugFieldConverters;
using Tp.Integration.Common;

namespace Tp.Bugzilla.BugFieldConverters
{
    public class DescriptionConverter : IBugConverter<BugzillaBug>
    {
        public void Apply(BugzillaBug thirdPartyBug, ConvertedBug convertedBug)
        {
            var description = thirdPartyBug.description;

            if (String.IsNullOrEmpty(description)) return;

            convertedBug.BugDto.Description = FormatDescription(description);
            convertedBug.ChangedFields.Add(BugField.Description);
        }

        public static string FormatDescription(string data)
        {
            data = CleanUpContent(data);
            data = data.Replace(Environment.NewLine, "<br/>");
            data = data.Replace("\r\n", "<br/>");
            data = data.Replace("\n", "<br/>");
            data = data.Replace(" ", "&nbsp;");

            return data;
        }

        public static string CleanUpContent(string content)
        {
            if (string.IsNullOrEmpty(content))
                return content;

            var builder = new StringBuilder();
            var allowedSymbols = new[] { 10, 13 };

            foreach (var symbol in content)
            {
                if (symbol <= 31 && !allowedSymbols.Contains(symbol))
                    continue;

                builder.Append(symbol);
            }

            return builder.ToString();
        }
    }
}

// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;
using System.Linq;
using Tp.Core;

namespace Tp.Integration.Plugin.Common.Activity
{
    public static class ActivityLogFile
    {
        public const string FileDatePattern = "yyyy-MM-dd_hh-mm-ss-FFF_tt";

        public static int GetOrder(string fileName)
        {
            int result;
            var extension = fileName.GetExtension();

            Int32.TryParse(extension.Substring(1), out result);

            return result;
        }

        public static string GetDatePart(string fileName)
        {
            var dateString = fileName.GetFileNameWithoutExtension();
            dateString = dateString.Substring(dateString.IndexOf('.') + 1);

            return dateString.Contains('.')
                ? dateString.Substring(0, dateString.IndexOf('.'))
                : string.Empty;
        }

        public static DateTime GetDate(string fileName)
        {
            var dateString = GetDatePart(fileName);

            return string.IsNullOrEmpty(dateString)
                ? CurrentDate.Value
                : DateTime.ParseExact(dateString, FileDatePattern, CultureInfo.InvariantCulture);
        }

        public static string ComposeFileName(string file, DateTime date)
        {
            var fileName = file.GetFileNameWithoutExtension();
            var extension = file.GetExtension();
            var dateString = date.ToString(FileDatePattern, DateTimeFormatInfo.InvariantInfo);

            return String.Format("{0}.{1}{2}", fileName, dateString, extension);
        }

        public static string GetWildcartPatternFor(string baseFileName)
        {
            return baseFileName.GetFileNameWithoutExtension() + "*" + baseFileName.GetExtension() + "*";
        }
    }
}

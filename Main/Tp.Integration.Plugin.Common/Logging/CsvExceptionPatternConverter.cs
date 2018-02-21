//
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System.IO;
using log4net.Core;
using log4net.Layout.Pattern;

namespace Tp.Integration.Plugin.Common.Logging
{
    public class CsvExceptionPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            writer.Write(loggingEvent.GetExceptionString().CsvEncode());
        }
    }
}

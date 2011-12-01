// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using log4net.Layout;

namespace Tp.Integration.Plugin.Common.Logging
{
	public class CsvLayout : PatternLayout
	{
		public const string DATE_TIME_FORMAT = "yyyy-MM-dd hh:mm:ss,FFF tt";
		public const char DELIMITER = '\t';

		private static readonly string PATTERN = string.Format(@"""%date{{{1}}}""{0}%-5level{0}""%messagecsv""{0}""%exceptioncsv""%newline", DELIMITER, DATE_TIME_FORMAT);

		public CsvLayout() : base(PATTERN)
		{
		}

		public CsvLayout(string format)
			: this()
		{
		}

		public override void ActivateOptions()
		{
			AddConverter("messagecsv", typeof(CsvMessagePatternConverter));
			AddConverter("exceptioncsv", typeof(CsvExceptionPatternConverter));

			IgnoresException = false;

			base.ActivateOptions();
		}
	}
}
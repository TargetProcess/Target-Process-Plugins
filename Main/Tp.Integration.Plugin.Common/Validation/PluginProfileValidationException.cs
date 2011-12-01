// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;

namespace Tp.Integration.Plugin.Common.Validation
{
	public class PluginProfileValidationException : ApplicationException
	{
		private readonly PluginProfileErrorCollection _errors;

		public PluginProfileValidationException(PluginProfileErrorCollection errors)
		{
			_errors = errors;
		}

		public PluginProfileErrorCollection Errors
		{
			get { return _errors; }
		}
	}
}
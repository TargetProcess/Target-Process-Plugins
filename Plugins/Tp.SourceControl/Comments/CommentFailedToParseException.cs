// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Text;
using TinyPG;
using Tp.SourceControl.Comments.DSL;

namespace Tp.SourceControl.Comments
{
	public class CommentFailedToParseException : Exception
	{
		private readonly ParseErrors _errors;
		private readonly string _description;

		public CommentFailedToParseException(ParseErrors errors, string description)
		{
			_errors = errors;
			_description = description;
		}

		public override string Message
		{
			get { return string.Format("Failed to parse '{0}'", _description); }
		}

		public override string ToString()
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Failed to parse '{0}' with the following errors:", _description);
			stringBuilder.AppendLine();
			foreach (var parseError in _errors)
			{
				stringBuilder.AppendLine(parseError.Message);
			}
			return stringBuilder.ToString();
		}
	}
}
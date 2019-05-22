// 
// Copyright (c) 2005-2018 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Text;
using TinyPG;

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

        public override string Message => $"Failed to parse '{_description}'";

        public override string ToString()
        {
            var stringBuilder = new StringBuilder($"Failed to parse '{_description}' with the following errors:");
            stringBuilder.AppendLine();
            foreach (var parseError in _errors)
            {
                stringBuilder.AppendLine(parseError.Message);
            }
            return stringBuilder.ToString();
        }
    }
}

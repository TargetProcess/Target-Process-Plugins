// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Text.RegularExpressions;

namespace Tp.Integration.Messages.SerializationPatches
{
	public class RemoveBackingFieldPatch : IPatch
	{
		private readonly string _keyType;

		public RemoveBackingFieldPatch(string keyType)
		{
			_keyType = keyType;
		}

		private const string BACKING_FIELD_PATTERN = "&lt;(?<test>.*?)&gt;k__BackingField";

		public bool NeedToApply(string text)
		{
			return Regex.IsMatch(text, BACKING_FIELD_PATTERN) &&
			       Regex.IsMatch(_keyType, "Tp.Integration.Common.(.*?), Tp.Integration.Messages");
		}

		public string Apply(string text)
		{
			return Regex.Replace(text, BACKING_FIELD_PATTERN, "$1");
		}
	}
}
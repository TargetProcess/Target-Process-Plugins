// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Runtime.Serialization;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.MashupManager.CustomCommands.Args
{
	[DataContract, Serializable]
	public class UpdateMashupCommandArg : Mashup
	{
		public const string OldNameField = "OldName";

		[DataMember]
		public string OldName { get; set; }

		public bool IsNameChanged()
		{
			return !Name.Equals(OldName, StringComparison.InvariantCultureIgnoreCase);
		}

		public PluginProfileErrorCollection ValidateUpdate(MashupManagerProfile profile)
		{
			var errors = new PluginProfileErrorCollection();

			ValidateNameNotEmpty(errors);
			ValidateNameContainsOnlyValidChars(errors);

			return errors;
		}
	}
}
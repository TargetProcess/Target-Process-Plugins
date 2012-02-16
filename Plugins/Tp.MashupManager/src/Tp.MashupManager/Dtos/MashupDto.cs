// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.MashupManager.Dtos
{
	[DataContract, Serializable]
	public class MashupDto
	{
		public const string PlaceholdersCfgFileName = "placeholders.cfg";
		public const string AccountCfgFileName = "account.cfg";
		public const string MashupFileName = "mashup.js";

		public const string PlaceholdersField = "Placeholders";
		public const string ScriptField = "Script";
		public const string NameField = "Name";

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Placeholders { get; set; }

		[DataMember]
		public string Script { get; set; }

		#region Validation

		public PluginProfileErrorCollection ValidateAdd(MashupManagerProfile profile)
		{
			var errors = new PluginProfileErrorCollection();

			ValidateNameNotEmpty(errors);
			ValidateNameContainsOnlyValidChars(errors);
			ValidateNameUniqueness(errors, profile);

			return errors;
		}

		protected void ValidateNameNotEmpty(PluginProfileErrorCollection errors)
		{
			if (string.IsNullOrEmpty(Name))
				errors.Add(new PluginProfileError
				{
					FieldName = NameField,
					Message = "Mashup name should be specified"
				});
		}

		protected void ValidateNameContainsOnlyValidChars(PluginProfileErrorCollection errors)
		{
			if (!ProfileDtoValidator.IsValid(Name))
				errors.Add(new PluginProfileError
				{
					FieldName = NameField,
					Message = "You can only use letters, numbers, space and underscore symbol in Mashup name"
				});
		}

		protected void ValidateNameUniqueness(PluginProfileErrorCollection errors, MashupManagerProfile profile)
		{
			if (errors.Any())
				return;

			var existsSuchName = profile != null && profile.MashupNames
															.Any(
																m => m.Equals(Name, StringComparison.InvariantCultureIgnoreCase));

			if (existsSuchName)
			{
				errors.Add(new PluginProfileError
				{
					FieldName = NameField,
					Message = "Mashup with the same name already exists"
				});
			}
		}

		#endregion

		#region Creation

		public PluginMashupMessage CreatePluginMashupMessage()
		{
			return CreatePluginMashupMessage(ObjectFactory.GetInstance<IPluginContext>().AccountName);
		}

		public PluginMashupMessage CreatePluginMashupMessage(AccountName accountName)
		{
			return new PluginMashupMessage
			{
				MashupName = GetMashupName(accountName),
				Placeholders = GetPlaceholders(),
				PluginMashupScripts = GetMashupScripts(accountName),
				PluginName = string.Empty
			};
		}

		#endregion

		#region Private methods

		private string GetMashupName(AccountName accountName)
		{
			return accountName == AccountName.Empty
				? Name
				: string.Format("{0} {1}", accountName.Value, Name);
		}

		private string[] GetPlaceholders()
		{
			return Placeholders == null ? new string[]{} : Placeholders.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
		}

		private PluginMashupScript[] GetMashupScripts(AccountName accountName)
		{
			if (string.IsNullOrEmpty(Script))
			{
				return new PluginMashupScript[] {};
			}

			var scripts = new List<PluginMashupScript>
			              	{
			              		new PluginMashupScript
			              			{
			              				FileName = string.Format("{0} {1}.js", Name, DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond),
			              				ScriptContent = Script
			              			}
			              	};
			AppendAccountMashupFile(accountName, scripts);
			return scripts.ToArray();
		}

		private void AppendAccountMashupFile(AccountName accountName, List<PluginMashupScript> scripts)
		{
			if (accountName != AccountName.Empty)
			{
				scripts.Add(new PluginMashupScript
				            	{
				            		FileName = AccountCfgFileName,
									ScriptContent = string.Format("{0}{1}", MashupConfig.AccountsConfigPrefix, accountName.Value)
				            	});
			}
		}

		#endregion
	}
}
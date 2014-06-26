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

namespace Tp.MashupManager
{
	

	[DataContract, Serializable]
	public class Mashup
	{
		public const string AccountCfgFileName = "account.cfg";
		public const string NameField = "Name";

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Placeholders { get; set; }

		/// <summary>
		/// Mashup files except of config files
		/// </summary>
		[DataMember]
		public List<MashupFile> Files { get; private set; }
		
		public Mashup(List<MashupFile> files = null)
		{
			Files = files ?? new List<MashupFile>();
		}

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
			if (string.IsNullOrWhiteSpace(Name))
				errors.Add(new PluginProfileError
				{
					FieldName = NameField,
					Message = "Mashup name cannot be empty or consist of whitespace characters only"
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
			var scripts = Files.Select(f => new PluginMashupScript {FileName = f.FileName, ScriptContent = f.Content}).ToList();
			if (!scripts.Empty())
			{
				AppendAccountMashupFile(accountName, scripts);
			}
			return scripts.ToArray();
		}

		private void AppendAccountMashupFile(AccountName accountName, List<PluginMashupScript> scripts)
		{
			if (accountName == AccountName.Empty)
			{
				return;
			}
			scripts.Add(new PluginMashupScript
				{
					FileName = AccountCfgFileName,
					ScriptContent = string.Format("{0}{1}", MashupConfig.AccountsConfigPrefix, accountName.Value)
				});
		}

		#endregion
	}
}
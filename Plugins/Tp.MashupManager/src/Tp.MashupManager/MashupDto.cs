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

namespace Tp.MashupManager
{
	[DataContract, Serializable]
	public class MashupDto
	{
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

		#region Public methods

		public static MashupDto CreateEmptyMashup(string name)
		{
			return new MashupDto
			       	{
			       		Name = name,
			       		Placeholders = string.Empty,
			       		Script = string.Empty
			       	};
		}

		public PluginMashupMessage CreatePluginMashupMessage()
		{
			return new PluginMashupMessage
			       	{
			       		MashupName = GetMashupName(),
			       		Placeholders = GetPlaceholders(),
			       		PluginMashupScripts = GetMashupScripts(),
			       		PluginName = IsAccountEmpty ? string.Empty : _context.AccountName.Value
			       	};
		}

		#endregion

		#region Private methods

		private IPluginContext _context
		{
			get { return ObjectFactory.GetInstance<IPluginContext>(); }
		}

		private string GetMashupName()
		{
			return Name;

//			return IsAccountEmpty
//				? Name
//				: string.Format("{0} {1}", _context.AccountName.Value, Name);
		}

		private bool IsAccountEmpty
		{
			get { return _context.AccountName == AccountName.Empty; }
		}

		private string[] GetPlaceholders()
		{
			return Placeholders.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
		}

		private PluginMashupScript[] GetMashupScripts()
		{
			if (string.IsNullOrEmpty(Script))
			{
				return new PluginMashupScript[] {};
			}

			var scripts = new List<PluginMashupScript>
			              	{
			              		new PluginMashupScript
			              			{
			              				FileName = MashupFileName,
			              				ScriptContent = Script
			              			}
			              	};
			AppendAccountMashupFile(scripts);
			return scripts.ToArray();
		}

		private void AppendAccountMashupFile(List<PluginMashupScript> scripts)
		{
			if (!IsAccountEmpty)
			{
				scripts.Add(new PluginMashupScript
				            	{
				            		FileName = AccountCfgFileName,
				            		ScriptContent = string.Format("{0}{1}", MashupConfig.AccountsConfigPrefix, _context.AccountName.Value)
				            	});
			}
		}

		#endregion
	}

	[DataContract, Serializable]
	public class UpdateMashupDto : MashupDto
	{
		public const string OldNameField = "OldName";

		[DataMember]
		public string OldName { get; set; }

		public bool IsNameChanged()
		{
			return !Name.Equals(OldName, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
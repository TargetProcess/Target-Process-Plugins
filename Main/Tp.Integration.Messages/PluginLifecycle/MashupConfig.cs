// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Linq;

namespace Tp.Integration.Messages.PluginLifecycle
{
	public class MashupConfig
	{
		public const string PlaceholderConfigPrefix = "Placeholders:";
		public const string AccountsConfigPrefix = "Accounts:";

		public MashupConfig(string[] placeholders, AccountName[] accounts)
		{
			Placeholders = placeholders;
			Accounts = accounts;
		}

		public AccountName[] Accounts { get; private set; }
		public string[] Placeholders { get; private set; }

		public bool Matches(MashupPlaceholder mashupPlaceHolderValue, AccountName accountName)
		{
			return Placeholders.Any(x => x.Equals(mashupPlaceHolderValue.Value,StringComparison.InvariantCultureIgnoreCase)) &&
				   (Accounts.Contains(accountName.Value.ToLower()) || Accounts.IsNullOrEmpty());
		}
	}
}
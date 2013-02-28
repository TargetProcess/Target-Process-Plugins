// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tp.Integration.Messages.PluginLifecycle
{
	public class MashupConfig
	{
		public const string PlaceholderConfigPrefix = "Placeholders:";
		public const string AccountsConfigPrefix = "Accounts:";

		public MashupConfig(string[] placeholders, AccountName[] accounts)
		{
			Placeholders = new List<string>(placeholders);
			Accounts = new List<AccountName>(accounts);
		}

		public MashupConfig(IEnumerable<string> configs)
		{
			var accounts = new List<string>();
			var placeholders = new List<string>();
			foreach (var line in configs)
			{
				accounts.AddRange(ExtractValues(line, AccountsConfigPrefix));
				placeholders.AddRange(ExtractValues(line, PlaceholderConfigPrefix));
			}

			Placeholders = placeholders.Select(x => x.Trim().ToLower()).Distinct().ToList();
			Accounts = accounts.Select(x => x.Trim().ToLower()).Distinct().Select(x => new AccountName(x)).ToList();
		}

		private static IEnumerable<string> ExtractValues(string line, string matchString)
		{
			if (line.Contains(matchString))
			{
				var values = line.Replace(matchString, string.Empty);
				return string.IsNullOrEmpty(values) ? new string[] { } : values.Split(',');
			}

			return new string[] { };
		}

		public List<AccountName> Accounts { get; private set; }
		public List<string> Placeholders { get; private set; }

		public bool Matches(MashupPlaceholder mashupPlaceHolderValue, AccountName accountName)
		{
			return Placeholders.Any(x => x.Equals(mashupPlaceHolderValue.Value,StringComparison.InvariantCultureIgnoreCase)) &&
				   (Accounts.Contains(accountName.Value.ToLower()) || Accounts.IsNullOrEmpty());
		}
	}
}
// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Domain;
using Tp.PopEmailIntegration.Rules.Parsing;

namespace Tp.PopEmailIntegration.Rules
{
	public class RuleParser
	{
		private readonly IStorageRepository _storageRepository;

		public RuleParser(IStorageRepository storageRepository, IActivityLogger log)
		{
			_storageRepository = storageRepository;
			_log = log;
		}

		public IEnumerable<MailRule> Parse()
		{
			return Parse(_storageRepository.GetProfile<ProjectEmailProfile>());
		}

		public IEnumerable<MailRule> Parse(ProjectEmailProfile profile)
		{
			foreach (var ruleLine in GetRuleLines(profile))
			{
				var parser = new Parser(new Scanner());
				var tree = parser.Parse(ruleLine);
				if (tree.Errors.Count > 0)
				{
					var errors = new StringBuilder();
					tree.Errors.ForEach(x => errors.Append(string.Format("<{0}>", x.Message)));
					_log.InfoFormat("rule '{0}' can not be parsed because of the following errors: {1}", ruleLine, errors);
					continue;
				}

				_log.InfoFormat("rule '{0}' parsed successfully and prepeared to process", ruleLine);

				var thenClause = _thenClauseFactory.CreateBy(tree);
				var whenClauses = new WhenClauseFactory(thenClause);
				var whenClause = whenClauses.CreateBy(tree);
				yield return new MailRule(whenClause, thenClause);
			}
		}

		public static IEnumerable<string> GetRuleLines(IRuleHandler profile)
		{
			return
				(profile.DecodedRules ?? string.Empty).Split(Environment.NewLine.ToCharArray()).Where(x => !string.IsNullOrEmpty(x));
		}

		private readonly ThenClauseFactory _thenClauseFactory = new ThenClauseFactory();
		private readonly IActivityLogger _log;
	}
}

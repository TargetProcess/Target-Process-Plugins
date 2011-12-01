// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using Tp.PopEmailIntegration.Rules.Parsing;

namespace Tp.PopEmailIntegration.Rules
{
	internal class ClauseFactory
	{
		public static ParseNode FindRecursive(TokenType keyword, params ParseNode[] nodes)
		{
			var result = nodes.FirstOrDefault(x => x.Token.Type == keyword);
			if (result != null)
			{
				return result;
			}

			foreach (var parseNode in nodes.Where(parseNode => parseNode.Nodes.Count != 0))
			{
				result = FindRecursive(keyword, parseNode.Nodes.ToArray());
				if (result != null)
				{
					return result;
				}
			}

			return result;
		}
	}
}
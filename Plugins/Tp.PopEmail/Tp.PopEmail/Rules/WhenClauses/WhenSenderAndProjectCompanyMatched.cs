// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Domain;
using Tp.PopEmailIntegration.Data;
using Tp.PopEmailIntegration.Rules.Parsing;

namespace Tp.PopEmailIntegration.Rules.WhenClauses
{
	public class WhenSenderAndProjectCompanyMatched : IWhenClause
	{
		private readonly int _projectId;
		private readonly IStorageRepository _storage;
		private readonly UserRepository _userRepository;

		private WhenSenderAndProjectCompanyMatched(ParseNode clauseNode, IStorageRepository storage,
		                                           UserRepository userRepository)
		{
			_projectId = Int32.Parse(ClauseFactory.FindRecursive(TokenType.NUMBER, clauseNode).Token.Text);
			_storage = storage;
			_userRepository = userRepository;
		}

		public bool IsMatched(EmailMessage email)
		{
			var project =
				_storage.Get<ProjectDTO>().FirstOrDefault(x => x.ProjectID == _projectId && x.DeleteDate == null);
			if (project == null || project.CompanyID == null)
			{
				return false;
			}

			var user = _userRepository.GetByEmail(email.FromAddress).FirstOrDefault(
				x => x.UserType == UserType.Requester);
			if (user == null || user.CompanyId == null)
			{
				return false;
			}

			return project.CompanyID == user.CompanyId;
		}

		public static IWhenClause Create(ParseNode clauseNode)
		{
			return new WhenSenderAndProjectCompanyMatched(clauseNode, ObjectFactory.GetInstance<IStorageRepository>(),
			                                              ObjectFactory.GetInstance<UserRepository>());
		}
	}
}

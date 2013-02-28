// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;

namespace Tp.Mercurial.Tests.Context
{
	public class ContextExpectations
	{
		private readonly VcsPluginContext _context;

		private readonly Dictionary<Type, CreateCommandExpectations> _createCommandExpectations = new Dictionary<Type, CreateCommandExpectations>();

		public ContextExpectations(VcsPluginContext context)
		{
			_context = context;
		}

		public CreateCommandExpectations CreateCommand<TDto>()
		{
			if (!_createCommandExpectations.ContainsKey(typeof (TDto)))
			{
				_createCommandExpectations[typeof (TDto)] = new CreateCommandExpectations(typeof (TDto), _context.Transport);
			}
			return _createCommandExpectations[typeof (TDto)];
		}
	}
}
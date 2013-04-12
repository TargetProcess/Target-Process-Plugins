// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using Tp.Core;
using Tp.Integration.Common;
using hOOt;

namespace Tp.Search.Model.Entity
{
	public interface IEntityIndexer
	{
		IndexResult AddGeneralIndex(GeneralDTO general);
		IndexResult UpdateGeneralIndex(GeneralDTO general, ICollection<GeneralField> changedFields);
		IndexResult RemoveGeneralIndex(GeneralDTO general);
		IndexResult AddAssignableIndex(AssignableDTO assignable);
		IndexResult UpdateAssignableIndex(AssignableDTO assignable, ICollection<AssignableField> changedFields, bool isIndexing);
		IndexResult RemoveAssignableIndex(AssignableDTO assignable);
		IndexResult AddTestCaseIndex(TestCaseDTO testCase);
		IndexResult UpdateTestCaseIndex(TestCaseDTO testCase, ICollection<TestCaseField> changedFields, bool isIndexing);
		IndexResult RemoveTestCaseIndex(TestCaseDTO testCase);
		IndexResult AddCommentIndex(CommentDTO comment);
		IndexResult UpdateCommentIndex(CommentDTO comment, ICollection<CommentField> changedFields, Maybe<int?> projectId, Maybe<int?> squadId);
		IndexResult RemoveCommentIndex(CommentDTO comment);
		void UpdateAssignablesForProjectProcessChange(ProjectDTO project);
		void OptimizeGeneralIndex();
		void OptimizeAssignableIndex();
		void OptimizeTestCaseIndex();
		void OptimizeCommentIndex();
	}
}
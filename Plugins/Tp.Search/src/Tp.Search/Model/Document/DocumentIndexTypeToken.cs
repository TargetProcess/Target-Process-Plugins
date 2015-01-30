// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Common;
using Tp.Search.Model.Document.IndexAttribute;

namespace Tp.Search.Model.Document
{
	public enum DocumentIndexTypeToken
	{
		[IndexFileName("Entity")]
		[GeneralIndex(new[] { GeneralField.Description, GeneralField.Name })]
		[AssignableIndex(new[] { AssignableField.Description, AssignableField.Name })]
		[TestCaseIndex(new[] { TestCaseField.Description, TestCaseField.Name })]
		[ImpedimentIndex(new[] { ImpedimentField.Description, ImpedimentField.Name })]
		[GeneralDocument(new[] { GeneralField.ParentProjectID, GeneralField .EntityTypeID})]
		[AssignableDocument(new[] { AssignableField.ProjectID, AssignableField.EntityTypeID, AssignableField.SquadID })]
		[TestCaseDocument(new[] { TestCaseField.ProjectID, TestCaseField.EntityTypeID })]
		[ImpedimentDocument(new[] { ImpedimentField.ProjectID, ImpedimentField.EntityTypeID })]
		[DocumentIndexDataType(new[] { DocumentIndexDataTypeToken.Characters, DocumentIndexDataTypeToken.Digits })]
		[DocumentIndexVersion(6)]
		Entity,

		[IndexFileName("Comment")]
		[CommentIndex(new[] { CommentField.Description })]
		[DocumentIndexDataType(new[] { DocumentIndexDataTypeToken.Characters, DocumentIndexDataTypeToken.Digits })]
		[DocumentIndexVersion(5)]
		Comment,

		[IndexFileName("EntityProject")]
		[GeneralIndex(new[] { GeneralField.ParentProjectID })]
		[AssignableIndex(new[] { AssignableField.ProjectID })]
		[TestCaseIndex(new[] { TestCaseField.ProjectID })]
		[ImpedimentIndex(new[] { ImpedimentField.ProjectID })]
		[DocumentIndexDataType(new[] { DocumentIndexDataTypeToken.Characters })]
		[DocumentIndexVersion(6)]
		EntityProject,

		[IndexFileName("CommentProject")]
		[DocumentIndexDataType(new[] { DocumentIndexDataTypeToken.Characters })]
		[DocumentIndexVersion(6)]
		CommentProject,

		[IndexFileName("EntityType")]
		[GeneralIndex(new[] { GeneralField.EntityTypeID })]
		[AssignableIndex(new[] { AssignableField.EntityTypeID })]
		[TestCaseIndex(new[] { TestCaseField.EntityTypeID })]
		[ImpedimentIndex(new[] { ImpedimentField.EntityTypeID })]
		[DocumentIndexDataType(new[] { DocumentIndexDataTypeToken.Characters })]
		[DocumentIndexVersion(5)]
		EntityType,

		[IndexFileName("EntityState")]
		[AssignableIndex(new[] { AssignableField.EntityStateID })]
		[ImpedimentIndex(new[] { ImpedimentField.EntityStateID })]
		[DocumentIndexDataType(new[] { DocumentIndexDataTypeToken.Characters })]
		[DocumentIndexVersion(5)]
		EntityState,

		[IndexFileName("EntitySquad")]
		[AssignableIndex(new[] { AssignableField.SquadID })]
		[DocumentIndexDataType(new[] { DocumentIndexDataTypeToken.Characters })]
		[DocumentIndexVersion(5)]
		EntitySquad,

		[IndexFileName("CommentSquad")]
		[DocumentIndexDataType(new[] { DocumentIndexDataTypeToken.Characters })]
		[DocumentIndexVersion(5)]
		CommentSquad,

		[IndexFileName("CommentEntityType")]
		[DocumentIndexDataType(new[] { DocumentIndexDataTypeToken.Characters })]
		[DocumentIndexVersion(5)]
		CommentEntityType,

		[IndexFileName("Impediment")]
		[ImpedimentIndex(new[] { ImpedimentField.IsPrivate, ImpedimentField.OwnerID, ImpedimentField.ResponsibleID })]
		[DocumentIndexDataType(new[] { DocumentIndexDataTypeToken.Characters })]
		[DocumentIndexVersion(5)]
		Impediment,
	}
}
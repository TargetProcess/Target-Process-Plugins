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
		[ImpedimentIndex(new[] { ImpedimentField.Description, ImpedimentField.Name })]
		[GeneralDocument(new[] { GeneralField.ParentProjectID, GeneralField .EntityTypeID})]
		[AssignableDocument(new[] { AssignableField.ProjectID, AssignableField.EntityTypeID, AssignableField.SquadID })]
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
		[DocumentIndexVersion(6)]
		EntitySquad,

		[IndexFileName("CommentSquad")]
		[DocumentIndexDataType(new[] { DocumentIndexDataTypeToken.Characters })]
		[DocumentIndexVersion(6)]
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

		[IndexFileName("TestStep")]
		[TestStepIndex(new[] { TestStepField.Description, TestStepField.Result })]
		[DocumentIndexDataType(new[] { DocumentIndexDataTypeToken.Characters, DocumentIndexDataTypeToken.Digits })]
		[DocumentIndexVersion(1)]
		TestStep,

		[IndexFileName("TestStepProject")]
		[DocumentIndexDataType(new[] { DocumentIndexDataTypeToken.Characters })]
		[DocumentIndexVersion(1)]
		TestStepProject,
	}
}
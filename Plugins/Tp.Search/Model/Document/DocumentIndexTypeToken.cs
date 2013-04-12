using Tp.Integration.Common;
using Tp.Search.Model.Document.IndexAttribute;

namespace Tp.Search.Model.Document
{
	public enum DocumentIndexTypeToken
	{
		[IndexFileName("Entity")]
		[GeneralIndex(new[] { GeneralField.Description, GeneralField.Name})]
		[AssignableIndex(new[] { AssignableField.Description, AssignableField.Name })]
		[TestCaseIndex(new[] { TestCaseField.Steps, TestCaseField.Success, TestCaseField.Name })]
		[GeneralDocument(new[] { GeneralField.ParentProjectID, GeneralField .EntityTypeID})]
		[AssignableDocument(new[] { AssignableField.ProjectID, AssignableField.EntityTypeID })]
		[TestCaseDocument(new[] { TestCaseField.ProjectID, TestCaseField.EntityTypeID })]
		[DocumentIndexVersionAttribute(1)]
		Entity,

		[IndexFileName("Comment")]
		[CommentIndex(new[] { CommentField.Description })]
		[DocumentIndexVersionAttribute(1)]
		Comment,

		[IndexFileName("EntityProject")]
		[GeneralIndex(new[] { GeneralField.ParentProjectID })]
		[AssignableIndex(new[] { AssignableField.ProjectID})]
		[TestCaseIndex(new[] { TestCaseField.ProjectID })]
		[DocumentIndexVersionAttribute(1)]
		EntityProject,

		[IndexFileName("CommentProject")]
		[DocumentIndexVersionAttribute(1)]
		CommentProject,

		[IndexFileName("EntityType")]
		[GeneralIndex(new[] { GeneralField.EntityTypeID })]
		[AssignableIndex(new[] { AssignableField.EntityTypeID })]
		[TestCaseIndex(new[] { TestCaseField.EntityTypeID })]
		[DocumentIndexVersionAttribute(1)]
		EntityType,

		[IndexFileName("EntityState")]
		[AssignableIndex(new[] { AssignableField.EntityStateID})]
		[DocumentIndexVersionAttribute(1)]
		EntityState,

		[IndexFileName("EntitySquad")]
		[AssignableIndex(new[] { AssignableField.SquadID })]
		[DocumentIndexVersionAttribute(1)]
		EntitySquad,

		[IndexFileName("CommentSquad")]
		[DocumentIndexVersionAttribute(1)]
		CommentSquad,

		[IndexFileName("CommentEntityType")]
		[DocumentIndexVersionAttribute(1)]
		CommentEntityType,
	}
}
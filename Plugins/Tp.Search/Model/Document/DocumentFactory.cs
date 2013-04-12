using System;
using System.Globalization;
using Tp.Integration.Common;
using Tp.Search.Model.Entity;
using Tp.Search.Model.Utils;

namespace Tp.Search.Model.Document
{
	class DocumentFactory
	{
		private readonly IDocumentIdFactory _documentIdFactory;
		private readonly TextOperations _textOperations;
		public DocumentFactory(IDocumentIdFactory documentIdFactory, TextOperations textOperations)
		{
			_documentIdFactory = documentIdFactory;
			_textOperations = textOperations;
		}

		public EntityDocument CreateGeneral(GeneralDTO general)
		{
			string name = EntityDocument.CreateName(general.GeneralID);
			string text = _textOperations.Prepare(String.Format("{0} {1} ", general.Name, general.Description ?? String.Empty));
			return new EntityDocument(name, text)
				{
					ProjectId = _documentIdFactory.CreateProjectId(general.ParentProjectID.GetValueOrDefault()),
					EntityTypeId = _documentIdFactory.CreateEntityTypeId(general.EntityTypeID.GetValueOrDefault()),
					SquadId = _documentIdFactory.CreateSquadId(0),
					DocNumber = -1
				};
		}

		public EntityDocument CreateAssignable(AssignableDTO assignable)
		{
			var name = EntityDocument.CreateName(assignable.AssignableID);
			var text = _textOperations.Prepare(string.Format("{0} {1} ", assignable.Name, assignable.Description ?? string.Empty));
			return new EntityDocument(name, text)
			{
				ProjectId = _documentIdFactory.CreateProjectId(assignable.ProjectID.GetValueOrDefault()),
				EntityTypeId = _documentIdFactory.CreateEntityTypeId(assignable.EntityTypeID.GetValueOrDefault()),
				SquadId = _documentIdFactory.CreateSquadId(assignable.SquadID.GetValueOrDefault()),
				DocNumber = -1
			};
		}

		public hOOt.Document CreateComment(CommentDTO comment, EntityDocument entityDocument)
		{
			var name = EntityDocument.CreateName(comment.CommentID);
			var text = _textOperations.Prepare(string.Format("{0}", comment.Description ?? string.Empty));
			return new hOOt.Document(name, text)
			{
				DocNumber = -1
			};
		}

		public EntityDocument CreateTestCase(TestCaseDTO testCase)
		{
			var steps = string.IsNullOrEmpty(testCase.Steps) ? string.Empty : string.Format(" {0}", testCase.Steps);
			var success = string.IsNullOrEmpty(testCase.Success) ? string.Empty : string.Format(" {0}", testCase.Success);
			var text = _textOperations.Prepare(string.Format("{0}{1}{2} ", testCase.Name, steps, success));
			return new EntityDocument(testCase.TestCaseID.Value.ToString(CultureInfo.InvariantCulture), text)
			{
				ProjectId = _documentIdFactory.CreateProjectId(testCase.ProjectID.GetValueOrDefault()),
				EntityTypeId = _documentIdFactory.CreateEntityTypeId(testCase.EntityTypeID.GetValueOrDefault()),
				SquadId = _documentIdFactory.CreateSquadId(0),
				DocNumber = -1
			};
		}
	}
}
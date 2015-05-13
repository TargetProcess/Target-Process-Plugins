using System;
using System.Linq;
using System.Text;
using Tp.Integration.Common;
using Tp.Integration.Messages.Entities;
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
			string nameDescription = string.Format("{0} {1}", general.Name, general.Description ?? String.Empty);
			var nameDescriptionCustomFields = AppendCustomFields(nameDescription, general.CustomFieldsMetaInfo);
			string text = _textOperations.Prepare(nameDescriptionCustomFields);
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
			string nameDescription = string.Format("{0} {1}", assignable.Name, assignable.Description ?? string.Empty);
			var text = _textOperations.Prepare(AppendCustomFields(nameDescription, assignable.CustomFieldsMetaInfo));
			return new EntityDocument(name, text)
			{
				ProjectId = _documentIdFactory.CreateProjectId(assignable.ProjectID.GetValueOrDefault()),
				EntityTypeId = _documentIdFactory.CreateEntityTypeId(assignable.EntityTypeID.GetValueOrDefault()),
				SquadId = _documentIdFactory.CreateSquadId(assignable.SquadID.GetValueOrDefault()),
				DocNumber = -1
			};
		}
		
		public EntityDocument CreateImpediment(ImpedimentDTO impediment)
		{
			string name = EntityDocument.CreateName(impediment.ImpedimentID);
			string nameDescription = string.Format("{0} {1}", impediment.Name, impediment.Description ?? String.Empty);
			var nameDescriptionCustomFields = AppendCustomFields(nameDescription, impediment.CustomFieldsMetaInfo);
			string text = _textOperations.Prepare(nameDescriptionCustomFields);
			return new EntityDocument(name, text)
			{
				ProjectId = _documentIdFactory.CreateProjectId(impediment.ProjectID.GetValueOrDefault()),
				EntityTypeId = _documentIdFactory.CreateEntityTypeId(impediment.EntityTypeID.GetValueOrDefault()),
				SquadId = _documentIdFactory.CreateSquadId(0),
				DocNumber = -1
			};
		}

		public hOOt.Document CreateComment(CommentDTO comment)
		{
			var name = EntityDocument.CreateName(comment.CommentID);
			var text = _textOperations.Prepare(string.Format("{0}", comment.Description ?? string.Empty));
			return new hOOt.Document(name, text)
			{
				DocNumber = -1
			};
		}

		public hOOt.Document CreateTestStep(TestStepDTO testStep)
		{
			var name = EntityDocument.CreateName(testStep.TestStepID);
			var descriptionWithResult =
				_textOperations.Prepare(string.Format("{0} {1}", testStep.Description ?? string.Empty,
					testStep.Result ?? string.Empty));
			return new hOOt.Document(name, descriptionWithResult)
			{
				DocNumber = -1
			};
		}

		public EntityDocument CreateEmptyEntityDocument(int docNumber, string name)
		{
			return new EntityDocument(name, string.Empty)
			{
				ProjectId = _documentIdFactory.CreateProjectId(0),
				EntityTypeId = _documentIdFactory.CreateEntityTypeId(0),
				SquadId = _documentIdFactory.CreateEntityTypeId(0),
				DocNumber = docNumber
			};
		}

		public hOOt.Document CreateEmptyhOOtDocument(int docNumber, string name)
		{
			return new hOOt.Document(name, string.Empty)
			{
				DocNumber = docNumber
			};
		}

		private static string AppendCustomFields(string initialValue, Field[] customFieldsMetaInfo)
		{
			if (customFieldsMetaInfo == null) return initialValue;
			var result = new StringBuilder(initialValue);
			foreach (
				var cfValue in
					customFieldsMetaInfo
						.Where(x =>
							x.FieldType == FieldTypeEnum.Text || x.FieldType == FieldTypeEnum.RichText ||
							x.FieldType == FieldTypeEnum.DropDown || x.FieldType == FieldTypeEnum.TemplatedURL)
						.Where(x => !string.IsNullOrEmpty(x.Value)).Select(x => x.Value))
			{
				result.AppendFormat(" {0}", cfValue);
			}

			foreach (
				var cfValue in
					customFieldsMetaInfo.Where(x => x.FieldType == FieldTypeEnum.MultipleSelectionList)
						.SelectMany(x => x.ParseMultipleSelectionListFieldValue()))
			{
				result.AppendFormat(" {0}", cfValue);
			}

			return result.ToString();
		}
	}
}
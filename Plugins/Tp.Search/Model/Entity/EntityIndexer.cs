// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Messages;
using Tp.Search.Model.Document;
using Tp.Search.Model.Utils;
using hOOt;

namespace Tp.Search.Model.Entity
{
	internal class EntityIndexer : IEntityIndexer
	{
		private readonly DocumentFactory _documentFactory;
		private readonly ILocalBus _localBus;
		private readonly IProfileReadonly _profile;
		private readonly IPluginContext _pluginContext;
		private readonly IDocumentIdFactory _documentIdFactory;
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly IEntityTypeProvider _entityTypeProvider;
		private readonly IActivityLogger _log;
		private readonly TextOperations _textOperations;

		public EntityIndexer(DocumentFactory documentFactory, ILocalBus localBus, IProfileReadonly profile, IPluginContext pluginContext, IDocumentIdFactory documentIdFactory, IDocumentIndexProvider documentIndexProvider, IEntityTypeProvider entityTypeProvider, IActivityLogger log)
		{
			_documentFactory = documentFactory;
			_localBus = localBus;
			_profile = profile;
			_pluginContext = pluginContext;
			_documentIdFactory = documentIdFactory;
			_documentIndexProvider = documentIndexProvider;
			_entityTypeProvider = entityTypeProvider;
			_log = log;
			_textOperations = new TextOperations();
		}

		#region IEntityIndexer Members

		public IndexResult AddGeneralIndex(GeneralDTO general)
		{
			if (general.GeneralID == null)
			{
				return new IndexResult();
			}
			if (Exists<EntityDocument>(general.GeneralID.Value, DocumentIndexTypeToken.Entity))
			{
				if (_profile.Initialized)
				{
					_log.WarnFormat("CANNOT PROCESS '{0}CreatedMessage' FOR ENTITY #{1} - '{2}'. ENTITY HAS ALREADY BEEN ADDED ON PROFILE INITIALIZATION OR ENTITY CREATION !!!", general.EntityTypeName, general.GeneralID.GetValueOrDefault(), general.Name);
				}
				else
				{
					_log.ErrorFormat("CANNOT PROCESS '{0}CreatedMessage' FOR ENTITY #{1} - '{2}'. ENTITY HAS ALREADY BEEN ADDED !!!", general.EntityTypeName, general.GeneralID.GetValueOrDefault(), general.Name);
				}
				return new IndexResult();
			}
			IDocumentIndex entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.Entity);
			EntityDocument document = _documentFactory.CreateGeneral(general);
			IndexResult indexResult = document == null ? new IndexResult() : entityIndex.Index(document, false);
			if (indexResult.DocNumber != -1)
			{
				IDocumentIndex projectContextIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityProject);
				projectContextIndex.Index(indexResult.DocNumber, _documentIdFactory.EncodeProjectId(general.ParentProjectID));
				Maybe<string> maybeEntityTypeName = _entityTypeProvider.GetEntityTypeName(general.EntityTypeID);
				string entityTypeName = maybeEntityTypeName.FailIfNothing(() => new ApplicationException("Entity type name was not found {0}".Fmt(general.EntityTypeID)));
				IDocumentIndex entityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityType);
				entityTypeIndex.Index(indexResult.DocNumber, entityTypeName);
			}
			_log.Debug(string.Format("Added {0} #{1} - '{2}':{3}", general.EntityTypeName, general.GeneralID.GetValueOrDefault(), general.Name, indexResult.WordsAdded.Any() ? string.Format(" added words - {0};", string.Join(",", indexResult.WordsAdded.Keys)) : " NO WORDS ADDED;"));
			return indexResult;
		}

		public void OptimizeGeneralIndex()
		{
			var indexes = new[] {DocumentIndexTypeToken.Entity, DocumentIndexTypeToken.EntityProject, DocumentIndexTypeToken.EntityType}.Select(d => _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, d));
			indexes.ForEach(i => i.Optimize(freeMemory:true));
		}

		public IndexResult UpdateGeneralIndex(GeneralDTO general, ICollection<GeneralField> changedFields)
		{
			if (general.GeneralID == null)
			{
				return new IndexResult();
			}
			IEnumerable<IDocumentIndex> indexes = _documentIndexProvider.GetOrCreateDocumentIndexes(_pluginContext.AccountName, DocumentIndexTypeToken.Entity, DocumentIndexTypeToken.EntityProject, DocumentIndexTypeToken.EntityType);
			if (!indexes.Any(i => changedFields.Any(f => i.Type.IsBelongedToIndexFields(f) || i.Type.IsBelongedToDocumentFields(f))))
			{
				return new IndexResult();
			}
			IDocumentIndex entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.Entity);
			var document = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(general.GeneralID));
			if(document == null)
			{
				_log.Error(string.Format("CANNOT PROCESS UPDATE '{0}UpdatedMessage' FOR ENTITY #{1} - '{2}'. ENTITY WAS NOT ADDED DURING PROFILE INITIALIZATION OR ENTITY CREATION !!!",general.EntityTypeName, general.GeneralID.GetValueOrDefault(), general.Name));
				return new IndexResult();
			}
			if (changedFields.Any(f => entityIndex.Type.IsBelongedToDocumentFields(f)))
			{
				document.ProjectId = _documentIdFactory.CreateProjectId(general.ParentProjectID.GetValueOrDefault());
				document.EntityTypeId = _documentIdFactory.CreateEntityTypeId(general.EntityTypeID.GetValueOrDefault());
				entityIndex.SaveDocument(document, false);
				_log.Debug(string.Format("Updated {0} #{1} - '{2}':{3}{4}", general.EntityTypeName, general.GeneralID.GetValueOrDefault(), general.Name, changedFields.Contains(GeneralField.ParentProjectID) ? string.Format(" Project - {0};", string.Join(",", general.ParentProjectName)) : string.Empty, changedFields.Contains(GeneralField.EntityTypeID) ? string.Format(" EntityType - {0};", string.Join(",", general.EntityTypeName)) : string.Empty));
			}
			if (changedFields.Any(f => entityIndex.Type.IsBelongedToIndexFields(f)))
			{
				var text = _textOperations.Prepare(string.Format("{0} {1} ", general.Name, general.Description ?? string.Empty));
				var indexResult = entityIndex.Update(document.FileName, text);
				_log.Debug(string.Format("Updated {0} #{1} - '{2}':{3}{4}", general.EntityTypeName,
				                        general.GeneralID.GetValueOrDefault(), general.Name,
				                        indexResult.WordsAdded.Any() ? string.Format(" added words - {0};", string.Join(",", indexResult.WordsAdded.Keys)) : " NO WORDS ADDED;",
				                        indexResult.WordsRemoved.Any() ? string.Format(" removed words - {0};", string.Join(",", indexResult.WordsRemoved)) : " NO WORDS REMOVED;"));
			}
			IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityProject);
			if (changedFields.Any(f => entityProjectIndex.Type.IsBelongedToIndexFields(f)) && document.DocNumber >= 0)
			{
				entityProjectIndex.Update(document.DocNumber, general.ParentProjectID.HasValue ? _documentIdFactory.EncodeProjectId(general.ParentProjectID.Value) : string.Empty);
				_localBus.SendLocal(new GeneralProjectChangedLocalMessage
				{
					GeneralId = general.GeneralID.Value,
					ProjectId = general.ParentProjectID
				});
			}
			IDocumentIndex entityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityType);
			if (changedFields.Any(f => entityTypeIndex.Type.IsBelongedToIndexFields(f)) && document.DocNumber >= 0)
			{
				entityTypeIndex.Index(document.DocNumber, general.EntityTypeName);
			}
			return new IndexResult { DocNumber = document.DocNumber};
		}

		public IndexResult RemoveGeneralIndex(GeneralDTO general)
		{
			if (general.GeneralID == null)
			{
				return new IndexResult();
			}
			IDocumentIndex entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.Entity);
			var doc = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(general.GeneralID));
			if (doc == null)
			{
				_log.Error(string.Format("CANNOT PROCESS UPDATE '{0}DeletedMessage' FOR ENTITY #{1} - '{2}'. ENTITY WAS NOT ADDED DURING PROFILE INITIALIZATION OR ENTITY CREATION !!!", general.EntityTypeName, general.GeneralID.GetValueOrDefault(), general.Name));
				return new IndexResult();
			}
			var emptyDocument = CreateEmptyEntityDocument(doc.DocNumber, EntityDocument.CreateName(general.GeneralID));
			var indexResult = entityIndex.Rebuild(emptyDocument);
			_log.Debug(string.Format("Removed {0} #{1} - '{2}'", general.EntityTypeName, general.GeneralID.GetValueOrDefault(), general.Name));
			if (doc.DocNumber >= 0)
			{
				IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityProject);
				entityProjectIndex.Update(doc.DocNumber, string.Empty);
				IDocumentIndex entityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityType);
				entityTypeIndex.Update(doc.DocNumber, string.Empty);
			}
			return indexResult;
		}

		public IndexResult AddAssignableIndex(AssignableDTO assignable)
		{
			if (assignable.AssignableID == null)
			{
				return new IndexResult();
			}
			if (Exists<EntityDocument>(assignable.AssignableID.Value, DocumentIndexTypeToken.Entity))
			{
				if (_profile.Initialized)
					_log.WarnFormat("CANNOT PROCESS '{0}CreatedMessage' FOR ENTITY #{1} - '{2}'. ENTITY HAS ALREADY BEEN ADDED ON PROFILE INITIALIZATION OR ENTITY CREATION !!!", assignable.EntityTypeName, assignable.AssignableID.GetValueOrDefault(), assignable.Name);
				else
					_log.ErrorFormat("CANNOT PROCESS '{0}CreatedMessage' FOR ENTITY #{1} - '{2}'. ENTITY HAS ALREADY BEEN ADDED !!!", assignable.EntityTypeName, assignable.AssignableID.GetValueOrDefault(), assignable.Name);
				return new IndexResult();
			}
			IDocumentIndex entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.Entity);
			EntityDocument doc = _documentFactory.CreateAssignable(assignable);
			var indexResult = doc == null ? new IndexResult() : entityIndex.Index(doc, false);
			if (indexResult.DocNumber != -1)
			{
				IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityProject);
				entityProjectIndex.Index(indexResult.DocNumber, _documentIdFactory.EncodeProjectId(assignable.ProjectID));
				Maybe<string> maybeEntityTypeName = _entityTypeProvider.GetEntityTypeName(assignable.EntityTypeID);
				string entityTypeName = maybeEntityTypeName.FailIfNothing(() => new ApplicationException("Entity type name was not found {0}".Fmt(assignable.EntityTypeID)));
				IDocumentIndex entityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityType);
				entityTypeIndex.Index(indexResult.DocNumber, entityTypeName);
				if (assignable.EntityStateID != null)
				{
					IDocumentIndex entityStateIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityState);
					entityStateIndex.Index(indexResult.DocNumber, _documentIdFactory.EncodeEntityStateId(assignable.EntityStateID.Value));
				}
				IDocumentIndex squadIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntitySquad);
				squadIndex.Index(indexResult.DocNumber, _documentIdFactory.EncodeSquadId(assignable.SquadID));
			}
			_log.Debug(string.Format("Added {0} #{1} - '{2}':{3}", assignable.EntityTypeName, assignable.AssignableID.GetValueOrDefault(), assignable.Name, indexResult.WordsAdded.Any() ? string.Format(" added words - {0};", string.Join(",", indexResult.WordsAdded.Keys)) : " NO WORDS ADDED;"));
			return indexResult;
		}

		public void OptimizeAssignableIndex()
		{
			var indexes = new[] { DocumentIndexTypeToken.Entity, DocumentIndexTypeToken.EntityProject, DocumentIndexTypeToken.EntityType, DocumentIndexTypeToken.EntityState, DocumentIndexTypeToken.EntitySquad }.Select(d => _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, d));
			indexes.ForEach(i => i.Optimize(freeMemory: true));
		}

		public IndexResult UpdateAssignableIndex(AssignableDTO assignable, ICollection<AssignableField> changedFields, bool isIndexing)
		{
			var indexResult = new IndexResult();
			if (assignable.AssignableID == null)
			{
				return new IndexResult();
			}
			IEnumerable<IDocumentIndex> indexes = _documentIndexProvider.GetOrCreateDocumentIndexes(_pluginContext.AccountName, DocumentIndexTypeToken.Entity, DocumentIndexTypeToken.EntityProject, DocumentIndexTypeToken.EntityType, DocumentIndexTypeToken.EntityState, DocumentIndexTypeToken.EntitySquad);
			if (!changedFields.Any(f => indexes.Any(i => i.Type.IsBelongedToDocumentFields(f) || i.Type.IsBelongedToIndexFields(f))))
			{
				return new IndexResult();
			}
			var entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.Entity);
			var document = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(assignable.AssignableID));
			if (document == null)
			{
				_log.Error(string.Format("CANNOT PROCESS UPDATE '{0}UpdatedMessage' FOR ENTITY #{1} - '{2}'. ENTITY WAS NOT ADDED DURING PROFILE INITIALIZATION OR ENTITY CREATION !!!",
					assignable.EntityTypeName, assignable.AssignableID.GetValueOrDefault(), assignable.Name));
				return indexResult;
			}
			if (changedFields.Any(f => entityIndex.Type.IsBelongedToDocumentFields(f)))
			{
				document.ProjectId = _documentIdFactory.CreateProjectId(assignable.ProjectID.GetValueOrDefault());
				document.SquadId = _documentIdFactory.CreateSquadId(assignable.SquadID.GetValueOrDefault());
				document.EntityTypeId = _documentIdFactory.CreateEntityTypeId(assignable.EntityTypeID.GetValueOrDefault());
				entityIndex.SaveDocument(document, false);
				_log.Debug(string.Format("Updated {0} #{1} - '{2}':{3}{4}{5}", assignable.EntityTypeName,
						 assignable.AssignableID.GetValueOrDefault(), assignable.Name,
						 changedFields.Contains(AssignableField.ProjectID) ? string.Format(" Project - {0};", string.Join(",", assignable.ProjectName)) : string.Empty,
						 changedFields.Contains(AssignableField.EntityTypeID) ? string.Format(" EntityType - {0};", string.Join(",", assignable.EntityTypeName)) : string.Empty,
						 changedFields.Contains(AssignableField.EntityStateID) ? string.Format(" EntityState - {0};", string.Join(",", assignable.EntityStateName)) : string.Empty));
			}
			if (document.DocNumber >= 0)
			{
				if (changedFields.Any(f => entityIndex.Type.IsBelongedToIndexFields(f)))
				{
					var text = _textOperations.Prepare(string.Format("{0} {1} ", assignable.Name, assignable.Description ?? string.Empty));
					indexResult = entityIndex.Update(document.DocNumber, text);
					_log.Debug(string.Format("Updated {0} #{1} - '{2}':{3}{4}", assignable.EntityTypeName,
						 assignable.AssignableID.GetValueOrDefault(), assignable.Name,
						 indexResult.WordsAdded.Any() ? string.Format(" added words - {0};", string.Join(",", indexResult.WordsAdded.Keys)) : " NO WORDS ADDED;",
						 indexResult.WordsRemoved.Any() ? string.Format(" removed words - {0}; ", string.Join(",", indexResult.WordsRemoved)) : " NO WORDS REMOVED;"));
				}
				IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityProject);
				if (changedFields.Any(f => entityProjectIndex.Type.IsBelongedToIndexFields(f)))
				{
					entityProjectIndex.Update(document.DocNumber, _documentIdFactory.EncodeProjectId(assignable.ProjectID));
					if (!isIndexing)
					{
						_localBus.SendLocal(new GeneralProjectChangedLocalMessage { GeneralId = assignable.AssignableID.Value, ProjectId = assignable.ProjectID });
					}
				}
				if (assignable.EntityTypeID != null)
				{
					IDocumentIndex entityTypeIndices = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityType);
					if (changedFields.Any(f => entityTypeIndices.Type.IsBelongedToIndexFields(f)))
					{
						entityTypeIndices.Update(document.DocNumber, assignable.EntityTypeName);
					}
				}
				if (assignable.EntityStateID != null)
				{
					IDocumentIndex entityStateIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityState);
					if (changedFields.Any(f => entityStateIndex.Type.IsBelongedToIndexFields(f)))
					{
						entityStateIndex.Update(document.DocNumber, _documentIdFactory.EncodeEntityStateId(assignable.EntityStateID.Value));
					}
				}
				IDocumentIndex entitySquadIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntitySquad);
				if (changedFields.Any(f => entitySquadIndex.Type.IsBelongedToIndexFields(f)))
				{
					entitySquadIndex.Update(document.DocNumber, _documentIdFactory.EncodeSquadId(assignable.SquadID));
					if (!isIndexing)
					{
						_localBus.SendLocal(new AssignableSquadChangedLocalMessage { AssignableId = assignable.AssignableID.Value, SquadId = assignable.SquadID });
					}
				}
			}
			return indexResult;
		}

		public IndexResult RemoveAssignableIndex(AssignableDTO assignable)
		{
			if (assignable.AssignableID == null)
			{
				return new IndexResult();
			}
			var entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.Entity);
			var doc = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(assignable.AssignableID));
			if (doc == null)
			{
				_log.Error(string.Format("CANNOT PROCESS UPDATE '{0}DeletedMessage' FOR ENTITY #{1} - '{2}'. ENTITY WAS NOT ADDED DURING PROFILE INITIALIZATION OR ENTITY CREATION !!!", assignable.EntityTypeName, assignable.AssignableID.GetValueOrDefault(), assignable.Name));
				return new IndexResult();
			}
			var indexResult = entityIndex.Rebuild(CreateEmptyEntityDocument(doc.DocNumber, EntityDocument.CreateName(assignable.AssignableID)));
			_log.Debug(string.Format("Removed {0} #{1} - '{2}'", assignable.EntityTypeName, assignable.AssignableID.GetValueOrDefault(), assignable.Name));
			if (doc.DocNumber >= 0)
			{
				IDocumentIndex projectContextIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityProject);
				projectContextIndex.Update(doc.DocNumber, string.Empty);
				IDocumentIndex entityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityType);
				entityTypeIndex.Update(doc.DocNumber, string.Empty);
				IDocumentIndex entityStateIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityState);
				entityStateIndex.Update(doc.DocNumber, string.Empty);
				IDocumentIndex entitySquadIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntitySquad);
				entitySquadIndex.Update(doc.DocNumber, string.Empty);
			}
			return indexResult;
		}

		public IndexResult AddTestCaseIndex(TestCaseDTO testCase)
		{
			if (testCase.TestCaseID == null)
			{
				return new IndexResult();
			}
			if (Exists<EntityDocument>(testCase.TestCaseID.Value, DocumentIndexTypeToken.Entity))
			{
				if (_profile.Initialized)
					_log.WarnFormat("CANNOT PROCESS '{0}CreatedMessage' FOR ENTITY #{1} - '{2}'. ENTITY HAS ALREADY BEEN ADDED ON PROFILE INITIALIZATION OR ENTITY CREATION !!!",
					testCase.EntityTypeName, testCase.TestCaseID.GetValueOrDefault(), testCase.Name);
				else
					_log.ErrorFormat("CANNOT PROCESS '{0}CreatedMessage' FOR ENTITY #{1} - '{2}'. ENTITY HAS ALREADY BEEN ADDED !!!",
					testCase.EntityTypeName, testCase.TestCaseID.GetValueOrDefault(), testCase.Name);
				return new IndexResult();
			}
			var entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.Entity);
			var doc = _documentFactory.CreateTestCase(testCase);
			var indexResult = doc == null ? new IndexResult() : entityIndex.Index(doc, false);
			if (indexResult.DocNumber != -1)
			{
				IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityProject);
				entityProjectIndex.Index(indexResult.DocNumber, _documentIdFactory.EncodeProjectId(testCase.ProjectID));
				if (testCase.EntityTypeID != null)
				{
					Maybe<string> maybeEntityTypeName = _entityTypeProvider.GetEntityTypeName(testCase.EntityTypeID);
					string entityTypeName = maybeEntityTypeName.FailIfNothing(() => new ApplicationException("Entity type name was not found {0}".Fmt(testCase.EntityTypeID)));
					IDocumentIndex entityTypeIndices = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityType);
					entityTypeIndices.Index(indexResult.DocNumber, entityTypeName);
				}
			}
			_log.Debug(string.Format("Added {0} #{1} - '{2}':{3}", testCase.EntityTypeName, testCase.TestCaseID.GetValueOrDefault(), testCase.Name, indexResult.WordsAdded.Any() ? string.Format(" added words - {0};", string.Join(",", indexResult.WordsAdded.Keys)) : " NO WORDS ADDED;"));
			return indexResult;
		}

		public void OptimizeTestCaseIndex()
		{
			var indexes = new[] { DocumentIndexTypeToken.Entity, DocumentIndexTypeToken.EntityProject, DocumentIndexTypeToken.EntityType}.Select(d => _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, d));
			indexes.ForEach(i => i.Optimize(freeMemory: true));
		}

		public IndexResult UpdateTestCaseIndex(TestCaseDTO testCase, ICollection<TestCaseField> changedFields, bool isIndexing)
		{
			if (testCase.TestCaseID == null)
			{
				return new IndexResult();
			}
			var indexes = _documentIndexProvider.GetOrCreateDocumentIndexes(_pluginContext.AccountName, DocumentIndexTypeToken.Entity, DocumentIndexTypeToken.EntityProject, DocumentIndexTypeToken.EntityType);
			if (!changedFields.Any(f => indexes.Any(i => i.Type.IsBelongedToDocumentFields(f) || i.Type.IsBelongedToIndexFields(f))))
			{
				return new IndexResult();
			}
			var entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.Entity);
			var doc = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(testCase.TestCaseID));
			if (doc == null)
			{
				_log.Error(string.Format("CANNOT PROCESS UPDATE '{0}UpdatedMessage' FOR ENTITY #{1} - '{2}'. ENTITY WAS NOT ADDED DURING PROFILE INITIALIZATION OR ENTITY CREATION !!!", testCase.EntityTypeName, testCase.TestCaseID.GetValueOrDefault(), testCase.Name));
				return new IndexResult();
			}
			if (changedFields.Any(f => entityIndex.Type.IsBelongedToDocumentFields(f)))
			{
				doc.ProjectId = _documentIdFactory.CreateProjectId(testCase.ProjectID.GetValueOrDefault());
				doc.EntityTypeId = _documentIdFactory.CreateEntityTypeId(testCase.EntityTypeID.GetValueOrDefault());
				entityIndex.SaveDocument(doc, false);
				_log.Debug(string.Format("Updated {0} #{1} - '{2}':{3}{4}", testCase.EntityTypeName,
						 testCase.TestCaseID.GetValueOrDefault(), testCase.Name,
						 changedFields.Contains(TestCaseField.ProjectID) ? string.Format(" Project - {0};", string.Join(",", testCase.ProjectName)) : string.Empty,
						 changedFields.Contains(TestCaseField.EntityTypeID) ? string.Format(" EntityType - {0};", string.Join(",", testCase.EntityTypeName)) : string.Empty));
			}
			var indexResult = new IndexResult();
			if (doc.DocNumber >= 0)
			{
				if (changedFields.Any(f => entityIndex.Type.IsBelongedToIndexFields(f)))
				{
					var newText = _textOperations.Prepare(string.Format("{0}{1}{2} ", testCase.Name, string.IsNullOrEmpty(testCase.Steps) ? string.Empty : string.Format(" {0}", testCase.Steps), string.IsNullOrEmpty(testCase.Success) ? string.Empty : string.Format(" {0}", testCase.Success)));
					indexResult = entityIndex.Update(doc.DocNumber, newText);
					_log.Debug(string.Format("Updated {0} #{1} - '{2}':{3}{4}", testCase.EntityTypeName, testCase.TestCaseID.GetValueOrDefault(), testCase.Name, indexResult.WordsAdded.Any() ? string.Format(" added words - {0};", string.Join(",", indexResult.WordsAdded.Keys)) : " NO WORDS ADDED;", indexResult.WordsRemoved.Any() ? string.Format(" removed words - {0};", string.Join(",", indexResult.WordsRemoved)) : " NO WORDS REMOVED;"));
				}
				IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityProject);
				if (changedFields.Any(f => entityProjectIndex.Type.IsBelongedToIndexFields(f)))
				{
					entityProjectIndex.Update(doc.DocNumber, _documentIdFactory.EncodeProjectId(testCase.ProjectID));
					if (!isIndexing)
					{
						_localBus.SendLocal(new GeneralProjectChangedLocalMessage
							{
								GeneralId = testCase.TestCaseID.Value,
								ProjectId = testCase.ProjectID
							});
					}
				}
				IDocumentIndex entityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityType);
				if (changedFields.Any(f => entityTypeIndex.Type.IsBelongedToIndexFields(f)))
				{
					entityTypeIndex.Update(doc.DocNumber, testCase.EntityTypeName);
				}
			}
			return indexResult;
		}

		public IndexResult RemoveTestCaseIndex(TestCaseDTO testCase)
		{
			if (testCase.TestCaseID == null)
			{
				return new IndexResult();
			}
			var entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.Entity);
			var doc = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(testCase.TestCaseID));
			if (doc == null)
			{
				_log.Error(string.Format("CANNOT PROCESS UPDATE '{0}DeletedMessage' FOR ENTITY #{1} - '{2}'. ENTITY WAS NOT ADDED DURING PROFILE INITIALIZATION OR ENTITY CREATION !!!", testCase.EntityTypeName, testCase.TestCaseID.GetValueOrDefault(), testCase.Name));
				return new IndexResult();
			}
			var indexResult = entityIndex.Rebuild(CreateEmptyEntityDocument(doc.DocNumber, EntityDocument.CreateName(testCase.TestCaseID)));
			_log.Debug(string.Format("Removed {0} #{1} - '{2}'", testCase.EntityTypeName, testCase.TestCaseID.GetValueOrDefault(), testCase.Name));
			if (doc.DocNumber >= 0)
			{
				IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityProject);
				entityProjectIndex.Update(doc.DocNumber, string.Empty);
				IDocumentIndex entityTypeIndices = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.EntityType);
				entityTypeIndices.Update(doc.DocNumber, string.Empty);
			}
			return indexResult;
		}

		public IndexResult AddCommentIndex(CommentDTO comment)
		{
			var indexResult = new IndexResult();
			if (comment.CommentID == null || comment.GeneralID == null)
			{
				return new IndexResult();
			}
			if (Exists<hOOt.Document>(comment.CommentID.Value, DocumentIndexTypeToken.Comment))
			{
				if (_profile.Initialized)
					_log.WarnFormat("CANNOT PROCESS 'CommentCreatedMessage' FOR COMMENT#{0} FOR ENTITY #{1} - '{2}'. COMMENT HAS ALREADY BEEN ADDED ON PROFILE INITIALIZATION OR ENTITY CREATION !!!",
					comment.CommentID.GetValueOrDefault(), comment.GeneralID.GetValueOrDefault(), comment.GeneralName);
				else
					_log.ErrorFormat("CANNOT PROCESS 'CommentCreatedMessage' FOR COMMENT#{0} FOR ENTITY #{1} - '{2}'. COMMENT HAS ALREADY BEEN ADDED !!!",
					comment.CommentID.GetValueOrDefault(), comment.GeneralID.GetValueOrDefault(), comment.GeneralName);
				return indexResult;
			}
			var entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.Entity);
			var commentEntity = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(comment.GeneralID));
			if (commentEntity == null)
			{
				_log.Error(string.Format("CANNOT PROCESS 'CommentCreatedMessage' FOR COMMENT#{0} FOR ENTITY #{1} - '{2}'. ENTITY WAS NOT ADDED DURING PROFILE INITIALIZATION OR ENTITY CREATION !!!",
					comment.CommentID.GetValueOrDefault(), comment.GeneralID.GetValueOrDefault(), comment.GeneralName));
				return indexResult;
			}
			var commentIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.Comment);
			var doc = _documentFactory.CreateComment(comment, commentEntity);
			if (doc == null)
			{
				return indexResult;
			}
			indexResult = commentIndex.Index(doc, false);
			_log.Debug(string.Format("Added comment #{0} to #{1} - '{2}':{3}", comment.CommentID.GetValueOrDefault(), comment.GeneralID.GetValueOrDefault(), comment.GeneralName, indexResult.WordsAdded.Any() ? string.Format(" added words - {0};", string.Join(",", indexResult.WordsAdded.Keys)) : " NO WORDS ADDED;"));
			if (indexResult.DocNumber != -1)
			{
				int? projectId = null;
				if (commentEntity.ProjectId != null)
				{
					int projectIdValue;
					if (int.TryParse(commentEntity.ProjectId, out projectIdValue))
					{
						projectId = projectIdValue == 0 ? null : (int?)projectIdValue;
					}
				}
				IDocumentIndex commentProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.CommentProject);
				commentProjectIndex.Index(indexResult.DocNumber, _documentIdFactory.EncodeProjectId(projectId));
				int? squadId = null;
				if (commentEntity.SquadId != null)
				{
					int squadIdValue;
					if (int.TryParse(commentEntity.SquadId, out squadIdValue))
					{
						squadId = squadIdValue == 0 ? null : (int?)squadIdValue;
					}
				}
				IDocumentIndex commentSquadIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.CommentSquad);
				commentSquadIndex.Index(indexResult.DocNumber, _documentIdFactory.EncodeSquadId(squadId));
				IDocumentIndex commentEntityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.CommentEntityType);
				Maybe<string> maybeEntityTypeName = _entityTypeProvider.GetEntityTypeName(int.Parse(commentEntity.EntityTypeId));
				string entityTypeName = maybeEntityTypeName.FailIfNothing(() => new ApplicationException("No entgity type name for {0}".Fmt(commentEntity.EntityTypeId)));
				commentEntityTypeIndex.Index(indexResult.DocNumber, entityTypeName);
			}
			return indexResult;
		}

		public void OptimizeCommentIndex()
		{
			var indexes = new[] { DocumentIndexTypeToken.Comment, DocumentIndexTypeToken.CommentProject, DocumentIndexTypeToken.CommentSquad, DocumentIndexTypeToken.CommentEntityType, }.Select(d => _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, d));
			indexes.ForEach(i => i.Optimize(freeMemory: true));
		}

		public IndexResult UpdateCommentIndex(CommentDTO comment, ICollection<CommentField> changedFields, Maybe<int?> projectId, Maybe<int?> squadId)
		{
			if (comment.CommentID == null || comment.GeneralID == null)
			{
				return new IndexResult();
			}
			var entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.Entity);
			var commentEntityDocument = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(comment.GeneralID));
			if (commentEntityDocument == null)
			{
				_log.Error(string.Format("CANNOT PROCESS 'CommentUpdatedMessage' FOR COMMENT#{0} FOR ENTITY #{1} - '{2}'. ENTITY WAS NOT ADDED DURING PROFILE INITIALIZATION OR ENTITY CREATION !!!", comment.CommentID.GetValueOrDefault(), comment.GeneralID.GetValueOrDefault(), comment.GeneralName));
				return new IndexResult();
			}
			var commentIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.Comment);
			var commentDocument = commentIndex.FindDocumentByName<hOOt.Document>(EntityDocument.CreateName(comment.CommentID));
			if (commentDocument == null)
			{
				_log.Error(string.Format("CANNOT PROCESS 'CommentUpdatedMessage' FOR COMMENT#{0} FOR ENTITY #{1} - '{2}'. COMMENT WAS NOT ADDED DURING PROFILE INITIALIZATION OR ENTITY CREATION !!!", comment.CommentID.GetValueOrDefault(), comment.GeneralID.GetValueOrDefault(), comment.GeneralName));
				return new IndexResult();
			}
			if (commentDocument.DocNumber >= 0)
			{
				if (projectId.HasValue)
				{
					IDocumentIndex commentProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.CommentProject);
					int? projectIdValue = projectId.Value;
					commentProjectIndex.Update(commentDocument.DocNumber, _documentIdFactory.EncodeProjectId(projectIdValue));
				}
				if (squadId.HasValue)
				{
					IDocumentIndex commentSquadIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.CommentSquad);
					int? squadIdValue = squadId.Value;
					commentSquadIndex.Update(commentDocument.DocNumber, _documentIdFactory.EncodeSquadId(squadIdValue));
				}
				if (changedFields.Any(f => commentIndex.Type.IsBelongedToIndexFields(f)))
				{
					var text = _textOperations.Prepare(string.Format("{0}", comment.Description ?? string.Empty));
					var indexResult = commentIndex.Update(commentDocument.FileName, text);
					_log.Debug(string.Format("Updated comment #{0} for #{1} - '{2}':{3}{4}", comment.CommentID.GetValueOrDefault(), comment.GeneralID.GetValueOrDefault(), comment.GeneralName, indexResult.WordsAdded.Any() ? string.Format(" added words - {0};", string.Join(",", indexResult.WordsAdded.Keys)) : " NO WORDS ADDED;", indexResult.WordsRemoved.Any() ? string.Format(" removed words - {0};", string.Join(",", indexResult.WordsRemoved)) : " NO WORDS REMOVED;"));
					return indexResult;
				}
			}
			return new IndexResult();
		}

		public IndexResult RemoveCommentIndex(CommentDTO comment)
		{
			if (comment.CommentID == null || comment.GeneralID == null)
			{
				return new IndexResult();
			}
			var entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.Entity);
			var commentEntity = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(comment.GeneralID));
			if (commentEntity == null)
			{
				_log.Error(string.Format("CANNOT PROCESS 'CommentRemovedMessage' FOR COMMENT#{0} FOR ENTITY #{1} - '{2}'. ENTITY WAS NOT ADDED DURING PROFILE INITIALIZATION OR ENTITY CREATION !!!", comment.CommentID.GetValueOrDefault(), comment.GeneralID.GetValueOrDefault(), comment.GeneralName));
				return new IndexResult();
			}
			var commentIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.Comment);
			var indexResult = commentIndex.Rebuild(CreateEmptyCommentDocument(comment.CommentID.Value), true);
			_log.Debug(string.Format("Removed comment #{0} from #{1} - '{2}'", comment.CommentID.GetValueOrDefault(), comment.GeneralID.GetValueOrDefault(), comment.GeneralName));
			if (indexResult.DocNumber >= 0)
			{
				IDocumentIndex commentProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.CommentProject);
				commentProjectIndex.Update(indexResult.DocNumber, string.Empty);
				IDocumentIndex commentSquadIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.CommentSquad);
				commentSquadIndex.Update(indexResult.DocNumber, string.Empty);
				IDocumentIndex commentEntityType = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.CommentEntityType);
				commentEntityType.Update(indexResult.DocNumber, string.Empty);
			}
			return indexResult;
		}

		public void UpdateAssignablesForProjectProcessChange(ProjectDTO project)
		{
			_localBus.SendLocal(new ProjectProcessChangedLocalMessage { ProjectId = project.ProjectID.GetValueOrDefault() });
		}

		#endregion

		private bool Exists<TDocument>(int entityId, DocumentIndexTypeToken indexType) where TDocument : hOOt.Document, new()
		{
			IDocumentIndex entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, indexType);
			return entityIndex.FindDocumentByName<TDocument>(EntityDocument.CreateName(entityId)) != null;
		}

		private EntityDocument CreateEmptyEntityDocument(int docNumber, string fileName)
		{
			return new EntityDocument(fileName, string.Empty)
			{
				ProjectId = _documentIdFactory.CreateProjectId(0),
				EntityTypeId = _documentIdFactory.CreateEntityTypeId(0),
				SquadId = _documentIdFactory.CreateEntityTypeId(0),
				DocNumber = docNumber
			};
		}

		private hOOt.Document CreateEmptyCommentDocument(int docNumber)
		{
			return new hOOt.Document(EntityDocument.CreateName(docNumber), string.Empty)
			{
				DocNumber = docNumber
			};
		}
	}
}
// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
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
using hOOt;

namespace Tp.Search.Model.Entity
{
	internal class EntityIndexer : IEntityIndexer
	{
		private readonly DocumentFactory _documentFactory;
		private readonly ILocalBus _localBus;
		private readonly IPluginContext _pluginContext;
		private readonly IDocumentIdFactory _documentIdFactory;
		private readonly IIndexDataFactory _indexDataFactory;
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly IEntityTypeProvider _entityTypeProvider;
		private readonly IActivityLogger _log;
		private readonly LogHelper _logHelper;

		public EntityIndexer(DocumentFactory documentFactory, ILocalBus localBus, IProfileReadonly profile, IPluginContext pluginContext, IDocumentIdFactory documentIdFactory, IIndexDataFactory indexDataFactory, IDocumentIndexProvider documentIndexProvider, IEntityTypeProvider entityTypeProvider, IActivityLogger log)
		{
			_documentFactory = documentFactory;
			_localBus = localBus;
			_pluginContext = pluginContext;
			_documentIdFactory = documentIdFactory;
			_indexDataFactory = indexDataFactory;
			_documentIndexProvider = documentIndexProvider;
			_entityTypeProvider = entityTypeProvider;
			_log = log;
			_logHelper = new LogHelper(log, profile);
		}

		#region IEntityIndexer Members
		
		public IndexResult AddGeneralIndex(GeneralDTO general, DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetNoOptimizeIfNull(optimizeSetup);
			if (general.GeneralID == null)
			{
				return new IndexResult();
			}
			string entityTypeName = GetEntityTypeName(general.EntityTypeID);
			if (Exists<EntityDocument>(general.GeneralID.Value, DocumentIndexTypeToken.Entity))
			{
				_logHelper.LogCreatedMessage(entityTypeName, general.GeneralID, general.Name);
				return new IndexResult();
			}
			IDocumentIndex entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			EntityDocument document = _documentFactory.CreateGeneral(general);
			IndexResult indexResult = document == null ? new IndexResult() : entityIndex.Index(document, false, optimizeSetup);
			if (indexResult.DocNumber != -1)
			{
				IDocumentIndex projectContextIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityProject);
				projectContextIndex.Index(indexResult.DocNumber, _indexDataFactory.CreateProjectData(general.ParentProjectID).ToString(), optimizeSetup);
				IDocumentIndex entityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityType);
				entityTypeIndex.Index(indexResult.DocNumber, entityTypeName, optimizeSetup);
			}
			_log.Debug(string.Format("Added {0} #{1} - '{2}':{3}", entityTypeName, general.GeneralID.GetValueOrDefault(), general.Name, indexResult.WordsAdded.Any() ? string.Format(" added words - {0};", string.Join(",", indexResult.WordsAdded.Keys)) : " NO WORDS ADDED;"));
			return indexResult;
		}

		
		private string GetEntityTypeName(int? entityTypeId)
		{
			Maybe<string> maybeEntityTypeName = _entityTypeProvider.GetEntityTypeName(entityTypeId);
			string entityTypeName = maybeEntityTypeName.GetOrThrow(() => new ApplicationException("Entity type name was not found {0}".Fmt(entityTypeId)));
			return entityTypeName;
		}

		public IndexResult AddReleaseProjectIndex(ReleaseProjectDTO releaseProject, DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetNoOptimizeIfNull(optimizeSetup);
			if (releaseProject.ReleaseProjectID == null)
			{
				return new IndexResult();
			}
			IDocumentIndex entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			EntityDocument document = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(releaseProject.ReleaseID));
			if (document == null)
			{
				_log.Debug("Cannot index releaseproject {0} for release {1} and project {2}".Fmt(releaseProject.ID, releaseProject.ReleaseName, releaseProject.ProjectName));
				return new IndexResult();
			}
			if (document.DocNumber != -1)
			{
				IDocumentIndex projectContextIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityProject);
				var indexData = projectContextIndex.GetExistingIndexByNumber(document.DocNumber);
				var currentProjectData = ProjectIndexData.Parse(indexData);
				var result = ProjectIndexData.Sum(currentProjectData, new ProjectIndexData(new[] {releaseProject.ProjectID}));
				projectContextIndex.Update(document.DocNumber, result.ToString(), optimizeSetup);
				return new IndexResult
				{
					DocNumber = document.DocNumber
				};
			}
			return new IndexResult();
		}

		public IndexResult RemoveReleaseProjectIndex(ReleaseProjectDTO releaseProject, DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetNoOptimizeIfNull(optimizeSetup);
			if (releaseProject.ReleaseProjectID == null)
			{
				return new IndexResult();
			}
			IDocumentIndex entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			var document = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(releaseProject.ReleaseID));
			if (document == null)
			{
				_log.Debug("ReleaseProject {0} for release {1} and project {2} has been already deleted".Fmt(releaseProject.ID, releaseProject.ReleaseName, releaseProject.ProjectName));
				return new IndexResult();
			}
			if (document.DocNumber != 0)
			{
				IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityProject);
				var indexData = entityProjectIndex.GetExistingIndexByNumber(document.DocNumber);
				var currentProjectData = ProjectIndexData.Parse(indexData);
				var result = ProjectIndexData.Substract(currentProjectData, new ProjectIndexData(new[] {releaseProject.ProjectID}));
				entityProjectIndex.Update(document.DocNumber, result.ToString(), optimizeSetup);
			}
			return new IndexResult();
		}

		public void OptimizeGeneralIndex(DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetImmediateOptimizeIfNull(optimizeSetup);
			var indexes = new[] {DocumentIndexTypeToken.Entity, DocumentIndexTypeToken.EntityProject, DocumentIndexTypeToken.EntityType}.Select(d => _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, d));
			indexes.ForEach(i => i.Optimize(optimizeSetup));
		}

		public IndexResult UpdateGeneralIndex(GeneralDTO general, ICollection<GeneralField> changedFields, DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetNoOptimizeIfNull(optimizeSetup);
			if (general.GeneralID == null)
			{
				return new IndexResult();
			}
			IEnumerable<IDocumentIndex> indexes = _documentIndexProvider.GetOrCreateDocumentIndexes(_pluginContext, DocumentIndexTypeToken.Entity, DocumentIndexTypeToken.EntityProject, DocumentIndexTypeToken.EntityType);
			if (!indexes.Any(i => changedFields.Any(f => i.Type.IsBelongedToIndexFields(f) || i.Type.IsBelongedToDocumentFields(f))))
			{
				return new IndexResult();
			}
			IDocumentIndex entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			var document = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(general.GeneralID));
			string entityTypeName = GetEntityTypeName(general.EntityTypeID);
			if(document == null)
			{
				_logHelper.LogUpdateMessage(entityTypeName, general.GeneralID, general.Name);
				return new IndexResult();
			}
			int oldProjectId = _documentIdFactory.ParseProjectId(document.ProjectId);
			if (changedFields.Any(f => entityIndex.Type.IsBelongedToDocumentFields(f)))
			{
				document.ProjectId = _documentIdFactory.CreateProjectId(general.ParentProjectID.GetValueOrDefault());
				document.EntityTypeId = _documentIdFactory.CreateEntityTypeId(general.EntityTypeID.GetValueOrDefault());
				entityIndex.SaveDocument(document, false);
				_log.Debug(string.Format("Updated {0} #{1} - '{2}':{3}{4}", entityTypeName, general.GeneralID.GetValueOrDefault(), general.Name, changedFields.Contains(GeneralField.ParentProjectID) ? string.Format(" Project - {0};", string.Join(",", general.ParentProjectName)) : string.Empty, changedFields.Contains(GeneralField.EntityTypeID) ? string.Format(" EntityType - {0};", string.Join(",", entityTypeName)) : string.Empty));
			}
			if (changedFields.Any(f => entityIndex.Type.IsBelongedToIndexFields(f)))
			{
				var text = _documentFactory.CreateGeneral(general).Text;
				var indexResult = entityIndex.Update(document.FileName, text, optimizeSetup);
				_log.Debug(string.Format("Updated {0} #{1} - '{2}':{3}{4}", entityTypeName,
				                        general.GeneralID.GetValueOrDefault(), general.Name,
				                        indexResult.WordsAdded.Any() ? string.Format(" added words - {0};", string.Join(",", indexResult.WordsAdded.Keys)) : " no words added;",
				                        indexResult.WordsRemoved.Any() ? string.Format(" removed words - {0};", string.Join(",", indexResult.WordsRemoved)) : " no words removed;"));
			}
			IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityProject);
			if (changedFields.Any(f => entityProjectIndex.Type.IsBelongedToIndexFields(f)) && document.DocNumber >= 0)
			{
				var projectIndexDataToAdd = _indexDataFactory.CreateProjectData(general.ParentProjectID.GetValueOrDefault());
				var projectIndexDataToRemove = _indexDataFactory.CreateProjectData(oldProjectId);
				var indexData = entityProjectIndex.GetExistingIndexByNumber(document.DocNumber);
				var currentProjectIndexData = ProjectIndexData.Parse(indexData);
				var result = ProjectIndexData.Substract(currentProjectIndexData, projectIndexDataToRemove);
				var finalResult = ProjectIndexData.Sum(result, projectIndexDataToAdd);
				entityProjectIndex.Update(document.DocNumber, finalResult.ToString(), optimizeSetup);
				_localBus.SendLocal(new GeneralProjectChangedLocalMessage
				{
					GeneralId = general.GeneralID.Value,
					ProjectId = general.ParentProjectID
				});
			}
			IDocumentIndex entityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityType);
			if (changedFields.Any(f => entityTypeIndex.Type.IsBelongedToIndexFields(f)) && document.DocNumber >= 0)
			{
				entityTypeIndex.Index(document.DocNumber, entityTypeName, optimizeSetup);
			}
			return new IndexResult { DocNumber = document.DocNumber};
		}

		public IndexResult RemoveGeneralIndex(GeneralDTO general, DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetNoOptimizeIfNull(optimizeSetup);
			if (general.GeneralID == null)
			{
				return new IndexResult();
			}
			IDocumentIndex entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			var document = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(general.GeneralID));
			string entityTypeName = GetEntityTypeName(general.EntityTypeID);
			if (document == null)
			{
				_logHelper.LogDeletedMessage(entityTypeName, general.GeneralID, general.Name);
				return new IndexResult();
			}
			var emptyDocument = _documentFactory.CreateEmptyEntityDocument(document.DocNumber, EntityDocument.CreateName(general.GeneralID));
			var indexResult = entityIndex.Rebuild(emptyDocument, false, optimizeSetup);
			_log.Debug(string.Format("Removed {0} #{1} - '{2}'", entityTypeName, general.GeneralID.GetValueOrDefault(), general.Name));
			if (document.DocNumber >= 0)
			{
				IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityProject);
				entityProjectIndex.Update(document.DocNumber, string.Empty, optimizeSetup);
				IDocumentIndex entityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityType);
				entityTypeIndex.Update(document.DocNumber, string.Empty, optimizeSetup);
			}
			return indexResult;
		}

		public IndexResult AddAssignableIndex(AssignableDTO assignable, DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetNoOptimizeIfNull(optimizeSetup);
			if (assignable.AssignableID == null)
			{
				return new IndexResult();
			}
			string entityTypeName = GetEntityTypeName(assignable.EntityTypeID);
			if (Exists<EntityDocument>(assignable.AssignableID.Value, DocumentIndexTypeToken.Entity))
			{
				_logHelper.LogCreatedMessage(entityTypeName, assignable.AssignableID, assignable.Name);
				return new IndexResult();
			}
			IDocumentIndex entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			EntityDocument doc = _documentFactory.CreateAssignable(assignable);
			var indexResult = doc == null ? new IndexResult() : entityIndex.Index(doc, false, optimizeSetup);
			if (indexResult.DocNumber != -1)
			{
				IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityProject);
				entityProjectIndex.Index(indexResult.DocNumber, _indexDataFactory.CreateProjectData(assignable.ProjectID).ToString(), optimizeSetup);
				IDocumentIndex entityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityType);
				entityTypeIndex.Index(indexResult.DocNumber, entityTypeName, optimizeSetup);
				if (assignable.EntityStateID != null)
				{
					IDocumentIndex entityStateIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityState);
					entityStateIndex.Index(indexResult.DocNumber, _indexDataFactory.CreateEntityStateData(assignable.EntityStateID.Value), optimizeSetup);
				}
				IDocumentIndex squadIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntitySquad);
				squadIndex.Index(indexResult.DocNumber, _indexDataFactory.CreateSquadData(assignable.SquadID), optimizeSetup);
			}
			_log.Debug(string.Format("Added {0} #{1} - '{2}':{3}", entityTypeName, assignable.AssignableID.GetValueOrDefault(), assignable.Name, indexResult.WordsAdded.Any() ? string.Format(" added words - {0};", string.Join(",", indexResult.WordsAdded.Keys)) : " no words added;"));
			return indexResult;
		}

		public void OptimizeAssignableIndex(DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetImmediateOptimizeIfNull(optimizeSetup);
			var indexes = new[] { DocumentIndexTypeToken.Entity, DocumentIndexTypeToken.EntityProject, DocumentIndexTypeToken.EntityType, DocumentIndexTypeToken.EntityState, DocumentIndexTypeToken.EntitySquad }.Select(d => _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, d));
			indexes.ForEach(i => i.Optimize(optimizeSetup));
		}

		public IndexResult UpdateAssignableIndex(AssignableDTO assignable, ICollection<AssignableField> changedFields, bool isIndexing, DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetNoOptimizeIfNull(optimizeSetup);
			var indexResult = new IndexResult();
			if (assignable.AssignableID == null)
			{
				return new IndexResult();
			}
			var indexes = _documentIndexProvider.GetOrCreateDocumentIndexes(_pluginContext, DocumentIndexTypeToken.Entity, DocumentIndexTypeToken.EntityProject, DocumentIndexTypeToken.EntityType, DocumentIndexTypeToken.EntityState, DocumentIndexTypeToken.EntitySquad);
			if (!changedFields.Any(f => indexes.Any(i => i.Type.IsBelongedToDocumentFields(f) || i.Type.IsBelongedToIndexFields(f))))
			{
				return new IndexResult();
			}
			var entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			var document = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(assignable.AssignableID));
			string entityTypeName = GetEntityTypeName(assignable.EntityTypeID);
			if (document == null)
			{
				_logHelper.LogUpdateMessage(entityTypeName, assignable.AssignableID, assignable.Name);
				return indexResult;
			}
			if (changedFields.Any(f => entityIndex.Type.IsBelongedToDocumentFields(f)))
			{
				document.ProjectId = _documentIdFactory.CreateProjectId(assignable.ProjectID.GetValueOrDefault());
				document.SquadId = _documentIdFactory.CreateSquadId(assignable.SquadID.GetValueOrDefault());
				document.EntityTypeId = _documentIdFactory.CreateEntityTypeId(assignable.EntityTypeID.GetValueOrDefault());
				entityIndex.SaveDocument(document, false);
				_log.Debug(string.Format("Updated {0} #{1} - '{2}':{3}{4}{5}", entityTypeName,
						 assignable.AssignableID.GetValueOrDefault(), assignable.Name,
						 changedFields.Contains(AssignableField.ProjectID) ? string.Format(" Project - {0};", string.Join(",", assignable.ProjectName)) : string.Empty,
						 changedFields.Contains(AssignableField.EntityTypeID) ? string.Format(" EntityType - {0};", string.Join(",", entityTypeName)) : string.Empty,
						 changedFields.Contains(AssignableField.EntityStateID) ? string.Format(" EntityState - {0};", string.Join(",", assignable.EntityStateName)) : string.Empty));
			}
			if (document.DocNumber >= 0)
			{
				if (changedFields.Any(f => entityIndex.Type.IsBelongedToIndexFields(f)))
				{
					var text = _documentFactory.CreateAssignable(assignable).Text;
					indexResult = entityIndex.Update(document.DocNumber, text, optimizeSetup);
					_log.Debug(string.Format("Updated {0} #{1} - '{2}':{3}{4}", entityTypeName,
						 assignable.AssignableID.GetValueOrDefault(), assignable.Name,
						 indexResult.WordsAdded.Any() ? string.Format(" added words - {0};", string.Join(",", indexResult.WordsAdded.Keys)) : " no words added;",
						 indexResult.WordsRemoved.Any() ? string.Format(" removed words - {0}; ", string.Join(",", indexResult.WordsRemoved)) : " no words removed;"));
				}
				IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityProject);
				if (changedFields.Any(f => entityProjectIndex.Type.IsBelongedToIndexFields(f)))
				{
					entityProjectIndex.Update(document.DocNumber, _indexDataFactory.CreateProjectData(assignable.ProjectID).ToString(), optimizeSetup);
					if (!isIndexing)
					{
						_localBus.SendLocal(new GeneralProjectChangedLocalMessage { GeneralId = assignable.AssignableID.Value, ProjectId = assignable.ProjectID });
					}
				}
				if (assignable.EntityTypeID != null)
				{
					IDocumentIndex entityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityType);
					if (changedFields.Any(f => entityTypeIndex.Type.IsBelongedToIndexFields(f)))
					{
						entityTypeIndex.Update(document.DocNumber, entityTypeName, optimizeSetup);
					}
				}
				if (assignable.EntityStateID != null)
				{
					IDocumentIndex entityStateIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityState);
					if (changedFields.Any(f => entityStateIndex.Type.IsBelongedToIndexFields(f)))
					{
						entityStateIndex.Update(document.DocNumber, _indexDataFactory.CreateEntityStateData(assignable.EntityStateID.Value), optimizeSetup);
					}
				}
				IDocumentIndex entitySquadIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntitySquad);
				if (changedFields.Any(f => entitySquadIndex.Type.IsBelongedToIndexFields(f)))
				{
					entitySquadIndex.Update(document.DocNumber, _indexDataFactory.CreateSquadData(assignable.SquadID), optimizeSetup);
					if (!isIndexing)
					{
						_localBus.SendLocal(new AssignableSquadChangedLocalMessage { AssignableId = assignable.AssignableID.Value, SquadId = assignable.SquadID });
					}
				}
			}
			return indexResult;
		}

		public IndexResult RemoveAssignableIndex(AssignableDTO assignable, DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetNoOptimizeIfNull(optimizeSetup);
			if (assignable.AssignableID == null)
			{
				return new IndexResult();
			}
			var entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			var document = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(assignable.AssignableID));
			string entityTypeName = GetEntityTypeName(assignable.EntityTypeID);
			if (document == null)
			{
				_logHelper.LogDeletedMessage(entityTypeName, assignable.AssignableID, assignable.Name);
				return new IndexResult();
			}
			var indexResult = entityIndex.Rebuild(_documentFactory.CreateEmptyEntityDocument(document.DocNumber, EntityDocument.CreateName(assignable.AssignableID)), false, optimizeSetup);
			_log.Debug(string.Format("Removed {0} #{1} - '{2}'", entityTypeName, assignable.AssignableID.GetValueOrDefault(), assignable.Name));
			if (document.DocNumber >= 0)
			{
				IDocumentIndex projectContextIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityProject);
				projectContextIndex.Update(document.DocNumber, string.Empty, optimizeSetup);
				IDocumentIndex entityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityType);
				entityTypeIndex.Update(document.DocNumber, string.Empty, optimizeSetup);
				IDocumentIndex entityStateIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityState);
				entityStateIndex.Update(document.DocNumber, string.Empty, optimizeSetup);
				IDocumentIndex entitySquadIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntitySquad);
				entitySquadIndex.Update(document.DocNumber, string.Empty, optimizeSetup);
			}
			return indexResult;
		}

		public IndexResult AddTestCaseIndex(TestCaseDTO testCase, DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetNoOptimizeIfNull(optimizeSetup);
			if (testCase.TestCaseID == null)
			{
				return new IndexResult();
			}
			string entityTypeName = GetEntityTypeName(testCase.EntityTypeID);
			if (Exists<EntityDocument>(testCase.TestCaseID.Value, DocumentIndexTypeToken.Entity))
			{
				_logHelper.LogCreatedMessage(entityTypeName, testCase.TestCaseID, testCase.Name);
				return new IndexResult();
			}
			var entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			var doc = _documentFactory.CreateTestCase(testCase);
			var indexResult = doc == null ? new IndexResult() : entityIndex.Index(doc, false, optimizeSetup);
			if (indexResult.DocNumber != -1)
			{
				IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityProject);
				entityProjectIndex.Index(indexResult.DocNumber, _indexDataFactory.CreateProjectData(testCase.ProjectID).ToString(), optimizeSetup);
				if (testCase.EntityTypeID != null)
				{
					IDocumentIndex entityTypeIndices = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityType);
					entityTypeIndices.Index(indexResult.DocNumber, entityTypeName, optimizeSetup);
				}
			}
			_log.Debug(string.Format("Added {0} #{1} - '{2}':{3}", entityTypeName, testCase.TestCaseID.GetValueOrDefault(), testCase.Name, indexResult.WordsAdded.Any() ? string.Format(" added words - {0};", string.Join(",", indexResult.WordsAdded.Keys)) : " no words added;"));
			return indexResult;
		}

		public void OptimizeTestCaseIndex(DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetImmediateOptimizeIfNull(optimizeSetup);
			var indexes = new[] { DocumentIndexTypeToken.Entity, DocumentIndexTypeToken.EntityProject, DocumentIndexTypeToken.EntityType}.Select(d => _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, d));
			indexes.ForEach(i => i.Optimize(optimizeSetup));
		}

		public IndexResult UpdateTestCaseIndex(TestCaseDTO testCase, ICollection<TestCaseField> changedFields, bool isIndexing, DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetNoOptimizeIfNull(optimizeSetup);
			if (testCase.TestCaseID == null)
			{
				return new IndexResult();
			}
			var indexes = _documentIndexProvider.GetOrCreateDocumentIndexes(_pluginContext, DocumentIndexTypeToken.Entity, DocumentIndexTypeToken.EntityProject, DocumentIndexTypeToken.EntityType);
			if (!changedFields.Any(f => indexes.Any(i => i.Type.IsBelongedToDocumentFields(f) || i.Type.IsBelongedToIndexFields(f))))
			{
				return new IndexResult();
			}
			var entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			var document = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(testCase.TestCaseID));
			string entityTypeName = GetEntityTypeName(testCase.EntityTypeID);
			if (document == null)
			{
				_logHelper.LogUpdateMessage(entityTypeName, testCase.TestCaseID, testCase.Name);
				return new IndexResult();
			}
			if (changedFields.Any(f => entityIndex.Type.IsBelongedToDocumentFields(f)))
			{
				document.ProjectId = _documentIdFactory.CreateProjectId(testCase.ProjectID.GetValueOrDefault());
				document.EntityTypeId = _documentIdFactory.CreateEntityTypeId(testCase.EntityTypeID.GetValueOrDefault());
				entityIndex.SaveDocument(document, false);
				_log.Debug(string.Format("Updated {0} #{1} - '{2}':{3}{4}", entityTypeName,
						 testCase.TestCaseID.GetValueOrDefault(), testCase.Name,
						 changedFields.Contains(TestCaseField.ProjectID) ? string.Format(" Project - {0};", string.Join(",", testCase.ProjectName)) : string.Empty,
						 changedFields.Contains(TestCaseField.EntityTypeID) ? string.Format(" EntityType - {0};", string.Join(",", entityTypeName)) : string.Empty));
			}
			var indexResult = new IndexResult();
			if (document.DocNumber >= 0)
			{
				if (changedFields.Any(f => entityIndex.Type.IsBelongedToIndexFields(f)))
				{
					var newText = _documentFactory.CreateTestCase(testCase).Text;
					indexResult = entityIndex.Update(document.DocNumber, newText, optimizeSetup);
					_log.Debug(string.Format("Updated {0} #{1} - '{2}':{3}{4}", entityTypeName, testCase.TestCaseID.GetValueOrDefault(), testCase.Name, indexResult.WordsAdded.Any() ? string.Format(" added words - {0};", string.Join(",", indexResult.WordsAdded.Keys)) : " NO WORDS ADDED;", indexResult.WordsRemoved.Any() ? string.Format(" removed words - {0};", string.Join(",", indexResult.WordsRemoved)) : " no words removed;"));
				}
				IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityProject);
				if (changedFields.Any(f => entityProjectIndex.Type.IsBelongedToIndexFields(f)))
				{
					entityProjectIndex.Update(document.DocNumber, _indexDataFactory.CreateProjectData(testCase.ProjectID).ToString(), optimizeSetup);
					if (!isIndexing)
					{
						_localBus.SendLocal(new GeneralProjectChangedLocalMessage
							{
								GeneralId = testCase.TestCaseID.Value,
								ProjectId = testCase.ProjectID
							});
					}
				}
				IDocumentIndex entityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityType);
				if (changedFields.Any(f => entityTypeIndex.Type.IsBelongedToIndexFields(f)))
				{
					entityTypeIndex.Update(document.DocNumber, entityTypeName, optimizeSetup);
				}
			}
			return indexResult;
		}

		public IndexResult RemoveTestCaseIndex(TestCaseDTO testCase, DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetNoOptimizeIfNull(optimizeSetup);
			if (testCase.TestCaseID == null)
			{
				return new IndexResult();
			}
			var entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			var document = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(testCase.TestCaseID));
			string entityTypeName = GetEntityTypeName(testCase.EntityTypeID);
			if (document == null)
			{
				_logHelper.LogDeletedMessage(entityTypeName, testCase.TestCaseID, testCase.Name);
				return new IndexResult();
			}
			var indexResult = entityIndex.Rebuild(_documentFactory.CreateEmptyEntityDocument(document.DocNumber, EntityDocument.CreateName(testCase.TestCaseID)), false, optimizeSetup);
			_log.Debug(string.Format("Removed {0} #{1} - '{2}'", entityTypeName, testCase.TestCaseID.GetValueOrDefault(), testCase.Name));
			if (document.DocNumber >= 0)
			{
				IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityProject);
				entityProjectIndex.Update(document.DocNumber, string.Empty, optimizeSetup);
				IDocumentIndex entityTypeIndices = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityType);
				entityTypeIndices.Update(document.DocNumber, string.Empty, optimizeSetup);
			}
			return indexResult;
		}

		public IndexResult AddImpedimentIndex(ImpedimentDTO impediment, DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetNoOptimizeIfNull(optimizeSetup);
			if (impediment.ImpedimentID == null)
			{
				return new IndexResult();
			}
			string entityTypeName = GetEntityTypeName(impediment.EntityTypeID);
			if (Exists<EntityDocument>(impediment.ImpedimentID.Value, DocumentIndexTypeToken.Entity))
			{
				_logHelper.LogCreatedMessage(entityTypeName, impediment.ImpedimentID, impediment.Name);
				return new IndexResult();
			}
			IDocumentIndex entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			EntityDocument doc = _documentFactory.CreateImpediment(impediment);
			var indexResult = doc == null ? new IndexResult() : entityIndex.Index(doc, false, optimizeSetup);
			if (indexResult.DocNumber != -1)
			{
				IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityProject);
				entityProjectIndex.Index(indexResult.DocNumber, _indexDataFactory.CreateProjectData(impediment.ProjectID).ToString(), optimizeSetup);
				IDocumentIndex entityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityType);
				entityTypeIndex.Index(indexResult.DocNumber, entityTypeName, optimizeSetup);
				if (impediment.EntityStateID != null)
				{
					IDocumentIndex entityStateIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityState);
					entityStateIndex.Index(indexResult.DocNumber, _indexDataFactory.CreateEntityStateData(impediment.EntityStateID.Value), optimizeSetup);
				}
				IDocumentIndex impedimentIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Impediment);
				var indexData = _indexDataFactory.CreateImpedimentData(impediment.IsPrivate, impediment.OwnerID, impediment.ResponsibleID);
				impedimentIndex.Index(indexResult.DocNumber, indexData, optimizeSetup);
			}
			_log.Debug(string.Format("Added {0} #{1} - '{2}':{3}", entityTypeName, impediment.ImpedimentID.GetValueOrDefault(), impediment.Name, indexResult.WordsAdded.Any() ? string.Format(" added words - {0};", string.Join(",", indexResult.WordsAdded.Keys)) : " no words added;"));
			return indexResult;
		}

		public void OptimizeImpedimentIndex(DocumentIndexOptimizeSetup optimizeSetup)
		{
			optimizeSetup = GetImmediateOptimizeIfNull(optimizeSetup);
			var indexes = new[] { DocumentIndexTypeToken.Entity, DocumentIndexTypeToken.EntityProject, DocumentIndexTypeToken.EntityType, DocumentIndexTypeToken.EntityState, DocumentIndexTypeToken.Impediment }.Select(d => _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, d));
			indexes.ForEach(i => i.Optimize(optimizeSetup));
		}

		public IndexResult UpdateImpedimentIndex(ImpedimentDTO impediment, ICollection<ImpedimentField> changedFields, bool isIndexing, DocumentIndexOptimizeSetup optimizeSetup)
		{
			optimizeSetup = GetNoOptimizeIfNull(optimizeSetup);
			if (impediment.ImpedimentID == null)
			{
				return new IndexResult();
			}
			var indexes = _documentIndexProvider.GetOrCreateDocumentIndexes(_pluginContext, DocumentIndexTypeToken.Entity, DocumentIndexTypeToken.EntityProject, DocumentIndexTypeToken.EntityType, DocumentIndexTypeToken.EntityState, DocumentIndexTypeToken.Impediment);
			if (!changedFields.Any(f => indexes.Any(i => i.Type.IsBelongedToDocumentFields(f) || i.Type.IsBelongedToIndexFields(f))))
			{
				return new IndexResult();
			}
			var entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			var document = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(impediment.ImpedimentID));
			string entityTypeName = GetEntityTypeName(impediment.EntityTypeID);
			if (document == null)
			{
				_logHelper.LogUpdateMessage(entityTypeName, impediment.ImpedimentID, impediment.Name);
				return new IndexResult();
			}
			if (changedFields.Any(f => entityIndex.Type.IsBelongedToDocumentFields(f)))
			{
				document.ProjectId = _documentIdFactory.CreateProjectId(impediment.ProjectID.GetValueOrDefault());
				document.EntityTypeId = _documentIdFactory.CreateEntityTypeId(impediment.EntityTypeID.GetValueOrDefault());
				entityIndex.SaveDocument(document, false);
				_log.Debug(string.Format("Updated {0} #{1} - '{2}':{3}{4}{5}", entityTypeName,
						 impediment.ImpedimentID.GetValueOrDefault(), impediment.Name,
						 changedFields.Contains(ImpedimentField.ProjectID) ? string.Format(" Project - {0};", string.Join(",", impediment.ProjectName)) : string.Empty,
						 changedFields.Contains(ImpedimentField.EntityTypeID) ? string.Format(" EntityType - {0};", string.Join(",", entityTypeName)) : string.Empty,
						 changedFields.Contains(ImpedimentField.EntityStateID) ? string.Format(" EntityState - {0};", string.Join(",", impediment.EntityStateName)) : string.Empty));
			}
			var indexResult = new IndexResult();
			if (document.DocNumber >= 0)
			{
				if (changedFields.Any(f => entityIndex.Type.IsBelongedToIndexFields(f)))
				{
					var newText = _documentFactory.CreateImpediment(impediment).Text;
					indexResult = entityIndex.Update(document.DocNumber, newText, optimizeSetup);
					_log.Debug(string.Format("Updated {0} #{1} - '{2}':{3}{4}", entityTypeName, impediment.ImpedimentID.GetValueOrDefault(), impediment.Name, indexResult.WordsAdded.Any() ? string.Format(" added words - {0};", string.Join(",", indexResult.WordsAdded.Keys)) : " NO WORDS ADDED;", indexResult.WordsRemoved.Any() ? string.Format(" removed words - {0};", string.Join(",", indexResult.WordsRemoved)) : " no words removed;"));
				}
				IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityProject);
				if (changedFields.Any(f => entityProjectIndex.Type.IsBelongedToIndexFields(f)))
				{
					entityProjectIndex.Update(document.DocNumber, _indexDataFactory.CreateProjectData(impediment.ProjectID).ToString(), optimizeSetup);
					if (!isIndexing)
					{
						_localBus.SendLocal(new GeneralProjectChangedLocalMessage
						{
							GeneralId = impediment.ImpedimentID.Value,
							ProjectId = impediment.ProjectID
						});
					}
				}
				if (impediment.EntityTypeID != null)
				{
					IDocumentIndex entityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityType);
					if (changedFields.Any(f => entityTypeIndex.Type.IsBelongedToIndexFields(f)))
					{
						entityTypeIndex.Update(document.DocNumber, entityTypeName, optimizeSetup);
					}
				}
				if (impediment.EntityStateID != null)
				{
					IDocumentIndex entityStateIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityState);
					if (changedFields.Any(f => entityStateIndex.Type.IsBelongedToIndexFields(f)))
					{
						entityStateIndex.Update(document.DocNumber, _indexDataFactory.CreateEntityStateData(impediment.EntityStateID.Value), optimizeSetup);
					}
				}
				IDocumentIndex impedimentIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Impediment);
				if (changedFields.Any(f => impedimentIndex.Type.IsBelongedToIndexFields(f)))
				{
					var indexData = _indexDataFactory.CreateImpedimentData(impediment.IsPrivate, impediment.OwnerID, impediment.ResponsibleID);
					impedimentIndex.Update(document.DocNumber, indexData, optimizeSetup);
				}
			}
			return indexResult;
		}

		public IndexResult RemoveImpedimentIndex(ImpedimentDTO impediment, DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetNoOptimizeIfNull(optimizeSetup);
			if (impediment.ImpedimentID == null)
			{
				return new IndexResult();
			}
			var entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			var document = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(impediment.ImpedimentID));
			string entityTypeName = GetEntityTypeName(impediment.EntityTypeID);
			if (document == null)
			{
				_logHelper.LogDeletedMessage(entityTypeName, impediment.ImpedimentID, impediment.Name);
				return new IndexResult();
			}
			var indexResult = entityIndex.Rebuild(_documentFactory.CreateEmptyEntityDocument(document.DocNumber, EntityDocument.CreateName(impediment.ImpedimentID)), false, optimizeSetup);
			_log.Debug(string.Format("Removed {0} #{1} - '{2}'", entityTypeName, impediment.ImpedimentID.GetValueOrDefault(), impediment.Name));
			if (document.DocNumber >= 0)
			{
				IDocumentIndex projectContextIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityProject);
				projectContextIndex.Update(document.DocNumber, string.Empty, optimizeSetup);
				IDocumentIndex entityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityType);
				entityTypeIndex.Update(document.DocNumber, string.Empty, optimizeSetup);
				IDocumentIndex entityStateIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityState);
				entityStateIndex.Update(document.DocNumber, string.Empty, optimizeSetup);
				IDocumentIndex impedimentIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Impediment);
				impedimentIndex.Update(document.DocNumber, string.Empty, optimizeSetup);
			}
			return indexResult;
		}

		public IndexResult AddCommentIndex(CommentDTO comment, DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetNoOptimizeIfNull(optimizeSetup);
			var indexResult = new IndexResult();
			if (comment.CommentID == null || comment.GeneralID == null)
			{
				return new IndexResult();
			}
			if (Exists<hOOt.Document>(comment.CommentID.Value, DocumentIndexTypeToken.Comment))
			{
				_logHelper.LogCommentCreatedMessage(comment.CommentID, comment.GeneralName, comment.GeneralID);
				return indexResult;
			}
			var entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			var commentEntity = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(comment.GeneralID));
			if (commentEntity == null)
			{
				_logHelper.LogCommentCreatedMessageIfThereIsNoGeneral(comment.CommentID, comment.GeneralName, comment.GeneralID);
				return indexResult;
			}
			var commentIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Comment);
			var doc = _documentFactory.CreateComment(comment);
			if (doc == null)
			{
				return indexResult;
			}
			indexResult = commentIndex.Index(doc, false, optimizeSetup);
			_log.Debug(string.Format("Added comment #{0} to #{1} - '{2}':{3}", comment.CommentID.GetValueOrDefault(), comment.GeneralID.GetValueOrDefault(), comment.GeneralName, indexResult.WordsAdded.Any() ? string.Format(" added words - {0};", string.Join(",", indexResult.WordsAdded.Keys)) : " no words added;"));
			if (indexResult.DocNumber != -1 && commentEntity.DocNumber != -1)
			{
				IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityProject);
				var entityProjectIndexResult = entityProjectIndex.GetExistingIndexByNumber(commentEntity.DocNumber);
				var entityProjectIndexData = ProjectIndexData.Parse(entityProjectIndexResult);
				IDocumentIndex commentProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.CommentProject);
				commentProjectIndex.Index(indexResult.DocNumber, entityProjectIndexData.ToString(), optimizeSetup);
				int? squadId = null;
				if (commentEntity.SquadId != null)
				{
					int squadIdValue;
					if (int.TryParse(commentEntity.SquadId, out squadIdValue))
					{
						squadId = squadIdValue == 0 ? null : (int?)squadIdValue;
					}
				}
				IDocumentIndex commentSquadIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.CommentSquad);
				commentSquadIndex.Index(indexResult.DocNumber, _indexDataFactory.CreateSquadData(squadId), optimizeSetup);
				IDocumentIndex commentEntityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.CommentEntityType);
				Maybe<string> maybeEntityTypeName = _entityTypeProvider.GetEntityTypeName(int.Parse(commentEntity.EntityTypeId));
				string entityTypeName = maybeEntityTypeName.GetOrThrow(() => new ApplicationException("No entity type name for {0}".Fmt(commentEntity.EntityTypeId)));
				commentEntityTypeIndex.Index(indexResult.DocNumber, entityTypeName, optimizeSetup);
			}
			return indexResult;
		}

		public void OptimizeCommentIndex(DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetImmediateOptimizeIfNull(optimizeSetup);
			var indexes = new[] { DocumentIndexTypeToken.Comment, DocumentIndexTypeToken.CommentProject, DocumentIndexTypeToken.CommentSquad, DocumentIndexTypeToken.CommentEntityType }.Select(d => _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, d));
			indexes.ForEach(i => i.Optimize(optimizeSetup));
		}

		public IndexResult UpdateCommentIndex(CommentDTO comment, ICollection<CommentField> changedFields, Maybe<int?> projectId, Maybe<int?> squadId, DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetNoOptimizeIfNull(optimizeSetup);
			if (comment.CommentID == null || comment.GeneralID == null)
			{
				return new IndexResult();
			}
			var entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			var commentEntityDocument = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(comment.GeneralID));
			if (commentEntityDocument == null)
			{
				_logHelper.LogCommentUpdateMessage(comment.CommentID, comment.GeneralName, comment.GeneralID);
				return new IndexResult();
			}
			var commentIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Comment);
			var commentDocument = commentIndex.FindDocumentByName<hOOt.Document>(EntityDocument.CreateName(comment.CommentID));
			if (commentDocument == null)
			{
				_logHelper.LogCommentUpdateMessage(comment.CommentID, comment.GeneralName, comment.GeneralID);
				return new IndexResult();
			}
			if (commentDocument.DocNumber >= 0)
			{
				if (projectId.HasValue)
				{
					IDocumentIndex entityProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityProject);
					var entityProjectIndexResult = entityProjectIndex.GetExistingIndexByNumber(commentEntityDocument.DocNumber);
					var entityProjectIndexData = ProjectIndexData.Parse(entityProjectIndexResult);
					IDocumentIndex commentProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.CommentProject);
					commentProjectIndex.Update(commentDocument.DocNumber, entityProjectIndexData.ToString(), optimizeSetup);
				}
				if (squadId.HasValue)
				{
					IDocumentIndex commentSquadIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.CommentSquad);
					int? squadIdValue = squadId.Value;
					commentSquadIndex.Update(commentDocument.DocNumber, _indexDataFactory.CreateSquadData(squadIdValue), optimizeSetup);
				}
				if (changedFields.Any(f => commentIndex.Type.IsBelongedToIndexFields(f)))
				{
					var text = _documentFactory.CreateComment(comment).Text;
					var indexResult = commentIndex.Update(commentDocument.FileName, text, optimizeSetup);
					_log.Debug(string.Format("Updated comment #{0} for #{1} - '{2}':{3}{4}", comment.CommentID.GetValueOrDefault(), comment.GeneralID.GetValueOrDefault(), comment.GeneralName, indexResult.WordsAdded.Any() ? string.Format(" added words - {0};", string.Join(",", indexResult.WordsAdded.Keys)) : " NO WORDS ADDED;", indexResult.WordsRemoved.Any() ? string.Format(" removed words - {0};", string.Join(",", indexResult.WordsRemoved)) : " no words removed;"));
					return indexResult;
				}
			}
			return new IndexResult();
		}

		public IndexResult RemoveCommentIndex(CommentDTO comment, DocumentIndexOptimizeSetup optimizeSetup = null)
		{
			optimizeSetup = GetNoOptimizeIfNull(optimizeSetup);
			if (comment.CommentID == null || comment.GeneralID == null)
			{
				return new IndexResult();
			}
			var entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			var commentEntity = entityIndex.FindDocumentByName<EntityDocument>(EntityDocument.CreateName(comment.GeneralID));
			if (commentEntity == null)
			{
				_logHelper.LogCommentDeletedMessage(comment.CommentID, comment.GeneralName, comment.GeneralID);
				return new IndexResult();
			}
			var commentIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Comment);
			var commentDoc = commentIndex.FindDocumentByName<hOOt.Document>(EntityDocument.CreateName(comment.CommentID));
			if(commentDoc == null)
			{
				_logHelper.LogCommentDeletedMessage(comment.CommentID, comment.GeneralName, comment.GeneralID);
                return new IndexResult();
			}
			var indexResult = commentIndex.Rebuild(_documentFactory.CreateEmptyCommentDocument(commentDoc.DocNumber, EntityDocument.CreateName(comment.CommentID)), false, optimizeSetup);
			_log.Debug(string.Format("Removed comment #{0} from #{1} - '{2}'", comment.CommentID.GetValueOrDefault(), comment.GeneralID.GetValueOrDefault(), comment.GeneralName));
			if (indexResult.DocNumber >= 0)
			{
				IDocumentIndex commentProjectIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.CommentProject);
				commentProjectIndex.Update(indexResult.DocNumber, string.Empty, optimizeSetup);
				IDocumentIndex commentSquadIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.CommentSquad);
				commentSquadIndex.Update(indexResult.DocNumber, string.Empty, optimizeSetup);
				IDocumentIndex commentEntityType = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.CommentEntityType);
				commentEntityType.Update(indexResult.DocNumber, string.Empty, optimizeSetup);
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
			IDocumentIndex entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, indexType);
			return entityIndex.FindDocumentByName<TDocument>(EntityDocument.CreateName(entityId)) != null;
		}

		private DocumentIndexOptimizeSetup GetNoOptimizeIfNull(DocumentIndexOptimizeSetup setup)
		{
			return setup ?? DocumentIndexOptimizeSetup.NoOptimize;
		}

		private DocumentIndexOptimizeSetup GetImmediateOptimizeIfNull(DocumentIndexOptimizeSetup setup)
		{
			return setup ?? DocumentIndexOptimizeSetup.ImmediateOptimize;
		}

		private class LogHelper
		{
			private const string CommentTypeName = "Comment";
			private readonly IActivityLogger _log;
			private readonly IProfileReadonly _profile;

			public LogHelper(IActivityLogger log, IProfileReadonly profile)
			{
				_log = log;
				_profile = profile;
			}

			public void LogCommentCreatedMessageIfThereIsNoGeneral(int? entityId, string name, int? boundEntityId)
			{
				LogCreatedMessageCore(CommentTypeName, entityId, name, boundEntityId, ReasonType.NotAdded);
			}

			public void LogCommentCreatedMessage(int? entityId, string name, int? boundEntityId = null)
			{
				LogCreatedMessageCore(CommentTypeName, entityId, name, boundEntityId, ReasonType.AlreadyAdded);
			}

			public void LogCommentUpdateMessage(int? entityId, string name, int? boundEntityId)
			{
				LogUpdateMessageCore(CommentTypeName, entityId, name, boundEntityId);
			}

			public void LogCommentDeletedMessage(int? entityId, string name, int? boundEntityId)
			{
				LogDeletedMessageCore(CommentTypeName, entityId, name, boundEntityId);
			}

			public void LogCreatedMessage(string entityTypeName, int? entityId, string name)
			{
				LogCreatedMessageCore(entityTypeName, entityId, name, null, ReasonType.AlreadyAdded);
			}

			public void LogUpdateMessage(string entityTypeName, int? entityId, string name)
			{
				LogUpdateMessageCore(entityTypeName, entityId, name, null);
			}

			public void LogDeletedMessage(string entityTypeName, int? entityId, string name)
			{
				LogDeletedMessageCore(entityTypeName, entityId, name, null);
			}

			private void LogDeletedMessageCore(string entityTypeName, int? entityId, string name, int? boundEntityId)
			{
				_log.DebugFormat("{0}DeletedMessage' for entity #{1} with name '{2}' was ignored. {3}. {4}", entityTypeName, entityId, name, GetReason(ReasonType.NotAdded), CreateBoundEntityIdTrace(boundEntityId));
			}

			private void LogUpdateMessageCore(string entityTypeName, int? entityId, string name, int? boundEntityId)
			{
				_log.DebugFormat("'{0}UpdatedMessage' for entity #{1} with name '{2}' was ignored. {3}. {4}", entityTypeName, entityId, name, GetReason(ReasonType.NotAdded), CreateBoundEntityIdTrace(boundEntityId));
			}

			private void LogCreatedMessageCore(string entityTypeName, int? entityId, string name, int? boundEntityId, ReasonType reasonType)
			{
				if (_profile.Initialized)
				{
					_log.DebugFormat("'{0}CreatedMessage' for entity #{1} with name '{2}' was ignored. {3}. {4}", entityTypeName, entityId, name, GetReason(reasonType), CreateBoundEntityIdTrace(boundEntityId));
				}
				else
				{
					_log.DebugFormat("'{0}CreatedMessage' for entity #{1} with name '{2}'. {3}. {4}", entityTypeName, entityId, name, GetReason(reasonType), CreateBoundEntityIdTrace(boundEntityId));
				}
			}

			private string CreateBoundEntityIdTrace(int? boundEntityId = null)
			{
				return boundEntityId != null ? "Bound entity id {0}".Fmt(boundEntityId) : String.Empty;
			}

			private enum ReasonType
			{
				AlreadyAdded,
				NotAdded
			}

			private string GetReason(ReasonType reasonType)
			{
				switch (reasonType)
				{
					case ReasonType.NotAdded:
						return "Entity was not added during index rebuilding or entity creation";
					case ReasonType.AlreadyAdded:
						return "Entity has already been added during index rebuilding or entity creation";
					default:
						return string.Empty;
				}
			}
		}
	}
}

// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using AutoMapper;
using NServiceBus;
using System.Linq;
using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;

namespace Tp.Search.Bus
{
	class SearchableEntityLifecycleHandler : IHandleMessages<ReleaseCreatedMessage>,
																								IHandleMessages<ReleaseUpdatedMessage>,
																								IHandleMessages<ReleaseDeletedMessage>,
																								IHandleMessages<IterationCreatedMessage>,
																								IHandleMessages<IterationUpdatedMessage>,
																								IHandleMessages<IterationDeletedMessage>,
																								IHandleMessages<UserStoryCreatedMessage>,
																								IHandleMessages<UserStoryUpdatedMessage>,
																								IHandleMessages<UserStoryDeletedMessage>,
																								IHandleMessages<TaskCreatedMessage>,
																								IHandleMessages<TaskUpdatedMessage>,
																								IHandleMessages<TaskDeletedMessage>,
																								IHandleMessages<BugCreatedMessage>,
																								IHandleMessages<BugUpdatedMessage>,
																								IHandleMessages<BugDeletedMessage>,
																								IHandleMessages<FeatureCreatedMessage>,
																								IHandleMessages<FeatureUpdatedMessage>,
																								IHandleMessages<FeatureDeletedMessage>,
																								IHandleMessages<TestCaseCreatedMessage>,
																								IHandleMessages<TestCaseUpdatedMessage>,
																								IHandleMessages<TestCaseDeletedMessage>,
																								IHandleMessages<TestPlanCreatedMessage>,
																								IHandleMessages<TestPlanUpdatedMessage>,
																								IHandleMessages<TestPlanDeletedMessage>,
																								IHandleMessages<TestPlanRunCreatedMessage>,
																								IHandleMessages<TestPlanRunUpdatedMessage>,
																								IHandleMessages<TestPlanRunDeletedMessage>,
																								IHandleMessages<RequestCreatedMessage>,
																								IHandleMessages<RequestUpdatedMessage>,
																								IHandleMessages<RequestDeletedMessage>,
																								IHandleMessages<ImpedimentCreatedMessage>,
																								IHandleMessages<ImpedimentUpdatedMessage>,
																								IHandleMessages<ImpedimentDeletedMessage>,
																								IHandleMessages<CommentCreatedMessage>,
																								IHandleMessages<CommentUpdatedMessage>,
																								IHandleMessages<CommentDeletedMessage>,
																								IHandleMessages<ProjectUpdatedMessage>
	{
		private readonly IEntityIndexer _entityIndexer;
		private readonly DocumentIndexRebuilder _documentIndexRebuilder;

		static SearchableEntityLifecycleHandler()
		{
			Mapper.CreateMap<ReleaseDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));
			Mapper.CreateMap<IterationDTO, GeneralDTO>();
			Mapper.CreateMap<UserStoryDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));
			Mapper.CreateMap<TaskDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));
			Mapper.CreateMap<BugDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));
			Mapper.CreateMap<FeatureDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));
			Mapper.CreateMap<TestCaseDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));
			Mapper.CreateMap<TestPlanDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));
			Mapper.CreateMap<TestPlanRunDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));
			Mapper.CreateMap<RequestDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));
			Mapper.CreateMap<ImpedimentDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));

			Mapper.CreateMap<UserStoryDTO, AssignableDTO>();
			Mapper.CreateMap<TaskDTO, AssignableDTO>();
			Mapper.CreateMap<BugDTO, AssignableDTO>();
			Mapper.CreateMap<FeatureDTO, AssignableDTO>();
			//Mapper.CreateMap<TestPlanDTO, AssignableDTO>();
			Mapper.CreateMap<TestPlanRunDTO, AssignableDTO>();
			Mapper.CreateMap<RequestDTO, AssignableDTO>();
		}

		public SearchableEntityLifecycleHandler(IEntityIndexer entityIndexer, DocumentIndexRebuilder documentIndexRebuilder)
		{
			_entityIndexer = entityIndexer;
			_documentIndexRebuilder = documentIndexRebuilder;
		}

		private static ICollection<AssignableField> GetAssignableChangedFields<TAssignableField>(IEnumerable<TAssignableField> assignableChangedFields)
		{
			return (from assignableField in assignableChangedFields
			        select assignableField.ToString()
			        into value where Enum.IsDefined(typeof (AssignableField), value)
			        select (AssignableField) Enum.Parse(typeof (AssignableField), value, true)).ToList();
		}

		private static ICollection<GeneralField> GetGeneralChangedFields<TGeneralField>(IEnumerable<TGeneralField> generalChangedFields)
		{
			var result = new List<GeneralField>();
			foreach (var value in generalChangedFields.Select(generalField => generalField.ToString()))
			{
				if (Enum.IsDefined(typeof(GeneralField), value))
				{
					result.Add((GeneralField)Enum.Parse(typeof(GeneralField), value, true));
				}
				else if(string.CompareOrdinal(value, AssignableField.ProjectID.ToString()) == 0)
				{
					result.Add(GeneralField.ParentProjectID);
				}
			}
			return result;
		}

		public void Handle(ReleaseCreatedMessage message)
		{
			AddGeneralIndex(Mapper.Map<ReleaseDTO, GeneralDTO>(message.Dto));
		}

		public void Handle(ReleaseUpdatedMessage message)
		{
			UpdateGeneralIndex(Mapper.Map<ReleaseDTO, GeneralDTO>(message.Dto), GetGeneralChangedFields(message.ChangedFields));
		}

		public void Handle(ReleaseDeletedMessage message)
		{
			RemoveGeneralIndex(Mapper.Map<ReleaseDTO, GeneralDTO>(message.Dto));
		}

		public void Handle(IterationCreatedMessage message)
		{
			AddGeneralIndex(Mapper.Map<IterationDTO, GeneralDTO>(message.Dto));
		}

		public void Handle(IterationUpdatedMessage message)
		{
			UpdateGeneralIndex(Mapper.Map<IterationDTO, GeneralDTO>(message.Dto), GetGeneralChangedFields(message.ChangedFields));
		}

		public void Handle(IterationDeletedMessage message)
		{
			RemoveGeneralIndex(Mapper.Map<IterationDTO, GeneralDTO>(message.Dto));
		}

		public void Handle(UserStoryCreatedMessage message)
		{
			AddAssignableIndex(Mapper.Map<UserStoryDTO, AssignableDTO>(message.Dto));
		}

		public void Handle(UserStoryUpdatedMessage message)
		{
			UpdateAssignableIndex(Mapper.Map<UserStoryDTO, AssignableDTO>(message.Dto), GetAssignableChangedFields(message.ChangedFields));
		}

		public void Handle(UserStoryDeletedMessage message)
		{
			RemoveAssignableIndex(Mapper.Map<UserStoryDTO, AssignableDTO>(message.Dto));
		}

		public void Handle(TaskCreatedMessage message)
		{
			AddAssignableIndex(Mapper.Map<TaskDTO, AssignableDTO>(message.Dto));
		}

		public void Handle(TaskUpdatedMessage message)
		{
			UpdateAssignableIndex(Mapper.Map<TaskDTO, AssignableDTO>(message.Dto), GetAssignableChangedFields(message.ChangedFields));
		}

		public void Handle(TaskDeletedMessage message)
		{
			RemoveAssignableIndex(Mapper.Map<TaskDTO,AssignableDTO>(message.Dto));
		}

		public void Handle(BugCreatedMessage message)
		{
			AddAssignableIndex(Mapper.Map<BugDTO, AssignableDTO>(message.Dto));
		}

		public void Handle(BugUpdatedMessage message)
		{
			UpdateAssignableIndex(Mapper.Map<BugDTO, AssignableDTO>(message.Dto), GetAssignableChangedFields(message.ChangedFields));
		}

		public void Handle(BugDeletedMessage message)
		{
			RemoveAssignableIndex(Mapper.Map<BugDTO, AssignableDTO>(message.Dto));
		}

		public void Handle(FeatureCreatedMessage message)
		{
			AddAssignableIndex(Mapper.Map<FeatureDTO, AssignableDTO>(message.Dto));
		}

		public void Handle(FeatureUpdatedMessage message)
		{
			UpdateAssignableIndex(Mapper.Map<FeatureDTO, AssignableDTO>(message.Dto), GetAssignableChangedFields(message.ChangedFields));
		}

		public void Handle(FeatureDeletedMessage message)
		{
			RemoveAssignableIndex(Mapper.Map<FeatureDTO, AssignableDTO>(message.Dto));
		}

		public void Handle(TestCaseCreatedMessage message)
		{
			AddTestCaseIndex(message.Dto);
		}

		public void Handle(TestCaseUpdatedMessage message)
		{
			UpdateTestCaseIndex(message.Dto, message.ChangedFields);
		}

		public void Handle(TestCaseDeletedMessage message)
		{
			RemoveTestCaseIndex(message.Dto);
		}

		public void Handle(TestPlanCreatedMessage message)
		{
			AddGeneralIndex(Mapper.Map<TestPlanDTO, GeneralDTO>(message.Dto));
			//AddAssignableIndex(Mapper.Map<TestPlanDTO, AssignableDTO>(message.Dto));
		}

		public void Handle(TestPlanUpdatedMessage message)
		{
			UpdateGeneralIndex(Mapper.Map<TestPlanDTO, GeneralDTO>(message.Dto), GetGeneralChangedFields(message.ChangedFields));
			//UpdateAssignableIndex(Mapper.Map<TestPlanDTO, AssignableDTO>(message.Dto), GetAssignableChangedFields(message.ChangedFields));
		}

		public void Handle(TestPlanDeletedMessage message)
		{
			RemoveGeneralIndex(Mapper.Map<TestPlanDTO, GeneralDTO>(message.Dto));
			//RemoveAssignableIndex(Mapper.Map<TestPlanDTO, AssignableDTO>(message.Dto));
		}

		public void Handle(TestPlanRunCreatedMessage message)
		{
			AddAssignableIndex(Mapper.Map<TestPlanRunDTO, AssignableDTO>(message.Dto));
		}

		public void Handle(TestPlanRunUpdatedMessage message)
		{
			UpdateAssignableIndex(Mapper.Map<TestPlanRunDTO, AssignableDTO>(message.Dto), GetAssignableChangedFields(message.ChangedFields));
		}

		public void Handle(TestPlanRunDeletedMessage message)
		{
			RemoveAssignableIndex(Mapper.Map<TestPlanRunDTO, AssignableDTO>(message.Dto));
		}

		public void Handle(RequestCreatedMessage message)
		{
			AddAssignableIndex(Mapper.Map<RequestDTO, AssignableDTO>(message.Dto));
		}

		public void Handle(RequestUpdatedMessage message)
		{
			UpdateAssignableIndex(Mapper.Map<RequestDTO, AssignableDTO>(message.Dto), GetAssignableChangedFields(message.ChangedFields));
		}

		public void Handle(RequestDeletedMessage message)
		{
			RemoveAssignableIndex(Mapper.Map<RequestDTO, AssignableDTO>(message.Dto));
		}

		public void Handle(ImpedimentCreatedMessage message)
		{
			AddImpedimentIndex(message.Dto);
		}

		public void Handle(ImpedimentUpdatedMessage message)
		{
			UpdateImpedimentIndex(message.Dto, message.ChangedFields);
		}

		public void Handle(ImpedimentDeletedMessage message)
		{
			RemoveImpedimentIndex(message.Dto);
		}

		public void Handle(CommentCreatedMessage message)
		{
			AddCommentIndex(message.Dto);
		}

		public void Handle(CommentUpdatedMessage message)
		{
			UpdateCommentIndex(message.Dto, message.ChangedFields);
		}

		public void Handle(CommentDeletedMessage message)
		{
			RemoveCommentIndex(message.Dto);
		}

		public void Handle(ProjectUpdatedMessage message)
		{
			if (message.ChangedFields.Contains(ProjectField.ProcessID))
			{
				_entityIndexer.UpdateAssignablesForProjectProcessChange(message.Dto);
			}
		}

		private void AddGeneralIndex(GeneralDTO generalDto)
		{
			RebuildOrAction(() => _entityIndexer.AddGeneralIndex(generalDto, DocumentIndexOptimizeSetup.DeferredOptimize));
		}

		private void AddAssignableIndex(AssignableDTO assignableDto)
		{
			RebuildOrAction(() => _entityIndexer.AddAssignableIndex(assignableDto, DocumentIndexOptimizeSetup.DeferredOptimize));
		}

		private void AddTestCaseIndex(TestCaseDTO testCaseDto)
		{
			RebuildOrAction(() => _entityIndexer.AddTestCaseIndex(testCaseDto, DocumentIndexOptimizeSetup.DeferredOptimize));
		}

		private void AddImpedimentIndex(ImpedimentDTO impedimentDto)
		{
			RebuildOrAction(() => _entityIndexer.AddImpedimentIndex(impedimentDto, DocumentIndexOptimizeSetup.DeferredOptimize));
		}

		private void UpdateGeneralIndex(GeneralDTO generalDto, ICollection<GeneralField> changedGeneralFields)
		{
			RebuildOrAction(() => _entityIndexer.UpdateGeneralIndex(generalDto, changedGeneralFields, DocumentIndexOptimizeSetup.DeferredOptimize));
		}

		private void UpdateAssignableIndex(AssignableDTO assignableDto, ICollection<AssignableField> changedAssignableFields)
		{
			RebuildOrAction(() => _entityIndexer.UpdateAssignableIndex(assignableDto, changedAssignableFields, false, DocumentIndexOptimizeSetup.DeferredOptimize));
		}

		private void UpdateTestCaseIndex(TestCaseDTO testCaseDto, ICollection<TestCaseField> changedTestCaseFields)
		{
			RebuildOrAction(() => _entityIndexer.UpdateTestCaseIndex(testCaseDto, changedTestCaseFields, false, DocumentIndexOptimizeSetup.DeferredOptimize));
		}

		private void UpdateImpedimentIndex(ImpedimentDTO impedimentDto, ICollection<ImpedimentField> changedImpedimentFields)
		{
			RebuildOrAction(() => _entityIndexer.UpdateImpedimentIndex(impedimentDto, changedImpedimentFields, false, DocumentIndexOptimizeSetup.DeferredOptimize));
		}

		private void RemoveGeneralIndex(GeneralDTO generalDto)
		{
			RebuildOrAction(() => _entityIndexer.RemoveGeneralIndex(generalDto, DocumentIndexOptimizeSetup.DeferredOptimize));
		}

		private void RemoveAssignableIndex(AssignableDTO assignableDto)
		{
			RebuildOrAction(() => _entityIndexer.RemoveAssignableIndex(assignableDto, DocumentIndexOptimizeSetup.DeferredOptimize));
		}

		private void RemoveTestCaseIndex(TestCaseDTO testCaseDto)
		{
			RebuildOrAction(() => _entityIndexer.RemoveTestCaseIndex(testCaseDto, DocumentIndexOptimizeSetup.DeferredOptimize));
		}

		private void RemoveImpedimentIndex(ImpedimentDTO impedimentDto)
		{
			RebuildOrAction(() => _entityIndexer.RemoveImpedimentIndex(impedimentDto));
		}

		private void AddCommentIndex(CommentDTO commentDto)
		{
			RebuildOrAction(() => _entityIndexer.AddCommentIndex(commentDto, DocumentIndexOptimizeSetup.DeferredOptimize));
		}

		private void UpdateCommentIndex(CommentDTO commentDto, ICollection<CommentField> changedCommentFields)
		{
			RebuildOrAction(() => _entityIndexer.UpdateCommentIndex(commentDto, changedCommentFields, Maybe.Nothing, Maybe.Nothing, DocumentIndexOptimizeSetup.DeferredOptimize));
		}

		private void RemoveCommentIndex(CommentDTO commentDto)
		{
			RebuildOrAction(() => _entityIndexer.RemoveCommentIndex(commentDto, DocumentIndexOptimizeSetup.DeferredOptimize));
		}

		private void RebuildOrAction(Action a)
		{
			if(_documentIndexRebuilder.RebuildIfNeeded())
			{
				return;
			}
			a();
		}
	}
}

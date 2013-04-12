// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
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
using Tp.Search.Model.Entity;

namespace Tp.Search.Bus
{
	public class SearchableEntityLifecycleHandler : IHandleMessages<ReleaseCreatedMessage>,
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
																								IHandleMessages<CommentCreatedMessage>,
																								IHandleMessages<CommentUpdatedMessage>,
																								IHandleMessages<CommentDeletedMessage>,
																								IHandleMessages<SolutionCreatedMessage>,
																								IHandleMessages<SolutionUpdatedMessage>,
																								IHandleMessages<SolutionDeletedMessage>,
																								IHandleMessages<ProjectUpdatedMessage>
	{
		private readonly IEntityIndexer _entityIndexer;

		public SearchableEntityLifecycleHandler(IEntityIndexer entityIndexer)
		{
			Mapper.CreateMap<ReleaseDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));
			Mapper.CreateMap<UserStoryDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));
			Mapper.CreateMap<TaskDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));
			Mapper.CreateMap<BugDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));
			Mapper.CreateMap<FeatureDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));
			Mapper.CreateMap<TestCaseDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));
			Mapper.CreateMap<TestPlanDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));
			Mapper.CreateMap<TestPlanRunDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));
			Mapper.CreateMap<RequestDTO, GeneralDTO>().ForMember(dest => dest.ParentProjectID, opt => opt.MapFrom(src => src.ProjectID));
			_entityIndexer = entityIndexer;
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
			AddGeneralIndex(Mapper.DynamicMap<GeneralDTO>(message.Dto));
		}

		public void Handle(ReleaseUpdatedMessage message)
		{
			UpdateGeneralIndex(Mapper.DynamicMap<GeneralDTO>(message.Dto), GetGeneralChangedFields(message.ChangedFields));
		}

		public void Handle(ReleaseDeletedMessage message)
		{
			RemoveGeneralIndex(Mapper.DynamicMap<GeneralDTO>(message.Dto));
		}

		public void Handle(IterationCreatedMessage message)
		{
			AddGeneralIndex(Mapper.DynamicMap<GeneralDTO>(message.Dto));
		}

		public void Handle(IterationUpdatedMessage message)
		{
			UpdateGeneralIndex(Mapper.DynamicMap<GeneralDTO>(message.Dto), GetGeneralChangedFields(message.ChangedFields));
		}

		public void Handle(IterationDeletedMessage message)
		{
			RemoveGeneralIndex(Mapper.DynamicMap<GeneralDTO>(message.Dto));
		}

		public void Handle(UserStoryCreatedMessage message)
		{
			AddAssignableIndex(Mapper.DynamicMap<AssignableDTO>(message.Dto));
		}

		public void Handle(UserStoryUpdatedMessage message)
		{
			UpdateAssignableIndex(Mapper.DynamicMap<AssignableDTO>(message.Dto), GetAssignableChangedFields(message.ChangedFields));
		}

		public void Handle(UserStoryDeletedMessage message)
		{
			RemoveAssignableIndex(Mapper.DynamicMap<AssignableDTO>(message.Dto));
		}

		public void Handle(TaskCreatedMessage message)
		{
			AddAssignableIndex(Mapper.DynamicMap<AssignableDTO>(message.Dto));
		}

		public void Handle(TaskUpdatedMessage message)
		{
			UpdateAssignableIndex(Mapper.DynamicMap<AssignableDTO>(message.Dto), GetAssignableChangedFields(message.ChangedFields));
		}

		public void Handle(TaskDeletedMessage message)
		{
			RemoveAssignableIndex(Mapper.DynamicMap<AssignableDTO>(message.Dto));
		}

		public void Handle(BugCreatedMessage message)
		{
			AddAssignableIndex(Mapper.DynamicMap<AssignableDTO>(message.Dto));
		}

		public void Handle(BugUpdatedMessage message)
		{
			UpdateAssignableIndex(Mapper.DynamicMap<AssignableDTO>(message.Dto), GetAssignableChangedFields(message.ChangedFields));
		}

		public void Handle(BugDeletedMessage message)
		{
			RemoveAssignableIndex(Mapper.DynamicMap<AssignableDTO>(message.Dto));
		}

		public void Handle(FeatureCreatedMessage message)
		{
			AddAssignableIndex(Mapper.DynamicMap<AssignableDTO>(message.Dto));
		}

		public void Handle(FeatureUpdatedMessage message)
		{
			UpdateAssignableIndex(Mapper.DynamicMap<AssignableDTO>(message.Dto), GetAssignableChangedFields(message.ChangedFields));
		}

		public void Handle(FeatureDeletedMessage message)
		{
			RemoveAssignableIndex(Mapper.DynamicMap<AssignableDTO>(message.Dto));
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
			AddGeneralIndex(Mapper.DynamicMap<GeneralDTO>(message.Dto));
		}

		public void Handle(TestPlanUpdatedMessage message)
		{
			UpdateGeneralIndex(Mapper.DynamicMap<GeneralDTO>(message.Dto), GetGeneralChangedFields(message.ChangedFields));
		}

		public void Handle(TestPlanDeletedMessage message)
		{
			RemoveGeneralIndex(Mapper.DynamicMap<GeneralDTO>(message.Dto));
		}

		public void Handle(TestPlanRunCreatedMessage message)
		{
			AddAssignableIndex(Mapper.DynamicMap<AssignableDTO>(message.Dto));
		}

		public void Handle(TestPlanRunUpdatedMessage message)
		{
			UpdateAssignableIndex(Mapper.DynamicMap<AssignableDTO>(message.Dto), GetAssignableChangedFields(message.ChangedFields));
		}

		public void Handle(TestPlanRunDeletedMessage message)
		{
			RemoveAssignableIndex(Mapper.DynamicMap<AssignableDTO>(message.Dto));
		}

		public void Handle(RequestCreatedMessage message)
		{
			AddAssignableIndex(Mapper.DynamicMap<AssignableDTO>(message.Dto));
		}

		public void Handle(RequestUpdatedMessage message)
		{
			UpdateAssignableIndex(Mapper.DynamicMap<AssignableDTO>(message.Dto), GetAssignableChangedFields(message.ChangedFields));
		}

		public void Handle(RequestDeletedMessage message)
		{
			RemoveAssignableIndex(Mapper.DynamicMap<AssignableDTO>(message.Dto));
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

		public void Handle(SolutionCreatedMessage message)
		{
			AddGeneralIndex(Mapper.DynamicMap<GeneralDTO>(message.Dto));
		}

		public void Handle(SolutionUpdatedMessage message)
		{
			UpdateGeneralIndex(Mapper.DynamicMap<GeneralDTO>(message.Dto), GetGeneralChangedFields(message.ChangedFields));
		}

		public void Handle(SolutionDeletedMessage message)
		{
			RemoveGeneralIndex(Mapper.DynamicMap<GeneralDTO>(message.Dto));
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
			_entityIndexer.AddGeneralIndex(generalDto);
			_entityIndexer.OptimizeGeneralIndex();
		}

		private void AddAssignableIndex(AssignableDTO assignableDto)
		{
			_entityIndexer.AddAssignableIndex(assignableDto);
			_entityIndexer.OptimizeAssignableIndex();
		}

		private void AddTestCaseIndex(TestCaseDTO testCaseDto)
		{
			_entityIndexer.AddTestCaseIndex(testCaseDto);
			_entityIndexer.OptimizeTestCaseIndex();
		}

		private void UpdateGeneralIndex(GeneralDTO generalDto, ICollection<GeneralField> changedGeneralFields)
		{
			_entityIndexer.UpdateGeneralIndex(generalDto, changedGeneralFields);
			_entityIndexer.OptimizeGeneralIndex();
		}

		private void UpdateAssignableIndex(AssignableDTO assignableDto, ICollection<AssignableField> changedAssignableFields)
		{
			if (!changedAssignableFields.Any(
				f => f == AssignableField.Description || f == AssignableField.Name || f == AssignableField.EntityStateID ||
						 f == AssignableField.ProjectID || f == AssignableField.EntityTypeID || f == AssignableField.SquadID)) return;

			_entityIndexer.UpdateAssignableIndex(assignableDto, changedAssignableFields, isIndexing:false);
			_entityIndexer.OptimizeAssignableIndex();
		}

		private void UpdateTestCaseIndex(TestCaseDTO testCaseDto, ICollection<TestCaseField> changedTestCaseFields)
		{
			if (!changedTestCaseFields.Any(
				f => f == TestCaseField.Steps || f == TestCaseField.Success || f == TestCaseField.Name ||
						 f == TestCaseField.ProjectID || f == TestCaseField.EntityTypeID)) return;

			_entityIndexer.UpdateTestCaseIndex(testCaseDto, changedTestCaseFields, isIndexing:false);
			_entityIndexer.OptimizeTestCaseIndex();
		}

		private void RemoveGeneralIndex(GeneralDTO generalDto)
		{
			_entityIndexer.RemoveGeneralIndex(generalDto);
		}

		private void RemoveAssignableIndex(AssignableDTO assignableDto)
		{
			_entityIndexer.RemoveAssignableIndex(assignableDto);
		}

		private void RemoveTestCaseIndex(TestCaseDTO testCaseDto)
		{
			_entityIndexer.RemoveTestCaseIndex(testCaseDto);
		}

		private void AddCommentIndex(CommentDTO commentDto)
		{
			_entityIndexer.AddCommentIndex(commentDto);
			_entityIndexer.OptimizeCommentIndex();
		}

		private void UpdateCommentIndex(CommentDTO commentDto, ICollection<CommentField> changedCommentFields)
		{
			_entityIndexer.UpdateCommentIndex(commentDto, changedCommentFields, Maybe.Nothing, Maybe.Nothing);
			_entityIndexer.OptimizeCommentIndex();
		}

		private void RemoveCommentIndex(CommentDTO commentDto)
		{
			_entityIndexer.RemoveCommentIndex(commentDto);
		}
	}
}
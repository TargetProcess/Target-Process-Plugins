using System;
using System.Xml.Serialization;

namespace Tp.Integration.Common
{ // ReSharper disable InconsistentNaming

	public partial class BuildDTO : IBuildDTO
	{
		[XmlIgnore]
		int? IGeneralNumericPriorityListItemDTO.EntityTypeID
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}

	public partial class BugDTO : IBugDTO
	{
		[XmlIgnore]
		int? IGeneralNumericPriorityListItemDTO.ParentProjectID
		{
			get { return ProjectID; }
			set { ProjectID = value; }
		}

		[XmlIgnore]
		string IGeneralDTO.ParentProjectName
		{
			get { return ProjectName; }
			set { ProjectName = value; }
		}
	}

	public partial class AssignableDTO : IAssignableDTO
	{
		[XmlIgnore]
		int? IGeneralNumericPriorityListItemDTO.ParentProjectID
		{
			get { return ProjectID; }
			set { ProjectID = value; }
		}

		[XmlIgnore]
		string IGeneralDTO.ParentProjectName
		{
			get { return ProjectName; }
			set { ProjectName = value; }
		}
	}

	public partial class TestPlanRunDTO : ITestPlanRunDTO
	{
		[XmlIgnore]
		int? IGeneralNumericPriorityListItemDTO.ParentProjectID
		{
			get { return ProjectID; }
			set { ProjectID = value; }
		}

		[XmlIgnore]
		string IGeneralDTO.ParentProjectName
		{
			get { return ProjectName; }
			set { ProjectName = value; }
		}
	}

	public partial class RequestDTO : IRequestDTO
	{
		[XmlIgnore]
		int? IGeneralNumericPriorityListItemDTO.ParentProjectID
		{
			get { return ProjectID; }
			set { ProjectID = value; }
		}

		[XmlIgnore]
		string IGeneralDTO.ParentProjectName
		{
			get { return ProjectName; }
			set { ProjectName = value; }
		}
	}

	public partial class UserStoryDTO : IUserStoryDTO
	{
		[XmlIgnore]
		int? IGeneralNumericPriorityListItemDTO.ParentProjectID
		{
			get { return ProjectID; }
			set { ProjectID = value; }
		}

		[XmlIgnore]
		string IGeneralDTO.ParentProjectName
		{
			get { return ProjectName; }
			set { ProjectName = value; }
		}
	}

	public partial class TaskDTO : ITaskDTO
	{
		[XmlIgnore]
		int? IGeneralNumericPriorityListItemDTO.ParentProjectID
		{
			get { return ProjectID; }
			set { ProjectID = value; }
		}

		[XmlIgnore]
		string IGeneralDTO.ParentProjectName
		{
			get { return ProjectName; }
			set { ProjectName = value; }
		}
	}

	public partial class AttachmentDTO : IAttachmentDTO
	{
	}

	public partial class ApplicationContextDataDTO : IApplicationContextDataDTO
	{
	}

	public partial class AttachmentFileDTO : IAttachmentFileDTO
	{
	}

	public partial class CommentDTO : ICommentDTO
	{
	}

	public partial class CompanyDTO : ICompanyDTO
	{
	}

	public partial class ContactDTO : IContactDTO
	{
	}

	public partial class GeneralFollowerDTO : IGeneralFollowerDTO
	{
	}

	public partial class TestStepDTO : ITestStepDTO
	{
	}

	public partial class TestStepRunDTO : ITestStepRunDTO
	{
	}

	public partial class CustomActivityDTO : ICustomActivityDTO
	{
	}

	public partial class CustomFieldDTO : ICustomFieldDTO
	{
	}

	public partial class CustomReportDTO : ICustomReportDTO
	{
	}

	public partial class EmailAttachmentDTO : IEmailAttachmentDTO
	{
	}

	public partial class EntityStateDTO : IEntityStateDTO
	{
	}

	public partial class EntityTypeDTO : IEntityTypeDTO
	{
	}

	public partial class ExternalReferenceDTO : IExternalReferenceDTO
	{
	}

	public partial class FeatureDTO : IFeatureDTO
	{
		int? IGeneralNumericPriorityListItemDTO.ParentProjectID
		{
			get { return ProjectID; }
			set { ProjectID = value; }
		}

		string IGeneralDTO.ParentProjectName
		{
			get { return ProjectName; }
			set { ProjectName = value; }
		}

		string IAssignableDTO.CommentOnChangingState
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}

	public partial class EpicDTO : IEpicDTO
	{
		int? IGeneralNumericPriorityListItemDTO.ParentProjectID
		{
			get { return ProjectID; }
			set { ProjectID = value; }
		}

		string IGeneralDTO.ParentProjectName
		{
			get { return ProjectName; }
			set { ProjectName = value; }
		}

		string IAssignableDTO.CommentOnChangingState
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}

	public partial class GeneralDTO : IGeneralDTO
	{
	}

	public partial class GeneralFieldExtensionDTO : IGeneralFieldExtensionDTO
	{
	}

	public partial class GeneralListItemDTO : IGeneralListItemDTO
	{
	}

	public partial class GeneralNumericPriorityListItemDTO : IGeneralNumericPriorityListItemDTO
	{
	}

	public partial class GeneralRelationDTO : IGeneralRelationDTO
	{
	}

	public partial class GeneralRelationTypeDTO : IGeneralRelationTypeDTO
	{
	}

	public partial class GeneralUserDTO : IGeneralUserDTO
	{
	}

	public partial class GlobalSettingDTO : IGlobalSettingDTO
	{
	}

	public partial class ImpedimentDTO : IImpedimentDTO
	{
		int? IGeneralNumericPriorityListItemDTO.ParentProjectID
		{
			get { return ProjectID; }
			set { ProjectID = value; }
		}

		string IGeneralDTO.ParentProjectName
		{
			get { return ProjectName; }
			set { ProjectName = value; }
		}
	}

	public partial class IterationDTO : IIterationDTO
	{
	}

	public partial class SquadIterationDTO : ISquadIterationDTO
	{
	}

	public partial class LicenseDTO : ILicenseDTO
	{
	}

	public partial class MessageDTO : IMessageDTO
	{
	}

	public partial class MessageGeneralDTO : IMessageGeneralDTO
	{
	}

	public partial class MessageUidDTO : IMessageUidDTO
	{
	}

	public partial class MilestoneDTO : IMilestoneDTO
	{
	}

	public partial class PasswordRecoveryDTO : IPasswordRecoveryDTO
	{
	}

	public partial class PluginProfileDTO : IPluginProfileDTO
	{
	}

	public partial class PluginProfileMessageDTO : IPluginProfileMessageDTO
	{
	}

	public partial class PracticeDTO : IPracticeDTO
	{
	}

	public partial class PriorityDTO : IPriorityDTO
	{
	}

	public partial class ProcessAdminDTO : IProcessAdminDTO
	{
	}

	public partial class ProcessDTO : IProcessDTO
	{
	}

	public partial class ProcessPracticeDTO : IProcessPracticeDTO
	{
	}

	public partial class ProgramDTO : IProgramDTO
	{
		[XmlIgnore]
		int? IGeneralNumericPriorityListItemDTO.EntityTypeID
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}

	public partial class ProjectDTO : IProjectDTO
	{
		[XmlIgnore]
		int? IGeneralNumericPriorityListItemDTO.EntityTypeID
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}

	public partial class ProjectMemberDTO : IProjectMemberDTO
	{
	}

	public partial class ReleaseDTO : IReleaseDTO
	{
		[XmlIgnore]
		int? IGeneralNumericPriorityListItemDTO.ParentProjectID
		{
			get { return ProjectID; }
			set { ProjectID = value; }
		}

		[XmlIgnore]
		string IGeneralDTO.ParentProjectName
		{
			get { return ProjectName; }
			set { ProjectName = value; }
		}
	}

	public partial class ReleaseProjectDTO : IReleaseProjectDTO
	{
	}

	public partial class RequesterDTO : IRequesterDTO
	{
	}

	public partial class RequestRequesterDTO : IRequestRequesterDTO
	{
	}

	public partial class RequestTypeDTO : IRequestTypeDTO
	{
	}

	public partial class RevisionAssignableDTO : IRevisionAssignableDTO
	{
	}

	public partial class RevisionDTO : IRevisionDTO
	{
	}

	public partial class RevisionFileDTO : IRevisionFileDTO
	{
	}

	public partial class RoleDTO : IRoleDTO
	{
	}

	public partial class RoleEffortDTO : IRoleEffortDTO
	{
	}

	public partial class RoleEntityTypeDTO : IRoleEntityTypeDTO
	{
	}

	public partial class RuleDTO : IRuleDTO
	{
	}

	public partial class SavedFilterDTO : ISavedFilterDTO
	{
	}

	public partial class SeverityDTO : ISeverityDTO
	{
	}

	public partial class SystemUserDTO : ISystemUserDTO
	{
	}

	public partial class TagBundleDTO : ITagBundleDTO
	{
	}

	public partial class TagBundleTagDTO : ITagBundleTagDTO
	{
	}

	public partial class TagDTO : ITagDTO
	{
	}

	public partial class TagGeneralDTO : ITagGeneralDTO
	{
	}

	public partial class TeamDTO : ITeamDTO
	{
	}

	public partial class TeamListItemDTO : ITeamListItemDTO
	{
	}

	public partial class TermDTO : ITermDTO
	{
	}

	public partial class TestCaseDTO : ITestCaseDTO
	{
		[XmlIgnore]
		int? IGeneralNumericPriorityListItemDTO.ParentProjectID
		{
			get { return ProjectID; }
			set { ProjectID = value; }
		}

		[XmlIgnore]
		string IGeneralDTO.ParentProjectName
		{
			get { return ProjectName; }
			set { ProjectName = value; }
		}
	}

	public partial class TestCaseRunDTO : ITestCaseRunDTO
	{
	}

	public partial class TestCaseTestPlanDTO : ITestCaseTestPlanDTO
	{
	}

	public partial class TestPlanDTO : ITestPlanDTO
	{
		[XmlIgnore]
		int? IGeneralNumericPriorityListItemDTO.ParentProjectID
		{
			get { return ProjectID; }
			set { ProjectID = value; }
		}

		[XmlIgnore]
		string IGeneralDTO.ParentProjectName
		{
			get { return ProjectName; }
			set { ProjectName = value; }
		}

		[XmlIgnore]
		string IAssignableDTO.CommentOnChangingState
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		[XmlIgnore]
		int? IAssignableDTO.PriorityID
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		[XmlIgnore]
		string IAssignableDTO.PriorityName
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		[XmlIgnore]
		decimal? IAssignableDTO.Progress
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}

	public partial class TimeDTO : ITimeDTO
	{
	}

	public partial class TpEventDTO : ITpEventDTO
	{
	}

	public partial class TpProfileDTO : ITpProfileDTO
	{
	}

	public partial class TpProjectProfileDTO : ITpProjectProfileDTO
	{
	}

	public partial class TpSessionDTO : ITpSessionDTO
	{
	}

	public partial class UserDTO : IUserDTO
	{
	}

	// ReSharper restore InconsistentNaming
}

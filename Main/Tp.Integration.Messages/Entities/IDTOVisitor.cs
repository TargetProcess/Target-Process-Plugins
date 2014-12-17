namespace Tp.Integration.Common
{
	public interface IDTOVisitor<T>
	{
		T VisitDataTransferObject(IDataTransferObject o);
		T VisitBuild(IBuildDTO build);
		T VisitBug(IBugDTO bug);
		T VisitAssignable(IAssignableDTO assignable);
		T VisitTestPlanRun(ITestPlanRunDTO testplanrun);
		T VisitRequest(IRequestDTO request);
		T VisitUserStory(IUserStoryDTO userstory);
		T VisitTask(ITaskDTO task);
		T VisitAttachment(IAttachmentDTO attachment);
		T VisitApplicationContextData(IApplicationContextDataDTO applicationcontextdata);
		T VisitAttachmentFile(IAttachmentFileDTO attachmentfile);
		T VisitComment(ICommentDTO comment);
		T VisitCompany(ICompanyDTO company);
		T VisitContact(IContactDTO contact);
		T VisitGeneralFollower(IGeneralFollowerDTO generalfollower);
		T VisitTestStep(ITestStepDTO teststep);
		T VisitTestStepRun(ITestStepRunDTO teststeprun);
		T VisitCustomActivity(ICustomActivityDTO customactivity);
		T VisitCustomField(ICustomFieldDTO customfield);
		T VisitCustomReport(ICustomReportDTO customreport);
		T VisitEmailAttachment(IEmailAttachmentDTO emailattachment);
		T VisitEntityState(IEntityStateDTO entitystate);
		T VisitEntityType(IEntityTypeDTO entitytype);
		T VisitExternalReference(IExternalReferenceDTO externalreference);
		T VisitFeature(IFeatureDTO feature);
		T VisitEpic(IEpicDTO epic);
		T VisitGeneral(IGeneralDTO general);
		T VisitGeneralFieldExtension(IGeneralFieldExtensionDTO generalfieldextension);
		T VisitGeneralListItem(IGeneralListItemDTO generallistitem);
		T VisitGeneralNumericPriorityListItem(IGeneralNumericPriorityListItemDTO generalnumericprioritylistitem);
		T VisitGeneralRelation(IGeneralRelationDTO generalrelation);
		T VisitGeneralRelationType(IGeneralRelationTypeDTO generalrelationtype);
		T VisitGeneralUser(IGeneralUserDTO generaluser);
		T VisitGlobalSetting(IGlobalSettingDTO globalsetting);
		T VisitImpediment(IImpedimentDTO impediment);
		T VisitIteration(IIterationDTO iteration);
		T VisitSquadIteration(ISquadIterationDTO iteration);
		T VisitLicense(ILicenseDTO license);
		T VisitMessage(IMessageDTO message);
		T VisitMessageGeneral(IMessageGeneralDTO messagegeneral);
		T VisitMessageUid(IMessageUidDTO messageuid);
		T VisitMilestone(IMilestoneDTO milestone);
		T VisitPasswordRecovery(IPasswordRecoveryDTO passwordrecovery);
		T VisitPluginProfile(IPluginProfileDTO pluginprofile);
		T VisitPluginProfileMessage(IPluginProfileMessageDTO pluginprofilemessage);
		T VisitPractice(IPracticeDTO practice);
		T VisitPriority(IPriorityDTO priority);
		T VisitProcessAdmin(IProcessAdminDTO processadmin);
		T VisitProcess(IProcessDTO process);
		T VisitProcessPractice(IProcessPracticeDTO processpractice);
		T VisitProgram(IProgramDTO program);
		T VisitProject(IProjectDTO project);
		T VisitProjectMember(IProjectMemberDTO projectmember);
		T VisitRelease(IReleaseDTO release);
		T VisitRequester(IRequesterDTO requester);
		T VisitRequestRequester(IRequestRequesterDTO requestrequester);
		T VisitRequestType(IRequestTypeDTO requesttype);
		T VisitRevisionAssignable(IRevisionAssignableDTO revisionassignable);
		T VisitRevision(IRevisionDTO revision);
		T VisitRevisionFile(IRevisionFileDTO revisionfile);
		T VisitRole(IRoleDTO role);
		T VisitRoleEffort(IRoleEffortDTO roleeffort);
		T VisitRoleEntityType(IRoleEntityTypeDTO roleentitytype);
		T VisitRule(IRuleDTO rule);
		T VisitSavedFilter(ISavedFilterDTO savedfilter);
		T VisitSeverity(ISeverityDTO severity);
		T VisitSystemUser(ISystemUserDTO systemuser);
		T VisitTagBundle(ITagBundleDTO tagbundle);
		T VisitTagBundleTag(ITagBundleTagDTO tagbundletag);
		T VisitTag(ITagDTO tag);
		T VisitTagGeneral(ITagGeneralDTO taggeneral);
		T VisitTeam(ITeamDTO team);
		T VisitTeamListItem(ITeamListItemDTO teamlistitem);
		T VisitTerm(ITermDTO term);
		T VisitTestCase(ITestCaseDTO testcase);
		T VisitTestCaseRun(ITestCaseRunDTO testcaserun);
		T VisitTestCaseTestPlan(ITestCaseTestPlanDTO testcasetestplan);
		T VisitTestPlan(ITestPlanDTO testplan);
		T VisitTime(ITimeDTO time);
		T VisitTpEvent(ITpEventDTO tpevent);
		T VisitTpProfile(ITpProfileDTO tpprofile);
		T VisitTpProjectProfile(ITpProjectProfileDTO tpprojectprofile);
		T VisitTpSession(ITpSessionDTO tpsession);
		T VisitUser(IUserDTO user);
	}

	public partial class BuildDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitBuild(this);
		}
	}
	public partial class BugDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitBug(this);
		}
	}
	public partial class AssignableDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitAssignable(this);
		}
	}
	public partial class TestPlanRunDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTestPlanRun(this);
		}
	}
	public partial class RequestDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitRequest(this);
		}
	}
	public partial class UserStoryDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitUserStory(this);
		}
	}
	public partial class TaskDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTask(this);
		}
	}
	public partial class AttachmentDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitAttachment(this);
		}
	}
	public partial class ApplicationContextDataDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitApplicationContextData(this);
		}
	}
	public partial class AttachmentFileDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitAttachmentFile(this);
		}
	}
	public partial class CommentDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitComment(this);
		}
	}
	public partial class CompanyDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitCompany(this);
		}
	}
	public partial class ContactDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitContact(this);
		}
	}
	public partial class GeneralFollowerDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitGeneralFollower(this);
		}
	}
	public partial class TestStepDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTestStep(this);
		}
	}
	public partial class TestStepRunDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTestStepRun(this);
		}
	}
	public partial class CustomActivityDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitCustomActivity(this);
		}
	}
	public partial class CustomFieldDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitCustomField(this);
		}
	}
	public partial class CustomReportDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitCustomReport(this);
		}
	}
	public partial class EmailAttachmentDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitEmailAttachment(this);
		}
	}
	public partial class EntityStateDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitEntityState(this);
		}
	}
	public partial class EntityTypeDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitEntityType(this);
		}
	}
	public partial class ExternalReferenceDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitExternalReference(this);
		}
	}
	public partial class FeatureDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitFeature(this);
		}
	}
	public partial class EpicDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitEpic(this);
		}
	}
	public partial class GeneralDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitGeneral(this);
		}
	}
	public partial class GeneralFieldExtensionDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitGeneralFieldExtension(this);
		}
	}
	public partial class GeneralListItemDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitGeneralListItem(this);
		}
	}
	public partial class GeneralNumericPriorityListItemDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitGeneralNumericPriorityListItem(this);
		}
	}
	public partial class GeneralRelationDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitGeneralRelation(this);
		}
	}
	public partial class GeneralRelationTypeDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitGeneralRelationType(this);
		}
	}
	public partial class GeneralUserDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitGeneralUser(this);
		}
	}
	public partial class GlobalSettingDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitGlobalSetting(this);
		}
	}
	public partial class ImpedimentDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitImpediment(this);
		}
	}
	public partial class IterationDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitIteration(this);
		}
	}

	public partial class SquadIterationDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitSquadIteration(this);
		}
	}
	public partial class LicenseDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitLicense(this);
		}
	}
	public partial class MessageDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitMessage(this);
		}
	}
	public partial class MessageGeneralDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitMessageGeneral(this);
		}
	}
	public partial class MessageUidDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitMessageUid(this);
		}
	}
	public partial class MilestoneDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitMilestone(this);
		}
	}
	public partial class PasswordRecoveryDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitPasswordRecovery(this);
		}
	}
	public partial class PluginProfileDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitPluginProfile(this);
		}
	}
	public partial class PluginProfileMessageDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitPluginProfileMessage(this);
		}
	}
	public partial class PracticeDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitPractice(this);
		}
	}
	public partial class PriorityDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitPriority(this);
		}
	}
	public partial class ProcessAdminDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitProcessAdmin(this);
		}
	}
	public partial class ProcessDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitProcess(this);
		}
	}
	public partial class ProcessPracticeDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitProcessPractice(this);
		}
	}
	public partial class ProgramDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitProgram(this);
		}
	}
	public partial class ProjectDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitProject(this);
		}
	}
	public partial class ProjectMemberDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitProjectMember(this);
		}
	}
	public partial class ReleaseDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitRelease(this);
		}
	}
	public partial class RequesterDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitRequester(this);
		}
	}
	public partial class RequestRequesterDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitRequestRequester(this);
		}
	}
	public partial class RequestTypeDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitRequestType(this);
		}
	}
	public partial class RevisionAssignableDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitRevisionAssignable(this);
		}
	}
	public partial class RevisionDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitRevision(this);
		}
	}
	public partial class RevisionFileDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitRevisionFile(this);
		}
	}
	public partial class RoleDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitRole(this);
		}
	}
	public partial class RoleEffortDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitRoleEffort(this);
		}
	}
	public partial class RoleEntityTypeDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitRoleEntityType(this);
		}
	}
	public partial class RuleDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitRule(this);
		}
	}
	public partial class SavedFilterDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitSavedFilter(this);
		}
	}
	public partial class SeverityDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitSeverity(this);
		}
	}
	public partial class SystemUserDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitSystemUser(this);
		}
	}
	public partial class TagBundleDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTagBundle(this);
		}
	}
	public partial class TagBundleTagDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTagBundleTag(this);
		}
	}
	public partial class TagDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTag(this);
		}
	}
	public partial class TagGeneralDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTagGeneral(this);
		}
	}
	public partial class TeamDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTeam(this);
		}
	}
	public partial class TeamListItemDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTeamListItem(this);
		}
	}
	public partial class TermDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTerm(this);
		}
	}
	public partial class TestCaseDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTestCase(this);
		}
	}
	public partial class TestCaseRunDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTestCaseRun(this);
		}
	}
	public partial class TestCaseTestPlanDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTestCaseTestPlan(this);
		}
	}
	public partial class TestPlanDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTestPlan(this);
		}
	}
	public partial class TimeDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTime(this);
		}
	}
	public partial class TpEventDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTpEvent(this);
		}
	}
	public partial class TpProfileDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTpProfile(this);
		}
	}
	public partial class TpProjectProfileDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTpProjectProfile(this);
		}
	}
	public partial class TpSessionDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitTpSession(this);
		}
	}
	public partial class UserDTO
	{
		public override T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitUser(this);
		}
	}
}
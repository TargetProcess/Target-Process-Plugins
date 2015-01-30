using System;

namespace Tp.Integration.Common
{
	public class DTOVisitor<T> : IDTOVisitor<T>
	{
		public virtual T VisitDataTransferObject(IDataTransferObject dto)
		{
			throw new System.NotImplementedException();
		}
		public virtual T VisitBuild(IBuildDTO build)
		{
			return VisitGeneral(build);
		}
		public virtual T VisitBug(IBugDTO bug)
		{
			return VisitAssignable(bug);
		}
		public virtual T VisitAssignable(IAssignableDTO assignable)
		{
			return VisitGeneral(assignable);
		}
		public virtual T VisitTestPlanRun(ITestPlanRunDTO testplanrun)
		{
			return VisitAssignable(testplanrun);
		}
		public virtual T VisitRequest(IRequestDTO request)
		{
			return VisitAssignable(request);
		}
		public virtual T VisitUserStory(IUserStoryDTO userstory)
		{
			return VisitAssignable(userstory);
		}
		public virtual T VisitTask(ITaskDTO task)
		{
			return VisitAssignable(task);
		}
		public virtual T VisitAttachment(IAttachmentDTO attachment)
		{
			return VisitDataTransferObject(attachment);
		}

		public virtual T VisitApplicationContextData(IApplicationContextDataDTO applicationcontextdata)
		{
			return VisitDataTransferObject(applicationcontextdata);
		}
		public virtual T VisitAttachmentFile(IAttachmentFileDTO attachmentfile)
		{
			return VisitDataTransferObject(attachmentfile);
		}
		public virtual T VisitComment(ICommentDTO comment)
		{
			return VisitDataTransferObject(comment);
		}
		public virtual T VisitCompany(ICompanyDTO company)
		{
			return VisitDataTransferObject(company);
		}
		public virtual T VisitContact(IContactDTO contact)
		{
			return VisitDataTransferObject(contact);
		}
		public virtual T VisitGeneralFollower(IGeneralFollowerDTO generalfollower)
		{
			return VisitDataTransferObject(generalfollower);
		}
		public virtual T VisitTestStep(ITestStepDTO teststep)
		{
			return VisitDataTransferObject(teststep);
		}
		public virtual T VisitTestStepRun(ITestStepRunDTO teststeprun)
		{
			return VisitDataTransferObject(teststeprun);
		}
		public virtual T VisitCustomActivity(ICustomActivityDTO customactivity)
		{
			return VisitDataTransferObject(customactivity);
		}
		public virtual T VisitCustomField(ICustomFieldDTO customfield)
		{
			return VisitDataTransferObject(customfield);
		}
		public virtual T VisitCustomReport(ICustomReportDTO customreport)
		{
			return VisitDataTransferObject(customreport);
		}
		public virtual T VisitEmailAttachment(IEmailAttachmentDTO emailattachment)
		{
			throw new NotImplementedException();
		}
		public virtual T VisitEntityState(IEntityStateDTO entitystate)
		{
			return VisitDataTransferObject(entitystate);
		}
		public virtual T VisitEntityType(IEntityTypeDTO entitytype)
		{
			return VisitDataTransferObject(entitytype);
		}
		public virtual T VisitExternalReference(IExternalReferenceDTO externalreference)
		{
			return VisitDataTransferObject(externalreference);
		}
		public virtual T VisitFeature(IFeatureDTO feature)
		{
			return VisitAssignable(feature);
		}
		public virtual T VisitEpic(IEpicDTO epic)
		{
			return VisitAssignable(epic);
		}
		public virtual T VisitGeneral(IGeneralDTO general)
		{
			return VisitGeneralFieldExtension(general);
		}
		public virtual T VisitGeneralFieldExtension(IGeneralFieldExtensionDTO generalfieldextension)
		{
			return VisitDataTransferObject(generalfieldextension);
		}
		public virtual T VisitGeneralListItem(IGeneralListItemDTO generallistitem)
		{
			return VisitDataTransferObject(generallistitem);
		}
		public virtual T VisitGeneralNumericPriorityListItem(IGeneralNumericPriorityListItemDTO generalnumericprioritylistitem)
		{
			return VisitDataTransferObject(generalnumericprioritylistitem);
		}
		public virtual T VisitGeneralRelation(IGeneralRelationDTO generalrelation)
		{
			return VisitDataTransferObject(generalrelation);
		}
		public virtual T VisitGeneralRelationType(IGeneralRelationTypeDTO generalrelationtype)
		{
			return VisitDataTransferObject(generalrelationtype);
		}
		public virtual T VisitGeneralUser(IGeneralUserDTO generaluser)
		{
			return VisitDataTransferObject(generaluser);
		}
		public virtual T VisitGlobalSetting(IGlobalSettingDTO globalsetting)
		{
			return VisitDataTransferObject(globalsetting);
		}
		public virtual T VisitImpediment(IImpedimentDTO impediment)
		{
			return VisitGeneral(impediment);
		}
		public virtual T VisitIteration(IIterationDTO iteration)
		{
			return VisitGeneral(iteration);
		}
		public virtual T VisitLicense(ILicenseDTO license)
		{
			return VisitDataTransferObject(license);
		}
		public virtual T VisitMessage(IMessageDTO message)
		{
			return VisitDataTransferObject(message);
		}
		public virtual T VisitMessageGeneral(IMessageGeneralDTO messagegeneral)
		{
			return VisitDataTransferObject(messagegeneral);
		}
		public virtual T VisitMessageUid(IMessageUidDTO messageuid)
		{
			return VisitDataTransferObject(messageuid);
		}
		public virtual T VisitMilestone(IMilestoneDTO milestone)
		{
			return VisitDataTransferObject(milestone);
		}
		public virtual T VisitPasswordRecovery(IPasswordRecoveryDTO passwordrecovery)
		{
			return VisitDataTransferObject(passwordrecovery);
		}
		public virtual T VisitPluginProfile(IPluginProfileDTO pluginprofile)
		{
			return VisitDataTransferObject(pluginprofile);
		}
		public virtual T VisitPluginProfileMessage(IPluginProfileMessageDTO pluginprofilemessage)
		{
			return VisitDataTransferObject(pluginprofilemessage);
		}
		public virtual T VisitPractice(IPracticeDTO practice)
		{
			return VisitDataTransferObject(practice);
		}
		public virtual T VisitPriority(IPriorityDTO priority)
		{
			return VisitDataTransferObject(priority);
		}
		public virtual T VisitProcessAdmin(IProcessAdminDTO processadmin)
		{
			return VisitDataTransferObject(processadmin);
		}
		public virtual T VisitProcess(IProcessDTO process)
		{
			return VisitDataTransferObject(process);
		}
		public virtual T VisitProcessPractice(IProcessPracticeDTO processpractice)
		{
			return VisitDataTransferObject(processpractice);
		}
		public virtual T VisitProgram(IProgramDTO program)
		{
			return VisitGeneral(program);
		}
		public virtual T VisitProject(IProjectDTO project)
		{
			return VisitGeneral(project);
		}
		public virtual T VisitProjectMember(IProjectMemberDTO projectmember)
		{
			return VisitDataTransferObject(projectmember);
		}
		public virtual T VisitRelease(IReleaseDTO release)
		{
			return VisitGeneral(release);
		}

		public T VisitReleaseProject(IReleaseProjectDTO releaseProject)
		{
			return VisitDataTransferObject(releaseProject);
		}

		public virtual T VisitRequester(IRequesterDTO requester)
		{
			return VisitDataTransferObject(requester);
		}
		public virtual T VisitRequestRequester(IRequestRequesterDTO requestrequester)
		{
			return VisitDataTransferObject(requestrequester);
		}
		public virtual T VisitRequestType(IRequestTypeDTO requesttype)
		{
			return VisitDataTransferObject(requesttype);
		}
		public virtual T VisitRevisionAssignable(IRevisionAssignableDTO revisionassignable)
		{
			return VisitDataTransferObject(revisionassignable);
		}
		public virtual T VisitRevision(IRevisionDTO revision)
		{
			return VisitDataTransferObject(revision);
		}
		public virtual T VisitRevisionFile(IRevisionFileDTO revisionfile)
		{
			return VisitDataTransferObject(revisionfile);
		}
		public virtual T VisitRole(IRoleDTO role)
		{
			return VisitDataTransferObject(role);
		}
		public virtual T VisitRoleEffort(IRoleEffortDTO roleeffort)
		{
			return VisitDataTransferObject(roleeffort);
		}
		public virtual T VisitRoleEntityType(IRoleEntityTypeDTO roleentitytype)
		{
			return VisitDataTransferObject(roleentitytype);
		}
		public virtual T VisitRule(IRuleDTO rule)
		{
			return VisitDataTransferObject(rule);
		}
		public virtual T VisitSavedFilter(ISavedFilterDTO savedfilter)
		{
			return VisitDataTransferObject(savedfilter);
		}
		public virtual T VisitSeverity(ISeverityDTO severity)
		{
			return VisitDataTransferObject(severity);
		}
		public virtual T VisitSystemUser(ISystemUserDTO systemuser)
		{
			return VisitDataTransferObject(systemuser);
		}
		public virtual T VisitTagBundle(ITagBundleDTO tagbundle)
		{
			return VisitDataTransferObject(tagbundle);
		}
		public virtual T VisitTagBundleTag(ITagBundleTagDTO tagbundletag)
		{
			return VisitDataTransferObject(tagbundletag);
		}
		public virtual T VisitTag(ITagDTO tag)
		{
			return VisitDataTransferObject(tag);
		}
		public virtual T VisitTagGeneral(ITagGeneralDTO taggeneral)
		{
			return VisitDataTransferObject(taggeneral);
		}
		public virtual T VisitTeam(ITeamDTO team)
		{
			return VisitDataTransferObject(team);
		}
		public virtual T VisitTeamListItem(ITeamListItemDTO teamlistitem)
		{
			return VisitDataTransferObject(teamlistitem);
		}
		public virtual T VisitTerm(ITermDTO term)
		{
			return VisitDataTransferObject(term);
		}
		public virtual T VisitTestCase(ITestCaseDTO testcase)
		{
			return VisitGeneral(testcase);
		}
		public virtual T VisitTestCaseRun(ITestCaseRunDTO testcaserun)
		{
			return VisitDataTransferObject(testcaserun);
		}
		public virtual T VisitTestCaseTestPlan(ITestCaseTestPlanDTO testcasetestplan)
		{
			return VisitDataTransferObject(testcasetestplan);
		}
		public virtual T VisitTestPlan(ITestPlanDTO testplan)
		{
			return VisitAssignable(testplan);
		}
		public virtual T VisitTime(ITimeDTO time)
		{
			return VisitDataTransferObject(time);
		}
		public virtual T VisitTpEvent(ITpEventDTO tpevent)
		{
			return VisitDataTransferObject(tpevent);
		}
		public virtual T VisitTpProfile(ITpProfileDTO tpprofile)
		{
			return VisitDataTransferObject(tpprofile);
		}
		public virtual T VisitTpProjectProfile(ITpProjectProfileDTO tpprojectprofile)
		{
			return VisitDataTransferObject(tpprojectprofile);
		}
		public virtual T VisitTpSession(ITpSessionDTO tpsession)
		{
			return VisitDataTransferObject(tpsession);
		}
		public virtual T VisitUser(IUserDTO user)
		{
			return VisitDataTransferObject(user);
		}

		public virtual T VisitSquadIteration(ISquadIterationDTO iteration)
		{
			return VisitGeneral(iteration);
		}
	}
}
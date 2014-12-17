using System;
using Tp.Integration.Common;
using Tp.Integration.Messages.Entities;

namespace Tp.Integration.Common
{
	// ReSharper disable InconsistentNaming
	// ReSharper disable ConvertNullableToShortForm
	// ReSharper disable RedundantNameQualifier
	public interface IBuildDTO : IGeneralDTO
	{
		Nullable<Int32> ProjectID { get; set; }
		String ProjectName { get; set; }
		Nullable<Int32> BuildID { get; set; }
		Nullable<DateTime> BuildDate { get; set; }
		Nullable<Int32> IterationID { get; set; }
		Nullable<Int32> ReleaseID { get; set; }
		String IterationName { get; set; }
		String ReleaseName { get; set; }
	}

	public interface IBugDTO : IAssignableDTO
	{
		Nullable<Int32> SeverityID { get; set; }
		string SeverityName { get; set; }
		Nullable<Int32> BuildID { get; set; }
		Nullable<Int32> UserStoryID { get; set; }
		string UserStoryName { get; set; }
		Nullable<Int32> FeatureID { get; set; } 
		string FeatureName { get; set; } 
	}

	public interface IAssignableDTO : IGeneralDTO
	{
		String CommentOnChangingState { get; set; }
		Nullable<Decimal> Effort { get; set; }
		Nullable<Decimal> EffortCompleted { get; set; }
		Nullable<Decimal> EffortToDo { get; set; }
		Nullable<Decimal> TimeSpent { get; set; }
		Nullable<Decimal> TimeRemain { get; set; }
		Nullable<Int32> EntityStateID { get; set; }
		Nullable<Int32> PriorityID { get; set; }
		Nullable<Int32> ProjectID { get; set; }
		Nullable<Int32> IterationID { get; set; }
		Nullable<Int32> ParentID { get; set; }
		Nullable<Int32> ReleaseID { get; set; }
		String EntityStateName { get; set; }
		String PriorityName { get; set; }
		String ProjectName { get; set; }
		String IterationName { get; set; }
		String ParentName { get; set; }
		String ReleaseName { get; set; }
		Nullable<Int32> SquadID { get; set; }
		String SquadName { get; set; }
		Nullable<DateTime> PlannedStartDate { get; set; }
		Nullable<DateTime> PlannedEndDate { get; set; }
		Nullable<Decimal> Progress { get; set; }
		Nullable<Int32> SquadIterationID { get; set; } 
		String SquadIterationName { get; set; } 
	}

	public interface ITestPlanRunDTO : IAssignableDTO
	{
		Nullable<Int32> TestPlanRunID { get; set; }
		Nullable<Int32> FailedCount { get; set; }
		Nullable<Int32> PassedCount { get; set; }
		Nullable<Int32> NotRunCount { get; set; }
		Nullable<Int32> TestPlanID { get; set; }
		Nullable<Int32> BuildID { get; set; }
		String TestPlanName { get; set; }
		String BuildName { get; set; }
	}

	public interface IRequestDTO : IAssignableDTO
	{
		Nullable<Int32> RequestID { get; set; }
		Nullable<RequestSourceEnum> SourceType { get; set; }
		Nullable<Boolean> IsPrivate { get; set; }
		Nullable<Boolean> IsReplied { get; set; }
		Nullable<Int32> RequestTypeID { get; set; }
		String RequestTypeName { get; set; }
	}

	public interface IUserStoryDTO : IAssignableDTO
	{
		Nullable<Int32> UserStoryID { get; set; }
		Nullable<Decimal> InitialEstimate { get; set; }
		Nullable<Int32> FeatureID { get; set; }
		String FeatureName { get; set; }
	}

	public interface ITaskDTO : IAssignableDTO
	{
		Nullable<Int32> TaskID { get; set; }

		Nullable<Int32> UserStoryID { get; set; }
		String UserStoryName { get; set; }
	}

	public interface IAttachmentDTO : IDataTransferObject
	{
		Nullable<Int64> FileSize { get; set; }
		Nullable<Int32> AttachmentID { get; set; }
		String OriginalFileName { get; set; }
		String UniqueFileName { get; set; }
		Nullable<DateTime> CreateDate { get; set; }
		String Description { get; set; }
		Nullable<Int32> GeneralID { get; set; }
		Nullable<Int32> MessageID { get; set; }
		Nullable<Int32> OwnerID { get; set; }
		Nullable<Int32> AttachmentFileID { get; set; }
		String GeneralName { get; set; }
		String GeneralType { get; set; }
	}

	public interface IApplicationContextDataDTO : IDataTransferObject
	{
		Nullable<Int32> ApplicationContextDataID { get; set; }
		String Hash { get; set; }
		String Data { get; set; }
	}

	public interface IAttachmentFileDTO : IDataTransferObject
	{

		Nullable<Int32> AttachmentFileID { get; set; }
		String UniqueFileName { get; set; }
		Byte[] Buffer { get; set; }
	}

	public interface ICommentDTO : IDataTransferObject
	{
		Nullable<Int32> CommentID { get; set; }
		String Description { get; set; }
		Nullable<DateTime> CreateDate { get; set; }
		Nullable<Int32> GeneralID { get; set; }
		Nullable<Int32> OwnerID { get; set; }
		Nullable<Int32> ParentID { get; set; }
		String GeneralName { get; set; }
		String GeneralType { get; set; }
	}

	public interface ICompanyDTO : IDataTransferObject
	{
		String CompanyName { get; set; }
		String CompanyUrl { get; set; }
		String Description { get; set; }
	}

	public interface IContactDTO : IDataTransferObject
	{
		Nullable<Int32> UserID { get; set; }
		String FirstName { get; set; }
		String LastName { get; set; }
		String Email { get; set; }
		String Login { get; set; }
		String Password { get; set; }
		Nullable<DateTime> CreateDate { get; set; }
		Nullable<DateTime> ModifyDate { get; set; }
		Nullable<DateTime> DeleteDate { get; set; }
		Nullable<Boolean> IsActive { get; set; }
		Nullable<Boolean> IsAdministrator { get; set; }
		String Skills { get; set; }
		Nullable<Int32> ContactOwnerID { get; set; }
	}

	public interface IGeneralFollowerDTO : IDataTransferObject
	{
		Nullable<Int32> GeneralFollowerID { get; set; }
		Nullable<Int32> UserID { get; set; }
		Nullable<Int32> GeneralID { get; set; }
	}

	public interface ITestStepDTO : IDataTransferObject
	{
		Nullable<Int32> TestStepID { get; set; }
		String Description { get; set; }
		String Result { get; set; }
		Nullable<Int32> RunOrder { get; set; }
		Nullable<Int32> TestCaseID { get; set; }
	}

	public interface ITestStepRunDTO : IDataTransferObject
	{

		Nullable<Int32> TestStepRunID { get; set; }
		Nullable<Boolean> Passed { get; set; }
		Nullable<Boolean> Runned { get; set; }
		Nullable<Int32> TestCaseRunID { get; set; }
		Nullable<Int32> TestStepID { get; set; }
	}

	public interface ICustomActivityDTO : IDataTransferObject
	{

		Nullable<Int32> CustomActivityID { get; set; }
		String Name { get; set; }
		Nullable<DateTime> CreateDate { get; set; }
		Nullable<Decimal> Estimate { get; set; }
		Nullable<Int32> ProjectID { get; set; }
		Nullable<Int32> UserID { get; set; }
		String ProjectName { get; set; }
	}

	public interface ICustomFieldDTO : IDataTransferObject
	{

		Nullable<Int32> CustomFieldID { get; set; }
		String Name { get; set; }
		String Value { get; set; }
		String EntityFieldName { get; set; }
		Nullable<Boolean> Required { get; set; }
		Nullable<FieldTypeEnum> FieldType { get; set; }
		Nullable<Boolean> EnabledForFilter { get; set; }
		Nullable<Int32> ProcessID { get; set; }
		String EntityTypeName { get; set; }
		String ProcessName { get; set; }
	}

	public interface ICustomReportDTO : IDataTransferObject
	{
		Nullable<Int32> CustomReportID { get; set; }
		String Name { get; set; }
		String ReportingEntityName { get; set; }
		Nullable<Boolean> IsPublic { get; set; }
		String Content { get; set; }
		Nullable<Int32> ProcessID { get; set; }
		Nullable<Int32> OwnerID { get; set; }
		String ProcessName { get; set; }
	}

	public interface IEmailAttachmentDTO 
	{
		Byte[] Buffer { get; set; }
		String Name { get; set; }
	}

	public interface IEntityStateDTO : IDataTransferObject
	{
		Nullable<Int32> EntityStateID { get; set; }
		String Name { get; set; }
		String NextStates { get; set; }
		Nullable<Boolean> Initial { get; set; }
		Nullable<Boolean> Final { get; set; }
		Nullable<Boolean> Planned { get; set; }
		Nullable<Boolean> RequiredComment { get; set; }
		Double NumericPriority { get; set; }
		Nullable<Int32> ProcessID { get; set; }
		Nullable<Int32> RoleID { get; set; }
		String ProcessName { get; set; }
		String EntityTypeName { get; set; }
		String RoleName { get; set; }
	}

	public interface IEntityTypeDTO : IDataTransferObject
	{
		Nullable<Int32> EntityTypeID { get; set; }
		String Name { get; set; }
		String Abbreviation { get; set; }
		Nullable<Boolean> IsAssignable { get; set; }
		Nullable<Boolean> IsOwnEffortAvailable { get; set; }
		Nullable<Boolean> IsGlobalSearchAvailable { get; set; }
		Nullable<Boolean> IsReleasePlannable { get; set; }
		Nullable<Boolean> IsIterationPlannable { get; set; }
		Nullable<Boolean> IsCustomFieldsHolder { get; set; }
		Nullable<Boolean> IsChildsContainer { get; set; }
		Nullable<Boolean> IsPrioritizable { get; set; }
		Nullable<Boolean> HasAuditHistory { get; set; }
		Nullable<Boolean> UnitInHourOnly { get; set; }
		Nullable<Boolean> HasInitialEstimate { get; set; }
		Nullable<Int32> PracticeID { get; set; }
		String PracticeName { get; set; }
	}

	public interface IExternalReferenceDTO : IDataTransferObject
	{
		Nullable<Int32> ExternalReferenceID { get; set; }
		Nullable<Int32> TpID { get; set; }
		String ExternalID { get; set; }
		Nullable<DateTime> CreateDate { get; set; }
		Nullable<Int32> PluginProfileID { get; set; }
		String EntityTypeName { get; set; }
	}

	public interface IFeatureDTO : IAssignableDTO
	{
		Nullable<Decimal> InitialEstimate { get; set; }
		Nullable<Int32> EpicID { get; set; }
		String EpicName { get; set; }
	}

	public interface IEpicDTO : IAssignableDTO
	{
		Nullable<Decimal> InitialEstimate { get; set; }
	}

	public interface IDataTransferObject
	{
		Nullable<Int32> ID { get; set; }

		T Accept<T>(IDTOVisitor<T> visitor);
	}

	public interface IGeneralDTO : IGeneralListItemDTO, IGeneralFieldExtensionDTO
	{
		String Description { get; set; }
		Nullable<DateTime> ModifyDate { get; set; }
		Nullable<DateTime> LastCommentDate { get; set; }
		Nullable<Int32> LastCommentUserID { get; set; }
		Nullable<Int32> OwnerID { get; set; }
		Nullable<Int32> LastEditorID { get; set; }
		String ParentProjectName { get; set; }

	}

	public interface IGeneralFieldExtensionDTO : IDataTransferObject
	{
		String CustomField1 { get; set; }
		String CustomField2 { get; set; }
		String CustomField3 { get; set; }
		String CustomField4 { get; set; }
		String CustomField5 { get; set; }
		String CustomField6 { get; set; }
		String CustomField7 { get; set; }
		String CustomField8 { get; set; }
		String CustomField9 { get; set; }
		String CustomField10 { get; set; }
		String CustomField11 { get; set; }
		String CustomField12 { get; set; }
		String CustomField13 { get; set; }
		String CustomField14 { get; set; }
		String CustomField15 { get; set; }
		String CustomField16 { get; set; }
		String CustomField17 { get; set; }
		String CustomField18 { get; set; }
		String CustomField19 { get; set; }
		String CustomField20 { get; set; }
		String CustomField21 { get; set; }
		String CustomField22 { get; set; }
		String CustomField23 { get; set; }
		String CustomField24 { get; set; }
		String CustomField25 { get; set; }
		String CustomField26 { get; set; }
		String CustomField27 { get; set; }
		String CustomField28 { get; set; }
		String CustomField29 { get; set; }
		String CustomField30 { get; set; }
		String CustomField31 { get; set; }
		String CustomField32 { get; set; }
		String CustomField33 { get; set; }
		String CustomField34 { get; set; }
		String CustomField35 { get; set; }
		String CustomField36 { get; set; }
		String CustomField37 { get; set; }
		String CustomField38 { get; set; }
		String CustomField39 { get; set; }
		String CustomField40 { get; set; }
		String CustomField41 { get; set; }
		String CustomField42 { get; set; }
		String CustomField43 { get; set; }
		String CustomField44 { get; set; }
		String CustomField45 { get; set; }
		String CustomField46 { get; set; }
		String CustomField47 { get; set; }
		String CustomField48 { get; set; }
		String CustomField49 { get; set; }
		String CustomField50 { get; set; }
		String CustomField51 { get; set; }
		String CustomField52 { get; set; }
		String CustomField53 { get; set; }
		String CustomField54 { get; set; }
		String CustomField55 { get; set; }
		String CustomField56 { get; set; }
		String CustomField57 { get; set; }
		String CustomField58 { get; set; }
		String CustomField59 { get; set; }
		String CustomField60 { get; set; }
		String EntityTypeName { get; set; }
		Field[] CustomFieldsMetaInfo { get; set; }
	}

	public interface IGeneralListItemDTO : IGeneralNumericPriorityListItemDTO
	{
		String Name { get; set; }
		Nullable<DateTime> CreateDate { get; set; }
		Nullable<DateTime> StartDate { get; set; }
		Nullable<DateTime> EndDate { get; set; }
	}

	public interface IGeneralNumericPriorityListItemDTO : IDataTransferObject
	{
		Nullable<Int32> ParentProjectID { get; set; }
		Nullable<Double> NumericPriority { get; set; }
		Nullable<Int32> EntityTypeID { get; set; }
	}

	public interface IGeneralRelationDTO : IDataTransferObject
	{
		Nullable<Int32> GeneralRelationID { get; set; }
		Nullable<Int32> MasterID { get; set; }
		Nullable<Int32> SlaveID { get; set; }
		Nullable<Int32> GeneralRelationTypeID { get; set; }
	}

	public interface IGeneralRelationTypeDTO : IDataTransferObject
	{
		String Name { get; set; }
		Nullable<Int32> GeneralRelationTypeID { get; set; }
	}

	public interface IGeneralUserDTO : IDataTransferObject
	{
		Nullable<Int32> UserID { get; set; }
		String FirstName { get; set; }
		String LastName { get; set; }
		String Email { get; set; }
		String Login { get; set; }
		String Password { get; set; }
		Nullable<DateTime> CreateDate { get; set; }
		Nullable<DateTime> ModifyDate { get; set; }
		Nullable<DateTime> DeleteDate { get; set; }
		Nullable<Boolean> IsActive { get; set; }
		Nullable<Boolean> IsAdministrator { get; set; }
		String Skills { get; set; }
	}

	public interface IGlobalSettingDTO : IDataTransferObject
	{
		Nullable<Int32> GlobalSettingID { get; set; }
		String CompanyName { get; set; }
		Byte[] Logo { get; set; }
		String SMTPServer { get; set; }
		Nullable<Int32> SMTPPort { get; set; }
		String SMTPLogin { get; set; }
		String SMTPPassword { get; set; }
		Nullable<Boolean> SMTPAuthentication { get; set; }
		String SMTPSender { get; set; }
		Nullable<Boolean> IsEmailNotificationsEnabled { get; set; }
		String HelpDeskPortalPath { get; set; }
		String AppHostAndPath { get; set; }
		Nullable<Boolean> NotifyRequester { get; set; }
		Nullable<Boolean> NotifyAutoCreatedRequester { get; set; }
		Nullable<Boolean> DisableHttpAccess { get; set; }
		String CsvExportDelimiter { get; set; }
	}

	public interface IImpedimentDTO : IGeneralDTO
	{
		Nullable<Int32> ImpedimentID { get; set; }
		
		Nullable<Boolean> IsPrivate { get; set; }
		
		Nullable<Int32> AssignableID { get; set; }
		Nullable<Int32> EntityStateID { get; set; }
		Nullable<Int32> PriorityID { get; set; }
		Nullable<Int32> ProjectID { get; set; }
		Nullable<Int32> ResponsibleID { get; set; }
		String ResponsibleName { get; set; }
	
		String AssignableName { get; set; }
		String EntityStateName { get; set; }
		String PriorityName { get; set; }
		String ProjectName { get; set; }
		
		Nullable<DateTime> PlannedStartDate { get; set; }
		Nullable<DateTime> PlannedEndDate { get; set; }
		
	}

	public interface IIterationDTO : IGeneralDTO
	{
		Nullable<Decimal> Velocity { get; set; }
		Nullable<Int32> Duration { get; set; }

		Nullable<Int32> ReleaseID { get; set; }


		String ReleaseName { get; set; }

		Nullable<Decimal> Progress { get; set; }
	}

	public interface ISquadIterationDTO : IGeneralDTO
	{
		Nullable<Decimal> Velocity { get; set; }
		Nullable<Int32> Duration { get; set; }
		Nullable<Decimal> Progress { get; set; }
	}

	public interface ILicenseDTO : IDataTransferObject
	{
		Nullable<Int32> LicenseID { get; set; }
		String LicenseKey { get; set; }
		Nullable<DateTime> LastUpdateDate { get; set; }
		Byte[] Content { get; set; }
	}

	public interface IMessageDTO : IDataTransferObject
	{

		Nullable<Int32> MessageID { get; set; }
		String Subject { get; set; }
		String Recipients { get; set; }
		Nullable<Boolean> IsRead { get; set; }
		Nullable<Boolean> IsProcessed { get; set; }
		String Body { get; set; }
		Nullable<DateTime> CreateDate { get; set; }
		Nullable<DateTime> SendDate { get; set; }
		Nullable<MessageTypeEnum> MessageType { get; set; }
		Nullable<ContentTypeEnum> ContentType { get; set; }
		Nullable<Int32> FromID { get; set; }
		Nullable<Int32> ToID { get; set; }
		Nullable<Int32> MessageUidID { get; set; }
		MessageUidDTO MessageUidDto { get; set; }
	}

	public interface IMessageGeneralDTO : IDataTransferObject
	{

		Nullable<Int32> MessageGeneralID { get; set; }
		Nullable<Int32> MessageID { get; set; }
		Nullable<Int32> GeneralID { get; set; }
		String GeneralName { get; set; }
	}

	public interface IMessageUidDTO : IDataTransferObject
	{

		Nullable<Int32> MessageUidID { get; set; }
		String UID { get; set; }
		String MailServer { get; set; }
		String MailLogin { get; set; }
	}

	public interface IMilestoneDTO : IDataTransferObject
	{
		Nullable<Int32> MilestoneId { get; set; }
		String Name { get; set; }
		String Description { get; set; }
		DateTime Date { get; set; }
		String CssClass { get; set; }

	}

	public interface IPasswordRecoveryDTO : IDataTransferObject
	{

		Nullable<Int32> PasswordRecoveryID { get; set; }
		String ActivationKey { get; set; }
		Nullable<DateTime> ActivateDate { get; set; }
		Nullable<Int32> UserID { get; set; }
	}

	public interface IPluginProfileDTO : IDataTransferObject
	{
	
		Nullable<Int32> PluginProfileID { get; set; }
		String PluginName { get; set; }
		String ProfileName { get; set; }
		Nullable<DateTime> SyncDate { get; set; }
		Nullable<Boolean> Active { get; set; }
		String Settings { get; set; }
	}

	public interface IPluginProfileMessageDTO : IDataTransferObject
	{

		Nullable<Int32> PluginProfileMessageID { get; set; }
		Nullable<DateTime> CreateDate { get; set; }
		Nullable<Boolean> Successfull { get; set; }
		String Message { get; set; }
		Nullable<Int32> PluginProfileID { get; set; }
	}

	public interface IPracticeDTO : IDataTransferObject
	{

		Nullable<Int32> PracticeID { get; set; }
		String Name { get; set; }
		String Description { get; set; }
		Nullable<Boolean> AlwaysOn { get; set; }
		String DisplayName { get; set; }
	}

	public interface IPriorityDTO : IDataTransferObject
	{

		Nullable<Int32> PriorityID { get; set; }
		String Name { get; set; }
		Nullable<Int32> Importance { get; set; }
		Nullable<Boolean> IsDefault { get; set; }
		String EntityTypeName { get; set; }
	}

	public interface IProcessAdminDTO : IDataTransferObject
	{

		Nullable<Int32> ProcessAdminID { get; set; }
		Nullable<Int32> UserID { get; set; }
		Nullable<Int32> ProcessID { get; set; }
	}

	public interface IProcessDTO : IDataTransferObject
	{

		Nullable<Int32> ProcessID { get; set; }
		String Name { get; set; }
		Nullable<Boolean> IsDefault { get; set; }
		String Description { get; set; }
	}

	public interface IProcessPracticeDTO : IDataTransferObject
	{
		Nullable<Int32> ProcessPracticeID { get; set; }
		String Settings { get; set; }
		Nullable<Int32> ProcessID { get; set; }
		Nullable<Int32> PracticeID { get; set; }
		String ProcessName { get; set; }
		String PracticeName { get; set; }
	}

	public interface IProgramDTO : IGeneralDTO
	{
		Nullable<Int32> ProgramID { get; set; }
	}

	public interface IProjectDTO : IGeneralDTO
	{
		Nullable<Int32> ProjectID { get; set; }

		String Abbreviation { get; set; }

		Nullable<Boolean> IsActive { get; set; }
		Nullable<DateTime> DeleteDate { get; set; }
		Nullable<SourceControlTypeEnum> SourceControlType { get; set; }
		String SCConnectionString { get; set; }
		String SCUser { get; set; }
		String SCPassword { get; set; }
		Nullable<Int32> SCStartingRevision { get; set; }
		Nullable<Boolean> IsProduct { get; set; }
		Nullable<Boolean> InboundMailCreateRequests { get; set; }
		Nullable<Int32> InboundMailAutomaticalEmailCheckTime { get; set; }
		Nullable<Boolean> IsInboundMailEnabled { get; set; }
		String InboundMailReplyAddress { get; set; }
		Nullable<Boolean> InboundMailAutoCheck { get; set; }
		String InboundMailServer { get; set; }
		Nullable<Int32> InboundMailPort { get; set; }
		Nullable<Boolean> InboundMailUseSSL { get; set; }
		String InboundMailLogin { get; set; }
		String InboundMailPassword { get; set; }
		String InboundMailProtocol { get; set; }

		Nullable<Int32> ProgramOfProjectID { get; set; }
		Nullable<Int32> ProcessID { get; set; }
		Nullable<Int32> CompanyID { get; set; }

		String ProgramOfProjectName { get; set; }
		String ProcessName { get; set; }
		Nullable<Decimal> Progress { get; set; }
	}

	public interface IProjectMemberDTO : IDataTransferObject
	{

		Nullable<Int32> ProjectMemberID { get; set; }
		Nullable<Decimal> WeeklyAvailableHours { get; set; }
		Nullable<DateTime> MembershipEndDate { get; set; }
		Nullable<Int32> Allocation { get; set; }
		Nullable<Int32> ProjectID { get; set; }
		Nullable<Int32> UserID { get; set; }
		Nullable<Int32> RoleID { get; set; }
		String ProjectName { get; set; }
		String RoleName { get; set; }
		Nullable<DateTime> MembershipStartDate { get; set; }
	}

	public interface IReleaseDTO : IGeneralDTO
	{
	}

	public interface IRequesterDTO : IDataTransferObject
	{
	
		Nullable<Int32> UserID { get; set; }
		String FirstName { get; set; }
		String LastName { get; set; }
		String Email { get; set; }
		String Login { get; set; }
		String Password { get; set; }
		Nullable<DateTime> CreateDate { get; set; }
		Nullable<DateTime> ModifyDate { get; set; }
		Nullable<DateTime> DeleteDate { get; set; }
		Nullable<Boolean> IsActive { get; set; }
		Nullable<Boolean> IsAdministrator { get; set; }
		String Skills { get; set; }
		String Phone { get; set; }
		String Notes { get; set; }
		Nullable<RequesterSourceTypeEnum> SourceType { get; set; }
		Nullable<Int32> CompanyID { get; set; }
	}

	public interface IRequestRequesterDTO : IDataTransferObject
	{

		Nullable<Int32> RequestRequesterID { get; set; }
		Nullable<Int32> RequestID { get; set; }
		Nullable<Int32> RequesterID { get; set; }
		String RequestName { get; set; }
	}

	public interface IRequestTypeDTO : IDataTransferObject
	{


		Nullable<Int32> RequestTypeID { get; set; }
		String Name { get; set; }
	}

	public interface IRevisionAssignableDTO : IDataTransferObject
	{

		Nullable<Int32> RevisionAssignableID { get; set; }
		Nullable<Int32> AssignableID { get; set; }
		Nullable<Int32> RevisionID { get; set; }
		String AssignableName { get; set; }
	}

	public interface IRevisionDTO : IDataTransferObject
	{

		Nullable<Int32> RevisionID { get; set; }
		String SourceControlID { get; set; }
		Nullable<DateTime> CommitDate { get; set; }
		Nullable<Int32> PluginProfileID { get; set; }
		String Description { get; set; }
		Nullable<Int32> ProjectID { get; set; }
		Nullable<Int32> AuthorID { get; set; }
		String ProjectName { get; set; }
	}

	public interface IRevisionFileDTO : IDataTransferObject
	{

		Nullable<Int32> RevisionFileID { get; set; }
		String FileName { get; set; }
		Nullable<FileActionEnum> FileAction { get; set; }
		Nullable<Int32> RevisionID { get; set; }
	}

	public interface IRoleDTO : IDataTransferObject
	{
		Nullable<Int32> RoleID { get; set; }
		String Name { get; set; }
		String Description { get; set; }
		Nullable<Boolean> TimeSheetAccess { get; set; }
		Nullable<Boolean> PeopleAccess { get; set; }
		Nullable<Boolean> PersonalEmailAccess { get; set; }
		Nullable<Boolean> HaveEffort { get; set; }
		Nullable<Boolean> CanChangeOwner { get; set; }
	}

	public interface IRoleEffortDTO : IDataTransferObject
	{
	
		Nullable<Int32> RoleEffortID { get; set; }
		Nullable<Decimal> InitialEstimate { get; set; }
		Nullable<Decimal> Effort { get; set; }
		Nullable<Decimal> EffortCompleted { get; set; }
		Nullable<Decimal> EffortToDo { get; set; }
		Nullable<Decimal> TimeSpent { get; set; }
		Nullable<Decimal> TimeRemain { get; set; }
		Nullable<Boolean> SubstractionFromParentEffort { get; set; }
		Nullable<Int32> AssignableID { get; set; }
		Nullable<Int32> RoleID { get; set; }
		String AssignableName { get; set; }
		String RoleName { get; set; }
	}

	public interface IRoleEntityTypeDTO : IDataTransferObject
	{

		Nullable<Int32> RoleEntityTypeID { get; set; }
		Nullable<Boolean> IsDeleteEnabled { get; set; }
		Nullable<Boolean> IsEditEnabled { get; set; }
		Nullable<Int32> RoleID { get; set; }
		String EntityTypeName { get; set; }
		String RoleName { get; set; }
	}

	public interface IRuleDTO : IDataTransferObject
	{
		Nullable<Int32> RuleID { get; set; }
		String AdditionalInfo { get; set; }
		String RuleClass { get; set; }
		Nullable<RoleExtensionEnum> RoleExtension { get; set; }
		Nullable<Int32> RoleID { get; set; }
		Nullable<Int32> TpEventID { get; set; }
		String RoleName { get; set; }
	}

	public interface ISavedFilterDTO: IDataTransferObject{
		Nullable<Int32> SavedFilterID { get; set; }
		String Name { get; set; }
		String Description { get; set; }
		String FilterType { get; set; }
		Nullable<Boolean> IsDefault { get; set; }
		Nullable<Boolean> IsPublic { get; set; }
		String QueryFilter { get; set; }
		Nullable<Int32> ProjectID { get; set; }
		Nullable<Int32> OwnerID { get; set; }
	}

	public interface ISeverityDTO : IDataTransferObject
	{
		Nullable<Int32> SeverityID { get; set; }
		String Name { get; set; }
		Nullable<Int32> Importance { get; set; }
		Nullable<Boolean> IsDefault { get; set; }
	}

	public interface ISystemUserDTO : IDataTransferObject
	{
		Nullable<Int32> UserID { get; set; }
		String FirstName { get; set; }
		String LastName { get; set; }
		String Email { get; set; }
		String Login { get; set; }
		String Password { get; set; }
		Nullable<DateTime> CreateDate { get; set; }
		Nullable<DateTime> ModifyDate { get; set; }
		Nullable<DateTime> DeleteDate { get; set; }
		Nullable<Boolean> IsActive { get; set; }
		Nullable<Boolean> IsAdministrator { get; set; }
		String Skills { get; set; }
	}

	public interface ITagBundleDTO : IDataTransferObject
	{
		Nullable<Int32> TagBundleID { get; set; }
		String Name { get; set; }
		Nullable<Boolean> Exclusive { get; set; }
		Nullable<Int32> ProjectID { get; set; }
		String ProjectName { get; set; }
	}

	public interface ITagBundleTagDTO : IDataTransferObject
	{
		Nullable<Int32> TagBundleTagID { get; set; }
		Nullable<Int32> TagBundleID { get; set; }
		Nullable<Int32> TagID { get; set; }
		String TagBundleName { get; set; }
		String TagName { get; set; }
	}

	public interface ITagDTO : IDataTransferObject
	{
		Nullable<Int32> TagID { get; set; }
		String Name { get; set; }
	}

	public interface ITagGeneralDTO : IDataTransferObject
	{
		Nullable<Int32> TagGeneralID { get; set; }
		Nullable<Int32> GeneralID { get; set; }
		Nullable<Int32> TagID { get; set; }
		String GeneralName { get; set; }
		String TagName { get; set; }
	}

	public interface ITeamDTO : IDataTransferObject
	{
		Nullable<Int32> TeamID { get; set; }
		Nullable<Int32> AssignableID { get; set; }
		Nullable<Int32> UserID { get; set; }
		Nullable<Int32> RoleID { get; set; }
		String AssignableName { get; set; }
		String RoleName { get; set; }
		String AssignableType { get; set; }
	}

	public interface ITeamListItemDTO : IDataTransferObject
	{
		Nullable<Int32> TeamID { get; set; }
		Nullable<Int32> RoleID { get; set; }
		Nullable<Int32> UserID { get; set; }
		Nullable<Int32> AssignableID { get; set; }
	}

	public interface ITermDTO : IDataTransferObject
	{
		Nullable<Int32> TermID { get; set; }
		String WordKey { get; set; }
		String Value { get; set; }
		Nullable<Int32> EntityTypeID { get; set; }
		Nullable<Int32> ProcessID { get; set; }
		String ProcessName { get; set; }
	}

	public interface ITestCaseDTO : IGeneralDTO
	{
		Nullable<Int32> TestCaseID { get; set; }

		String Steps { get; set; }
		String Success { get; set; }
		Nullable<Boolean> LastStatus { get; set; }
		String LastFailureComment { get; set; }
		Nullable<DateTime> LastRunDate { get; set; }

		Nullable<Int32> UserStoryID { get; set; }
		Nullable<Int32> ProjectID { get; set; }
		Nullable<Int32> PriorityID { get; set; }

		String UserStoryName { get; set; }
		String ProjectName { get; set; }
		String PriorityName { get; set; }
	}

	public interface ITestCaseRunDTO : IDataTransferObject
	{
		Nullable<Int32> TestCaseRunID { get; set; }
		Nullable<DateTime> RunDate { get; set; }
		Nullable<Boolean> Passed { get; set; }
		Nullable<Boolean> Runned { get; set; }
		String Comment { get; set; }
		Nullable<Int32> TestPlanRunID { get; set; }
		Nullable<Int32> TestCaseTestPlanID { get; set; }
		String TestPlanRunName { get; set; }
	}

	public interface ITestCaseTestPlanDTO : IDataTransferObject
	{
		Nullable<Int32> TestCaseTestPlanID { get; set; }
		Nullable<Int32> TestPlanID { get; set; }
		Nullable<Int32> TestCaseID { get; set; }
		String TestPlanName { get; set; }
		String TestCaseName { get; set; }
	}

	public interface ITestPlanDTO : IAssignableDTO
	{
		Nullable<Int32> TestPlanID { get; set; }

		Nullable<Decimal> InitialEstimate { get; set; }
	}

	public interface ITimeDTO : IDataTransferObject
	{
		Nullable<Int32> TimeID { get; set; }
		String Description { get; set; }
		Nullable<Decimal> Spent { get; set; }
		Nullable<Decimal> Remain { get; set; }
		Nullable<DateTime> CreateDate { get; set; }
		Nullable<DateTime> SpentDate { get; set; }
		Nullable<Boolean> Estimation { get; set; }
		String CustomField1 { get; set; }
		String CustomField2 { get; set; }
		String CustomField3 { get; set; }
		String CustomField4 { get; set; }
		String CustomField5 { get; set; }
		String CustomField6 { get; set; }
		String CustomField7 { get; set; }
		String CustomField8 { get; set; }
		String CustomField9 { get; set; }
		String CustomField10 { get; set; }
		String CustomField11 { get; set; }
		String CustomField12 { get; set; }
		String CustomField13 { get; set; }
		String CustomField14 { get; set; }
		String CustomField15 { get; set; }
		String CustomField16 { get; set; }
		String CustomField17 { get; set; }
		String CustomField18 { get; set; }
		String CustomField19 { get; set; }
		String CustomField20 { get; set; }
		String CustomField21 { get; set; }
		String CustomField22 { get; set; }
		String CustomField23 { get; set; }
		String CustomField24 { get; set; }
		String CustomField25 { get; set; }
		String CustomField26 { get; set; }
		String CustomField27 { get; set; }
		String CustomField28 { get; set; }
		String CustomField29 { get; set; }
		String CustomField30 { get; set; }
		String CustomField31 { get; set; }
		String CustomField32 { get; set; }
		String CustomField33 { get; set; }
		String CustomField34 { get; set; }
		String CustomField35 { get; set; }
		String CustomField36 { get; set; }
		String CustomField37 { get; set; }
		String CustomField38 { get; set; }
		String CustomField39 { get; set; }
		String CustomField40 { get; set; }
		String CustomField41 { get; set; }
		String CustomField42 { get; set; }
		String CustomField43 { get; set; }
		String CustomField44 { get; set; }
		String CustomField45 { get; set; }
		String CustomField46 { get; set; }
		String CustomField47 { get; set; }
		String CustomField48 { get; set; }
		String CustomField49 { get; set; }
		String CustomField50 { get; set; }
		String CustomField51 { get; set; }
		String CustomField52 { get; set; }
		String CustomField53 { get; set; }
		String CustomField54 { get; set; }
		String CustomField55 { get; set; }
		String CustomField56 { get; set; }
		String CustomField57 { get; set; }
		String CustomField58 { get; set; }
		String CustomField59 { get; set; }
		String CustomField60 { get; set; }
		Nullable<Int32> ProjectID { get; set; }
		Nullable<Int32> UserID { get; set; }
		Nullable<Int32> AssignableID { get; set; }
		Nullable<Int32> CustomActivityID { get; set; }
		Nullable<Int32> RoleID { get; set; }
		String ProjectName { get; set; }
		String AssignableName { get; set; }
		String CustomActivityName { get; set; }
		String RoleName { get; set; }
		Field[] CustomFieldsMetaInfo { get; set; }
	}

	public interface ITpEventDTO : IDataTransferObject
	{
		Nullable<Int32> TpEventID { get; set; }
		Nullable<ActionTypeEnum> ActionType { get; set; }
		String EntityTypeName { get; set; }
		Nullable<Int32> EntityStateID { get; set; }
		String StateName { get; set; }
	}

	public interface ITpProfileDTO : IDataTransferObject
	{
		Nullable<Int32> ProfileID { get; set; }
		String PropertyName { get; set; }
		String PropertyValue { get; set; }
		Nullable<Int32> UserID { get; set; }
	}

	public interface ITpProjectProfileDTO : IDataTransferObject
	{
		Nullable<Int32> ProjectProfileID { get; set; }
		String PropertyName { get; set; }
		String PropertyValue { get; set; }
		Nullable<Int32> ProjectID { get; set; }
		String ProjectName { get; set; }
	}

	public interface ITpSessionDTO : IDataTransferObject
	{
		Nullable<Int32> TpSessionID { get; set; }
		Nullable<Int32> UserID { get; set; }
		Nullable<Boolean> IsCompressed { get; set; }
		String ObjKey { get; set; }
		Byte[] Buffer { get; set; }
		Nullable<DateTime> ModifyDate { get; set; }
	}

	public interface IUserDTO : IGeneralUserDTO
	{
		String ActiveDirectoryName { get; set; }
		Nullable<Decimal> WeeklyAvailableHours { get; set; }
		Nullable<Int32> CurrentAllocation { get; set; }
		Nullable<Decimal> CurrentAvailableHours { get; set; }
		Nullable<DateTime> AvailableFrom { get; set; }
		Nullable<Int32> AvailableFutureAllocation { get; set; }
		Nullable<Decimal> AvailableFutureHours { get; set; }
		Nullable<Boolean> IsObserver { get; set; }
		Nullable<Int32> RoleID { get; set; }
		String DefaultRoleName { get; set; }
	}
}

// ReSharper restore ConvertNullableToShortForm
// ReSharper restore RedundantNameQualifier
// ReSharper restore InconsistentNaming
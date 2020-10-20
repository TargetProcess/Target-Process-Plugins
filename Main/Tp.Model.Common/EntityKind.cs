using System.ComponentModel;

// ReSharper disable once CheckNamespace

namespace Tp.BusinessObjects
{
    public enum EntityKind
    {
        None,
        Term,
        ProjectMember,
        MessageUid,
        Comment,
        PluginProfileMessage,
        GeneralListItem,
        Message,
        RevisionAssignable,
        MessageGeneral,
        EntityState,
        Severity,
        AttachmentFile,
        CustomActivity,
        Attachment,
        CustomField,
        TpEvent,
        RevisionFile,
        TpProfile,
        TestCaseTestPlan,
        Tag,
        Company,
        RoleEffort,
        GeneralNumericPriorityListItem,
        RequestType,
        ExternalReference,
        GlobalSetting,
        Priority,
        License,
        EntityType,
        GeneralUser,
        User,
        SystemUser,
        Requester,
        Contact,
        Revision,
        PluginProfile,
        CustomReport,
        General,
        Build,
        Assignable,
        UserStory,
        TestPlanRun,
        Task,
        Request,
        Feature,
        Bug,
        TestPlan,
        TestCase,
        Release,
        Project,
        Program,
        Iteration,
        Impediment,
        ProcessPractice,
        PasswordRecovery,
        TestCaseRun,
        TeamListItem,
        [Description("Assignment")] Team,
        GeneralFieldExtension,
        Rule,
        TpSession,
        Process,
        Time,
        Role,
        RequestRequester,
        TagGeneral,
        Practice,
        RoleEntityType,
        TpProjectProfile,
        Milestone,
        [Description("Team")] Squad,
        [Description("TeamIteration")] SquadIteration,
        [Description("TeamMember")] SquadMember,
        [Description("TeamProject")] SquadProject,
        ReleaseProject,
        GeneralRelationType,
        GeneralRelation,
        ProcessAdmin,
        TestStep,
        TestStepRun,
        TestPlanTestPlan,
        GeneralFollower,
        TestCaseRunBug,
        Epic,
        Workflow,
        [Description("TeamAssignment")] AssignableSquad,
        TestPlanRunBug,
        FreezedTestStepInfo,
        FreezedTestCaseInfo,
        CustomRule,
        SsoSettings,
        [Description("InboundRelation")] InboundAssignable,
        [Description("OutboundRelation")] OutboundAssignable,
        ProjectAllocation,
        UserProjectAllocation,
        [Description("TeamProjectAllocation")] SquadProjectAllocation,
        TestRunItemHierarchyLink,
        GeneralConversion,
        PortfolioEpic,
        EntityPermission,
        RoleEntityTypeProcessSetting,
        MilestoneProject
    }
}
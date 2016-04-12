using System.ComponentModel;
// ReSharper disable InconsistentNaming

namespace Tp.Core.Features
{
	public enum TpFeature
	{
		None = 0,
		InboundEmailIntegration,
		LightEdition,
		[ClientFeature("tp3.only")] TP3OnlyMode,
		ProEdition,
		SwitchToProEdition,
		SecureJsonp,
		[ClientFeature("comet.notifications")] [Mashup("KanbanNotifications")] [Mashup("BoardNotifications")] ServerNotifications,
		SliceSpecificCases,
		NewPrioritization,
		ParallelExpanders,
		SliceCellFilterPropertiesAutoSet,
		ParallelSliceCalls,
		OptimizeForUnknownQueryHint,

		[Description(
			@"The solution to handle ""parameter sniffing"" based on assigning the stored procedure parameters to local variables and then using the local variables in the query."
			)] QueryWithLocalVariables,
		[ClientFeature("context.cacheresponse")] CacheContextResponse,
		[Description(@"Toggles possibility to save images and files added with CkEditor as Tp Attachment.")] [ClientFeature("ckFilesAsAttachments")] StoreCkEditorFilesAsAttachments,
		Diagnostics,
		TrackProgressInDb,
		FlattenInnerReport,
		[ClientFeature("board.cardBatchMoveVisualEffects")] CardBatchMoveVisualEffects,
		[ClientFeature("context.v2")] Context2,
		[ClientFeature("contextPrototype")] ContextPrototype,

		[OverrideableByMashup] Cdn,

		PerforcePlugin,
		DisableProtocolCheck,
		SkipSecurityCheckingForSecondLevelEntities,
		BatchFlushForTestCases,
		[ClientFeature("cache.axis")] CacheAxis,
		[ClientFeature("board.menu.comet")] ViewsMenuCometClient,
		ViewsMenuCometServer,
		LockAccountOnTransactionOperation,
		DisableTransactionalMsmqMessageProcessing,

		Follow,

		/// <summary>
		/// When enabled, old DTOs will be extended with original DTO value and author of changes
		/// </summary>
		ExtendedOldDto,

		/// <summary>
		/// Toggles client subscriptions on entity newlist comet hub
		/// </summary>
		[ClientFeature("entity.new.list.comet")] EntityNewListComet,

		/// <summary>
		/// Toggles visibility of dashboard in views menu
		/// </summary>
		[ClientFeature("dashboards")] Dashboards,

		/// <summary>
		/// Toggles logging for email notifications
		/// </summary>
		EmailNotificationsLogging,

		/// <summary>
		/// Toggles visibility of reports in views menu
		/// </summary>
		[ClientFeature("customReports")] CustomReports,

		/// <summary>
		/// Toggles possibility to see hierarchies of test plans and test plan runs in slice
		/// Does not toggle hierarchy in rest
		/// </summary>
		[ClientFeature("qa.area.hierarchy")] QaAreaHierarchy,

		/// <summary>
		/// Toggles returning total child count for a sub-tree of a List view.
		/// See US#90142 for details.
		/// </summary>
		[ClientFeature("new.list.deep.counts")] DeepListCounts,

		/// <summary>
		/// When enabled slice will use table instead of view for hierarchies of test plan runs
		/// </summary>
		UseTestRunItemTable,

		/// <summary>
		/// See US#92442 for details.
		/// </summary>
		[ClientFeature("rule.engine")] RuleEngine,

		/// <summary>
		/// Prefetch template data for general quick add on each context change. See US#95641 for details.
		/// </summary>
		[ClientFeature("general.quick.add.data.prefetch")] GeneralQuickAddDataPrefetch,

		/// <summary>
		/// See US#95444 for details.
		/// </summary>
		[ClientFeature("single.signon")] SingleSignOn,

		/// <summary>
		/// Enable visaul encoding for cards.
		/// </summary>
		VisualEncoding,

		/// <summary>
		/// Enables different order of test plan and test cases in different parent test plans. See US#84391.
		/// </summary>
		TestItemPrioritization,

		DoNotUseTableValueParameterInSqlQuery,

		/// <summary>
		/// Enable calculation of estimates over Test Plan hierarchy
		/// </summary>
		TestPlanHierarchyEstimates,

		/// <summary>
		/// Cache for a short term requests for context, teams and projects on client-side. See US#98645 for details.
		/// </summary>
		[ClientFeature("short.term.requests.cache")] ShortTermRequestsCache,

		/// <summary>
		/// Enable Calculated Custom Fields
		/// </summary>
		[ClientFeature("calculated.custom.fields")] CalculatedCustomFields,

		/// <summary>
		/// Enable Collections in calculated Custom Fields
		/// </summary>
		CalculatedCustomFieldsCollections,

		LockAccountOnConvertOperation,

		/// <summary>
		/// Enables Linked Test Plan related functionality. See F#83118 for details.
		/// </summary>
		[ClientFeature("linked.test.plan")] LinkedTestPlan,

		/// <summary>
		/// Disables persistence of the clipboard. See US#100550 for details.
		/// </summary>
		[ClientFeature("disabled.clipboard.persistence")] DisabledClipboardPersistence,

		/// <summary>
		/// Toggles restriction to assign multiple teams on one assignable
		/// </summary>
		[ClientFeature("multiple.teams")] MultipleTeams,

		/// <summary>
		/// Use Team workflow for Burndown charts
		/// </summary>
		[ClientFeature("team.workflow.based.burndown")] TeamWorkflowBasedBurndown,

		/// <summary>
		/// Find mostly used workflow by team and automatically assign it. SEE US#111527
		/// </summary>
		[ClientFeature("default.team.workflow")] DefaultTeamWorkflow,

		/// <summary>
		/// Relations as cards
		/// </summary>
		[ClientFeature("relation.cards")] RelationCards,

		/// <summary>
		/// User Project Allocation. See US#100454 for details.
		/// </summary>
		[ClientFeature("user.project.allocation")] UserProjectAllocation,

		/// <summary>
		/// Team Project Allocation. See US#100455 for details.
        /// </summary>
		[ClientFeature("team.project.allocation")] TeamProjectAllocation,

        /// <summary>
        /// Remove lock on any entity modification operation(with lock entity update always occurs sequentially).
        /// Dead lock on Sql Server may occur. US#104373
        /// </summary>
		[ClientFeature("disable.account.lock")] DoNotLockAccountOnOperation,

		/// <summary>
		/// Private Projects.
		/// </summary>
		[ClientFeature("private.projects")] PrivateProjects,

		/// <summary>
		/// Internationalization.
		/// </summary>
		[ClientFeature("i18n")] I18n,

		/// <summary>
		/// Client localization tools.
		/// </summary>
		[ClientFeature("localizationTools")] LocalizationTools,

		TriggersBatchFlush,

		/// <summary>
		/// Help flow, floating blue marker tour after first login.
		/// </summary>
		[ClientFeature("helpFlow")] HelpFlow,

		Metrics,
		MetricsEffortViaRelations,

		MetricsUserStoryEffort,
		MetricsFeatureEffort,
		MetricsEpicEffort,
		MetricsRequestEffort,

		MetricsSettings,

		/// <summary>
		/// Enables intermediate page that allows to perform redirect to mobile app for mobile browsers on navigating to /entity/{id} urls.
		/// See US#109068 for details.
		/// </summary>
		MobileDeviceDeepLinking,

		[ClientFeature("userStoryNewLists")] UserStoryNewLists,

		/// <summary>
		/// Enables the optimized comet behavior to send (and make DB requests to) actually changed entity fields only.
		/// See BUG#113947 for details.
		/// </summary>
		CometSendChangedFieldsOnly,

		/// <summary>
		/// Shows info message for missing attachments removed by antivirus on storage server.
		/// See US##115176 for details.
		/// </summary>
		[ClientFeature("attachments.antivirus.protect")] AttachmentsAntivirusProtect,

		/// <summary>
		/// Executes Custom Reports queries with 'nolock' query hint
		/// </summary>
		[ClientFeature("nolock")]
		NoLock,

		/// <summary>
		/// Enables unlimited paging for REST api
		/// </summary>
		UnlimitedRestPaging
	}
}

using System.ComponentModel;

// ReSharper disable InconsistentNaming

namespace Tp.Core.Features
{
    public enum TpFeature
    {
        None = 0,

        [ClientFeature("comet.notifications")]
        [Mashup("KanbanNotifications")]
        [Mashup("BoardNotifications")]
        ServerNotifications,

        OptimizeForUnknownQueryHint,

        [Description(
            @"The solution to handle ""parameter sniffing"" based on assigning the stored procedure parameters to local variables and then using the local variables in the query."
        )]
        QueryWithLocalVariables,

        [ClientFeature("context.cacheresponse")]
        CacheContextResponse,

        [Description(@"Toggles possibility to save images and files added with CkEditor as Tp Attachment.")]
        [ClientFeature("ckFilesAsAttachments")]
        StoreCkEditorFilesAsAttachments,

        FlattenInnerReport,

        [ClientFeature("board.cardBatchMoveVisualEffects")]
        CardBatchMoveVisualEffects,

        [OverrideableByMashup]
        Cdn,

        DisableProtocolCheck,

        LockAccountOnTransactionOperation,

        DisableTransactionalMsmqMessageProcessing,

        /// <summary>
        /// Toggles logging for email notifications
        /// </summary>
        EmailNotificationsLogging,

        /// <summary>
        /// When enabled slice will use table instead of view for hierarchies of test plan runs
        /// </summary>
        UseTestRunItemTable,

        /// <summary>
        /// See US#95444 for details.
        /// </summary>
        [ClientFeature("single.signon")]
        SingleSignOn,

        DoNotUseTableValueParameterInSqlQuery,
        DoNotUseTableValueParameterForEntityTypeIdInSqlQuery,

        LockAccountOnConvertOperation,

        /// <summary>
        /// Toggles restriction to assign multiple teams on one assignable
        /// </summary>
        [ClientFeature("multiple.teams")]
        MultipleTeams,

        /// <summary>
        /// Remove lock on any entity modification operation(with lock entity update always occurs sequentially).
        /// Dead lock on Sql Server may occur. US#104373
        /// </summary>
        [ClientFeature("disable.account.lock")]
        DoNotLockAccountOnOperation,

        /// <summary>
        /// Private Projects.
        /// </summary>
        [ClientFeature("private.projects")]
        PrivateProjects,

        /// <summary>
        /// Internationalization.
        /// </summary>
        [ClientFeature("i18n")]
        I18n,

        /// <summary>
        /// Client localization tools.
        /// </summary>
        [ClientFeature("localizationTools")]
        LocalizationTools,

        TriggersBatchFlush,

        /// <summary>
        /// Help flow, floating blue marker tour after first login.
        /// </summary>
        [ClientFeature("helpFlow")]
        HelpFlow,

        /// <summary>
        /// Toggles the display of metrics recalculation status. Relies on the external computation status tracking service.
        /// See US#137427 for details
        /// </summary>
        [ClientFeature("metricsComputationStatus")]
        MetricsComputationStatus,

        [ClientFeature("unsafe_MetricsRecalculations")]
        Unsafe_MetricsRecalculations,

        /// <summary>
        /// Per server feature that enables metric calculator entry point.
        /// Can be enabled only on app(write) instances.
        /// </summary>
        MetricsCalculator,

        /// <summary>
        /// Per server feature that enables read rest via queue entry point.
        /// Can be enabled both on rest(read) and app(write) instances.
        /// </summary>
        MetricsRestViaQueue,

        /// <summary>
        /// Per account feature that enables sending results for external apply to queue.
        /// </summary>
        [ClientFeature("metricsExternalApplier")]
        MetricsExternalApplier,

        /// <summary>
        /// Per account feature that enables sending results for external apply to queue for inProc calculated metrics.
        /// </summary>
        [ClientFeature("highPriorityMetricsExternalApplier")]
        MetricsHighPriorityExternalApplier,

        /// <summary>
        /// Per server feature that enables write rest via queue entry point. (US#143276)
        /// </summary>
        MetricsWriteRestViaQueue,

        /// <summary>
        /// Enables using locks for metric execution results applying synchromization to prevent race conditions (BUG#142991)
        /// </summary>
        MetricsSyncApply,

        /// <summary>
        /// US#176365 and US#176364
        /// Send update requests to external loop prevention service in case in proc loop prevention is used
        /// </summary>
        [ClientFeature("metricsUpdateExternalLoopPrevention")]
        MetricsUpdateExternalLoopPrevention,

        /// <summary>
        /// US#183714
        /// Use external loop prevention service
        /// </summary>
        [ClientFeature("metricsUseExternalLoopPrevention")]
        MetricsUseExternalLoopPrevention,

        /// <summary>
        /// Enables publishing of split ResourceChangedEvent messages (one entity per message instead of all modified in transaction)
        /// to Rule Engine service.
        /// See US#189930 for details.
        /// </summary>
        [ClientFeature("ruleEngineV2PublishingSplit")]
        RuleEngineV2PublishingSplit,

        /// <summary>
        /// Adds a metric which disables default metric for effort calculation.
        /// </summary>
        [ClientFeature("metricNoEffortCalculation")]
        MetricNoEffortCalculation,

        /// <summary>
        /// Adds a metric which disables default metric for time calculation.
        /// </summary>
        [ClientFeature("metricNoTimeCalculation")]
        MetricNoTimeCalculation,

        /// <summary>
        /// Shows info message for missing attachments removed by antivirus on storage server.
        /// See US##115176 for details.
        /// </summary>
        [ClientFeature("attachments.antivirus.protect")]
        AttachmentsAntivirusProtect,

        /// <summary>
        /// Introduces hot keys for Web application to provide quick access to some actions such as 'View Setup', 'Open Quick Add', etc.
        /// See F#120415 for details
        /// </summary>
        [ClientFeature("ui.keyboard.shortcuts")]
        UiKeyboardShortcuts,

        /// <summary>
        /// Executes Custom Reports queries with 'nolock' query hint
        /// </summary>
        [ClientFeature("nolock")]
        NoLock,

        /// <summary>
        /// Enables unlimited paging for REST api
        /// </summary>
        UnlimitedRestPaging,

        /// <summary>
        /// Enables generation of link entities between TestCaseRun and its each parent TestPlanRun
        /// </summary>
        TestRunItemHierarchyLink,
        TrackApplicationContextStatistics,

        /// <summary>
        /// Shows Team filter on Release Burn Down
        /// </summary>
        [ClientFeature("teamFilterOnBurnDownChart")]
        TeamFilterOnBurnDownChart,

        /// <summary>
        /// Enables Batch Actions
        /// </summary>
        [OverrideableByMashup]
        BatchActions,

        /// <summary>
        /// Try to perform a batch action in one transaction. If it fail - rollback the transaction and do it one-by-one.
        /// </summary>
        [OverrideableByMashup]
        BatchActionsOptimisticExecution,

        [OverrideableByMashup]
        BatchActionsAssignments,

        /// <summary>
        /// Enables Clipboard
        /// </summary>
        [OverrideableByMashup]
        Clipboard,

        /// <summary>
        /// Enables Smtp settings section in OnDemand mode
        /// </summary>
        [OverrideableByMashup]
        EditableSmtpSettingsSection,

        /// <summary>
        /// Enables tracking of comet statistics to file.
        /// </summary>
        TrackCometStatistics,

        [ClientFeature("comet.send.server.error.notifications")]
        SendServerErrorCometNotifications,

        /// <summary>
        /// Enables null value ignore for value fields in JSON serialization for comet queue
        /// </summary>
        CompactJsonSerializationCometQueue,

        /// <summary>
        /// Enables vizydrop report
        /// </summary>
        VizydropReport,

        /// <summary>
        /// Uses vizydrop report editor by default
        /// </summary>
        UseVizydropByDefault,

        CacheUserAuthData,
        CacheGlobalSettingsData,

        //asc: technical debt. Used for unit tests only
        BlockingCometBuildNotifications,

        /// <summary>
        /// Disables UserVoice widget
        /// </summary>
        DisableUserVoiceWidgets,

        /// <summary>
        /// Opens Custom Field URL from current domain in the same window. See US#133056 for details.
        /// </summary>
        [ClientFeature("openCustomFieldUrlInSameWindow")]
        OpenCustomFieldUrlInSameWindow,

        /// <summary>
        /// Forces TP to download license as JWT
        /// </summary>
        DefaultToJwtLicense,

        /// <summary>
        /// Enables emoji support (F#129510)
        /// </summary>
        Emoji,

        /// <summary>
        /// Enables Global Visual Encoding (US#130234)
        /// </summary>
        [ClientFeature("globalVisualEncoding")]
        GlobalVisualEncoding,

        /// <summary>
        /// Hides Comment Required checkbox in TP3 Process Setup
        /// </summary>
        [ClientFeature("hideCommentRequiredCheckbox")]
        HideCommentRequiredCheckbox,

        /// <summary>
        /// Enables app context by entity ids caching
        /// </summary>
        [ClientFeature("context.cache.by.entities")]
        ExtendedApplicationContextByEntitiesCache,

        /// <summary>
        /// Disable view refresh on comet disconnect. See US#132981 for details.
        /// </summary>
        [ClientFeature("refreshOnCometDisconnect")]
        RefreshOnCometDisconnect,

        /// <summary>
        /// Enables app context by team ids and project ids caching
        /// </summary>
        [ClientFeature("context.cache.by.teams.and.projects")]
        ExtendedApplicationContextByTeamsAndProjectsCache,

        /// <summary>
        /// Randomize (lower-upper bounds) inactivity timeout  (US#133166)
        /// </summary>
        [ClientFeature("randomizeInactivityTimeoutOnCometDisconnect")]
        RandomizeInactivityTimeoutOnCometDisconnect,

        UseParallelCustomFieldsExpander,
        UseCopyGeneralToProjectAccountLock,
        UseCreateGeneralFromRequestAccountLock,
        UseStatefulSlice,

        /// <summary>
        /// Enables GlobalDataTemplates caching
        /// </summary>
        [ClientFeature("cache.slice.globalDataTemplates")]
        GlobalDataTemplatesResponseCache,

        /// <summary>
        /// Enables ViewMenu caching
        /// </summary>
        [ClientFeature("cache.viewMenu")]
        ViewMenuResponseCache,
        UseMemoizedCometClient,

        /// <summary>
        /// Uses AjaxMin minifier instead of YUI+jsmin port
        /// </summary>
        AjaxMinMinifier,

        /// <summary>
        /// Emit non-minified scripts (mashups, staticlibsincluder) for development, while checking for correctness by minifying
        /// </summary>
        EmitScriptsInDevMode,

        /// <summary>
        /// Treats no allocations in time period as zero allocation in capacity calculations (BUG#138256)
        /// </summary>
        NoAllocationsMeansZeroCapacity,
        UsePrecompiledDslFilter,

        /// <summary>
        /// Processes and show items related to user's recent work and browsing history
        /// </summary>
        [ClientFeature("recentItems")]
        RecentItems,

        /// <summary>
        /// Disable client side comet subscription with origin 'model.assignmentsList.comet'
        /// It is an assignemts widget in old lists
        /// </summary>
        [ClientFeature("comet.disable.model.assignmentsList")]
        DisableAssignmentsListCometSubscription,

        /// <summary>
        /// Detect conflict in case of simultaneous description edits (BUG#136080)
        /// </summary>
        [ClientFeature("descriptionEditsConflictDetection")]
        DescriptionEditsConflictDetection,

        /// <summary>
        /// Enables assertion that doesn't allow to save Time instance linked to an Assignable with Role property set to null (BUG#75802)
        /// </summary>
        RequiredRoleForAssignableTime,

        /// <summary>
        /// F#132985 Search permission service
        /// </summary>
        PermissionGroupsNotifications,

        ///<summary>
        /// New global search
        ///</summary>
        [ClientFeature("search2")]
        Search2,

        [ClientFeature("oauth")]
        OAuth,

        /// <summary>
        /// Enables portfolio epics. See https://plan.tpondemand.com/entity/188521-portfolio-epic
        /// </summary>
        PortfolioEpic,

        /// <summary>
        /// Hide Iterations. See https://plan.tpondemand.com/entity/256372-hide-iterations-from-various-places-in
        /// </summary>
        [ClientFeature("hideProjectIterations")]
        HideProjectIterations,

        /// <summary>
        /// Use full context for Burn Down
        /// </summary>
        BurnDownFullContext,

        /// <summary>
        /// Automaticaly move child bugs to same project when feature's project changes.
        /// </summary>
        MoveBugsToNewFeatureProject,

        ProcessSoftDelete,

        [ClientFeature("include.stack.traces.in.response")]
        IncludeStackTracesInResponse,

        /// <summary>
        /// Sync team list on userstory and it's tasks
        /// </summary>
        SyncTeamsOnUserStoryAndTasks,

        /// <summary>
        /// Allows marking custom fields as "system" - they can be changed only by system activities (e.g. metrics), but
        /// not by end-users directly.
        /// </summary>
        [ClientFeature("systemCustomFields")]
        SystemCustomFields,

        /// <summary>
        /// Determine whether acid parameter is shown in urls
        /// </summary>
        [ClientFeature("showAcidInUrl")]
        ShowAcidInUrl,

        /// <summary>
        /// Client feature that change newlist with assignables sort to RankWithFinalInTheBottom
        /// </summary>
        [ClientFeature("newlists.moveFinalBottom")]
        MoveFinalEntitiesToTheBottom,

        /// <summary>
        /// Display user capacity in board cells (F#142837)
        /// </summary>
        CapacityInBoardCells,

        /// <summary>
        /// Enables relations management on board. Activates show-relations mashup (https://gitlab.tpondemand.net/relations/show-relations)
        /// </summary>
        BoardRelationsUI,

        /// <summary>
        /// Enables board integration module (F#142837)
        /// </summary>
        BoardIntegrationModule,

        /// <summary>
        /// Uses better dates comparison logic in dsl engine (BUG#111590)
        /// </summary>
        DslBetterDatesComparison,

        /// <summary>
        /// Enables some speedup in editing team workflows. See #96862.
        /// </summary>
        TeamWorkflowEditSpeedup,

        /// <summary>
        /// Enables ability to select related project/teams in context selector
        /// </summary>
        [ClientFeature("relatedSelectionForContextSelector")]
        RelatedSelectionForContextSelector,

        [ClientFeature("new.list.refresh.on.projects.change")]
        RefreshNewListOnProjectsChange,

        /// <summary>
        /// Enables caching of Full LightContext instances for HttpContext (if available) or ThreadLocal scope. See BUG#164611 for details
        /// </summary>
        CacheFullLightContextsForThreadScope,

        [ClientFeature("preventNavigationOnCometChangesBoardPlus")]
        PreventNavigationOnCometChangesBoardPlus,

        /// <summary>
        /// Fix logic of calculating allocation end date, see BUG#123
        /// </summary>
        FixAllocationsEndDate,

        /// <summary>
        /// Enables conversion to widening type for binary expression's operands with different types.
        /// See BUG#166712 and BUG#166867 for details.
        /// </summary>
        DslDifferentTypesWideningConversion,

        /// <summary>
        /// Includes entity name in a copy link.
        /// See US#167783 for details.
        /// </summary>
        [ClientFeature("copyLink.withTitle")]
        CopyLinkWithTitle,

        /// <summary>
        /// Enforces Number and Money CF rounding when loading data via Business Objects.
        /// See BUG#164609 for details
        /// </summary>
        NormalizeNumericCustomFieldsRounding,

        /// <summary>
        /// US#168834. When enabled, forces QueryModelVisitor to always generate ordering expressions
        /// instead of specifying particular fields in QuerySpec.
        /// There is an intention to make this behavior enabled by default in the future,
        /// and this feature-toggle is added as a safety mechanism for possible regressions in production.
        /// </summary>
        AlwaysUseOrderingExpressions,

        /// <summary>
        /// Fixes prioritisation of multiple entities with ascending sorting. See BUG#125079
        /// </summary>
        FixAscendingPrioritisation,

        /// <summary>
        /// Fixes #168641
        /// </summary>
        DslProgramOfGeneralImprovement,

        /// <summary>
        /// Enables support for entity.new.list component configuration where X axis doesn't depend on values of parent Entity Axis
        /// </summary>
        IndependentAxesInEntityLists,

        /// <summary>
        /// Enables link to user profile page in the Views Menu
        /// See Feature #167540 for details.
        /// Requires <see cref="IndependentAxesInEntityLists"/> feature to be enabled.
        /// </summary>
        [ClientFeature("board.menu.user.profile")]
        UserProfileInViewsMenu,

        /// <summary>
        /// Replaces old assignments list with entity.new.list on the User page.
        /// See #164883 for details.
        /// Requires <see cref="IndependentAxesInEntityLists"/> feature to be enabled.
        /// </summary>
        [ClientFeature("user.view.newAssignmentsList")]
        NewAssignmentsListOnUserEntityView,

        /// <summary>
        /// Use feature toggles from cluster.yaml config
        /// </summary>
        ExternalFeatureToggles,

        /// <summary>
        /// Return all entities which was affected on numeric priority recalculation
        /// </summary>
        [ClientFeature("returnAllEntitiesOnNumericPriorityRecalculation")]
        ReturnAllEntitiesOnNumericPriorityRecalculation,

        /// <summary>
        /// 500 per page for lists.
        /// </summary>
        [ClientFeature("newList500ItemsPerPage")]
        NewList500ItemsPerPage,

        /// <summary>
        /// Replaces built-in Views Menu with the one provided by the Views Menu Mashup
        /// See Feature #171906
        /// </summary>
        [ClientFeature("viewsMenuMashup")]
        ViewsMenuAsMashup,

        /// <summary>
        /// Extends DSL filters for collection members with Project.IsActive is True predicate
        /// </summary>
        ExtendDslWithProjectIsActiveFilterOnCollectionMember,

        /// <summary>
        /// Improves support for Hide Empty Lanes functionality. See #170861, #170862.
        /// Also improves support of polymorphic filters that was added in RestImprovePolymorphicFilters feature.
        /// </summary>
        HideEmptyLanesOnBackend,

        /// <summary>
        /// BUG#174144 Don't update effort by call for the card, wait for comet
        /// </summary>
        DoNotReloadTotalEffortAfterUpdate,

        /// <summary>
        /// Forces the currently open view to be always visible in the Views Menu even if it doesn't
        /// match specified search criteria.
        /// See US#174568
        /// </summary>
        [ClientFeature("board.menu.search.showCurrentView")]
        AlwaysShowCurrentViewWhenSearchingInViewsMenu,

        /// <summary>
        /// US#174262 Remove excessive request from Units field
        /// </summary>
        RemoveUnnecessaryRequestFromUnitsField,

        ImproveTeamWorkflowSuggestionPerformance,

        /// <summary>
        /// Fix a few N+1 problems in context retrieving code
        /// </summary>
        FixNPlus1InContext,

        FixNPlus1InQuickAddTemplates,

        /// <summary>
        /// Protects description component content from beign overwritten by stakle comet notification. See #176307.
        /// </summary>
        [ClientFeature("fixCometRaceConditionInDescriptionEditor")]
        FixCometRaceConditionInDescriptionEditor,

        /// <summary>
        /// BUG#179677 Adds usage of comet notification deprecation logic in model.board.slice.comet.js.
        /// </summary>
        [ClientFeature("clientCometNotificationDeprecationCache")]
        ClientCometNotificationDeprecationCache,

        /// <summary>
        /// US#166640 Whether entity ID on entity view should be a link or not.
        /// </summary>
        EntityIdIsLink,

        /// <summary>
        /// Use INNER JOIN when possible instead of IN (select * from @TableParam), US#173197.
        /// </summary>
        ReplaceInCheckWithInnerJoin,

        /// <summary>
        /// Use IN (x, y, z) instead of IN (select * from @TableParam) for small values sets, US#173197.
        /// </summary>
        InlineSmallInExpressionRangesInQueries,

        /// <summary>
        /// Starts query selector compilation in parallel with query execution
        /// See `/adr/2018-05-25 - Expression compilation.md` for details
        /// </summary>
        AsyncQuerySelectorCompiling,

        /// <summary>
        /// Applies other axis filters when calculate axis counters. See #67718.
        /// </summary>
        AxesCountersWithFullFilters,

        /// <summary>
        /// Enables integration of Notification Center UI in TP
        /// </summary>
        [ClientFeature("notificationCenter")]
        NotificationCenter,

        /// <summary>
        /// Enables refreshing of labels using slice comet subscription on detailed view tabs with entity collection lists
        /// See US#178806 for details
        /// </summary>
        [ClientFeature("label.refresher.comet")]
        LabelRefresherComet,

        /// <summary>
        /// Adds support of inactive teams.
        /// Obsolete. This feature was not implemented to be production ready and was abandoned.
        /// Use InactiveTeamsSimple feature instead.
        /// </summary>
        [ClientFeature("inactiveTeams")]
        InactiveTeams,

        /// <summary>
        /// Hides inactive teams from user interface.
        /// Unlike as projects, does not affect visibility of entities assigned to inactive teams.
        /// </summary>
        [ClientFeature("inactiveTeamsSimple")]
        InactiveTeamsSimple,

        /// <summary>
        /// US#180358 Fix N+1 problems in capacity calculator
        /// </summary>
        FixNPlus1InCapacityCalculator,

        /// <summary>
        /// US#180714
        /// </summary>
        ImproveTagsFilteringPerformance,

        /// <summary>
        /// Disables mashups JavaScript code minification. See US#181349 for details.
        /// Requires <see cref="EmitScriptsInDevMode"/> feature to be enabled.
        /// </summary>
        DisableMashupMinifier,

        /// <summary>
        /// Always fill full entity custom field resource. See https://plan.tpondemand.com/entity/182373-product-structure-when-change-some-cf
        /// </summary>
        AlwaysConvertEntityCustomFieldToFullResource,

        /// <summary>
        /// When enabled, displays "Custom rules v2" section in settings menu.
        /// This feature-toggle is not used in Targetprocess codebase,
        /// it is used by integration mashup
        /// https://tp.githost.io/business-rules/rule-engine-ui/tree/develop/mashup-integration
        /// </summary>
        RuleEngineV2ManagementUI,

        /// <summary>
        /// Use StoredProcedure for Cumulative Flow Diagram Data instead of TVF
        /// </summary>
        [ClientFeature("cumulativeFlowDiagramDataViaProcedure")]
        CumulativeFlowDiagramDataViaProcedure,

        /// <summary>
        /// Use LEFT JOIN instead of COUNT in filters.
        /// </summary>
        [ClientFeature("leftJoinCountExpressions")]
        LeftJoinCountExpressions,

        /// <summary>
        /// Use WHERE IN instead of COUNT in filters.
        /// </summary>
        [ClientFeature("whereInCountExpressions")]
        WhereInCountExpressions,

        /// <summary>
        /// Use WHERE IN instead of LEFT JOIN in permission expressions.
        /// </summary>
        [ClientFeature("whereInPermissionExpressions")]
        WhereInPermissionExpressions,

        /// <summary>
        /// Enables opening cards on boards view with single click
        /// See US#178104 for details
        /// </summary>
        [ClientFeature("singleClickViewEntity")]
        SingleClickViewEntity,

        /// <summary>
        /// Enables color coding display on min-sized cards on Boards and Timelines
        /// See US#178104 (comment #2) for details
        /// </summary>
        [ClientFeature("showMinSizeCardsColor")]
        ShowMinSizeCardsColor,

        /// <summary>
        /// Enables Release field for Team Iterations.
        /// See US#184694 for details
        /// </summary>
        [ClientFeature("releaseForTeamIteration")]
        ReleaseForTeamIteration,

        /// <summary>
        /// Team Iteration field for features
        /// </summary>
        [ClientFeature("teamIterationForFeature")]
        TeamIterationForFeature,

        /// <summary>
        /// Team Iteration field for epics
        /// </summary>
        [ClientFeature("teamIterationForEpic")]
        TeamIterationForEpic,

        /// <summary>
        /// Use assets service to serve frontend assets
        /// </summary>
        AssetsService,

        /// <summary>
        /// Enables separated frontend from https://tp.githost.io/frontend/targetprocess-frontend
        /// </summary>
        SeparatedFrontend,

        /// <summary>
        /// US#229387 Enabled short-lived cache of responses returned by assets service.
        /// </summary>
        SeparatedFrontendCache,

        /// <summary>
        /// Allow to have direct access to entities. See https://plan.tpondemand.com/entity/182308-direct-access-to-entities
        /// </summary>
        [ClientFeature("directAccessToEntities")]
        DirectAccessToEntities,

        /// <summary>
        /// Allow owners to gran direct access to entities. See https://plan.tpondemand.com/entity/198646-entity-owners-can-see-direct-access-panel
        /// </summary>
        [ClientFeature("directAccessToEntitiesOwnersCanGrantAccess")]
        DirectAccessToEntitiesOwnersCanGrantAccess,

        /// <summary>
        /// Whether statistics for selected processes and terms in the context should be gathered
        /// </summary>
        StatisticsForProcessesInContext,

        /// <summary>
        /// Adds support of terms in Python DSL (filters on boards, visual encoding).
        /// See Feature #188160 for details.
        /// </summary>
        [ClientFeature("termsInPythonDsl")]
        TermsInPythonDsl,

        /// <summary>
        /// Enables the metric which automatically calculates Team Iteration of a Feature based on its User Stories.
        /// Requires <see cref="TeamIterationForFeature"/> toggle to be enabled.
        /// </summary>
        [ClientFeature("metricTeamIterationForFeature")]
        MetricTeamIterationForFeature,

        /// <summary>
        /// Executes Custom Reports indexed view queries with 'NOEXPAND' query hint.
        /// </summary>
        [ClientFeature("noExpandIndexedView")]
        NoExpandIndexedView,

        /// <summary>
        /// Disables order checks for relation lines (effectively disables highlighting as well).
        /// See User Stories #188503 and #188507 for details.
        /// </summary>
        [ClientFeature("disableOrderChecksForRelationLines")]
        DisableOrderChecksForRelationLines,

        /// <summary>
        /// Enable mashup integration for User Management
        /// </summary>
        [ClientFeature("workspacesUserManagement")]
        WorkspacesUserManagement,

        /// <summary>
        /// Enable Notification Center In App notifications
        /// </summary>
        InAppNotifications,

        /// <summary>
        /// When enabled, removes endDate field from Quick Add UI for Release
        /// and adds 'iterations count' and 'iteration duration' fields for Release in Quick Add UI
        /// This feature-toggle is not used in Targetprocess codebase,
        /// it is used by integration mashup
        /// https://gitlab.tpondemand.net/safe/iteration-generation-ui/tree/master/mashup-integration/example_production
        /// </summary>
        [ClientFeature("iterationGeneration")]
        IterationGeneration,

        /// <summary>
        /// Display user and team capacity in board cells (F#190857)
        /// </summary>
        [ClientFeature("boardCapacityMashup")]
        BoardCapacityMashup,

        /// <summary>
        /// When enabled, Release of a Feature is propagated to its User Stories (which are not in final state and
        /// have no logged progress).
        /// See US#200104 for details
        /// </summary>
        PropagateReleaseFromFeatureToStories,

        /// <summary>
        /// When enabled, Release of an Epic is propagated to its Features (which are not in final state and
        /// have no logged progress).
        /// See US#200104 for details
        /// </summary>
        PropagateReleaseFromEpicToFeatures,

        /// <summary>
        /// Fix for BUG#203892 Update access to Test Case Run is denied when user unlinks test case
        /// </summary>
        DoNotCheckPermissionsWhenFrozingTestCaseRun,

        /// <summary>
        /// When enabled, account configuration (connection string, culture etc.) is trying to read
        /// from local cache first (Hosting.LocalCache). If reading from local cache fails,
        /// remote sources (Hosting.Root, Hosting.AlternateRoot) are used.
        /// See US#204467 for details
        /// </summary>
        HostingLocalCacheFirst,

        /// <summary>
        /// When enabled, export to CSV only visible columns on List view.
        /// https://plan.tpondemand.com/entity/197378-investigate-export-list-to-csv-using
        /// </summary>
        CsvCustomizedExport,

        /// <summary>
        /// When enabled, export to CSV could work with axes.
        /// https://plan.tpondemand.com/entity/199822-export-system-units-from-axes
        /// </summary>
        CsvCustomizedExportParentData,

        /// <summary>
        /// When enabled, export to CSV via XMLHttpRequest + Blob API to gather more stats.
        /// https://plan.tpondemand.com/entity/199947-better-stats-about-csv-export-via
        /// </summary>
        CsvExportViaBlob,

        /// <summary>
        /// When enabled, allows users to create new entity types and attributes.
        /// Also changes query engine and various mapping components to allow reading and modifying such entities and attributes.
        /// </summary>
        [ClientFeature("extendableDomain")]
        ExtendableDomain,

        /// <summary>
        /// When enabled, history infrastructure will be updated for native entites when ED attribute added
        /// </summary>
        UpdateHistoryInfraForNativeEntitiesWhenEDAttributeAdded,

        /// <summary>
        /// If there are several projects available in particular quick add selector, do not preselect first one.
        /// See US#207651
        /// </summary>
        QuickAddNoSelectedDefaultProject,

        /// <summary>
        /// Adds labels to quick add fields. See US#207791.
        /// </summary>
        QuickAddWithLabels,

        /// <summary>
        /// Preselect correct term in QA. See US#208136.
        /// </summary>
        QuickAddDefaultTerm,

        /// <summary>
        /// When enabled, batch custom fields API will not refresh resource after update (so performance grows).
        /// See https://plan.tpondemand.com/entity/205716-research-optimize-batch-action-set-cf
        /// </summary>
        DoNotRefreshResourceAfterBatchCustomFieldsUpdate,

        /// <summary>
        /// US#207395
        /// When enabled, extendable domain entities are included in `/api/v1/Index/meta` and `/api/v1/meta` endpoint response by default.
        /// </summary>
        IncludeExtendableDomainInMetaByDefault,

        /// <summary>
        /// When enabled, ensures saved entity custom field entity available.
        /// See https://plan.tpondemand.com/entity/209255-inconsistency-when-setting-targetprocess-entity-custom
        /// </summary>
        SaveOnlyEntityCustomFieldWhichValueAvailable,

        /// <summary>
        /// When enabled, check saved entity custom field has type from limited by meta set of entity types, if any.
        /// See https://plan.tpondemand.com/entity/209255-inconsistency-when-setting-targetprocess-entity-custom
        /// </summary>
        SaveOnlyEntityCustomFieldWithTypeFromAllowedByMetaSet,

        /// <summary>
        /// https://plan.tpondemand.com/entity/207720-project-for-wrong-entity-is-changed
        /// </summary>
        ShowWarningFeatureCustomFieldsLoss,

        /// <summary>
        /// Allows batch move of Iterations. See comment in IterationShifter.GetConflictedIterationIternal. See BUG#214047.
        /// </summary>
        FixIterationsBatchMove,

        /// <summary>
        /// Add predefined date ranges for timeline. See US#209973.
        /// </summary>
        TimelineDefaultRanges,

        /// <summary>
        /// Basic support for Kanban boards on touch screens
        /// https://plan.tpondemand.com/entity/198816-basic-support-for-kanban-boards-on
        /// </summary>
        TouchScreenBoards,

        /// <summary>
        /// Basic timeline grid lines functionality. US#209986.
        /// </summary>
        TimelineGridLinesV2,

        /// <summary>
        /// Advanced timeline grid lines functionality. US#209986.
        /// </summary>
        TimelineGridLinesV2Axes,

        /// <summary>
        /// Improves milestone: allows to show them on detailed view, as cells on board,
        /// allows to set Milestone for any General except Project and Squad.
        /// </summary>
        MilestoneImprovements,

        /// <summary>
        /// Enables permission checks for milestones
        /// </summary>
        MilestonePermissions,

        /// <summary>
        /// Apply axes filters (up to 64 items on axis) for one-by-one view. See https://plan.tpondemand.com/entity/211388-apply-dsl-filters-for-lanes-in
        /// </summary>
        UseAxisFiltersOneByOneViews,

        /// <summary>
        /// US#210019. When enabled, user can create TP boards that can be linked to Miro boards and display their content.
        /// </summary>
        MiroBoard,

        ///<summary>
        /// US#217072
        /// If enabled disable multiple (count > 2) assigned team propagation from Portfolio Epic to Epic, Portfolio Epic to Feature and from Epic to Feature
        /// </summary>
        DoNotSuggestMultipleAssignedTeamsInQuickAdd,

        /// <summary>
        /// Allow set filters for X axis on timeline for gridlines
        /// </summary>
        TimelineGridlinesAxisFilter,

        /// <summary>
        /// When enabled, newly created projects will have NumericPriority calculated in the same way it's done for
        /// other Generals. If disabled, custom NumericPriority calculation algorithm (based on number of existing non-
        /// deleted projects) is used.
        /// See https://plan.tpondemand.com/entity/215527-newly-created-projects-get-wrong-numericpriority for details
        /// </summary>
        DefaultInitialNumericPriorityForProjects,

        /// <summary>
        /// Applies logic enabled by feature DatesWithTZ for date custom fields
        /// </summary>
        DatesWithTZForCF,

        /// <summary>
        /// When enabled, users can assign role on entity type in process directly.
        /// See https://plan.tpondemand.com/entity/221196-roles-are-addedremoved-to-master-table for details
        /// </summary>
        AssignableRolesIndependentFromWorkflows,

        /// <summary>
        /// When turned on, indicates that back-end supports creating custom formula metrics targeting not only custom, but also native fields.
        /// See US#224786 for details.
        /// </summary>
        [ClientFeature("customFormulaMetricForNativeFields")]
        CustomFormulaMetricForNativeFields,

        /// <summary>
        /// Enables optimistic updates for some inline editors in lists.
        /// See https://plan.tpondemand.com/entity/222328-optimistic-updates-in-number-money-text for details
        /// </summary>
        [ClientFeature("optimisticUpdatesInLists")]
        OptimisticUpdatesInLists,

        /// <summary>
        /// US#223186.
        /// Enables execution of batch update of single CF for a set of entities. Entities should belong to the same process and entity type.
        /// Implemented inside SetCustomFieldBatchCommand. Updated are flushed directly into db. No validation happens and no events are generated.
        /// </summary>
        UpdateCFBatchCommand,

        /// <summary>
        /// When enabled, allow user to drag non-prioritizable items in lists, showing corresponding notification
        /// US#218354
        /// </summary>
        PrioritizationDisabledWarning,

        /// <summary>
        /// Enables ability to sort cards on timeline view.
        /// See https://plan.tpondemand.com/entity/224996-arrange-cards-on-timeline-manually-prioritization for details
        /// </summary>
        TimelineCardPrioritization,

        /// <summary>
        /// Client-side feature toggle.
        /// When enabled, Search string is modified to AND model in multi-word search.
        /// </summary>
        [ClientFeature("searchPhraseApplyAndConditionOnEveryWord")]
        SearchPhraseApplyAndConditionOnEveryWord,

        /// <summary>
        /// When enabled, displays Creation Date filter field in Search.
        /// </summary>
        [ClientFeature("searchUICreationDateFilter")]
        SearchUICreationDateFilter,

        /// <summary>
        /// Enables integration for view context service
        /// </summary>
        ViewContextService,

        /// <summary>
        /// Enables integration for quick add customizer
        /// </summary>
        [ClientFeature("quickAddCustomizer")]
        QuickAddCustomizer,
        QuickAddCustomizerIncludesUserBaseFields,
        QuickAddCustomizerIncludesStateValueFields,
        QuickAddCustomizerIncludesBooleanValueFields,
        QuickAddCustomizerIncludesCustomFields,

        /// <summary>
        /// Enables nested quick add in added-by-customizer entity selectors (it's kinda buggy right now for release)
        /// </summary>
        [ClientFeature("quickAddCustomizerNestedQuickAdd")]
        QuickAddCustomizerNestedQuickAdd,

        /// <summary>
        /// When enabled, parallel batch move cards when possible.
        /// See https://plan.tpondemand.com/entity/222416-improve-batch-move-performance-by-partially for details.
        /// </summary>
        UseParallelBatchMoveCards,

        /// <summary>
        /// When enabled, users can notify team members about add/edit comment.
        /// See https://plan.tpondemand.com/entity/217138-add-comment-on-entity-details-view for details
        /// </summary>
        NotifyProjectMembersFromAddCommentForm,

        /// <summary>
        /// Makes density queries in parallel.
        /// </summary>
        CalculateTimelineDensityInParallel,

        /// <summary>
        /// See US#229357.
        /// </summary>
        BoardScrollSyncImprovementsV3,

        /// <summary>
        /// US#230216 Timeline navigator should update timeline only at the end
        /// </summary>
        TimelineNavigatorDisableLiveSync,

        /// <summary>
        /// US#231451
        /// When enabled, fetches frontend integration info from Consul.
        /// </summary>
        ExternalFrontendIntegrations,

        /// <summary>
        /// US#233659 When enabled, applies configurable TTLs to incoming message for out-of-proc comet.
        /// If message has been in a queue longer than specified TTL, it won't be handled by comet server.
        /// </summary>
        CometMessageTtl,

        /// <summary>
        /// BUG#220338
        /// When this feature is enabled TP frontend should logout user every time session in auth service is not found.
        /// </summary>
        [ClientFeature("oauth.logoutWhenLoginRequired")]
        OAuthLogoutWhenLoginRequired,

        /// <summary>
        /// US#239312
        /// Temp feature to detect on frontend if TP version with builds for requests is deployed.
        /// Can be deleted after several releases
        /// </summary>
        [ClientFeature("buildForRequests")]
        BuildForRequests,

        /// <summary>
        /// US#234461
        /// When this feature is disabled, menu item with old Help Desk settings is not shown to users on TP frontend
        /// </summary>
        [ClientFeature("helpDeskSupport")]
        HelpDeskSupport,

        /// <summary>
        /// US#230600 Add Effort and Assignments to Split template
        /// </summary>
        EffortInSplit,

        /// <summary>
        /// See BUG#184414
        /// </summary>
        FixHideEmptyLanesWithStateAxis,

        /// <summary>
        /// When enabled, maintains in-memory per-account cache of Resource Model -> Lang Types,
        /// which avoids redundant metamodel lookups.
        /// </summary>
        RestDslTypeSystemCache,

        /// <summary>
        /// When enabled, entity changed events will be published asynchronously. See
        /// https://plan.tpondemand.com/entity/217214-try-to-make-notifications-graph-async
        /// </summary>
        AsyncEntitiesChangedEventPublishing,

        /// <summary>
        /// When enabled, entity changed events statistics will be logged. This may cause slowdowns. See
        /// https://plan.tpondemand.com/entity/232997-log-async-graph-metrics-to-kibana
        /// https://plan.tpondemand.com/entity/233073-log-async-graph-metrics-via-prometheus
        /// </summary>
        WriteAsyncEntitiesChangedEventPublishingStatistics,

        /// <summary>
        /// When enabled, forces TP to use old service bus session for plugin &lt;-&gt; TP communication. Needed for backward
        /// compatibility. To enforce old entity changed events publishing behaviour enable this and disable
        /// AsyncEntitiesChangedEventPublishing feature, then restart TP.
        /// See https://plan.tpondemand.com/entity/217214-try-to-make-notifications-graph-async
        /// </summary>
        ForceUseBackwardCompatibleServiceBusSession,

        /// <summary>
        /// Enables possibility to set custom field for SSO RelayState data
        /// Enables possibility to set custom query string for SSO RelayState data
        /// </summary>
        SsoRelayStateQueryStringParameterName,

        /// <summary>
        /// When enabled, priorities and severities will be cached on start
        /// </summary>
        [ClientFeature("prioritiesAndSeveritiesCache")]
        PrioritiesAndSeveritiesCache,

        /// <summary>
        /// When enabled, prevents calls to taus via global handler.
        /// </summary>
        [ClientFeature("preventTausCalls")]
        PreventTausCalls,

        /// <summary>
        /// US#242916
        /// When enabled, appends filter like `x => x.EntityType.Id in [...]` for cells only when definition has more than 1 cell type,
        /// assuming that otherwise the data is always selected from the proper database table, and no additional filter is needed.
        /// </summary>
        AppendSliceEntityTypeIdFilterOnlyWhenMultipleCellTypes,

        /// <summary>
        /// US#227095
        /// When enabled, maintains per-account version-based cache of users
        /// </summary>
        UsersInMemoryCache,

        /// <summary>
        /// US#227095
        /// When enabled, maintains per-account cache of SystemUser
        /// </summary>
        SystemUserInMemoryCache,

        /// <summary>
        /// Filters on collections with 'It is None' match empty collections: ?AssignedUser.Where(It is None).
        /// See US#243225
        /// </summary>
        SupportWhereItIsNoneInDsl,

        /// <summary>
        /// Adds support into DSL engine autoapply logic for properties from ED (e.g. UserStory.Capability)
        /// See US#240241
        /// </summary>
        FixDslAutoApplyForEDProperties,

        /// <summary>
        /// This feature toggle just mark if server have separated Requirements practice with Feature and Epic, so new frontend can
        /// determinate which practice to check for show epic and feature.  When SeparatedFeaturesAndEpicsPractices is enabled frontend
        /// for show feature should check Feature practice if SeparatedFeaturesAndEpicsPractices is disabled frontend should check
        /// Requirements practice.
        /// </summary>
        SeparatedFeaturesAndEpicsPractices,

        /// <summary>
        /// US#247535
        /// Enables optimization of collection filters from
        ///   Collection.Count(i => predicate1(i)) > 0 || Collection.Count(i => predicate2(i)) > 0 || ...
        /// to
        ///   Collection.Count(i => predicate1(i) || predicate2(i) || ...) > 0
        /// </summary>
        CombineCollectionFilters,
        CombineCollectionFiltersLimitDepth,

        /// <summary>
        /// BUG#241709
        /// When enabled, hides labels with assigned effort and velocity on iteration and team iteration axes
        /// </summary>
        [ClientFeature("hideVelocityInCellHeader")]
        HideVelocityInCellHeader,

        /// <summary>
        /// US#247551
        /// When enabled, an option for disabling 4th level of list appears at list setup
        /// </summary>
        [ClientFeature("allowDisableInnerLevelsInList")]
        AllowDisableInnerLevelsInList,

        /// <summary>
        /// BUG#249252
        /// When enabled, hides corresponding card types on board setup if some of practices disabled.
        /// </summary>
        HideCardTypeOnBoardSetupIfPracticeDisabled,

        /// <summary>
        /// US#251262
        /// </summary>
        [ClientFeature("cometExpansionStateInTreeViewSubscription")]
        CometExpansionStateInTreeViewSubscription,

        /// <summary>
        /// BUG#251172
        /// When enabled WWW-Authorization header will be returned in response
        /// independently of SSO settings
        /// </summary>
        AlwaysChallengeBasicAuthWhen401,

        /// <summary>
        /// US#249327
        /// If enabled throw specific exception if cell type wasn't found during slice request
        /// </summary>
        ThrowCellsNotFoundInSliceIfUnavailableCellType,

        /// <summary>
        /// F#243461
        /// Add support of ED entities and properties to Import
        /// </summary>
        ImportV2,

        /// <summary>
        /// F#243462
        /// When enabled, it is possible to convert ED entities in to assignables and vice versa (WIP)
        /// </summary>
        ExtendableDomainConvert,

        /// <summary>
        /// When enabled, extendable domain metamodel returns typed List<T> interface for collection properties
        /// </summary>
        ExtendableDomainTypedCollectionInMetamodel,

        /// <summary>
        /// US#251411
        /// When enabled, splits polymorphic cells query (when multiple card types are selected) into separate slices per card type.
        /// Supposed to increase performance in some cases.
        /// </summary>
        SliceSeparateCellsPerType,

        /// <summary>
        /// BUG#251626
        /// </summary>
        FixTimelineCardResizeSideDetecting,

        /// <summary>
        /// BUG#241455.
        /// Skips applying of intermediate responses from MovePlannedDates to reduce visual card jumping
        /// </summary>
        FixTimelineCardResizeResponseProcessing,

        /// <summary>
        /// US#247674
        /// Use [Assignable].[HasActiveSquad] flag in custom reports queries for better performance when filtering data by context.
        /// </summary>
        UseAssignableHasActiveSquadFlagInContextFilter,

        /// <summary>
        /// US#247674, US#247670
        /// Use [AssignableSquad].[IsSquadActive] flag instead of joining [Squad] table and using [Squad].[IsActive] flag where possible.
        /// </summary>
        UseAssignableSquadIsSquadActiveFlag,

        /// <summary>
        /// US#248407
        /// When enabled, generates fields like `Release.Capabilities` when ED entity `Capability` is created
        /// </summary>
        ExtendableDomainGenerateInheritedOppositeCollections,

        /// <summary>
        /// US#251086
        /// Set of features that allow to replace some parameters with constants (actual values) in queries generated by Custom Reports.
        /// It could help SQL Server to use statistics in a more efficient way when building query plans.
        ///
        /// *** InlineBooleanValuesForRelationalExpressions ***
        /// Inline boolean values in comparison statements, e.g.
        ///   `t2.IsActive = 1` instead of `t2.IsActive = @P1`.
        ///
        /// *** InlineGeneralEntityTypeId,
        /// *** InlineGeneralParentProjectId,
        /// *** InlineTpUserType,
        /// *** InlineAssignableSquadSquadId,
        /// *** InlineAssignableSquadUserId,
        /// *** InlineEntityPermissionUserId,
        /// *** InlineProjectMemberUserId,
        /// *** InlineTpUserUserId,
        /// *** InlineSquadMemberUserId,
        /// Inline integer values only for particular Table + Column pairs, e.g.
        ///   `g0.EntityTypeId IN (4, 8)` instead of `g0.EntityTypeID IN (@P1, @P2)`
        /// where `g0` is alias for `General` table
        ///   `tu0.Type = 1` instead of `tu0.Type = @P1`
        /// where `tu0` is alias for `TpUser` table
        ///
        /// </summary>
        InlineBooleanValuesForRelationalExpressions,
        InlineGeneralEntityTypeId,
        InlineGeneralParentProjectId,
        InlineTpUserType,
        InlineAssignableSquadSquadId,
        InlineAssignableSquadUserId,
        InlineEntityPermissionUserId,
        InlineProjectMemberUserId,
        InlineTpUserUserId,
        InlineSquadMemberUserId,


        /// <summary>
        /// Force generating table aliases based on table name in Custom Reports.
        /// By default aliases always prefixed with "t" and have format "t{number}".
        /// [General] => t0
        /// [Assignable] => t1
        /// [UserStory] => t2
        ///
        /// When feature is enabled aliases are built as concatenation of the first and all other upper letters in table name.
        /// [General] => g0
        /// [Assignable] => a1
        /// [UserStory] => us2
        /// </summary>
        BetterCustomReportsAliasNames,

        /// <summary>
        /// US#247733
        /// Affected APIs: slice/v1, api/v2
        /// Simplify process access in both slice/v1 and api/v2 when filtering by CFs for General resources
        /// by using GeneralDto.ProcessOfProject property mapped to [General].[ParentProcessID] column in DB
        /// instead of using GeneralDto.Project.Process property.
        /// [General].[ParentProcessID] is readonly column that refers process of the project referenced by [General].[ParentProjectID] column.
        /// This column is managed by DB trigger.
        /// </summary>
        MatchCustomFieldsByProcessOfProjectInSliceAndApiV2,

        /// <summary>
        /// Join [Project] table to check if resource's project is deleted by [General].[ParentProjectID] instead of [General].[SecurityProjectID]
        /// except the case when resources are queried as General instances in order not to return deleted projects.
        /// </summary>
        ReplaceSecurityProjectIdWithParentProjectIdInPermissionsFilterWhenPossible,

        /// <summary>
        /// US#251912
        /// Disables the behaviour when Parent Assignable EntityState is changed from Final to Initial (or In Dev)
        /// when new Child Assignable is added (e.g. Feature in Done state transitions to Planned if new User Story is added)
        /// </summary>
        DisableUpdatingParentAssignableStateOnChildrenChange,

        /// <summary>
        /// US#256103
        /// Replace project activity filter
        /// `g0.[ParentProjectID] IS NULL OR p10.[IsActive] = 1` with `p10.[IsActive] = 1`
        /// in SQL generated by slice API for all native Assignables and non-global ExD Assignables,
        /// because condition `g0.[ParentProjectID] IS NULL` is always `false` for them.
        /// </summary>
        OptimizeIsActiveProjectCheckForAssignablesInSlice,

        /// <summary>
        /// US#254766
        /// Client-side feature-toggle. When enabled, instructs the client to use new /cellsV2 slice API,
        /// which calculates both axes and cells on the server and returns them with a single request.
        /// </summary>
        UseSliceCellsV2,

        /// <summary>
        /// US#254761
        /// When enabled, splits polymorphic TreeView and EntityTreeView cells query (when multiple card types are selected)
        /// into separate slices per card type.
        /// Supposed to increase performance in some cases.
        /// </summary>
        TreeViewSeparateCellsPerType,

        /// <summary>
        /// US#254762
        /// When enabled, splits polymorphic axes counts query (when multiple card types are selected) into separate slices per card type.
        /// Supposed to increase performance in some cases.
        /// </summary>
        SliceSeparateAxesCountsPerType,

        /// <summary>
        /// US#254762
        /// When enabled, splits polymorphic tree view counts query (when multiple card types are selected) into separate slices per card type.
        /// Supposed to increase performance in some cases.
        /// </summary>
        TreeViewSeparateCountsPerType,

        /// <summary>
        /// US#254762
        /// Use pre-computed and stored in db slice id, e.g. 'testplanrun:1234', for TestRunItem filtering instead of
        /// inefficient sql expression like CASE WHEN "t4"."TestPlanRunID" IS NOT NULL THEN ((LOWER("t6"."Abbreviation") + @P1)
        /// + CONVERT(NVARCHAR(10),"t4"."TestPlanRunID")) ELSE @P2 END
        /// </summary>
        OptimizeTestRunItemIdFilters,

        /// <summary>
        /// US#247672
        /// For Custom Reports queries that use filters similar to either `Count(...) > 0` or `Count(...) != 0`
        /// that are semantically equivalent to `Any(...)`
        /// `ISNULL` function could be removed safely in SQL predicate
        /// for better DB statistics usage and building better query plans.
        ///
        /// Optimization is based on the following NULL value comparison rules in SQL:
        ///     `NULL != 0` => FALSE and `ISNULL(NULL, 0) != 0` => FALSE
        /// and
        ///     `NULL > 0` => FALSE and `ISNULL(NULL, 0) > 0)` => FALSE
        ///
        /// As result, in the following query:
        ///     ...
        ///     FROM "General" AS "g0"
        ///     INNER JOIN "Assignable" AS "a1" ON "g0"."GeneralID" = "a1"."AssignableID"
        ///     LEFT JOIN (
        ///       SELECT COUNT(*) AS "User Story:AssignedTeams",
        ///              "as6"."AssignableID"
        ///         FROM "AssignableSquad" AS "as6"
        ///              ...
        ///     ) AS "sq9" ON "a1"."AssignableID" = "sq9"."AssignableID"
        ///     ...
        ///     WHERE ... AND (ISNULL("sq9"."User Story:AssignedTeams", 0) != 0) AND ...
        ///
        /// predicate could be simplified to:
        ///     WHERE ... AND "sq9"."User Story:AssignedTeams" != 0 AND ...
        /// </summary>
        RemoveIsNullFunctionForCountSubQueryIfPossible,

        /// <summary>
        /// US#247672
        /// If "count" part of predicate satisfaction is required for entire predicate satisfaction, e.g
        ///     WHERE "g0"."EntityTypeID" = 4 AND (ISNULL("sq9"."User Story:AssignedTeams", 0) != 0)
        /// then "count" subquery could be joined with INNER JOIN and "count" part of predicate is removed:
        ///     ...
        ///     FROM "General" AS "g0"
        ///     INNER JOIN "Assignable" AS "a1" ON "g0"."GeneralID" = "a1"."AssignableID"
        ///     INNER JOIN (
        ///       SELECT COUNT(*) AS "User Story:AssignedTeams",
        ///              "as6"."AssignableID"
        ///         FROM "AssignableSquad" AS "as6"
        ///              ...
        ///     ) AS "sq9" ON "a1"."AssignableID" = "sq9"."AssignableID"
        ///     WHERE "g0"."EntityTypeID" = 4
        /// </summary>
        InnerJoinCountSubQueryIfPossible,

        /// <summary>
        /// US#260185
        /// Super users such as MetricsUser, RuleEngine and SystemUser can always modify any data,
        /// so permission check for them is redundant before saving created/updated business objects.
        /// The same is true for any admin user when <see cref="PrivateProjects"> feature is disabled.
        /// </summary>
        SkipEntityActionsValidationForAdmins,

        /// <summary>
        /// US#260185
        /// 1. Do not execute CalculateEffortsTrigger NHibernate trigger for such Assignables
        /// that don't have any effort related fields in ChangedFields collection.
        /// 2. Do not save Assignable itself in CalculateEffortsTrigger,
        /// save only modified RoleEffort entities instead.
        /// </summary>
        OptimizeNHibernateEffortTriggers,

        /// <summary>
        /// US#256149
        /// When enabled, cacheable by version custom fields used in resource-to-entity/entity-to-resource mapping for optimizing
        /// custom fields mapping performance
        /// </summary>
        UseCacheableCustomFieldsForMapping,

        /// <summary>
        /// BUG#255379
        /// Transform SubQueryExpression nodes like "{field} in [1,2,3]" in selectors into Enumerable.Contains calls
        /// <see cref="ConstantSubQueryParserDecorator" />
        /// </summary>
        FixSelectorInOperator,

        /// <summary>
        /// BUG#254372
        /// Returns actual values of AnyProject and AnyTeam flags in context response to use it in context auto update logic when Project
        /// or Team is added using view quick add.
        /// Previously AnyTeam was always false and AnyProject was true only when no one existing project available for user from selected
        /// </summary>
        ReturnActualAnyProjectAnyTeamInContextResponse,

        /// <summary>
        /// BUG#261749
        /// Fix issue with entity custom field conversion in API v2 parser
        /// so that `Feature.EntityCFName` works fine, and users don't have to use `Feature.CustomValues["EntityCFName"]`
        ///
        /// Must be enabled for all roles: APP, REST, COMET.
        /// </summary>
        FixEntityCustomFieldConvertWhenReferenceByNameInApiV2,

        /// <summary>
        /// US#259412
        /// Part of "batch updates in metrics" improvement.
        /// When this feature-toggle is enabled, Metrics Calculator does not generate new computation ID for metric results
        /// created when processing CalculateMetricCommandMessage,
        /// and sends empty computation ID to Result Applier instead,
        /// which allows Result Applier to batch update those results.
        /// </summary>
        DoNotGenerateComputationIdInMetricCalculator,

        /// <summary>
        /// BUG#260198
        /// When enabled, data selector expression for each type in multi type cell slice is wrapped in compilation boundary expression.
        /// Before whole selector expression is compiled, each compilation boundary replaced with dynamic invocation of separate,
        /// lazily compiled delegate. Thus, result selector in code branches representing creating result for each entity type consists
        /// of invocation code of type-specific delegate instead of inlined body.
        /// Such a change significantly reduce the compiled delegate size that allows us to compile slice selector containing lots of types
        /// without throwing StackOverflowException because the delegate's locals doesn't fit on stack
        /// of the default size (ASP.NET app running in IIS: 32bit - 256 KB, 64bit - 512 KB).
        ///
        /// Also see `/adr/2020-08-12 - Expression compilation boundaries.md` for details.
        /// </summary>
        CompileSeparateDelegatePerTypeInSelector,

        /// <summary>
        /// US#262184
        /// When enabled, uses optimized query to fetch details of entity custom field during entity-into-resource mapping,
        /// instead of using generic slow NHibernate retrieval.
        ///
        /// Can be enabled only for APP and REST, no need to enable for COMET role.
        /// </summary>
        OptimizeEntityCustomFieldMapping,

        /// <summary>
        /// US#256466
        ///
        /// - when feature-toggle is disabled: GeneralId column is always compared with filter value,
        ///   e.g. `General.Id.ToString().StartsWith("Hello") or General.Id.ToString().EndsWith("Hello")`.
        ///   This is redundant and slow.
        /// - when feature-toggle is enabled: GeneralId is included in filter only if filter text looks like a number
        /// </summary>
        ExcludeGeneralIdFromSimpleFilterWhenPossible,


        /// <summary>
        /// F#261376
        /// Add support of Hide Empty Lanes to List view mode
        /// </summary>
        HideEmptyLanesForLists,

        /// <summary>
        /// US#254633
        /// When enabled, retrieves General entities in an optimized way using their entity type info
        /// to avoid huge NHibernate queries with LEFT JOIN for each possible General type.
        ///
        /// Can be enabled for all roles.
        /// </summary>
        RetrieveGeneralsOfSpecificType,

        /// <summary>
        /// BUG#264141
        /// Uses NO LOCK hint for ComponentVersions table
        ///
        /// Can be enabled for all roles.
        /// </summary>
        NoLockForComponentVersionsByDefault,

        /// <summary>
        /// Uses NO LOCK hint for Custom Field component version,
        /// even if <see cref="NoLockForComponentVersionsByDefault"/> is disabled.
        ///
        /// Can be enabled for all roles.
        /// </summary>
        NoLockForCustomFieldVersion,

        /// <summary>
        /// US#265193
        /// Marker feature-toggle, informing the clients that domain schema already supports EntityType.Origin field.
        /// Not expected to be turned off.
        /// </summary>
        EntityTypeOrigin,

        /// <summary>
        /// US#256467
        /// Marker flag for front-end. When enabled, sends "applySimpleFilterToTags: false" in slice definition,
        /// which makes lookup component queries faster.
        /// </summary>
        DoNotApplySimpleFilterToTagsInLookups,

        /// <summary>
        /// US#250872
        /// Marker feature-toggle, informing the clients that domain schema API already supports changing IsGlobal field.
        /// Not expected to be turned off.
        /// </summary>
        EntityTypeSwitchIsGlobal,

        /// <summary>
        /// US#236931
        /// Previously, customize card unit sets were generated for each extendable domain field separately. Even if fields are same and have
        /// same type for selected cards. From now on, when the feature is enabled, units are generated for each unique field and type pair.
        /// It means that for Capability field of type Capability added for Feature and UserStory there would be single set of units
        /// generated, allowed to be used for both Feature and UserStory cards.
        /// </summary>
        [ClientFeature("mergeExtendableDomainUnits")]
        MergeExtendableDomainUnits,

        /// <summary>
        /// BUG#266008
        /// Compatibility toggle for some clients relying on unexpected behavior of comparing entity custom fields with numbers in API v2,
        /// e.g. `?where=EntityCf != 10` instead of `?where=EntityCf.Id != 10`.
        ///
        /// Would be great to get rid of such behavior in the future for consistency,
        /// but it requires manual modifications to configured user-data like automation rules.
        ///
        /// Can be enabled for all roles.
        /// </summary>
        AllowToFilterByEntityCustomFieldDirectly,

        /// <summary>
        /// US#226806
        /// Use new chunk names for TP3 and TP2 pages, and expect targetprocess-frontend to load mashups bundle
        /// and globals by itself, without script injection, for pages other than Board.aspx (__tauinit__ used there instead)
        /// </summary>
        FrontendCodeSplitting,

        /// <summary>
        /// US#266578 Allows assignment of multiple teams to a Request without using Team Workflows
        /// </summary>
        AssignMultipleTeamsToRequests,

        /// <summary>
        /// US#266746
        /// Use raw SQL instead of REST to fetch data required for default metric calculation.
        /// </summary>
        UseRawSqlInDefaultMetrics,

        /// <summary>
        /// US#266982
        /// Optimization toggle. When enabled, skips slow validation of custom field names for performance boost on hot path,
        /// assuming that passed custom field names (CustomField1, 2, etc.) are always valid
        /// since there are no known use-cases when they can be invalid.
        ///
        /// Can be enabled for all roles.
        /// </summary>
        DoNotValidateCustomFieldNamesOnRead,

        /// <summary>
        /// F#239939
        /// If enables then view context filters will be managed by separate service
        /// </summary>
        ManagingViewContextFromService,

        /// <summary>
        /// US#241857
        /// When enabled, relations tab is shown on detailed view of extendable domain entity
        /// </summary>SpaceCalculator.cs
        [ClientFeature("extendableDomainRelationsTab")]
        ExtendableDomainRelationsTab,

        /// <summary>
        /// Client-side feature toggle.
        /// Used to generate extendable domain tabs using new format of axis ids
        /// </summary>
        [ClientFeature("customSliceManyToMany")]
        CustomSliceManyToMany,

		/// <summary>
        /// https://plan.tpondemand.com/entity/245538-add-possibility-to-customize-money-format
        /// Allow to specify custom format for money custom field value during custom field creation.
        /// </summary>
        AllowSpecifyCustomMoneyFormat,

        /// <summary>
        /// US#266539
        /// Allows to use Stored List widgets on dashboards. Stored List - list view which definition stored inside dashboard definition.
        /// Unlike Linked List, Stored List is not showed as separate view menu item.
        /// </summary>
        [ClientFeature("dashboardStoredListTemplate")]
        DashboardStoredListTemplate,
    }
}

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

        SliceSpecificCases,
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

        [ClientFeature("metrics")]
        Metrics,

        [ClientFeature("metricsProgress")]
        MetricsProgress,

        [ClientFeature("metricsEffort")]
        MetricsEffort,

        [ClientFeature("metricsEffortForReleaseAndIteration")]
        MetricsEffortForReleaseAndIteration,

        [ClientFeature("metricsEffortViaRelations")]
        MetricsEffortViaRelations,

        [ClientFeature("metricsTimeSpentRemain")]
        MetricsTimeSpentRemain,

        [ClientFeature("metricsCustomFormula")]
        MetricsCustomFormula,
        MetricsCustomFormulaCache,

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
        /// Apply metric results in context of separate clean Portal
        /// in order not to lose some changes because of stale entity state in NHibernate cache of existing Portal (BUG#162420)
        /// </summary>
        MetricsUseSeparatePortalForEntityUpdate,

        /// <summary>
        /// US#176365 and US#176364
        /// Send update requests to external loop prevetion service. But still use internal loop prevention for checks
        /// </summary>
        [ClientFeature("metricsUpdateExternalLoopPrevention")]
        MetricsUpdateExternalLoopPrevention,

        /// <summary>
        /// Enables publishing of resource change messages to Rule Engine service.
        /// See US#148418 for details
        /// </summary>
        [ClientFeature("ruleEngineV2Publishing")]
        RuleEngineV2Publishing,

        /// <summary>
        /// Per server feature that enables execution of REST commands via queue entry point.
        /// US#150053
        /// </summary>
        MetricsAndRulesExecuteCommandViaQueue,

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

        /// <summary>
        /// Allow apply user context for vizydrop report on view
        /// </summary>
        AllowApplyUserContextForVizydrop,

        /// <summary>
        /// Allow apply user context for vizydrop report on dashboard
        /// </summary>
        AllowApplyUserContextForVizydropOnDashboard,

        CacheUserAuthData,
        CacheGlobalSettingsData,
        CacheLiteContextByAcid,

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
        /// US#136219, temporary feature-toggle to easily avoid breaking changes due to removed extension methods in public API (if required)
        /// </summary>
        LimitPublicExtensionMethods,


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
        UseUiQueueForComet,
        UseMemoizedCometClient,
        UseSliceCompiledDelegateCache,


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
        UseDslEnginePool,
        UseDslTypeSystemMemoizationPerDefinition,
        UseAccountOwnConnectionPool,

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

        /// <summary>
        /// Old global search
        /// </summary>
        [ClientFeature("search")]
        Search,

        ///<summary>
        /// New global search
        ///</summary>
        [ClientFeature("search2")]
        Search2,

        [ClientFeature("oauth")]
        OAuth,

        /// <summary>
        /// Toggles client-side support for multiple final states in workflow editor
        /// </summary>
        [ClientFeature("multipleFinalStatesEditor")]
        MultipleFinalStatesEditor,

        /// <summary>
        /// Use full context for Burn Down
        /// </summary>
        BurnDownFullContext,

        /// <summary>
        /// Automaticaly move child bugs to same project when feature's project changes.
        /// </summary>
        MoveBugsToNewFeatureProject,

        /// <summary>
        /// Build faster queries with less checks for deleted projects whenever possible
        /// </summary>
        OptimizedDeletedProjectCheck,

        ProcessSoftDelete,

        [Mashup("Search2 Search2UI")]
        Search2UIIntegration,

        [ClientFeature("search2ui")]
        Search2UI,

        [ClientFeature("include.stack.traces.in.response")]
        IncludeStackTracesInResponse,

        /// <summary>
        /// Sync team list on userstory and it's tasks
        /// </summary>
        SyncTeamsOnUserStoryAndTasks,

        /// <summary>
        /// Use strictier type inference logic in dsl engine. Fixes #75331.
        /// </summary>
        StrictDslTypes,

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
        /// Enables board integration module (F#142837)
        /// </summary>
        BoardIntegrationModule,

        [ClientFeature("disableSliceBase64")]
        DisableSliceBase64,

        /// <summary>
        /// Uses better dates comparison logic in dsl engine (BUG#111590)
        /// </summary>
        DslBetterDatesComparison,

        /// <summary>
        /// See ProtectFromNullReferenceVisitor.ShouldProtectMethodCall. Fixes #159216.
        /// </summary>
        RestProtectAllMethodCallsFromNull,

        /// <summary>
        /// Enables some speedup in editing team workflows. See #96862.
        /// </summary>
        TeamWorkflowEditSpeedup,

        /// <summary>
        /// Backwards-compatibility feature toggle which makes LimitedParallelExecutor use TaskFactory to run callbacks.
        /// Expected to be turned off by default.
        /// Should be removed when testing shows that TP works fine without it in production.
        /// </summary>
        LimitedParallelExecutorTaskFactory,

        /// <summary>
        /// US#162069 When enabled, schedules in-proc metrics reacting to data modifications in foreground threads instead of background ones.
        /// </summary>
        MetricsForegroundThreadObserver,

        /// <summary>
        /// Enables ability to select related project/teams in context selector
        /// </summary>
        [ClientFeature("relatedSelectionForContextSelector")]
        RelatedSelectionForContextSelector,

        [ClientFeature("new.list.refresh.on.projects.change")]
        RefreshNewListOnProjectsChange,

        /// <summary>
        /// Forces all CF triggers to use serializable isolation level
        /// See BUG#163843 for details
        /// </summary>
        UseSerializableTransactionsForCustomFieldsTrigger,

        /// <summary>
        /// Assume that CFs in cloned process are already mapped in an optimal way,
        /// and don't run additional remappings in triggers.
        ///
        /// This is a safety feature-toggle, it should be removed when we are sure that this behavior
        /// doesn't cause regressions in production.
        ///
        /// See US#164044 for details.
        /// </summary>
        DoNotAdjustCustomFieldMappingWhenCloningProcess,

        /// <summary>
        /// When performing custom field remappings in triggers,
        /// try to adjust created/modified CF so that it is mapped to the same column as other fields with the same name
        /// instead of remapping all existing fields to make them correspond to created/modified field.
        ///
        /// This is a safety feature-toggle, it should be removed when we are sure that this behavior
        /// doesn't cause regressions in production.
        ///
        /// See US#164044 for details.
        /// </summary>
        RemapModifiedCustomFieldOnly,

        /// <summary>
        /// Enables scoped account lock for Views Menu items manipulations to avoid deadlocks
        /// which might be used by concurrent modifications.
        /// See BUG#161327 for details.
        /// </summary>
        [ClientFeature("accountLockForViewsMenuOperations")]
        AccountLockForViewsMenuOperations,

        /// <summary>
        /// Increases number of cases rest support in filter expressions. See US#164255.
        /// </summary>
        RestImprovePolymorphicFilters,

        /// <summary>
        /// Increases number of cases rest support in select expressions. See US#164255.
        /// </summary>
        RestImprovePolymorphicSelectors,

        /// <summary>
        /// Enables caching of Full LightContext instances for HttpContext (if awailable) or ThreadLocal scope. See BUG#164611 for details
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
        /// US#168834 Adds support for CASE WHEN ... THEN ... END expressions in ORDER BY clauses.
        /// Used to order cards and axes by custom fields with the same name
        /// even when those custom fields are mapped to different database columns.
        /// </summary>
        EnableOrderByCustomFieldExpression,

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
        /// BUG#170990 Rule Engine processes events of wrong accounts
        /// </summary>
        RuleEngineObserveOnCurrentThreadScheduledObservable,

        /// <summary>
        /// Add move to top functionality to context menu on board and list
        /// </summary>
        [ClientFeature("moveToTop")]
        MoveToTop,

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
        /// List large page render optimization. Feature toggle can be removed on second release if no issues.
        /// </summary>
        [ClientFeature("newListLargePageOptimization")]
        NewListLargePageOptimization,

        /// <summary>
        /// US#171023 Removes double NULL check from sql generated for axis projection
        /// </summary>
        RemoveDoubleNullCheckFromAxisProjectionExpression,

        /// <summary>
        /// US#171030 Remove unnecessary joins from polymorphic axes
        /// </summary>
        RemoveUnnecessaryJoinsFromPolymorphicAxes,

        /// <summary>
        /// US#171145 Remove excessive joins when filtering child collections
        /// </summary>
        ReduceJoinsWhenAggregateCollections,

        /// <summary>
        /// US#171146 Join child subqueries to parent column
        /// </summary>
        OptimizeJoinConditionForCountSubQuery,

        // <summary>
        // Enables retrospectives service
        // </summary>
        [ClientFeature("retrospectives")]
        Retrospectives,

        /// <summary>
        /// Replaces built-in Views Menu with the one provided by the Views Menu Mashup
        /// See Feature #171906
        /// </summary>
        [ClientFeature("viewsMenuMashup")]
        ViewsMenuAsMashup,

        /// <summary>
        /// Fixes join condition generation for one-to-many collections that has same type as their parent, e.g. TestPlanRun.TestPlanRuns, #173942
        /// </summary>
        FixJoinGenerationForSameTable,

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
        /// Applies other axis filters when calculate axis counters. See #67718.
        /// </summary>
        AxesCountersWithFullFilters,

        /// <summary>
        /// Enables integration of Notification Center UI in TP
        /// </summary>
        [ClientFeature("notificationCenter")]
        NotificationCenter,

        /// <summary>
        /// Allows to use custom field names directly in API v2 queries
        /// instead of `CustomValues["Name"]`.
        /// See US#179383 for details
        /// </summary>
        ReferenceCustomFieldsByNameInApiV2,

        /// <summary>
        /// Enables refreshing of labels using slice comet subscription on detailed view tabs with entity collection lists
        /// See US#178806 for details
        /// </summary>
        [ClientFeature("label.refresher.comet")]
        LabelRefresherComet,
        /// <summary>
        /// Adds support of inactive teams
        /// </summary>
        [ClientFeature("inactiveTeams")]
        InactiveTeams,
        
        /// <summary>
        /// US#180358 Fix N+1 problems in capacity calculator
        /// </summary>
        FixNPlus1InCapacityCalculator,
        
        /// <summary>
        /// US#180714
        /// </summary>
        ImproveTagsFilteringPerformance,

        /// <summary>
        /// Use entity custom fields instead of text ones in hierarchy.link component.
        /// </summary>
        [ClientFeature("useEntityCustomFieldsInHierarchyLinkComponent")]
        UseEntityCustomFieldsInHierarchyLinkComponent,
    }
}

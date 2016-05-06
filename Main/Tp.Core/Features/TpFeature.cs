using System.ComponentModel;
// ReSharper disable InconsistentNaming

namespace Tp.Core.Features
{
	public enum TpFeature
	{
		None = 0,

		InboundEmailIntegration,

		LightEdition,

		[ClientFeature("tp3.only")]
		TP3OnlyMode,

		ProEdition,

		[ClientFeature("comet.notifications")]
		[Mashup("KanbanNotifications")]
		[Mashup("BoardNotifications")]
		ServerNotifications,

		SliceSpecificCases,
		OptimizeForUnknownQueryHint,

		[Description(@"The solution to handle ""parameter sniffing"" based on assigning the stored procedure parameters to local variables and then using the local variables in the query.")]
		QueryWithLocalVariables,

		[ClientFeature("context.cacheresponse")]
		CacheContextResponse,

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
		[ClientFeature("single.signon")] SingleSignOn,

		DoNotUseTableValueParameterInSqlQuery,

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
		/// Toggles restriction to assign multiple teams on one assignable
		/// </summary>	
		[ClientFeature("multiple.teams")] MultipleTeams,

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

		[ClientFeature("metrics")] Metrics,
		[ClientFeature("metricsEffortViaRelations")] MetricsEffortViaRelations,

		[ClientFeature("metricsUserStoryEffort")] MetricsUserStoryEffort,
		[ClientFeature("metricsFeatureEffort")] MetricsFeatureEffort,
		[ClientFeature("metricsEpicEffort")] MetricsEpicEffort,
		[ClientFeature("metricsRequestEffort")] MetricsRequestEffort,


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
		UnlimitedRestPaging,
		
		/// <summary>
		/// Enables generation of link entities between TestCaseRun and its each parent TestPlanRun
		/// </summary>
		TestRunItemHierarchyLink,
	}
}

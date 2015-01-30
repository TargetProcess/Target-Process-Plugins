//
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System.Collections.Generic;
using System.ComponentModel;
using StructureMap;
using System;
using System.Linq;

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
		SwitchToProEdition,
		SecureJsonp,
		[ClientFeature("comet.notifications")]
		[Mashup("KanbanNotifications")]
		[Mashup("BoardNotifications")]
		ServerNotifications,
		SliceSpecificCases,
		NewPrioritization,
		ParallelExpanders,
		SliceCellFilterPropertiesAutoSet,
		ParallelSliceCalls,
		OptimizeForUnknownQueryHint,
		[Description(@"The solution to handle ""parameter sniffing"" based on assigning the stored procedure parameters to local variables and then using the local variables in the query.")]
		QueryWithLocalVariables,
		Diagnostics,
		TrackProgressInDb,
		FlattenInnerReport,
		[ClientFeature("board.cardBatchMoveVisualEffects")]
		CardBatchMoveVisualEffects,
		[ClientFeature("context.v2")]
		Context2,
		[ClientFeature("contextPrototype")]
		ContextPrototype,

		[OverrideableByMashup]
		Cdn,

		PerforcePlugin,
		DisableProtocolCheck,
		SkipSecurityCheckingForSecondLevelEntities,
		BatchFlushForTestCases,
		[ClientFeature("cache.axis")]
		CacheAxis,
		[ClientFeature("board.menu.comet")]
		ViewsMenuCometClient,
		ViewsMenuCometServer,
		SqlTimeProfiling,
		[ClientFeature("load.progress")]
		LoadProgress,
		OrderPreservingEntityStateGrouping,
		[ClientFeature("mashups.loader")]
		MashupsLoader,
		LockAccountOnTransactionOperation,
		DisableTransactionalMsmqMessageProcessing,

		/// <summary>
		/// Toggles easy process setup on client side
		/// </summary>
		[ClientFeature("process.setup")]
		ProcessSetup,

		/// <summary>
		/// Toggles restriction for sub workflow creation on serve side
		/// and team workflow editor on client side
		/// </summary>
		[ClientFeature("process.sub.workflows")]
		SubWorkflows,

		/// <summary>
		/// Toggles restriction to assign multiple team on one card
		/// </summary>
		TeamStates,

		[ClientFeature("warning.noProjectIsDeprecated")]
		WarningNoProjectIsDeprecated,

		/// <summary>
		/// Toggles visibility of 'No project' in UI (context menu, project selector on view, project axis)
		/// </summary>
		[ClientFeature("hide.noProject")]
		HideNoProject,

		Follow,
		[ClientFeature("move.highlight")]
		MoveHighlight,

		/// <summary>
		/// When enabled, old DTOs will be extended with original DTO value and author of changes
		/// </summary>
		ExtendedOldDto,

		/// <summary>
		/// Used to toggle the visibility of Epics entity in view setup. Does not affect REST and Slice API visibility
		/// </summary>
		[ClientFeature("entity.epics")]
		Epics,

		/// <summary>
		/// Toggles client subscriptions on entity newlist comet hub
		/// </summary>
		[ClientFeature("entity.new.list.comet")]
		EntityNewListComet,

		/// <summary>
		/// Toggles visibility of dasboards in views menu
		/// </summary>
		[ClientFeature("dashboards")]
		Dashboards,

		/// <summary>
		/// Toggles cross project releases support
		/// </summary>
		[ClientFeature("release.crossproject")]
		CrossProjectReleases,

		/// <summary>
		/// Toggles logging for email notifications
		/// </summary>
		EmailNotificationsLogging,

		/// <summary>
		/// Toggles visibility of reports in views menu
		/// </summary>
		[ClientFeature("customReports")]
		CustomReports,

		/// <summary>
		/// Toggles posibility to see hierarchies of test plans and test plan runs in slice
		/// Does not toggle hierarchy in rest
		/// </summary>
		[ClientFeature("qa.area.hierarchy")]
		QaAreaHierarchy,

		/// <summary>
		/// Toggles returning total child count for a sub-tree of a List view.
		/// See US#90142 for details.
		/// </summary>
		[ClientFeature("new.list.deep.counts")]
		DeepListCounts,

		/// <summary>
		/// When enabled slice will use table instead of view for hierarchies of test plan runs
		/// </summary>
		UseTestRunItemTable,

		/// <summary>
		/// See US#92442 for details.
		/// </summary>
		[ClientFeature("rule.engine")]
		RuleEngine,

		/// <summary>
		/// Prefetch template data for general quick add on each context change. See US#95641 for details.
		/// </summary>
		[ClientFeature("general.quick.add.data.prefetch")]
		GeneralQuickAddDataPrefetch,
	}

	public interface ITpFeatureList
	{
		bool IsEnabled(TpFeature tpFeature);
	}

	public static class TpFeatureListExtensions
	{
		public static TpFeature[] GetEnabledFeatures(this ITpFeatureList list)
		{
			return Enum.GetValues(typeof(TpFeature)).Cast<TpFeature>().Where(list.IsEnabled).ToArray();
		}

		public static Dictionary<TpFeature, bool> GetAllFeatures()
		{
			return Enum.GetValues(typeof(TpFeature)).Cast<TpFeature>().Where(x => x != TpFeature.None).ToDictionary(x => x, x => x.IsEnabled());
		}

		public static bool IsEnabled(this TpFeature feature)
		{
			return ObjectFactory.GetInstance<ITpFeatureList>().IsEnabled(feature);
		}

		public static string GetClientName(this TpFeature feature)
		{
			return feature.GetAttribute<ClientFeatureAttribute>().Select(x => x.ClientFeatureName).GetOrElse(() => feature.ToString().CamelCase());
		}

		public static IEnumerable<string> GetMashupNames(this TpFeature feature)
		{
			return feature.GetAttributes<MashupAttribute>().Select(x => x.MashupName);
		}
	}
}

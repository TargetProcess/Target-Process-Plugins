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
		NewFlushStrategy,
		PublishBusinessRuleSideEffects,
		SliceSpecificCases,
		NewPrioritization,
		ParallelExpanders,
		SliceCellFilterPropertiesAutoSet,
		ParallelSliceCalls,
		OptimizeForUnknownQueryHint,
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
		[ClientFeature("process.setup")]
		ProcessSetup,
		[ClientFeature("warning.noProjectIsDeprecated")]
		WarningNoProjectIsDeprecated,
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
		[ClientFeature("release.crossproject")]
		CrossProjectReleases,

		/// <summary>
		/// Toggles logging for email notifications
		/// </summary>
		EmailNotificationsLogging
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
			return feature.GetAttribute<ClientFeatureAttribute>().Bind(x => x.ClientFeatureName).GetOrElse(() => feature.ToString().CamelCase());
		}

		public static IEnumerable<string> GetMashupNames(this TpFeature feature)
		{
			return feature.GetAttributes<MashupAttribute>().Select(x => x.MashupName);
		}
	}
}

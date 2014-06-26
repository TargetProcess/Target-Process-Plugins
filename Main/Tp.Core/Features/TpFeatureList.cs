//
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using StructureMap;
using System;
using System.Linq;

namespace Tp.Core.Features
{
	public enum TpFeature
	{
		None = 0,
		InboundEmailIntegration,
		TestCaseLibrary,
		UserStoriesCountReport,
		BugsProgressReport,
		NewPluginList,
		LightEdition,
		[ClientFeature("tp3.only")]
		TP3OnlyMode,
		NewBugzillaIntegration,
		ProEdition,
		MashupManagerPlugin,
		GitPlugin,
		SubversionViewDiff,
		GitViewDiff,
		SwitchToProEdition,
		SecureJsonp,
		[ClientFeature("comet.notifications")]
		ServerNotifications,
		NewFlushStrategy,
		PublishBusinessRuleSideEffects,
		SliceSpecificCases,
		[ClientFeature("quick.add")]
		QuickAdd,
		[ClientFeature("axes.quick.add")]
		AxesQuickAdd,
		[ClientFeature("full.counts")]
		FullCounts,
		NewPrioritization,
		[ClientFeature("boardTemplates")]
		BoardTemplates,
		ParallelExpanders,
		SliceCellFilterPropertiesAutoSet,
		[ClientFeature("entity.quickAdd")]
		EntityQuickAdd,
		ParallelSliceCalls,
		[ClientFeature("flowHelp")]
		FlowHelp,
		[ClientFeature("boardEditorPrioritization")]
		BoardEditorPrioritization,
		OptimizeForUnknownQueryHint,
		Diagnostics,
		[ClientFeature("board.customize.list")]
		CustomizeCardsList,
		[ClientFeature("board.lists")]
		Lists,
		[ClientFeature("newlist.operations")]
		ListsOperations,
		FirstTimeTp3Login,
		[ClientFeature("board.timeline.forecast")]
		TimelineForecast,
		TrackProgressInDb,
		FlattenInnerReport,
		[ClientFeature("board.cardBatchMoveVisualEffects")]
		CardBatchMoveVisualEffects,
		CustomFieldsServerNotification,
		[ClientFeature("context.v2")]
		Context2,
		[ClientFeature("contextPrototype")]
		ContextPrototype,
		[ClientFeature("boardSettings.cache")]
		BoardSettingsCache,

		[OverrideableByMashup]
		Cdn,

		PerforcePlugin,
		DisableProtocolCheck,
		SkipSecurityCheckingForSecondLevelEntities,
		BatchFlushForMove,
		BatchFlushForPrioritize,
		BatchFlushForTestCases,
		[ClientFeature("cache.axis")]
		CacheAxis,
		[ClientFeature("new.board.menu")]
		NewBoardMenu,
		SqlTimeProfiling,
		[ClientFeature("terms.tp3")]
		TermsInTp3
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

		public static bool IsEnabled(this TpFeature feature)
		{
			return ObjectFactory.GetInstance<ITpFeatureList>().IsEnabled(feature);
		}

		public static string GetClientName(this TpFeature feature)
		{
			return feature.GetAttribute<ClientFeatureAttribute>().Bind(x => x.ClientFeatureName).GetOrElse(() => feature.ToString().CamelCase());
		}

	}
}

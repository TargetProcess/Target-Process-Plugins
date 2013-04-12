// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;

namespace Tp.Core.Features
{
	public enum TpFeature
	{
		None = 0,
		InboundEmailIntegration,
		TestCaseLibrary,
		Solution,
		UserStoriesCountReport,
		BugsProgressReport,
		NewPluginList,
		LightEdition,
		NewBugzillaIntegration,
		ProEdition,
		MashupManagerPlugin,
		GitPlugin,
		SubversionViewDiff,
		GitViewDiff,
		SwitchToProEdition,
		SecureJsonp,
		ServerNotifications,
		NewFlushStrategy,
		PublishBusinessRuleSideEffects,
		SliceSpecificCases,
		QuickAdd,
		AxesQuickAdd,
		FullCounts,
		CachedCompile,
		NewPrioritization,
		BoardTemplates,
		ParallelExpanders,
		SliceCellFilterPropertiesAutoSet,
		RelationsNetwork,
		EntityQuickAdd,
		ParallelSliceCalls,
		FlowHelp,
        BoardEditorPrioritization
	}

	public interface ITpFeatureList
	{
		bool IsEnabled(TpFeature tpFeature);
	}

	public static class TpFeatureListExtensions
	{
		public static TpFeature[] GetEnabledFeatures(this ITpFeatureList list)
		{
			return Enum.GetValues(typeof (TpFeature)).Cast<TpFeature>().Where(list.IsEnabled).ToArray();
		}
	}
}

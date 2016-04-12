using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Tp.Core.Diagnostics.Time
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public enum ProfilerTarget
	{
		[HttpHeader("X-Thread-Execution-Stopwatch")] [Description("Http request handling time")] ThreadExecution,
		[HttpHeader("X-Sql-Stopwatch")] [Description("Time spent in database")] SqlExecution,
		[HttpHeader("X-Stopwatch")] [Description("Http request handling time")] WebRequestHandling,
		[HttpHeader("X-AccountLockWaitTime")] [Description("Time spent waiting account lock")] WaitOnAccountLock,
		[HttpHeader("X-MvcActionExecution")] [Description("Time spent on executing web controller action")] MvcActionExecution,
		[HttpHeader("X-MvcResultExecution")] [Description("Time spent on creating web controller result")] MvcResultExecution,
		[HttpHeader("X-GlobalDataTemplates-Total")] [Description("Time spent on creating global data templates")] GlobalDataTemplates_Total,
		[HttpHeader("X-BasicTemplatesProvider-GetBasicTemplates")] [Description("Time spent on GetBasicTemplate")] BasicTemplatesProvider_GetBasicTemplates,
		[HttpHeader("X-CellQuickAddActionsBase-GetPossibleActions")] [Description("Time spent on GetPossibleActions")] CellQuickAddActionsBase_GetPossibleActions,
		[HttpHeader("X-MatrixSliceController-GetSliceData")] [Description("Time spent on GetSliceData")] MatrixSliceController_GetSliceData,
		[HttpHeader("X-MvcSerializeResult")] [Description("Time spent on serializing result")] MvcSerializeResult,
		[HttpHeader("X-CellDataEditor-GetListActions")] [Description("Time spent on GetListActions")] CellDataEditor_GetListActions,
		[HttpHeader("X-CellDataEditor-GetListDataTemplates")] [Description("Time spent on GetListDataTemplates")] CellDataEditor_GetListDataTemplates,
		[HttpHeader("X-Dynamic-LoadResourceFromDbReader")] [Description("Time spent on loading dynamic resource from data reader")] DynamicTypedLoadResourceFromDbReader,
		[HttpHeader("X-Static-LoadResourceFromDbReader")] [Description("Time spent on loading static resource from data reader")] StaticTypedLoadResourceFromDbReader,
		[HttpHeader("X-DslFilterAutoApply")] [Description("Time spent on dsl filter applying")] DslFilterAutoApply,
		[HttpHeader("X-DslFilterBuild")] [Description("Time spent on dsl filter building")] DslFilterBuild,
		[HttpHeader("X-DslFilterBuildSuggestions")] [Description("Time spent on dsl suggestions filter building")] DslFilterBuildSuggestions
	}
}

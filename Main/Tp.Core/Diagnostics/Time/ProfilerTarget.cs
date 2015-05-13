using System.ComponentModel;

namespace Tp.Core.Diagnostics.Time
{
	public enum ProfilerTarget
	{
		[Description("X-Thread-Execution-Stopwatch")]
		ThreadExecution,
		[Description("X-Sql-Stopwatch")]
		SqlExecution,
		[Description("X-Web-Request-Handling-Stopwatch")]
		WebRequestHandling
	}
}
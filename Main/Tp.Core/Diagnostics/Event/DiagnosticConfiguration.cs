using System;
using System.Configuration;

namespace Tp.Core.Diagnostics.Event
{
    public class DiagnosticConfiguration
    {
        private static readonly TryCreateValueOnEveryFailedUnboxLazy<bool> _shouldTraceDetails =
            Lazy.CreateLazyTryingToCreateValueOnEveryFailedUnbox(
                () => ConfigurationManager.AppSettings["Profiler.ShouldTraceDetails"].NothingIfNull().Select(bool.Parse).GetOrDefault());

        private static readonly TryCreateValueOnEveryFailedUnboxLazy<bool> _shouldIncludeStackTraces =
            Lazy.CreateLazyTryingToCreateValueOnEveryFailedUnbox(
                () =>
                    ConfigurationManager.AppSettings["Profiler.ShouldIncludeStackTraces"].NothingIfNull()
                        .Select(bool.Parse)
                        .GetOrDefault());

        private static readonly TryCreateValueOnEveryFailedUnboxLazy<bool> _shouldIncludeTraceContexts =
            Lazy.CreateLazyTryingToCreateValueOnEveryFailedUnbox(
                () =>
                    ConfigurationManager.AppSettings["Profiler.ShouldIncludeTraceContext"].NothingIfNull()
                        .Select(bool.Parse)
                        .GetOrDefault());

        public bool ShouldTraceDetails => _shouldTraceDetails.Value;
        public bool ShouldIncludeStackTraces => _shouldIncludeStackTraces.Value;
        public bool ShouldIncludeTraceContexts => _shouldIncludeTraceContexts.Value;
    }
}

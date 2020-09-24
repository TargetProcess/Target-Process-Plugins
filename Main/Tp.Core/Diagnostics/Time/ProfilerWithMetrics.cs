using System;
using App.Metrics;
using App.Metrics.Histogram;
using Tp.Core.Annotations;

namespace Tp.Core.Diagnostics.Time
{
    public static class ProfilerWithMetrics
    {
        public static TResult MeasureDuration<TResult>(
            [NotNull] this IMeasureMetrics measureMetrics,
            [NotNull] HistogramOptions histogramOptions,
            [NotNull] [InstantHandle] Func<TResult> action)
        {
            var (res, duration) = Profiler.Measure(action);
            measureMetrics.Histogram.Update(histogramOptions, (long) duration.TotalMilliseconds);
            return res;
        }

        public static IDisposable DurationHistogram(
            [NotNull] this IMeasureMetrics measureMetrics,
            [NotNull] HistogramOptions histogramOptions)
        {
            var startedAt = DateTimeOffset.Now;
            return Disposable.Create(() =>
            {
                var finishedAt = DateTimeOffset.Now;
                var duration = finishedAt - startedAt;
                measureMetrics.Histogram.Update(histogramOptions, (long) duration.TotalMilliseconds);
            });
        }
    }
}

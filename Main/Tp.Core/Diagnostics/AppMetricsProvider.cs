using App.Metrics;
using StructureMap;
using Tp.Core.Annotations;
using Tp.Core.Configuration;

namespace Tp.Core.Diagnostics
{
    public static class AppMetricsProvider
    {
        public static readonly string RoleTag = AppSettingsReader.ReadString("PrometheusMetricsRoleTag", "unknown");

        private static readonly IMetrics _nullInstance = new MetricsBuilder().Build();

        private static IMetrics _instance;

        /// <remarks>
        /// The main purpose is to expose registered instance as a lazy property
        /// and avoid calls to <see cref="ObjectFactory"/> after initialization,
        /// to make metrics cheap on hot execution paths.
        ///
        /// Metrics don't depend on account-specific environment,
        /// so it should be safe to reuse the instance.
        ///
        /// It's also fine to lose some metrics which are captured before container is initialized (passed to <see cref="_nullInstance"/>)
        /// </remarks>
        [NotNull]
        public static IMetrics Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = ObjectFactory.TryGetInstance<IMetrics>();
                }

                return _instance ?? _nullInstance;
            }
        }
    }
}

using System.Threading.Tasks;
using TwitterStub.Interfaces.Metrics;

namespace TwitterStub.Core.Metrics
{
    public abstract class MetricsTracker : IMetricTracker
    {
        public MetricsTracker(IMetricSource metricSource)
        {
            Source = metricSource;
        }

        protected IMetricSource Source { get; private set; }

        public abstract string Name { get; }

        public abstract Task<object> GetMetric();
    }
}

using System.Linq;
using System.Threading.Tasks;
using TwitterStub.Interfaces.Metrics;
using TwitterStub.Models;

namespace TwitterStub.Core.Metrics
{
    public class TotalTweetsMetric : MetricsTracker
    {
        public TotalTweetsMetric(IMetricSource source) : base(source)
        {

        }

        public override string Name => "Total Tweets";

        public override async Task<object> GetMetric()
        {
            if (Source?.Repository == null)
                return 0;

            return (await Source.Repository.GetAll<Tweet>()).Count();
        }

    }
}
